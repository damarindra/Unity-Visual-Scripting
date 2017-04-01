using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DI.VisualScripting {
	public class DIVisualScriptingData {
		/// <summary>
		/// Root Component in Visual Scripting Window
		/// . To Communicate with non-editor script
		/// </summary>
		public static FieldInfo inspectRootField = null;
		/// <summary>
		/// Inspect target in Visual Scripting Window. Basically Inspect Target is MonoBehaviour at Selection GameObject
		/// . To Communicate with non-editor script
		/// </summary>
		public static IVisualScripting inspectTarget = null;


		//Event when any changes happen inside Window, will call the end of execution
		//Will set to null when the start of execution OnGUI
		//need to manually add with += operator if any changes happen
		public delegate void ExecuteAtTheEnd();
		public static ExecuteAtTheEnd atTheEnd;
		public static int currentWindowControl = 0;
		public static Texture minimizeTex, maximizeTex, removeTex;

		public static bool needToBackupGlobalVariable = false;


#if UNITY_EDITOR
		public static bool replaceActAsAppend {
			get {
				return EditorPrefs.GetBool("Replace Act As Append", true);
			}
			set {
				EditorPrefs.SetBool("Replace Act As Append", value);
			}
		}
		public static bool bezierLine {
			get {
				return EditorPrefs.GetBool("Bezier Line Visual Scripting", false);
			}
			set {
				EditorPrefs.SetBool("Bezier Line Visual Scripting", value);
			}
		}

		[PreferenceItem("Visual Scripting")]
		public static void PreferenceGUI() {
			GUILayout.Space(23);
			EditorGUILayout.LabelField("Saved File", EditorStyles.boldLabel);
			replaceActAsAppend = EditorGUILayout.Toggle(new GUIContent("Append File", "When saving window prompt show, and choose existing file. Old file not replaced with the new one, but the new one will appended in"), replaceActAsAppend);
			GUILayout.Space(15);
			EditorGUILayout.LabelField("Visual Window", EditorStyles.boldLabel);
			bezierLine = EditorGUILayout.Toggle(new GUIContent("Bezier Line", "Draw bezier line instead of straight line"), bezierLine);

		}
#endif
	}
}
