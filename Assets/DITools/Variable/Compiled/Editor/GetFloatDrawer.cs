#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace DI.VisualScripting
{
	[VSCustomPropertyDrawer(typeof(GetFloat))]
	public class GetFloatDrawer : GetVariableBaseDrawer {

		public override void PropertyDrawer(FieldInfo field, DIVisualComponent comp)
		{
			base.PropertyDrawer(field, comp);
			GetFloat _float = field.GetValue(comp) as GetFloat;
			if (_float.getFrom != GetFrom.Exact)
			{
				if (_float.targetObj == null)
					return;
				var fieldInfos = _float.targetObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
				for (int i = 0; i < fieldInfos.Count; i++)
				{
					if (fieldInfos[i].FieldType == typeof(float) || fieldInfos[i].FieldType == typeof(DIVariableFloat)
						|| (typeof(IList).IsAssignableFrom(fieldInfos[i].FieldType) &&
						(fieldInfos[i].FieldType == typeof(float[]) || fieldInfos[i].FieldType == typeof(DIVariableFloat[])
						|| (fieldInfos[i].FieldType == typeof(List<float>) || fieldInfos[i].FieldType == typeof(List<DIVariableFloat>))
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
				int fieldIdx = fieldInfoName.IndexOf(_float.fieldName);
				if (fieldIdx < 0)
					fieldIdx = 0;
				fieldIdx = EditorGUILayout.Popup("Field", fieldIdx, fieldInfoName.ToArray());
				_float.fieldName = fieldInfoName[fieldIdx];

				if (fieldInfos.Count == 0)
					return;

				if (typeof(IList).IsAssignableFrom(fieldInfos[fieldIdx].FieldType))
				{
					if (_float.indexArray > ((IList)fieldInfos[fieldIdx].GetValue(_float.targetObj)).Count)
						_float.indexArray = ((IList)fieldInfos[fieldIdx].GetValue(_float.targetObj)).Count - 1;
					List<string> fieldArrayNames = new List<string>();
					var fieldArray = ((IList)fieldInfos[fieldIdx].GetValue(_float.targetObj));

					//If regular, just show index
					if (fieldInfos[fieldIdx].FieldType == typeof(float[]) || (fieldInfos[fieldIdx].FieldType == typeof(List<float>)))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add("Index - " + i.ToString());
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_float.indexArray = EditorGUILayout.Popup("Index", _float.indexArray, fieldArrayNames.ToArray());
					}
					//if DIVar show varname
					else if (fieldInfos[fieldIdx].FieldType == typeof(DIVariableFloat[]) || fieldInfos[fieldIdx].FieldType == typeof(List<DIVariableFloat>))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add(((DIVariableFloat)fieldArray[i]).varName);
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_float.indexArray = EditorGUILayout.Popup("Index", _float.indexArray, fieldArrayNames.ToArray());
					}
				}else
					_float.indexArray = -1;

			}
			//Exact value
			else {
				_float.exactValue = EditorGUILayout.FloatField("Value", _float.exactValue);
			}
		}
	}

}
#endif