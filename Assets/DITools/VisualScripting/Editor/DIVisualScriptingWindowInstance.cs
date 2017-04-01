using UnityEngine;
using System.Collections;
using UnityEditor;

namespace DI.VisualScripting
{
	public class DIVisualScriptingWindowInstance : DIVisualScriptingWindow
	{
		public static new DIVisualScriptingWindowInstance Window {
			get
			{
				if (window == null)
				{
					return null;
					//window = (GetWindow(typeof(DIGlobalVariableWindowInstance), false, "Global Variable") as DIGlobalVariableWindowInstance);
				}
				return window as DIVisualScriptingWindowInstance;
			}
		}

		static DIVisualScriptingWindowInstance persistentWindow
		{
			get
			{
				if (window == null)
				{
					window = (GetWindow(typeof(DIVisualScriptingWindowInstance), false, "Visual Scripting") as DIVisualScriptingWindowInstance);
				}
				return window as DIVisualScriptingWindowInstance;
			}
		}

		[MenuItem("Window/DI Tools/Visual Scripting")]
		public static void CreateWindow()
		{
			persistentWindow.Show();
		}

		protected override void OnGUI()
		{
			base.OnGUI();
		}

	}
}
