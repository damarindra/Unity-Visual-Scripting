using UnityEngine;
using System.Collections.Generic;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

namespace DI.VisualScripting {
	public static class DIComponentHelper {
		public static DIVisualComponent ComponentChooser(this DIVisualComponent self)
		{
#if UNITY_EDITOR
			int i = EditorGUILayout.Popup(0, DIAttributeData.Instance.componentPathPopup.ToArray());
			if (i != 0)
			{
				DIVisualComponent next = ScriptableObject.CreateInstance(DIAttributeData.Instance.componentsWithAttribute[i - 1]) as DIVisualComponent;
				return next;
			}
#endif
			return null;
		}
		public static void EGLLabel(string label, GUIStyle style = null) {
#if UNITY_EDITOR
			if (style != null)
				EditorGUILayout.LabelField(label, style);
			else
				EditorGUILayout.LabelField(label);
#endif
		}
		public static void EGLBeginHorizontal()
		{
#if UNITY_EDITOR
			EditorGUILayout.BeginHorizontal();
#endif
		}
		public static void EGLEndHorizontal()
		{
#if UNITY_EDITOR
			EditorGUILayout.EndHorizontal();
#endif
		}
		public static GUIStyle boldLabel {
			get {
#if UNITY_EDITOR
				return UnityEditor.EditorStyles.boldLabel;
#else
				return null;
#endif
			}
		}
		/// <summary>
		/// Save current component
		/// </summary>
		/// <param name="self"></param>
		/// <param name="rootParent">null for saving, not null for export</param>
		/// <param name="previous">previous comp</param>
		/// <param name="autoAssignNextComponentOfPrevious">jadi result.previous.next akan diassign otomatis, jika component hasil dari percabangan (ex : branch, if) atur ke false untuk menonaktifkan automatisasi</param>
		public static DIVisualComponent _SaveComponent(this DIVisualComponent self, UnityEngine.Object rootParent, DIVisualComponent previous, bool autoAssignNextComponentOfPrevious = true)
		{
#if UNITY_EDITOR
			string debug = "";
			self.name = string.IsNullOrEmpty(self.windowName) ? self.GetType().ToString() : self.windowName;
			self.name += " - " + DIVisualScriptingData.inspectRootField.Name;
			DIVisualComponent result = null;
			//export
			if (rootParent != null) {
				debug += "Export ";
				result = Object.Instantiate(self);
				result.previous = previous;
				if(autoAssignNextComponentOfPrevious)
					result.previous.next = result;
				result.name = self.name;
				AssetDatabase.AddObjectToAsset(result, rootParent);
				debug += " - New";
			}
			//Save to scene
			else
			{
				if (UnityEditor.AssetDatabase.IsSubAsset(self) || UnityEditor.AssetDatabase.IsMainAsset(self)) {
					result = Object.Instantiate(self);
					result.name = self.name;
					result.next = self.next;
					if (previous != null) {
						result.previous = previous;
						if (autoAssignNextComponentOfPrevious)
							result.previous.next = result;
					}
					result.position = self.position;
				} else
					result = self;
				debug += "Saved";
			}

			debug += " - "+ self.name;

#if DI_DEBUG
			Debug.Log(debug);
#endif

			if (result.next != null)
				result.next.SaveComponent(rootParent, result);
			return result;
#endif
		}
		#region DEPRECATED
		/* This method is advance save, old implementation which don't used anymore. I keep this for backup in the future.
		public static void _SaveComponent(this DIVisualComponent self, UnityEngine.Object rootParent, DIVisualComponent previous) {
			///The idea is, when save component, we need to check relation between self and rootParent.
			///The method of checking relation is
			///First : Check root Parent null or not? null : Saved in scene | not : Saved as file/ prefab
			///If not null then
			//////Check if self isSubAsset or not?
			//////If true, check MainAsset of self is equal with root parent? true : DO NOTHING | False : create a copy
			//////If false means self currently not saved in any file or prefab -> Assign to parent
			///If null
			//////Check if self isSubAsset or not?
			//////If true, create a copy
			//////if note, DO NOTHING
			///Saved to scene
#if UNITY_EDITOR
			//SEt the asset name
			self.name = string.IsNullOrEmpty(self.windowName) ? self.GetType().ToString() : self.windowName;
			self.name += " - " + DIVisualScriptingData.inspectRootField.Name;
			string debug = "";
			if (AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(self)) != null)
				debug = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(self)).ToString();
			if (string.IsNullOrEmpty(debug) || rootParent == null)
				debug = "Scene";

			var result = self;
			//Save as sub asset
			if (rootParent != null)
			{
				//If subasset,
				if (AssetDatabase.IsSubAsset(result))
				{
					//Check if self main asset is same as rootParent
					if (AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(result)) == rootParent || AssetDatabase.GetAssetPath(result) == AssetDatabase.GetAssetPath(rootParent))
					{
						//Save normally means DO NOTHING
						debug += " - Saved";
					}
					else {
						//Make a copy and add to root parent
						result = Object.Instantiate(result);
						result.previous = previous;
						result.previous.next = result;
						result.name = self.name;
						AssetDatabase.AddObjectToAsset(result, rootParent);
						debug += " - New";
					}
				}
				//otherwise, self is not member of anything or we can say saved in scene.
				else {
					//then add as subasset to rootparent
					AssetDatabase.AddObjectToAsset(result, rootParent);
					if (debug == "Scene")
						debug = rootParent.ToString();
					debug += " - Add";
				}
			}
			//Save to scene
			else {
				//if Sub Asset, create new one
				if (AssetDatabase.IsSubAsset(result))
				{
					result = Object.Instantiate(result);
					result.previous = previous;
					result.previous.next = result;
					result.name = self.name;
					debug += " - New";
				}
				else
					debug += " - Saved";

			}
			debug += " - " + result.name;
#if DI_DEBUG
			Debug.Log(debug);
#endif

			if (result.next != null) {
				result.next.SaveComponent(rootParent, result);
			}
#endif
		}*/
		#endregion


		public static void DrawConnectLine(this DIVisualComponent self, Vector2 origin, DIVisualComponent nextComponent)
		{
#if UNITY_EDITOR
			Vector2 fromPos = origin;
			Vector2 toPos = nextComponent.position.position + Vector2.right * nextComponent.position.width / 2;
			if (DIVisualScriptingData.bezierLine)
				UnityEditor.Handles.DrawBezier(fromPos, toPos, fromPos + Vector2.up * 50, toPos + Vector2.down * 50, Color.black, null, 2);
			else
			{
				int midLine = (int)((toPos.y - fromPos.y) / 2);
				if (midLine < 0)
					midLine = 20;
				Vector2 midPoint = fromPos + Vector2.up * midLine;
				Color _color = Handles.color;
				Handles.color = Color.black;
				Handles.DrawAAPolyLine(3, new Vector3[4] { fromPos, midPoint, new Vector2(toPos.x, midPoint.y), toPos });
				Handles.color = _color;

			}
			UnityEditor.Handles.Label(fromPos - Vector2.right * 7, UnityEditor.EditorGUIUtility.Load("radio on") as Texture);
			UnityEditor.Handles.Label(toPos - Vector2.right * 7 + Vector2.down * 12, UnityEditor.EditorGUIUtility.Load("radio on") as Texture);
#endif
		}
		public static void DrawConnectLine(this DIVisualComponent self, float percentageFromLeft, DIVisualComponent nextComponent)
		{
#if UNITY_EDITOR
			Vector2 fromPos = self.position.position + Vector2.right * self.position.width * percentageFromLeft + Vector2.up * self.position.height;
			Vector2 toPos = nextComponent.position.position + Vector2.right * nextComponent.position.width / 2;
			if(DIVisualScriptingData.bezierLine)
				UnityEditor.Handles.DrawBezier(fromPos, toPos, fromPos + Vector2.up * 50, toPos + Vector2.down * 50, Color.black, null, 2);
			else {
				int midLine = (int)((toPos.y - fromPos.y) / 2);
				if (midLine < 0)
					midLine = 20;
				Vector2 midPoint = fromPos + Vector2.up * midLine;
				Color _color = Handles.color;
				Handles.color = Color.black;
				Handles.DrawAAPolyLine(2, new Vector3[4] { fromPos, midPoint, new Vector2(toPos.x, midPoint.y), toPos });
				Handles.color = _color;
			}
			UnityEditor.Handles.Label(fromPos - Vector2.right * 7, UnityEditor.EditorGUIUtility.Load("radio on") as Texture);
			UnityEditor.Handles.Label(toPos - Vector2.right * 7 + Vector2.down * 12, UnityEditor.EditorGUIUtility.Load("radio on") as Texture);
#endif
		}

		/// <summary>
		/// Function to show component window
		/// </summary>
		/// <param name="component"></param>
		public static void ShowWindow(this DIVisualComponent component)
		{
#if UNITY_EDITOR
			UnityEditor.EditorGUI.BeginChangeCheck();
			UnityEditor.Undo.RecordObject(component, "Undo Visual Scripting");

			DIVisualScriptingData.currentWindowControl += 1;

			//Draw Window
			if (!component.isRemoveAction)
				component.position = GUILayout.Window(DIVisualScriptingData.currentWindowControl, component.position, component.BuildWindow, component.windowName, GUILayout.Width(200), GUILayout.Height(20));
			else
				component.position = GUILayout.Window(DIVisualScriptingData.currentWindowControl, component.position, component.RemoveAction, component.windowName, GUILayout.Width(200), GUILayout.Height(20));

			//normalize the value, because window only accept pixel, floating number will blur the content
			component.position = new Rect((int)component.position.x, (int)component.position.y, (int)component.position.width, (int)component.position.height);
			
			if (AssetDatabase.IsMainAsset((UnityEngine.Object)(DIVisualScriptingData.inspectRootField.GetValue(DIVisualScriptingData.inspectTarget)))
				||
				AssetDatabase.IsSubAsset((UnityEngine.Object)(DIVisualScriptingData.inspectRootField.GetValue(DIVisualScriptingData.inspectTarget))))
			{
				GUI.Box(new Rect(component.position.x - 4, component.position.y - 4, component.position.width + 8, component.position.height + 8), MakeTexture((int)component.position.width + 8, (int)component.position.height + 8, Color.red), GUIStyle.none);
			}
			#region DEPRECATED
			/* Draw Box when new component Added
			if (!Application.isPlaying && (DIVisualScriptingData.inspectRootField.GetValue(DIVisualScriptingData.inspectTarget) as DIRootComponent).saveState != DIRootComponent.SaveState.Scene)
			{
				//If not subasset, 
				if (!AssetDatabase.IsSubAsset(component) &&
					//and the inspectorRoot is sub asset, means this asset is child of prefab
					(AssetDatabase.IsSubAsset((DIVisualScriptingData.inspectRootField.GetValue(DIVisualScriptingData.inspectTarget) as DIRootComponent))
					//or if not main asset, that means prefab too
					|| AssetDatabase.IsMainAsset((DIVisualScriptingData.inspectRootField.GetValue(DIVisualScriptingData.inspectTarget) as DIRootComponent)))
					)
				{
					//check again if inspectRoot doesn't have next and inspectRoot is Main Asset
					if (AssetDatabase.IsMainAsset((DIVisualScriptingData.inspectRootField.GetValue(DIVisualScriptingData.inspectTarget) as DIRootComponent)) && (DIVisualScriptingData.inspectRootField.GetValue(DIVisualScriptingData.inspectTarget) as DIRootComponent) == component && component.next == null)
					{
						//Do not draw anything
					}
					else
						//then mark it with box, so we now if this component not saved yet
						GUI.Box(new Rect(component.position.x - 4, component.position.y - 4, component.position.width + 8, component.position.height + 8), MakeTexture((int)component.position.width + 8, (int)component.position.height + 8, Color.red), GUIStyle.none);
				}
			}*/
			#endregion
			/* TO DO : Debugging window when application.isplaying
			else if (isRunning) {
				//Draw blue rect for indicating current component is running
				GUI.Box(new Rect(_comp.position.x - 4, _comp.position.y - 4, _comp.position.width + 8, _comp.position.height + 8), DI.DIEditorHelper.MakeTexture((int)_comp.position.width + 8, (int)_comp.position.height + 8, Color.blue), GUIStyle.none);
			}*/


			//Create side button
			Rect removeBtnRect = new Rect(component.position.x + component.position.width, component.position.y, 15, 15);
			if (GUI.Button(removeBtnRect, DIVisualScriptingData.removeTex, GUIStyle.none))
			{
				//Show Popup yes or no
				component.isRemoveAction = true;
			}
			Rect minimizeBtnRect = new Rect(component.position.x - 15, component.position.y, 15, 15);
			if (GUI.Button(minimizeBtnRect, component.isMinimize ? DIVisualScriptingData.maximizeTex : DIVisualScriptingData.minimizeTex, GUIStyle.none))
			{
				//Show Popup yes or no
				component.isMinimize = !component.isMinimize;
			}

			//Show next window
			if (component.next != null)
			{
				component.next.ShowWindow();
			}

			//function can be overriding to draw custom things
			component.LateShowWindow();


			if (UnityEditor.EditorGUI.EndChangeCheck())
			{
				UnityEditor.EditorUtility.SetDirty(component);
			}
#endif
		}

		public static Texture2D MakeTexture(int width, int height, Color col, bool border = true)
		{
			Color[] pix = new Color[width * height];
			int currentIndex = 0;

			for (int j = 0; j < height; j++)
			{
				for (int i = 0; i < width; i++)
				{
					if (border && (i == 0 || i == width - 1 || j == 0 || j == height - 1))
						pix[currentIndex] = Color.black;
					else
						pix[currentIndex] = col;
					currentIndex += 1;
				}
			}

			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();
			return result;
		}

		public static void DrawField(FieldInfo _fieldInfo, DIVisualComponent comp, bool ignoreNotSupportedType )
		{
#if UNITY_EDITOR
			if (_fieldInfo.FieldType == typeof(int))
				DrawIntField(_fieldInfo, comp);
			else if (_fieldInfo.FieldType == typeof(float))
				DrawFloatField(_fieldInfo, comp);
			else if (_fieldInfo.FieldType == typeof(string))
				DrawStringField(_fieldInfo, comp);
			else if (_fieldInfo.FieldType == typeof(bool))
				DrawToggleField(_fieldInfo, comp);
			else if (_fieldInfo.FieldType.BaseType == typeof(System.Enum))
				DrawEnumPopup(_fieldInfo, comp);
			else if (_fieldInfo.FieldType == typeof(GameObject))
				DrawGameObjectField(_fieldInfo, comp);
			else if (_fieldInfo.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
				DrawObjectField(_fieldInfo, comp);
			/* TODO : Array or List Property Drawer
			else if (typeof(IList).IsAssignableFrom(_fieldInfo.FieldType)) {
				EditorGUILayout.HelpBox("Array are not supported", MessageType.Warning);
				DrawList(_fieldInfo, comp);
			}
			*/
			else
			{
				bool founded = false;
				foreach (DIAttributeData.VSCustomPropertyDrawerAttributeData att in DIAttributeData.Instance.propertyDrawers)
				{
					if (_fieldInfo.FieldType == att.att.type)
					{
						att.type.GetMethod("PropertyDrawer").Invoke(att.instance, new object[2] { _fieldInfo, comp });
						founded = true;
					}
				}
				if(!founded && !ignoreNotSupportedType)
					EditorGUILayout.HelpBox(_fieldInfo.FieldType + " : are not supported", MessageType.Warning);
			}
#endif
		}

		public static void DrawIntField(FieldInfo _fieldInt, DIVisualComponent comp)
		{
#if UNITY_EDITOR
			int result = EditorGUILayout.IntField(_fieldInt.Name, (int)_fieldInt.GetValue(comp));
			_fieldInt.SetValue(comp, result);
#endif
		}
		public static void DrawStringField(FieldInfo _field, DIVisualComponent comp)
		{
#if UNITY_EDITOR
			string result = EditorGUILayout.TextField(_field.Name, (string)_field.GetValue(comp));
			_field.SetValue(comp, result);
#endif
		}
		public static void DrawFloatField(FieldInfo _field, DIVisualComponent comp)
		{
#if UNITY_EDITOR
			float result = EditorGUILayout.FloatField(_field.Name, (float)_field.GetValue(comp));
			_field.SetValue(comp, result);
#endif
		}
		public static void DrawEnumPopup(FieldInfo _field, DIVisualComponent comp)
		{
#if UNITY_EDITOR
			var result = EditorGUILayout.EnumPopup(_field.Name, (System.Enum)_field.GetValue(comp));
			_field.SetValue(comp, result);
#endif
		}
		public static void DrawToggleField(FieldInfo _field, DIVisualComponent comp)
		{
#if UNITY_EDITOR
			bool result = EditorGUILayout.Toggle(_field.Name, (bool)_field.GetValue(comp));
			_field.SetValue(comp, result);
#endif
		}
		public static void DrawGameObjectField(FieldInfo _field, DIVisualComponent comp)
		{
#if UNITY_EDITOR
			GameObject result = EditorGUILayout.ObjectField(_field.Name, (GameObject)_field.GetValue(comp), typeof(GameObject), true) as GameObject;
			_field.SetValue(comp, result);
#endif
		}
		public static void DrawObjectField(FieldInfo _field, DIVisualComponent comp)
		{
#if UNITY_EDITOR
			UnityEngine.Object result = EditorGUILayout.ObjectField(_field.Name, (UnityEngine.Object)_field.GetValue(comp), _field.FieldType, true) as UnityEngine.Object;
			_field.SetValue(comp, result);
#endif
		}

		//TO DO : Supported List / array
		/*
		public static void DrawList(FieldInfo _field, DIVisualComponent comp)
		{
#if UNITY_EDITOR
			EditorGUILayout.LabelField(_field.Name, EditorStyles.boldLabel);
			var list = ((IList)_field.GetValue(comp));
			EGLBeginHorizontal();
			GUI.enabled = false;
			EditorGUILayout.IntField("Size", list.Count);
			GUI.enabled = true;
			if (GUILayout.Button("+"))
				list.Add(null);
			if (GUILayout.Button("-"))
				list.RemoveAt(list.Count - 1);
			EGLEndHorizontal();
			System.Type type = null;
			for (int i = 0; i < list.Count; i++) {
				if (type == null)
					type = list[i].GetType();
				if (type == typeof(int))
					DrawIntField(i.ToString(), (int)list[i]);
				else if (type == typeof(float)) 
					DrawFloatField(i.ToString(), (float)list[i]);
				else if (type == typeof(string))
					DrawStringField(i.ToString(), (string)list[i]);
				else if (type == typeof(bool))
					DrawToggleField(i.ToString(), (bool)list[i]);
				else if (type == typeof(System.Enum))
					DrawEnumPopup(i.ToString(), (System.Enum)list[i]);
				else if (type == typeof(GameObject))
					DrawGameObjectField(i.ToString(), (GameObject)list[i]);
				else if (type == typeof(Object))
					DrawObjectField(i.ToString(), (Object)list[i]);
				else {
					EditorGUILayout.HelpBox("Type : " + type.ToString() + " not Supported!", MessageType.Warning);
				}
			}
#endif
		}

		public static int DrawIntField(string label, int value)
		{
#if UNITY_EDITOR
			return EditorGUILayout.IntField(label, value);
#endif
		}
		public static string DrawStringField(string label, string value)
		{
#if UNITY_EDITOR
			return EditorGUILayout.TextField(label, value);
#endif
		}
		public static float DrawFloatField(string label, float value)
		{
#if UNITY_EDITOR
			return EditorGUILayout.FloatField(label, value);
#endif
		}
		public static System.Enum DrawEnumPopup(string label, System.Enum selected)
		{
#if UNITY_EDITOR
			return EditorGUILayout.EnumPopup(label, selected);
#endif
		}
		public static bool DrawToggleField(string label, bool value)
		{
#if UNITY_EDITOR
			return EditorGUILayout.Toggle(label, value);
#endif
		}
		public static GameObject DrawGameObjectField(string label, GameObject value)
		{
#if UNITY_EDITOR
			return EditorGUILayout.ObjectField(label, value, typeof(GameObject), true) as GameObject;
#endif
		}
		public static Object DrawObjectField(string label, Object value)
		{
#if UNITY_EDITOR
			return EditorGUILayout.ObjectField(label, value, typeof(UnityEngine.Object), true) as UnityEngine.Object;
#endif
		}*/

		public static FieldInfo[] GatherFieldInfos(DIVisualComponent comp)
		{
			return comp.GetType().GetFields(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
		}
	}
}
