#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace DI.VisualScripting
{
	[VSCustomPropertyDrawer(typeof(GetBool))]
	public class GetBoolDrawer : GetVariableBaseDrawer {

		public override void PropertyDrawer(FieldInfo field, DIVisualComponent comp)
		{
			base.PropertyDrawer(field, comp);
			GetBool _bool = field.GetValue(comp) as GetBool;
			if (_bool.getFrom != GetFrom.Exact)
			{
				if (_bool.targetObj == null)
					return;
				var fieldInfos = _bool.targetObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
				for (int i = 0; i < fieldInfos.Count; i++)
				{
					if (fieldInfos[i].FieldType == typeof(bool) || fieldInfos[i].FieldType == typeof(DIVariableBool)
						|| (typeof(IList).IsAssignableFrom(fieldInfos[i].FieldType) &&
						(fieldInfos[i].FieldType == typeof(bool[]) || fieldInfos[i].FieldType == typeof(DIVariableBool[])
						|| (fieldInfos[i].FieldType == typeof(List<bool>) || fieldInfos[i].FieldType == typeof(List<DIVariableBool>))
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
				int fieldIdx = fieldInfoName.IndexOf(_bool.fieldName);
				if (fieldIdx < 0)
					fieldIdx = 0;
				fieldIdx = EditorGUILayout.Popup("Field", fieldIdx, fieldInfoName.ToArray());
				_bool.fieldName = fieldInfoName[fieldIdx];

				if (fieldInfos.Count == 0)
					return;

				if (typeof(IList).IsAssignableFrom(fieldInfos[fieldIdx].FieldType))
				{
					if (_bool.indexArray > ((IList)fieldInfos[fieldIdx].GetValue(_bool.targetObj)).Count)
						_bool.indexArray = ((IList)fieldInfos[fieldIdx].GetValue(_bool.targetObj)).Count - 1;
					List<string> fieldArrayNames = new List<string>();
					var fieldArray = ((IList)fieldInfos[fieldIdx].GetValue(_bool.targetObj));

					//If regular bool, just show index
					if (fieldInfos[fieldIdx].FieldType == typeof(bool[]) || (fieldInfos[fieldIdx].FieldType == typeof(List<bool>)))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add("Index - " + i.ToString());
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_bool.indexArray = EditorGUILayout.Popup("Index", _bool.indexArray, fieldArrayNames.ToArray());
					}
					//if DIVarBool show varname
					else if (fieldInfos[fieldIdx].FieldType == typeof(DIVariableBool[]) || fieldInfos[fieldIdx].FieldType == typeof(List<DIVariableBool>))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add(((DIVariableBool)fieldArray[i]).varName);
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_bool.indexArray = EditorGUILayout.Popup("Index", _bool.indexArray, fieldArrayNames.ToArray());
					}
				}else
					_bool.indexArray = -1;

			}
			//Exact value
			else {
				_bool.exactValue = EditorGUILayout.Toggle("Value", _bool.exactValue);
			}
		}
	}

}
#endif