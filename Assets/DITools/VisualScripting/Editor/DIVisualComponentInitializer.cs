using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;

namespace DI.VisualScripting {
	[InitializeOnLoad]
	public class DIVisualComponentInitializer  {


		static DIVisualComponentInitializer() {
			EditorApplication.update += update;
			EditorApplication.hierarchyWindowItemOnGUI += hirearchyWindowItem;
		}

		static bool isDuplicating = false;
		private static void hirearchyWindowItem(int instanceID, Rect selectionRect)
		{
			if (Event.current != null && Selection.activeGameObject != null)
			{
				if (Event.current.commandName == "Duplicate" || Event.current.commandName == "Paste")
				{
					isDuplicating = true;
				}
				else if (isDuplicating)
				{
					isDuplicating = false;
					foreach (GameObject go in Selection.gameObjects) {
						var visualScripting = go.GetComponent<IVisualScripting>();
						if (visualScripting != null)
						{
							visualScripting.rootComponents = null;
							/*DIRootComponent[] comps = visualScripting.rootComponents;
							for (int i = 0; i < comps.Length; i++)
							{
								if (comps[i] != null)
								{
									Debug.Log(comps[i].GetInstanceID());
									comps[i] = comps[i].DuplicateAndSaveToScene();
									Debug.Log(comps[i].GetInstanceID());
									Debug.Log("DUPLICATED");
								}
							}
							visualScripting.rootComponents = comps;
							EditorUtility.SetDirty(visualScripting.getMono);
							Debug.Log(visualScripting.rootComponents[0] == comps[0]);*/

						}
					}
				}
			}
		}

		private static void update()
		{
			if(DIAttributeData.Instance.components.Count == 0)
				RefreshAllVisualComponent();
			if (EditorApplication.isCompiling)
				EditorPrefs.SetBool("Need To Refresh Visual Component", true);
			if (!EditorApplication.isCompiling && EditorPrefs.GetBool("Need To Refresh Visual Component", false)) {
				RefreshAllVisualComponent();
				EditorPrefs.SetBool("Need To Refresh Visual Component", false);
			}

		}

		//Only called after compilling done, only one call, for performance
		private static void RefreshAllVisualComponent()
		{
			//DIAttributeData.Instance.components = typeof(DIVisualComponent).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(DIVisualComponent))).ToList();
			DIAttributeData.Instance.components = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
														  from type in assembly.GetTypes()
														  where type.IsSubclassOf(typeof(DIVisualComponent))
														  select (System.Type)type).ToList();
			DIAttributeData.Instance.componentPathPopup.Clear();
			DIAttributeData.Instance.componentPathPopup.Add("Component");
			foreach (Type comp in DIAttributeData.Instance.components) {
				VisualComponentAttribute att = (VisualComponentAttribute)Attribute.GetCustomAttribute(comp, typeof(VisualComponentAttribute));
				if (att != null)
				{
					DIAttributeData.Instance.componentPathPopup.Add(att.path + "/" + AddSpacesToSentence(comp.Name, true));
					DIAttributeData.Instance.componentsWithAttribute.Add(comp);
				}
			}

			var propertyDrawerCandidate = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
										   from type in assembly.GetTypes()
										   where type.IsSubclassOf(typeof(VSPropertyDrawer))
										   select (System.Type)type).ToArray();
			DIAttributeData.Instance.propertyDrawers.Clear();
			foreach (Type drawer in propertyDrawerCandidate) {
				VSCustomPropertyDrawerAttribute att = (VSCustomPropertyDrawerAttribute)Attribute.GetCustomAttribute(drawer, typeof(VSCustomPropertyDrawerAttribute));
				if (att != null) {
					DIAttributeData.Instance.propertyDrawers.Add(new DIAttributeData.VSCustomPropertyDrawerAttributeData(att, drawer));
				}
			}
		}
		static string AddSpacesToSentence(string text, bool preserveAcronyms)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;
			System.Text.StringBuilder newText = new System.Text.StringBuilder(text.Length * 2);
			newText.Append(text[0]);
			for (int i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]))
					if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
						(preserveAcronyms && char.IsUpper(text[i - 1]) &&
						 i < text.Length - 1 && !char.IsUpper(text[i + 1])))
						newText.Append(' ');
				newText.Append(text[i]);
			}
			return newText.ToString();
		}
	}
}
