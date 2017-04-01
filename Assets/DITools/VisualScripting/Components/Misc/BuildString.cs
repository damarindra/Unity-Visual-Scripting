using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DI.VisualScripting {
	[VisualComponent("Misc")]
	public class BuildString : DIVisualComponent {

		public List<GetString> getStrings = new List<GetString>();
		public GetString store;

		protected override void WhatDo()
		{
			string result = "";
			foreach (GetString gs in getStrings) {
				result += gs.value;
			}
			if (store != null)
				store.value = result;
			Finish();
		}

		public override string windowName
		{
			get
			{
				return "Build String";
			}
		}

#if UNITY_EDITOR

		public override void BuildWindow()
		{
			DrawGetStringList();
			DrawDefaultWindow(true);
		}

		void DrawGetStringList() {
			UnityEditor.EditorGUILayout.BeginHorizontal();
			GUI.enabled = false;
			UnityEditor.EditorGUILayout.IntField("Size", getStrings.Count);
			GUI.enabled = true;
			if (GUILayout.Button("+"))
				getStrings.Add(new GetString());
			if (GUILayout.Button("-"))
				getStrings.RemoveAt(getStrings.Count - 1);
			UnityEditor.EditorGUILayout.EndHorizontal();
			for (int i = 0; i < getStrings.Count; i++) {
				UnityEditor.EditorGUILayout.LabelField(i.ToString(), UnityEditor.EditorStyles.boldLabel);
				DrawBaseVariable(getStrings[i]);
				DrawGetString(getStrings[i]);
			}
		}
		void DrawBaseVariable(GetVariableBase _base)
		{
			_base.getFrom = (GetFrom)UnityEditor.EditorGUILayout.EnumPopup("Source", _base.getFrom);
			if (_base.getFrom == GetFrom.GameObject)
			{
				_base.target = (GameObject)UnityEditor.EditorGUILayout.ObjectField("Target", _base.target, typeof(GameObject), true);

				if (_base.target != null)
				{
					//Show MonoScript
					var monobehaviours = _base.target.GetComponents<MonoBehaviour>().ToList();
					int monoIdx = monobehaviours.IndexOf(_base.monoscript);
					List<string> monobehaviourString = new List<string>();
					for (int i = 0; i < monobehaviours.Count; i++)
					{
						monobehaviourString.Add("(" + i.ToString() + ")" + monobehaviours[i].ToString().Replace(_base.target.name, ""));
					}
					if (monobehaviourString.Count < 0)
						monobehaviourString.Add("");
					if (monoIdx < 0)
						monoIdx = 0;
					monoIdx = UnityEditor.EditorGUILayout.Popup("Script", monoIdx, monobehaviourString.ToArray());
					_base.monoscript = monobehaviours[monoIdx];
				}
			}
		}
		void DrawGetString(GetString _string)
		{
			if (_string.getFrom != GetFrom.Exact)
			{
				if (_string.targetObj == null)
					return;
				var fieldInfos = _string.targetObj.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).ToList();
				for (int i = 0; i < fieldInfos.Count; i++)
				{
					if (fieldInfos[i].FieldType == typeof(string) || fieldInfos[i].FieldType == typeof(DIVariableString)
						|| (typeof(IList).IsAssignableFrom(fieldInfos[i].FieldType) &&
						(fieldInfos[i].FieldType == typeof(string[]) || fieldInfos[i].FieldType == typeof(DIVariableString[])
						|| (fieldInfos[i].FieldType == typeof(List<string>) || fieldInfos[i].FieldType == typeof(List<DIVariableString>))
						)))
					{
						//DO NOTHING
					}
					else
					{
						fieldInfos.RemoveAt(i);
						i -= 1;
					}
				}

				var fieldInfoName = fieldInfos.Select(i => i.Name).ToList();
				if (fieldInfoName.Count == 0)
					fieldInfoName.Add("");
				int fieldIdx = fieldInfoName.IndexOf(_string.fieldName);
				if (fieldIdx < 0)
					fieldIdx = 0;
				fieldIdx = UnityEditor.EditorGUILayout.Popup("Field", fieldIdx, fieldInfoName.ToArray());
				_string.fieldName = fieldInfoName[fieldIdx];

				if (fieldInfos.Count == 0)
					return;

				if (typeof(IList).IsAssignableFrom(fieldInfos[fieldIdx].FieldType))
				{
					if (_string.indexArray > ((IList)fieldInfos[fieldIdx].GetValue(_string.targetObj)).Count)
						_string.indexArray = ((IList)fieldInfos[fieldIdx].GetValue(_string.targetObj)).Count - 1;
					List<string> fieldArrayNames = new List<string>();
					var fieldArray = ((IList)fieldInfos[fieldIdx].GetValue(_string.targetObj));

					//If regular, just show index
					if (fieldInfos[fieldIdx].FieldType == typeof(string[]) || (fieldInfos[fieldIdx].FieldType == typeof(List<string>)))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add("Index - " + i.ToString());
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_string.indexArray = UnityEditor.EditorGUILayout.Popup("Index", _string.indexArray, fieldArrayNames.ToArray());
					}
					//if DIVar show varname
					else if (fieldInfos[fieldIdx].FieldType == typeof(DIVariableString[]) || fieldInfos[fieldIdx].FieldType == typeof(List<DIVariableString>))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add(((DIVariableString)fieldArray[i]).varName);
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_string.indexArray = UnityEditor.EditorGUILayout.Popup("Index", _string.indexArray, fieldArrayNames.ToArray());
					}
				}
				else
					_string.indexArray = -1;

			}
			//Exact value
			else
			{
				_string.exactValue = UnityEditor.EditorGUILayout.TextField("Value", _string.exactValue);
			}
		}
#endif
	}
}
