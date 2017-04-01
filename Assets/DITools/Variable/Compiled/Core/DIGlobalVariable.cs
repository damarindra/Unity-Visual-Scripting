using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

namespace DI.VisualScripting
{
	public class DIGlobalVariable : ScriptableObject {
		public List<DIVariableInt> intContainer = new List<DIVariableInt>();
		public List<DIVariableFloat> floatContainer = new List<DIVariableFloat>();
		public List<DIVariableString> stringContainer = new List<DIVariableString>();
		public List<DIVariableBool> boolContainer = new List<DIVariableBool>();
		public List<DIVariableVector2> vector2Container = new List<DIVariableVector2>();
		public List<DIVariableVector3> vector3Container = new List<DIVariableVector3>();
		public List<DIVariableGameObject> gameObjectContainer = new List<DIVariableGameObject>();
		public List<DIVariableObject> objectContainer = new List<DIVariableObject>();


		private List<DIVariableInt> _b_intContainer = new List<DIVariableInt>();
		private List<DIVariableFloat> _b_floatContainer = new List<DIVariableFloat>();
		private List<DIVariableString> _b_stringContainer = new List<DIVariableString>();
		private List<DIVariableBool> _b_boolContainer = new List<DIVariableBool>();
		private List<DIVariableVector2> _b_vector2Container = new List<DIVariableVector2>();
		private List<DIVariableVector3> _b_vector3Container = new List<DIVariableVector3>();
		private List<DIVariableGameObject> _b_gameObjectContainer = new List<DIVariableGameObject>();
		private List<DIVariableObject> _b_objectContainer = new List<DIVariableObject>();

		static string path = "Assets/DITools/Resources";
		static string folderName = "GamePreferences";
		
		public static DIGlobalVariable Instance {
			get {
				if (instance == null) {
#if UNITY_EDITOR
					if (!AssetDatabase.IsValidFolder(path + "/" + folderName))
					{
						AssetDatabase.CreateFolder(path, folderName);
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
					}
					instance = AssetDatabase.LoadAssetAtPath(path + "/" + folderName + "/GlobalVariable.asset", typeof(DIGlobalVariable)) as DIGlobalVariable;
					if (instance == null) {
						//Create
						instance = CreateInstance(typeof(DIGlobalVariable)) as DIGlobalVariable;
						AssetDatabase.CreateAsset(instance, path + "/" + folderName + "/GlobalVariable.asset");
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
					}
#endif
				}
				return instance;
			}
		}
		static DIGlobalVariable instance;

		public void Backup() {
			_b_intContainer = new List<DIVariableInt>(intContainer);
			_b_floatContainer = new List<DIVariableFloat>(floatContainer);
			_b_stringContainer = new List<DIVariableString>(stringContainer);
			_b_boolContainer = new List<DIVariableBool>(boolContainer);
			_b_vector2Container = new List<DIVariableVector2>(vector2Container);
			_b_vector3Container = new List<DIVariableVector3>(vector3Container);
			_b_gameObjectContainer = new List<DIVariableGameObject>(gameObjectContainer);
			_b_objectContainer= new List<DIVariableObject>(objectContainer);
			Debug.Log(_b_intContainer[0].value);
		}
		public void Restore()
		{
			intContainer = new List<DIVariableInt>(_b_intContainer);
			floatContainer = new List<DIVariableFloat>(_b_floatContainer);
			stringContainer = new List<DIVariableString>(_b_stringContainer);
			boolContainer = new List<DIVariableBool>(_b_boolContainer);
			vector2Container = new List<DIVariableVector2>(_b_vector2Container);
			vector3Container = new List<DIVariableVector3>(_b_vector3Container);
			gameObjectContainer = new List<DIVariableGameObject>(_b_gameObjectContainer);
			objectContainer = new List<DIVariableObject>(_b_objectContainer);
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(DIGlobalVariable), true)]
	public class DIGlobalVariableEditor : Editor {

		ReorderableList intList, floatList, stringList, boolList, vector2List, vector3List, gameObjectList, objectList;

		void OnEnable() {
			SetupList(ref intList, serializedObject, serializedObject.FindProperty("intContainer"),
				 ((Rect rect, int index, bool isActive, bool isFocused) => {
					 SerializedProperty _sProp = serializedObject.FindProperty("intContainer");
					 GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
					 labelStyle.normal = EditorStyles.label.normal;
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue, labelStyle);
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").intValue = EditorGUI.IntField(new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").intValue);
					 serializedObject.ApplyModifiedProperties();
				 })
				);

			SetupList(ref floatList, serializedObject, serializedObject.FindProperty("floatContainer"),
				 ((Rect rect, int index, bool isActive, bool isFocused) => {
					 SerializedProperty _sProp = serializedObject.FindProperty("floatContainer");
					 GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
					 labelStyle.normal = EditorStyles.label.normal;
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue, labelStyle);
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").floatValue = EditorGUI.FloatField(new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").floatValue);
					 serializedObject.ApplyModifiedProperties();
				 })
				);

			SetupList(ref stringList, serializedObject, serializedObject.FindProperty("stringContainer"),
				 ((Rect rect, int index, bool isActive, bool isFocused) => {
					 SerializedProperty _sProp = serializedObject.FindProperty("stringContainer");
					 GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
					 labelStyle.normal = EditorStyles.label.normal;
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue, labelStyle);
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").stringValue = EditorGUI.TextField(new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").stringValue);
					 serializedObject.ApplyModifiedProperties();
				 })
				);

			SetupList(ref boolList, serializedObject, serializedObject.FindProperty("boolContainer"),
				 ((Rect rect, int index, bool isActive, bool isFocused) => {
					 SerializedProperty _sProp = serializedObject.FindProperty("boolContainer");
					 GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
					 labelStyle.normal = EditorStyles.label.normal;
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue, labelStyle);
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").boolValue = EditorGUI.Toggle(new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").boolValue);
					 serializedObject.ApplyModifiedProperties();
				 })
				);

			SetupList(ref vector2List, serializedObject, serializedObject.FindProperty("vector2Container"),
				 ((Rect rect, int index, bool isActive, bool isFocused) => {
					 SerializedProperty _sProp = serializedObject.FindProperty("vector2Container");
					 GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
					 labelStyle.normal = EditorStyles.label.normal;
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue, labelStyle);
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").vector2Value = EditorGUI.Vector2Field(new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), "", _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").vector2Value);
					 serializedObject.ApplyModifiedProperties();
				 })
				);

			SetupList(ref vector3List, serializedObject, serializedObject.FindProperty("vector3Container"),
				 ((Rect rect, int index, bool isActive, bool isFocused) => {
					 SerializedProperty _sProp = serializedObject.FindProperty("vector3Container");
					 GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
					 labelStyle.normal = EditorStyles.label.normal;
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue, labelStyle);
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").vector3Value = EditorGUI.Vector3Field(new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), "", _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").vector3Value);
					 serializedObject.ApplyModifiedProperties();
				 })
				);

			SetupList(ref gameObjectList, serializedObject, serializedObject.FindProperty("gameObjectContainer"),
				 ((Rect rect, int index, bool isActive, bool isFocused) => {
					 SerializedProperty _sProp = serializedObject.FindProperty("gameObjectContainer");
					 GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
					 labelStyle.normal = EditorStyles.label.normal;
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue, labelStyle);
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").objectReferenceValue = EditorGUI.ObjectField(new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").objectReferenceValue, typeof(GameObject), true);
					 serializedObject.ApplyModifiedProperties();
				 })
				);

			SetupList(ref objectList, serializedObject, serializedObject.FindProperty("objectContainer"),
				 ((Rect rect, int index, bool isActive, bool isFocused) => {
					 SerializedProperty _sProp = serializedObject.FindProperty("objectContainer");
					 GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
					 labelStyle.normal = EditorStyles.label.normal;
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("varName").stringValue, labelStyle);
					 _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").objectReferenceValue = EditorGUI.ObjectField(new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), _sProp.GetArrayElementAtIndex(index).FindPropertyRelative("value").objectReferenceValue, typeof(UnityEngine.Object), true);
					 serializedObject.ApplyModifiedProperties();
				 })
				);
		}

		void SetupList(ref ReorderableList list, SerializedObject _sObj, SerializedProperty _sProp, ReorderableList.ElementCallbackDelegate element) {
			list = new ReorderableList(_sObj, _sProp, true, true, true, true);
			list.drawHeaderCallback = ((Rect rect) => {
				EditorGUI.LabelField(rect, _sProp.displayName);
			});
			list.drawElementCallback = element;
			list.onAddCallback = ((ReorderableList l) =>
			{
				l.serializedProperty.InsertArrayElementAtIndex(l.serializedProperty.arraySize);
				l.serializedProperty.GetArrayElementAtIndex(l.serializedProperty.arraySize - 1).FindPropertyRelative("varName").stringValue = "Variable Name";
				serializedObject.ApplyModifiedProperties();
			});
			list.onRemoveCallback = ((ReorderableList l) =>
			{
				l.serializedProperty.DeleteArrayElementAtIndex(l.index);
				serializedObject.ApplyModifiedProperties();
			});
		}

		public override void OnInspectorGUI()
		{
			if (Selection.activeObject == target)
			{
				EditorGUILayout.HelpBox("Sorry, you are not allowed to edit from Asset file", MessageType.Warning);
				return;
			}
			EditorGUILayout.Space();
			EditorPrefs.SetInt("SelectedVariableMenu",
				EditorGUILayout.Popup("Variable Type", EditorPrefs.GetInt("SelectedVariableMenu", 0), new string[8] { "Int", "Float", "String", "Bool", "Vector2", "Vector3", "GameObject", "Object" })
				);
			if (Application.isPlaying)
				GUI.enabled = false;
			if (EditorPrefs.GetInt("SelectedVariableMenu") == 0)
				intList.DoLayoutList();
			else if (EditorPrefs.GetInt("SelectedVariableMenu") == 1)
				floatList.DoLayoutList();
			else if (EditorPrefs.GetInt("SelectedVariableMenu") == 2)
				stringList.DoLayoutList();
			else if (EditorPrefs.GetInt("SelectedVariableMenu") == 3)
				boolList.DoLayoutList();
			else if (EditorPrefs.GetInt("SelectedVariableMenu") == 4)
				vector2List.DoLayoutList();
			else if (EditorPrefs.GetInt("SelectedVariableMenu") == 5)
				vector3List.DoLayoutList();
			else if (EditorPrefs.GetInt("SelectedVariableMenu") == 6)
				gameObjectList.DoLayoutList();
			else if (EditorPrefs.GetInt("SelectedVariableMenu") == 7)
				objectList.DoLayoutList();
			GUI.enabled = true;
			//base.OnInspectorGUI();
		}
	}
#endif
}
