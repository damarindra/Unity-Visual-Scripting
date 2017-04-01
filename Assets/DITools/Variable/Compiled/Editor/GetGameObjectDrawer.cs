#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace DI.VisualScripting
{
	[VSCustomPropertyDrawer(typeof(GetGameObject))]
	public class GetGameObjectDrawer : GetVariableBaseDrawer {

		public override void PropertyDrawer(FieldInfo field, DIVisualComponent comp)
		{
			base.PropertyDrawer(field, comp);
			GetGameObject _gameObject = field.GetValue(comp) as GetGameObject;
			if (_gameObject.getFrom != GetFrom.Exact)
			{
				if (_gameObject.targetObj == null)
					return;
				var fieldInfos = _gameObject.targetObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
				for (int i = 0; i < fieldInfos.Count; i++)
				{
					if (fieldInfos[i].FieldType == typeof(GameObject) || fieldInfos[i].FieldType == typeof(DIVariableGameObject)
						|| (typeof(IList).IsAssignableFrom(fieldInfos[i].FieldType) &&
						(fieldInfos[i].FieldType == typeof(GameObject[]) || fieldInfos[i].FieldType == typeof(DIVariableGameObject[])
						|| (fieldInfos[i].FieldType == typeof(List<GameObject>) || fieldInfos[i].FieldType == typeof(List<DIVariableGameObject>))
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
				int fieldIdx = fieldInfoName.IndexOf(_gameObject.fieldName);
				if (fieldIdx < 0)
					fieldIdx = 0;
				fieldIdx = EditorGUILayout.Popup("Field", fieldIdx, fieldInfoName.ToArray());
				_gameObject.fieldName = fieldInfoName[fieldIdx];

				if (fieldInfos.Count == 0)
					return;

				if (typeof(IList).IsAssignableFrom(fieldInfos[fieldIdx].FieldType))
				{
					if (_gameObject.indexArray > ((IList)fieldInfos[fieldIdx].GetValue(_gameObject.targetObj)).Count)
						_gameObject.indexArray = ((IList)fieldInfos[fieldIdx].GetValue(_gameObject.targetObj)).Count - 1;
					List<string> fieldArrayNames = new List<string>();
					var fieldArray = ((IList)fieldInfos[fieldIdx].GetValue(_gameObject.targetObj));

					//If regular, just show index
					if (fieldInfos[fieldIdx].FieldType == typeof(GameObject[]) || (fieldInfos[fieldIdx].FieldType == typeof(List<GameObject>)))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add("Index - " + i.ToString());
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_gameObject.indexArray = EditorGUILayout.Popup("Index", _gameObject.indexArray, fieldArrayNames.ToArray());
					}
					//if DIVar show varname
					else if (fieldInfos[fieldIdx].FieldType == typeof(DIVariableGameObject[]) || fieldInfos[fieldIdx].FieldType == typeof(List<DIVariableGameObject>))
					{
						for (int i = 0; i < fieldArray.Count; i++)
						{
							fieldArrayNames.Add(((DIVariableGameObject)fieldArray[i]).varName);
						}
						if (fieldArrayNames.Count == 0)
							fieldArrayNames.Add("");
						_gameObject.indexArray = EditorGUILayout.Popup("Index", _gameObject.indexArray, fieldArrayNames.ToArray());
					}
				}else
					_gameObject.indexArray = -1;

			}
			//Exact value
			else {
				_gameObject.exactValue = EditorGUILayout.ObjectField("Value", _gameObject.exactValue, typeof(GameObject), true) as GameObject;
			}
		}
	}

}
#endif