using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace DI.VisualScripting
{
	public class DIGlobalVariableHelper {

		static string intId = "B_GV_INT";
		static string floatId = "B_GV_FLOAT";
		static string stringId = "B_GV_STRING";
		static string boolId = "B_GV_BOOL";

		[System.Obsolete("Not used anymore, change with DIGlobalVariable.Backup")]
		public static void BackupVariable()
		{
#if UNITY_EDITOR
			//Debug.Log("Backup");
			//Backup Int
			EditorPrefs.DeleteKey(intId);
			for (int i = 0; i < DIGlobalVariable.Instance.intContainer.Count; i++)
			{
				EditorPrefs.SetString(intId, DIGlobalVariable.Instance.intContainer[i].value.ToString() + "~");
			}

			//Backup Float
			EditorPrefs.DeleteKey(floatId);
			for (int i = 0; i < DIGlobalVariable.Instance.floatContainer.Count; i++)
			{
				EditorPrefs.SetString(floatId, DIGlobalVariable.Instance.floatContainer[i].value.ToString() + "~");
			}

			//Backup String
			EditorPrefs.DeleteKey(stringId);
			for (int i = 0; i < DIGlobalVariable.Instance.stringContainer.Count; i++)
			{
				EditorPrefs.SetString(stringId, DIGlobalVariable.Instance.stringContainer[i].value.ToString() + "~");
			}

			//Backup Bool
			EditorPrefs.DeleteKey(boolId);
			for (int i = 0; i < DIGlobalVariable.Instance.boolContainer.Count; i++)
			{
				EditorPrefs.SetString(boolId, DIGlobalVariable.Instance.boolContainer[i].value.ToString() + "~");
			}

			EditorPrefs.SetBool("GV_B_DONE", true);
#endif
		}

		[System.Obsolete("Not used anymore, change with DIGlobalVariable.Restore")]
		public static void Restore()
		{
#if UNITY_EDITOR
			//Debug.Log("Restore");
			//Restore int;
			string value = EditorPrefs.GetString(intId);
			var intArray = value.Split(new char[1] { '~' });
			for (int i = 0; i < DIGlobalVariable.Instance.intContainer.Count; i++)
			{
				DIGlobalVariable.Instance.intContainer[i].value = System.Int32.Parse(intArray[i]);
			}

			//Restore float;
			value = EditorPrefs.GetString(floatId);
			var floatArray = value.Split(new char[1] { '~' });
			for (int i = 0; i < DIGlobalVariable.Instance.floatContainer.Count; i++)
			{
				Debug.Log(floatArray[i] + i.ToString());
				DIGlobalVariable.Instance.floatContainer[i].value = System.Single.Parse(floatArray[i]);
			}

			//Restore int;
			value = EditorPrefs.GetString(stringId);
			var stringArray = value.Split(new char[1] { '~' });
			for (int i = 0; i < DIGlobalVariable.Instance.stringContainer.Count; i++)
			{
				DIGlobalVariable.Instance.stringContainer[i].value = (stringArray[i]);
			}

			//Restore int;
			value = EditorPrefs.GetString(boolId);
			var boolArray = value.Split(new char[1] { '~' });
			for (int i = 0; i < DIGlobalVariable.Instance.boolContainer.Count; i++)
			{
				DIGlobalVariable.Instance.boolContainer[i].value = Convert.ToBoolean(boolArray[i]);
			}

#endif
		}

	}

}
