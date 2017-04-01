#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace DI.VisualScripting
{
	[VSCustomPropertyDrawer(typeof(GetString))]
	public class GetStringDrawer : GetVariableBaseDrawer {

		public override void PropertyDrawer(FieldInfo field, DIVisualComponent comp)
		{
			base.PropertyDrawer(field, comp);
			GetString _string = field.GetValue(comp) as GetString;
			DrawGetString(_string);
		}

		//When edit this method, please edit DrawGetString method at BuildString.cs
		public static void DrawGetString(GetString _string) {
			if (_string.getFrom != GetFrom.Exact)
			{
				if (_string.targetObj == null)
					return;
				var fieldInfos = _string.targetObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
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
				fieldIdx = EditorGUILayout.Popup("Field", fieldIdx, fieldInfoName.ToArray());
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
						_string.indexArray = EditorGUILayout.Popup("Index", _string.indexArray, fieldArrayNames.ToArray());
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
						_string.indexArray = EditorGUILayout.Popup("Index", _string.indexArray, fieldArrayNames.ToArray());
					}
				}
				else
					_string.indexArray = -1;

			}
			//Exact value
			else
			{
				_string.exactValue = EditorGUILayout.TextField("Value", _string.exactValue);
			}
		}
	}

}
#endif