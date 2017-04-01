#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace DI.VisualScripting
{
	[VSCustomPropertyDrawer(typeof(GetObject))]
	public class GetObjectDrawer : GetVariableBaseDrawer {

		public override void PropertyDrawer(FieldInfo field, DIVisualComponent comp)
		{
			base.PropertyDrawer(field, comp);
			GetObject _object = field.GetValue(comp) as GetObject;
			if (_object.getFrom != GetFrom.Exact)
			{
				if (_object.targetObj == null)
					return;
				var fieldInfos = _object.targetObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
				for (int i = 0; i < fieldInfos.Count; i++)
				{
					if (fieldInfos[i].FieldType == typeof(Object) || fieldInfos[i].FieldType == typeof(DIVariableObject)
						|| (typeof(IList).IsAssignableFrom(fieldInfos[i].FieldType) &&
						(fieldInfos[i].FieldType == typeof(Object[]) || fieldInfos[i].FieldType == typeof(DIVariableObject[])
						|| (fieldInfos[i].FieldType == typeof(List<Object>) || fieldInfos[i].FieldType == typeof(List<DIVariableObject>))
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
				int fieldIdx = fieldInfoName.IndexOf(_object.fieldName);
				if (fieldIdx < 0)
					fieldIdx = 0;
				fieldIdx = EditorGUILayout.Popup("Field", fieldIdx, fieldInfoName.ToArray());
				_object.fieldName = fieldInfoName[fieldIdx];

				if (fieldInfos.Count == 0)
					return;

				if (typeof(IList).IsAssignableFrom(fieldInfos[fieldIdx].FieldType))
				{
					if (_object.indexArray > ((IList)fieldInfos[fieldIdx].GetValue(_object.targetObj)).Count)
						_object.indexArray = ((IList)fieldInfos[fieldIdx].GetValue(_object.targetObj)).Count - 1;
					List<string> fieldArrayNames = new List<string>();
					var fieldArray = ((IList)fieldInfos[fieldIdx].GetValue(_object.targetObj));

					//If regular, just show index
					if (fieldInfos[fieldIdx].FieldType == typeof(Object[]) || (fieldInfos[fieldIdx].FieldType == typeof(List<Object>)))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add("Index - " + i.ToString());
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_object.indexArray = EditorGUILayout.Popup("Index", _object.indexArray, fieldArrayNames.ToArray());
					}
					//if DIVar show varname
					else if (fieldInfos[fieldIdx].FieldType == typeof(DIVariableObject[]) || fieldInfos[fieldIdx].FieldType == typeof(List<DIVariableObject>))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add(((DIVariableObject)fieldArray[i]).varName);
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_object.indexArray = EditorGUILayout.Popup("Index", _object.indexArray, fieldArrayNames.ToArray());
					}
				}else
					_object.indexArray = -1;

			}
			//Exact value
			else {
				_object.exactValue = EditorGUILayout.ObjectField("Value", _object.exactValue, typeof(Object), true) as Object;
			}
		}
	}

}
#endif