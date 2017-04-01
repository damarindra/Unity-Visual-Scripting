using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace DI.VisualScripting {
	[InitializeOnLoad]
	public class DIVariableInitializer {
		
		static bool lastFrameIsPlaying = false;

		static DIVariableInitializer() {
			EditorApplication.update += update;
		}

		private static void update()
		{
			if (lastFrameIsPlaying && !EditorApplication.isPlaying) {
				DIGlobalVariable.Instance.Restore();
				DIGlobalVariableWindow.restored = true;
			}
			
			lastFrameIsPlaying = EditorApplication.isPlaying;
		}
	}
}
