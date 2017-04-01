#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace DI.VisualScripting {
	public class DIGlobalVariableWindow : EditorWindow {

		protected static DIGlobalVariableWindow window = null;
		
		static Editor globalVarEditor = null;

		protected virtual void OnGUI() {
			EditorGUI.BeginChangeCheck();
			if (globalVarEditor == null)
				globalVarEditor = Editor.CreateEditor(DIGlobalVariable.Instance);

			globalVarEditor.OnInspectorGUI();

			if (EditorGUI.EndChangeCheck() && !Application.isPlaying) {
				DIGlobalVariable.Instance.Backup();
			}
		}
		
		public static bool restored = false;
		void OnInspectorUpdate() {
			Repaint();
			if (DIGlobalVariableWindow.restored) {
				UpdateView();
				DIGlobalVariableWindow.restored = false;
			}
		}
		
		public void UpdateView()
		{
			EditorUtility.SetDirty(DIGlobalVariable.Instance);
			globalVarEditor = Editor.CreateEditor(DIGlobalVariable.Instance);
			Repaint();
			
		}
	}
}
#endif