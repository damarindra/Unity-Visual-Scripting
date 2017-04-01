using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DI.VisualScripting {
	public class DIRootComponent : DIVisualComponent {

		protected override void WhatDo()
		{
			Finish();
		}

		public override string windowName
		{
			get
			{
				return "Root";
			}
		}

#if UNITY_EDITOR
		public override void BuildWindow()
		{
			withoutComponentType = true;
			isRoot = true;
		}

		public DIRootComponent DuplicateAndSaveToScene() {
			DIRootComponent result = Instantiate(this);
			result.name = name;
			result.next = next;
			result.position = position;
			next.SaveComponent(null, result);
			if (UnityEditor.PrefabUtility.GetPrefabObject(DIVisualScriptingData.inspectTarget.getMono.gameObject) != null)
			{
				//DO NOTHING
				//UnityEditor.PrefabUtility.DisconnectPrefabInstance(DIVisualScriptingData.inspectTarget.getMono.gameObject);
				UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(DIVisualScriptingData.inspectTarget.getMono.gameObject);
			}
			return result;
		}

		public void SaveRoot() {
			if (UnityEditor.AssetDatabase.IsSubAsset(this) || UnityEditor.AssetDatabase.IsMainAsset(this))
			{
				if (UnityEditor.PrefabUtility.GetPrefabObject(DIVisualScriptingData.inspectTarget.getMono.gameObject) != null)
				{
					//DO NOTHING
					//disconnecting the main assets from the game object.
					//UnityEditor.PrefabUtility.DisconnectPrefabInstance(DIVisualScriptingData.inspectTarget.getMono.gameObject);
					UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(DIVisualScriptingData.inspectTarget.getMono.gameObject);
				}
				DIVisualScriptingData.inspectRootField.SetValue(DIVisualScriptingData.inspectTarget, DuplicateAndSaveToScene());
			}
			else
				next.SaveComponent(null, this);
		}
		public void Export() {
			string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save Visual Scripting", "VisualScript", "asset", "Create Visual Scripting Asset");
			Object backupFile = null;
			if (!string.IsNullOrEmpty(path))
			{
				//Check if assets is exists
				if (AssetDatabase.LoadAssetAtPath(path, typeof(DIVisualComponent)) != null) {
					//I want to rename the file, but I need to check if the target rename file doesn't exist. I need to loop check for it
					int iterationCandidate = 1;
					string pathCandidate = path;
					pathCandidate = pathCandidate.Insert(pathCandidate.Length - 6, iterationCandidate.ToString());
					while (AssetDatabase.LoadAssetAtPath(pathCandidate, typeof(Object)) != null){
						iterationCandidate += 1;
						pathCandidate = pathCandidate.Insert(pathCandidate.Length - 6, iterationCandidate.ToString());
					}

					//Backup First, by changing the name
					AssetDatabase.RenameAsset(path, AssetDatabase.LoadAssetAtPath(path, typeof(DIVisualComponent)).name + iterationCandidate.ToString());
					backupFile = AssetDatabase.LoadAssetAtPath(pathCandidate, typeof(DIVisualComponent));
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
				}
				DIRootComponent result = Instantiate(this);
				result.name = name;
				UnityEditor.AssetDatabase.CreateAsset(result, path);
				/*
				if (!DIVisualScriptingData.replaceActAsAppend)
				{
					UnityEditor.AssetDatabase.DeleteAsset(path);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					UnityEditor.AssetDatabase.CreateAsset(result, path);
				}
				else
				UnityEditor.AssetDatabase.AddObjectToAsset(result, path);*/
				result.next = next;
				if (next != null)
					next.SaveComponent(result, result);

			}

			//Delete backup file
			if (backupFile != null)
				Debug.Log(AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(backupFile)));
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
		}

		#region DEPRECATED

		/*
		public enum SaveState { Scene, File, Prefab }
		public SaveState saveState = SaveState.Scene;
		/// <summary>
		/// Save Root. If saveMode not equal with scene.
		/// </summary>
		/// <param name="saveMode">Save Mode, scene / file / prefab</param>
		public void SaveRoot(SaveState saveMode) {
			name = string.IsNullOrEmpty(windowName) ? GetType().ToString() : windowName;
			name += " - " + DIVisualScriptingData.inspectRootField.Name;
			bool complete = false;
			DIRootComponent result = this;
			if (saveMode != saveState)
			{
				if (saveMode == SaveState.Scene)
				{
					//Instantiate not working when saving to scene, instead of instantiating, we can create a new one, since root is simple
					result = Instantiate(this);
					result.name = name;
					result.next = next;
					result.position = position;
					next.SaveComponent(null, result);
					DIVisualScriptingData.inspectRootField.SetValue(DIVisualScriptingData.inspectTarget, result);
					complete = true;
					if (UnityEditor.PrefabUtility.GetPrefabObject(DIVisualScriptingData.inspectTarget.getMono.gameObject) != null)
					{
						//DO NOTHING
						UnityEditor.PrefabUtility.DisconnectPrefabInstance(DIVisualScriptingData.inspectTarget.getMono.gameObject);
					}
				}
				else if (saveMode == SaveState.File)
				{
					string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save Visual Scripting", "VisualScript", "asset", "Create Visual Scripting Asset");
					if (!string.IsNullOrEmpty(path))
					{
						result = Instantiate(this);
						result.name = name;
						if (!DIVisualScriptingData.replaceActAsAppend)
						{
							UnityEditor.AssetDatabase.DeleteAsset(path);
							UnityEditor.AssetDatabase.SaveAssets();
							UnityEditor.AssetDatabase.Refresh();
							UnityEditor.AssetDatabase.CreateAsset(result, path);
						}
						else
							UnityEditor.AssetDatabase.AddObjectToAsset(result, path);
						result.next = next;
						if (result.next != null)
							result.next.SaveComponent(result, result);
						DIVisualScriptingData.inspectRootField.SetValue(DIVisualScriptingData.inspectTarget, result);
						//set inspectRoot at the end because we need to check at SaveComponent method if last inpectRoot is not equal with the new one
						complete = true;
						if (UnityEditor.PrefabUtility.GetPrefabObject(DIVisualScriptingData.inspectTarget.getMono.gameObject) != null) {
							//DO NOTHING
							UnityEditor.PrefabUtility.DisconnectPrefabInstance(DIVisualScriptingData.inspectTarget.getMono.gameObject);
						}
					}
				}
				else if (saveMode == SaveState.Prefab)
				{
					string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save As Prefab", DIVisualScriptingData.inspectTarget.getMono.gameObject.name, "prefab", "Save as Prefab");
					GameObject prefab = null;

					if (!string.IsNullOrEmpty(path)) {
						prefab = UnityEditor.PrefabUtility.GetPrefabParent(DIVisualScriptingData.inspectTarget.getMono.gameObject) as GameObject;
						//Prefab exist, replace the old one
						if (!string.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(prefab)) && UnityEditor.AssetDatabase.GetAssetPath(prefab) == path)
						{
							if (!DIVisualScriptingData.replaceActAsAppend)
							{
								UnityEditor.AssetDatabase.DeleteAsset(path);
								UnityEditor.AssetDatabase.SaveAssets();
								UnityEditor.AssetDatabase.Refresh();
								prefab = UnityEditor.PrefabUtility.CreatePrefab(path, DIVisualScriptingData.inspectTarget.getMono.gameObject, UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
							}else
								UnityEditor.PrefabUtility.ReplacePrefab(DIVisualScriptingData.inspectTarget.getMono.gameObject, prefab, UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
						}
						//create new prefab
						else {
							prefab = UnityEditor.PrefabUtility.CreatePrefab(path, DIVisualScriptingData.inspectTarget.getMono.gameObject, UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
						}

						//if root an asset type, we need to make a copy, if not, just assign
						if (!string.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(this)))
						{
							result = Object.Instantiate(this);
							result.name = name;
							result.next = next;
							UnityEditor.AssetDatabase.AddObjectToAsset(result, path);
							UnityEditor.AssetDatabase.SaveAssets();
							UnityEditor.AssetDatabase.Refresh();
							Debug.Log(UnityEditor.AssetDatabase.GetAssetPath(result));
							if (result.next != null)
								result.next.SaveComponent(prefab, result);
							DIVisualScriptingData.inspectRootField.SetValue(DIVisualScriptingData.inspectTarget, result);
						}
						else {
							UnityEditor.AssetDatabase.AddObjectToAsset(this, path);
							DIVisualScriptingData.inspectRootField.SetValue(DIVisualScriptingData.inspectTarget, this);
							if(next != null)
								next.SaveComponent(prefab, this);
						}
						//Do replace because last prefab has not update with the new configuration of Component
						UnityEditor.PrefabUtility.ReplacePrefab(DIVisualScriptingData.inspectTarget.getMono.gameObject, prefab, UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
						complete = true;
					}

				}
			}
			else {
				if (next != null) {
					if (saveMode == SaveState.Prefab)
					{
						next.SaveComponent(UnityEditor.AssetDatabase.LoadAssetAtPath(
							UnityEditor.AssetDatabase.GetAssetPath(
								UnityEditor.PrefabUtility.GetPrefabParent(
									DIVisualScriptingData.inspectTarget.getMono.gameObject)),
							typeof(Object)), this);
					}
					else if (saveMode == SaveState.File)
						next.SaveComponent(this, this);
					else
						next.SaveComponent(null, this);
				}
				complete = true;
			}

			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			if(complete)
				result.saveState = saveMode;
		}
		*/
		#endregion

#pragma warning disable 0809
		[System.Obsolete("Root is unique component, you can use SaveRoot instead")]
		public override DIVisualComponent SaveComponent(Object rootParent, DIVisualComponent previous = null, bool autoAssignNextComponentOfPrevious = true)
		{
			return base.SaveComponent(rootParent, previous, autoAssignNextComponentOfPrevious);
		}
#pragma warning restore 0809
#endif
	}
}
