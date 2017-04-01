using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace DI {
	//[InitializeOnLoad]
	public class AutoFixApiCompatibilityLevel {

		static AutoFixApiCompatibilityLevel() {
			EditorApplication.update += update;
		}

		private static void update()
		{
			if (EditorApplication.isCompiling && EditorPrefs.GetBool("Change Api Compatibility Level Now", true) && PlayerSettings.apiCompatibilityLevel == ApiCompatibilityLevel.NET_2_0_Subset) {
				EditorPrefs.SetBool("Change Api Compatibility Level Now", true);
			}
			if (EditorPrefs.GetBool("Change Api Compatibility Level Now", false)) {
				PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
				EditorPrefs.SetBool("Change Api Compatibility Level Now", false);
				Debug.Log("Auto Fix API Compatibility Level to : .NET 2.0");
			}
		}
	}
}
