#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace DI.VisualScripting {
	public class DIVisualScriptingWindow : EditorWindow {
#if UNITY_EDITOR
		protected static DIVisualScriptingWindow window = null;

		public DIRootComponent inspectRoot {
			get {
				if (DIVisualScriptingData.inspectTarget != null) {
					try
					{
						return (DIRootComponent)DIVisualScriptingData.inspectRootField.GetValue(DIVisualScriptingData.inspectTarget);
					}
					catch {
						DIVisualScriptingData.inspectTarget = Selection.activeGameObject.GetComponent<IVisualScripting>();
						if (DIVisualScriptingData.inspectTarget != null) {
							GatherRootInfo();
							if (fieldInfoRoot.Count > 0) {
								DIVisualScriptingData.inspectRootField = fieldInfoRoot[0];
								return (DIRootComponent)DIVisualScriptingData.inspectRootField.GetValue(DIVisualScriptingData.inspectTarget);
							}
						}
						return null;
					}
				}
				else
					return null; }
		}
		[SerializeField]
		private int inspectRootIndex = 0;
		private List<FieldInfo> fieldInfoRoot = new List<FieldInfo>();

		public Vector2 scrollPosition;
		Vector2 lastFrameDragPosition;

		Rect effectiveArea;

		public Rect getEffectiveArea { get { return effectiveArea; } }

		GUIContent saveIcon = new GUIContent();
		GUIContent saveAsIcon = new GUIContent();
		Texture backgroundTexture;

		public static DIVisualScriptingWindow Window
		{
			get
			{
				if (window != null)
					return window;
				window = EditorWindow.GetWindow(typeof(DIVisualScriptingWindow), false, "Visual Scripting") as DIVisualScriptingWindow;
				return window;
			}
		}

		public static void RepaintWindow() {
			Window.Repaint();
		}

		public static Rect GetSize {
			get { return Window.position; }
		}

		public static bool isMouseInsideWindow() {
			return GetSize.Contains(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
		}

		void OnInspectorUpdate() {
			Repaint();
		}
		private float oriLabelWidth;

		protected virtual void OnGUI() {
			DIVisualScriptingData.atTheEnd = null;

			EditorGUI.BeginChangeCheck();
			SetupTex();
			GetInspectTargetIfNeeded();
			if (DIVisualScriptingData.inspectTarget == null || EditorApplication.isCompiling)
				GUI.enabled = false;
			if (DIVisualScriptingData.inspectTarget != null)
			{
				if (fieldInfoRoot.Count == 0)
					GatherRootInfo();
			}

			oriLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 100;
			DIVisualScriptingData.currentWindowControl = 0;

			//TO-DO : add color to window
			//ColorizeWindow();

			EditorGUILayout.LabelField("", EditorStyles.toolbar);
			Rect toolbarRect = GUILayoutUtility.GetLastRect();

			TryToCreateToolbarButton(toolbarRect);

			if (effectiveArea == new Rect())
				effectiveArea = new Rect(0, toolbarRect.y + toolbarRect.height, GetSize.width + 300, GetSize.height + 300);

			if (inspectRoot == null && DIVisualScriptingData.inspectTarget != null)
				CreateBigText("Double Click To Generate Root");
			else if (DIVisualScriptingData.inspectTarget != null)
				CreateBigText(DIVisualScriptingData.inspectRootField.Name);

			scrollPosition = GUI.BeginScrollView(new Rect(0, toolbarRect.y + toolbarRect.height, GetSize.width, GetSize.height - toolbarRect.height), scrollPosition, effectiveArea);

			if (DIVisualScriptingData.inspectTarget != null)
			{
				BeginWindows();
				Undo.RecordObject(DIVisualScriptingData.inspectTarget.getMono, "Undo Visual Scripting");

				if (inspectRoot != null) {
					inspectRoot.ShowWindow();
				}
				else
				{
					DoubleClickGenerateRoot();
				}
				EndWindows();
			}
			GUI.EndScrollView();

			MiddleMouseDrag();

			EditorGUIUtility.labelWidth = oriLabelWidth;

			if (DIVisualScriptingData.atTheEnd != null) {
				DIVisualScriptingData.atTheEnd();
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			if (EditorGUI.EndChangeCheck()) {
				if (DIVisualScriptingData.inspectTarget != null) {
					EditorUtility.SetDirty(DIVisualScriptingData.inspectTarget.getMono);
				}
			}
			GUI.enabled = true;

		}

		//========================================================================================================================================
		//CHANGES START HERE
		//========================================================================================================================================

		//MIGRATE ALL TO HELPER
		void SetupTex()
		{
			if (DIVisualScriptingData.minimizeTex == null)
				DIVisualScriptingData.minimizeTex = AssetDatabase.LoadAssetAtPath("Assets/DITools/Resources/Images/minimize.png", typeof(Texture)) as Texture;
			if (DIVisualScriptingData.maximizeTex == null)
				DIVisualScriptingData.maximizeTex = AssetDatabase.LoadAssetAtPath("Assets/DITools/Resources/Images/maximize.png", typeof(Texture)) as Texture;
			if (DIVisualScriptingData.removeTex == null)
				DIVisualScriptingData.removeTex = AssetDatabase.LoadAssetAtPath("Assets/DITools/Resources/Images/remove.png", typeof(Texture)) as Texture;
		}

		//========================================================================================================================================
		//CHANGES END HERE
		//========================================================================================================================================

		//TO-DO : add color to window
		void ColorizeWindow() {
			Color backgroundColor = new Color32(120, 120, 120, 255);
			Rect position = GetSize;
			position.position = Vector2.zero;
			if (backgroundTexture == null || (backgroundTexture.height != (int)position.height && backgroundTexture.width != (int)position.width))
				backgroundTexture = DIComponentHelper.MakeTexture((int)position.width, (int)position.height, backgroundColor, false);
			GUI.DrawTexture(position, backgroundTexture);
		}

		void CreateBigText(string text) {
			int oriFontSize = EditorStyles.label.fontSize;
			EditorStyles.label.fontSize = 30;
			EditorStyles.label.fontStyle = FontStyle.Bold;
			Color oriColor = EditorStyles.label.normal.textColor;
			EditorStyles.label.normal.textColor = new Color(.4f, .4f, .4f, .7f);
			Vector2 size = EditorStyles.label.CalcSize(new GUIContent(text));
			EditorGUI.LabelField(new Rect(new Vector2(50, 50), size), text);
			EditorStyles.label.fontStyle = FontStyle.Normal;
			EditorStyles.label.fontSize = oriFontSize;
			EditorStyles.label.normal.textColor = oriColor;
		}

		void DoubleClickGenerateRoot() {
			if (Event.current.clickCount >= 2) {
				DIVisualScriptingData.inspectRootField.SetValue(DIVisualScriptingData.inspectTarget, DIVisualComponent.CreateInstance<DIRootComponent>() as DIRootComponent);
				inspectRoot.position.position = Event.current.mousePosition;
			}
		}


		void TryToCreateToolbarButton(Rect toolbarRect)
		{
			if (saveIcon.image == null)
			{
				saveIcon = new GUIContent(AssetDatabase.LoadAssetAtPath("Assets/DITools/Resources/Images/saveIconBW.png", typeof(Texture)) as Texture, "Save Visual Scripting to Scene");
			}
			Vector2 saveIconSize = EditorStyles.toolbarButton.CalcSize(saveAsIcon);

			//inspect selection				
			if (Selection.activeGameObject != null)
			{
				Rect inspectSelectionRect = new Rect(toolbarRect.x + toolbarRect.width - 400, toolbarRect.y, 250, toolbarRect.height);
				if (inspectSelectionRect.x < toolbarRect.x + saveIconSize.x * 2)
				{
					inspectSelectionRect.x = toolbarRect.x + saveIconSize.x * 2;
					inspectSelectionRect.width = toolbarRect.width - 150 - saveIconSize.x * 2;
				}
				var inspects = Selection.activeGameObject.GetComponents<IVisualScripting>().ToList();
				var inspectNames = inspects.Select(i => i.visualScriptingName).ToList();
				for (int i = 0; i < inspectNames.Count; i++)
				{
					inspectNames[i] = "(" + i.ToString() + ") " + inspectNames[i];
				}
				/* alternative
				 * 
				List<string> inspectNames = new List<string>();
				for (int i = 0; i < inspects.Count; i++) {
					var strings = inspects[i].getMono.ToString().Split(new char[1] { '.' });
					string result = strings[strings.Length - 1];
					result = result.Remove(result.Length - 1);
					inspectNames.Add(result);
				}
				 */
				int inspectidx = 0;
				try
				{
					inspectidx = inspects.IndexOf(DIVisualScriptingData.inspectTarget);
					inspectidx = EditorGUI.Popup(inspectSelectionRect, inspectidx, inspectNames.ToArray(), EditorStyles.toolbarPopup);
					if (DIVisualScriptingData.inspectTarget != inspects[inspectidx]) {
						DIVisualScriptingData.inspectTarget = inspects[inspectidx];
						GatherRootInfo();

					}
				}
				catch
				{
					if (inspects.Count() == 0)
						DIVisualScriptingData.inspectTarget = null;
					else
						DIVisualScriptingData.inspectTarget = inspects[0];
				}

				//if (inspectRoot != null)
					//Draw Save State
				//	EditorGUI.LabelField(new Rect(inspectSelectionRect.x - 200, inspectSelectionRect.y, 200, inspectSelectionRect.height), "Saved to " + inspectRoot.saveState.ToString(), EditorStyles.toolbarButton);
			}
			else
				DIVisualScriptingData.inspectTarget = null;

			if (DIVisualScriptingData.inspectTarget != null) {
				//root selection
				if (DIVisualScriptingData.inspectRootField == null && fieldInfoRoot.Count != 0)
				{
					GatherRootInfo();
					if (inspectRootIndex >= fieldInfoRoot.Count)
						inspectRootIndex = fieldInfoRoot.Count - 1;
					if (inspectRootIndex < 0)
						inspectRootIndex = 0;
					DIVisualScriptingData.inspectRootField = fieldInfoRoot[inspectRootIndex];
				}
				if (DIVisualScriptingData.inspectRootField != null && inspectRootIndex >= 0)
				{
					Rect rootSelectionRect = new Rect(toolbarRect.x + toolbarRect.width - 150, toolbarRect.y, 150, toolbarRect.height);
					var fieldNames = fieldInfoRoot.Select(i => i.Name);
					if (inspectRootIndex >= fieldNames.Count())
						inspectRootIndex = fieldNames.Count() - 1;

					inspectRootIndex = EditorGUI.Popup(rootSelectionRect, inspectRootIndex, fieldNames.ToArray(), EditorStyles.toolbarPopup);
					DIVisualScriptingData.inspectRootField = fieldInfoRoot[inspectRootIndex];
				}

			}
			if (saveIcon.image != null)
			{
				if (GUI.Button(new Rect(toolbarRect.position + Vector2.right, saveIconSize), saveIcon, EditorStyles.toolbarButton))
				{
					inspectRoot.SaveRoot();
				}
			}

			if (saveAsIcon.image == null)
			{
				saveAsIcon = new GUIContent(AssetDatabase.LoadAssetAtPath("Assets/DITools/Resources/Images/saveAsIcon.png", typeof(Texture)) as Texture, "Export");
			}

			if (DIVisualScriptingData.inspectTarget == null)
				GUI.enabled = false;
			if (saveAsIcon.image != null)
			{
				if (GUI.Button(new Rect(toolbarRect.position + Vector2.right * saveIconSize.x , saveIconSize), saveAsIcon, EditorStyles.toolbarButton))
				{
					inspectRoot.Export();
				}
			}


			#region DEPRECATED
			/*
			//Save asset, using last save state
			if (saveIcon.image != null)
			{
				if (GUI.Button(new Rect(toolbarRect.position + Vector2.right, saveIconSize), saveIcon, EditorStyles.toolbarButton))
				{
					if (inspectRoot.saveState == DIRootComponent.SaveState.Scene)
					{
						//Save To Scene
						inspectRoot.SaveRoot(DIRootComponent.SaveState.Scene);
					}
					else if (inspectRoot.saveState == DIRootComponent.SaveState.File)
					{
						//Save to File
						inspectRoot.SaveRoot(DIRootComponent.SaveState.File);
					}
					else if (inspectRoot.saveState == DIRootComponent.SaveState.Prefab) {
						//Save to Prefab
						inspectRoot.SaveRoot(DIRootComponent.SaveState.Prefab);
					}
				}
			}

			//Save To Scene
			if (saveToScene.image == null)
				saveToScene = new GUIContent(EditorGUIUtility.FindTexture("SceneAsset Icon"), "Save Asset to Scene");
			if (saveToScene.image != null)
			{
				if (GUI.Button(new Rect(toolbarRect.position + Vector2.right * saveIconSize.x, saveIconSize), saveToScene, EditorStyles.toolbarButton))
				{
					inspectRoot.SaveRoot(DIRootComponent.SaveState.Scene);
				}
			}

			//Save to prefab
			if (saveToPrefabIcon.image == null)
				saveToPrefabIcon = new GUIContent(EditorGUIUtility.FindTexture("PrefabNormal Icon"), "Save Asset to prefab");
			if (saveToPrefabIcon.image != null) {
				//try to get path prefab
				if (GUI.Button(new Rect(toolbarRect.position + Vector2.right * saveIconSize.x * 3, saveIconSize), saveToPrefabIcon, EditorStyles.toolbarButton)) {
					if (DIVisualScriptingData.inspectTarget != null)
					{
						inspectRoot.SaveRoot(DIRootComponent.SaveState.Prefab);
					}
				}
			}

			//Save as asset
			if (saveAsIcon.image == null) {
				saveAsIcon = new GUIContent(AssetDatabase.LoadAssetAtPath("Assets/DITools/Resources/Images/saveAsIcon.png", typeof(Texture)) as Texture, "Save Asset as File");
			}

			if (DIVisualScriptingData.inspectTarget == null)
				GUI.enabled = false;
			if (saveAsIcon.image != null)
			{
				if (GUI.Button(new Rect(toolbarRect.position + Vector2.right * saveIconSize.x * 2, saveIconSize), saveAsIcon, EditorStyles.toolbarButton))
				{
					inspectRoot.SaveRoot(DIRootComponent.SaveState.File);
				}
			}
			*/
			#endregion

			GUI.enabled = true;
		}
		/// <summary>
		/// Mouse Drag handler
		/// </summary>
		void MiddleMouseDrag() {
			Event e = Event.current;
			if (e.isMouse && e.button == 2) {
				if (e.type == EventType.MouseDown) {
					lastFrameDragPosition = e.mousePosition;
					e.Use();
				}
				else if (e.type == EventType.MouseDrag) {
					Vector2 movement = lastFrameDragPosition - e.mousePosition;
					movement = new Vector2((int)movement.x, (int)movement.y);

					scrollPosition += movement;

					//Controll extends
					if (scrollPosition.x <= 0 && movement.x < 0) {
						effectiveArea.x += movement.x;
						effectiveArea.width += Mathf.Abs(movement.x);
					}
					if (scrollPosition.y <= 0 && movement.y < 0) {
						effectiveArea.y += movement.y;
						effectiveArea.height += Mathf.Abs(movement.y);
					}
					if (scrollPosition.x + GetSize.width > effectiveArea.width)
					{
						effectiveArea.width += Mathf.Abs(movement.x);
					}
					if (scrollPosition.y + GetSize.height > effectiveArea.height)
					{
						effectiveArea.height += Mathf.Abs(movement.y);
					}

					lastFrameDragPosition = e.mousePosition;
					e.Use();
				}
				else if (e.type == EventType.MouseUp) {

					e.Use();
				}
			}
		}

		void GetInspectTargetIfNeeded() {
			if (DIVisualScriptingData.inspectTarget == null) {
				if (Selection.activeGameObject)
					DIVisualScriptingData.inspectTarget = Selection.activeGameObject.GetComponent<IVisualScripting>();
				else
					DIVisualScriptingData.inspectTarget = null;
			}
		}
		void GatherRootInfo()
		{
			var fields = DIVisualScriptingData.inspectTarget.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			fieldInfoRoot.Clear();
			foreach (FieldInfo field in fields)
			{
				if (field.FieldType == typeof(DIRootComponent))
					fieldInfoRoot.Add(field);
			}
			
		}
#endif
	}
}
#endif