using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DI.VisualScripting {
	[ExecuteInEditMode()]
	public class DIVisualScripting : MonoBehaviour, IVisualScripting {
		//[HideInInspector]
		[SerializeField]
		private string vName = "Visual Scripting";
		public DIRootComponent start, update;

		public MonoBehaviour getMono
		{
			get
			{
				return this;
			}
		}

		public string visualScriptingName
		{
			get
			{
				return vName;
			}
		}

		public DIRootComponent[] rootComponents
		{
			get
			{
				return new DIRootComponent[2] { start, update };
			}

			set
			{
				if (value == null) {
					start = null;
					update = null;
					return;
				}
				if (value.Length > 0) {
					start = value[0];
				}
				if (value.Length > 1)
					update = value[1];
			}
		}

		void Start() {
			if (!Application.isPlaying)
				return;
			if(start != null)
				start.Do();
		}
		void Update()
		{
			if (!Application.isPlaying)
				return;
			if (update != null)
				update.Do();
		}

		void OnDestroy() {
			if (!Application.isPlaying)
			{
#if UNITY_EDITOR
				for (int i = 0; i < rootComponents.Length; i++)
				{
					if (rootComponents[i] != null)
					{
						if (AssetDatabase.IsMainAsset(rootComponents[i]) || AssetDatabase.IsSubAsset(rootComponents[i]))
						{
							//DO NOTHING, BECAUSE ROOT IS ASSET FILE
						}
						else
						{
							//DELETE
#if DI_DEBUG
							Debug.Log("Delete Root : " + rootComponents[i].name + " at Scene");
#endif
							rootComponents[i].FindChildAndDestroy();
						}
					}
				}
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
#endif
			}
		}
		
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(DIVisualScripting), true)]
	public class DIVisualScriptingEditor : Editor {
		//To Do : Auto Delete root when component removed
		
		DIVisualComponent[] rootBackupBeforeDeleted;
		
		void OnDestroy() {
			if (Application.isPlaying || rootBackupBeforeDeleted == null)
				return;
			
			if ((DIVisualScripting)target == null)
			{
				for (int i = 0; i < rootBackupBeforeDeleted.Length; i++) {
					if (rootBackupBeforeDeleted[i] != null) {
						if (AssetDatabase.IsMainAsset(rootBackupBeforeDeleted[i]) || AssetDatabase.IsSubAsset(rootBackupBeforeDeleted[i]))
						{
							//DO NOTHING, BECAUSE ROOT IS ASSET FILE
						}
						else {
							//DELETE
	#if DI_DEBUG
							Debug.Log("Delete Root : " + rootBackupBeforeDeleted[i].name + " at Scene");
	#endif
							rootBackupBeforeDeleted[i].FindChildAndDestroy();
						}
					}
				}
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			rootBackupBeforeDeleted = ((DIVisualScripting)target).rootComponents;
		}
	}
#endif
}
