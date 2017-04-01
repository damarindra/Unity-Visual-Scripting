#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace DI.VisualScripting
{
	[VSCustomPropertyDrawer(typeof(GetInt))]
	public class GetIntDrawer : GetVariableBaseDrawer {

		public override void PropertyDrawer(FieldInfo field, DIVisualComponent comp)
		{
			base.PropertyDrawer(field, comp);
			GetInt _int = field.GetValue(comp) as GetInt;
			if (_int.getFrom != GetFrom.Exact)
			{
				if (_int.targetObj == null)
					return;
				var fieldInfos = _int.targetObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
				for (int i = 0; i < fieldInfos.Count; i++)
				{
					if (fieldInfos[i].FieldType == typeof(int) || fieldInfos[i].FieldType == typeof(DIVariableInt)
						|| (typeof(IList).IsAssignableFrom(fieldInfos[i].FieldType) &&
						(fieldInfos[i].FieldType == typeof(int[]) || fieldInfos[i].FieldType == typeof(DIVariableInt[])
						|| (fieldInfos[i].FieldType == typeof(List<int>) || fieldInfos[i].FieldType == typeof(List<DIVariableInt>))
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
				int fieldIdx = fieldInfoName.IndexOf(_int.fieldName);
				if (fieldIdx < 0)
					fieldIdx = 0;
				fieldIdx = EditorGUILayout.Popup("Field", fieldIdx, fieldInfoName.ToArray());
				_int.fieldName = fieldInfoName[fieldIdx];

				if (fieldInfos.Count == 0)
					return;

				if (typeof(IList).IsAssignableFrom(fieldInfos[fieldIdx].FieldType))
				{
					if (_int.indexArray > ((IList)fieldInfos[fieldIdx].GetValue(_int.targetObj)).Count)
						_int.indexArray = ((IList)fieldInfos[fieldIdx].GetValue(_int.targetObj)).Count - 1;
					List<string> fieldArrayNames = new List<string>();
					var fieldArray = ((IList)fieldInfos[fieldIdx].GetValue(_int.targetObj));

					//If regular, just show index
					if (fieldInfos[fieldIdx].FieldType == typeof(int[]) || (fieldInfos[fieldIdx].FieldType == typeof(List<int>)))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add("Index - " + i.ToString());
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_int.indexArray = EditorGUILayout.Popup("Index", _int.indexArray, fieldArrayNames.ToArray());
					}
					//if DIVar show varname
					else if (fieldInfos[fieldIdx].FieldType == typeof(DIVariableInt[]) || fieldInfos[fieldIdx].FieldType == typeof(List<DIVariableInt>))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add(((DIVariableInt)fieldArray[i]).varName);
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_int.indexArray = EditorGUILayout.Popup("Index", _int.indexArray, fieldArrayNames.ToArray());
					}
				}else
					_int.indexArray = -1;

			}
			//Exact value
			else {
				_int.exactValue = EditorGUILayout.IntField("Value", _int.exactValue);
			}
		}
	}

}
#endif