#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace DI.VisualScripting
{
	[VSCustomPropertyDrawer(typeof(GetVector3))]
	public class GetVector3Drawer : GetVariableBaseDrawer {

		public override void PropertyDrawer(FieldInfo field, DIVisualComponent comp)
		{
			base.PropertyDrawer(field, comp);
			GetVector3 _getVector = field.GetValue(comp) as GetVector3;
			if (_getVector.getFrom != GetFrom.Exact)
			{
				if (_getVector.targetObj == null)
					return;
				var fieldInfos = _getVector.targetObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
				for (int i = 0; i < fieldInfos.Count; i++)
				{
					if (fieldInfos[i].FieldType == typeof(Vector3) || fieldInfos[i].FieldType == typeof(DIVariableVector3)
						|| (typeof(IList).IsAssignableFrom(fieldInfos[i].FieldType) &&
						(fieldInfos[i].FieldType == typeof(Vector3[]) || fieldInfos[i].FieldType == typeof(DIVariableVector3[])
						|| (fieldInfos[i].FieldType == typeof(List<Vector3>) || fieldInfos[i].FieldType == typeof(List<DIVariableVector3>))
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
				int fieldIdx = fieldInfoName.IndexOf(_getVector.fieldName);
				if (fieldIdx < 0)
					fieldIdx = 0;
				fieldIdx = EditorGUILayout.Popup("Field", fieldIdx, fieldInfoName.ToArray());
				_getVector.fieldName = fieldInfoName[fieldIdx];

				if (fieldInfos.Count == 0)
					return;

				if (typeof(IList).IsAssignableFrom(fieldInfos[fieldIdx].FieldType))
				{
					if (_getVector.indexArray > ((IList)fieldInfos[fieldIdx].GetValue(_getVector.targetObj)).Count)
						_getVector.indexArray = ((IList)fieldInfos[fieldIdx].GetValue(_getVector.targetObj)).Count - 1;
					List<string> fieldArrayNames = new List<string>();
					var fieldArray = ((IList)fieldInfos[fieldIdx].GetValue(_getVector.targetObj));

					//If regular bool, just show index
					if (fieldInfos[fieldIdx].FieldType == typeof(Vector3[]) || (fieldInfos[fieldIdx].FieldType == typeof(List<Vector3>)))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add("Index - " + i.ToString());
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_getVector.indexArray = EditorGUILayout.Popup("Index", _getVector.indexArray, fieldArrayNames.ToArray());
					}
					//if DIVarBool show varname
					else if (fieldInfos[fieldIdx].FieldType == typeof(DIVariableVector3[]) || fieldInfos[fieldIdx].FieldType == typeof(List<DIVariableVector3>))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add(((DIVariableVector3)fieldArray[i]).varName);
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_getVector.indexArray = EditorGUILayout.Popup("Index", _getVector.indexArray, fieldArrayNames.ToArray());
					}
				}else
					_getVector.indexArray = -1;

			}
			//Exact value
			else 
				_getVector.exactValue = EditorGUILayout.Vector3Field("Value", _getVector.exactValue);
			
		}
	}

}
#endif