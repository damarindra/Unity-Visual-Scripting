using UnityEngine;
using System.Collections;
using UnityEditor;

namespace DI.VisualScripting
{
	public class DIGlobalVariableWindowInstance : DIGlobalVariableWindow
	{
		public static DIGlobalVariableWindowInstance Window {
			get
			{
				if (window == null)
				{
					return null;
					//window = (GetWindow(typeof(DIGlobalVariableWindowInstance), false, "Global Variable") as DIGlobalVariableWindowInstance);
				}
				return window as DIGlobalVariableWindowInstance;
			}
		}

		static DIGlobalVariableWindowInstance persistentWindow
		{
			get
			{
				if (window == null)
				{
					window = (GetWindow(typeof(DIGlobalVariableWindowInstance), false, "Global Variable") as DIGlobalVariableWindowInstance);
				}
				return window as DIGlobalVariableWindowInstance;
			}
		}

		[MenuItem("Window/DI Tools/Global Variable")]
		public static void DisplayWindow()
		{
			persistentWindow.Show();
		}

		protected override void OnGUI()
		{
			base.OnGUI();
		}

	}
}
