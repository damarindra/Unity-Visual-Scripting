using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace DI.VisualScripting {
	[System.Serializable]
	public class GetGameObject : GetVariableBase{
		private FieldInfo field {
			get {
				return targetObj.GetType().GetField(fieldName);
			}
		}
		public string fieldName;
		public int indexArray = -1;
		public GameObject exactValue;

		public GameObject value {
			get {
				if (getFrom != GetFrom.Exact)
				{
					if (targetObj == null || field == null)
					{
						Debug.LogError("Get GameObject not setup properly, please setup it first");
						return null;
					}

					if (indexArray == -1)
					{
						if (field.FieldType == typeof(GameObject))
							return (GameObject)field.GetValue(targetObj);
						else
							return ((DIVariableGameObject)field.GetValue(targetObj)).value;
					}
					else {
						if (field.FieldType == typeof(GameObject[]) || (field.FieldType == typeof(List<GameObject>)))
							return (GameObject)((IList)field.GetValue(targetObj))[indexArray];
						else
							return ((DIVariableGameObject)((IList)field.GetValue(targetObj))[indexArray]).value;
					}
				}
				else {
					return exactValue;
				}
			}
			set
			{
				if (getFrom != GetFrom.Exact)
				{
					if (targetObj == null || field == null)
					{
						Debug.LogError("Get GameObject not setup properly, please setup it first");
						return;
					}
				}
				if (indexArray == -1)
				{
					if (field.FieldType == typeof(GameObject))
						field.SetValue(targetObj, value);
					else
						((DIVariableGameObject)field.GetValue(targetObj)).value = value;
				}
				else
				{
					if (field.FieldType == typeof(GameObject[]) || (field.FieldType == typeof(List<GameObject>)))
						((IList)field.GetValue(targetObj))[indexArray] = value;
					else
						((DIVariableGameObject)((IList)field.GetValue(targetObj))[indexArray]).value = value;
				}
			}
		}
	}
}
