using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace DI.VisualScripting {
	[System.Serializable]
	public class GetObject : GetVariableBase{
		private FieldInfo field {
			get {
				return targetObj.GetType().GetField(fieldName);
			}
		}
		public string fieldName;
		public int indexArray = -1;
		public Object exactValue;

		public Object value {
			get {
				if (getFrom != GetFrom.Exact)
				{
					if (targetObj == null || field == null)
					{
						Debug.LogError("Get Object not setup properly, please setup it first");
						return null;
					}

					if (indexArray == -1)
					{
						if (field.FieldType == typeof(Object))
							return (Object)field.GetValue(targetObj);
						else
							return ((DIVariableObject)field.GetValue(targetObj)).value;
					}
					else {
						if (field.FieldType == typeof(Object[]) || (field.FieldType == typeof(List<Object>)))
							return (Object)((IList)field.GetValue(targetObj))[indexArray];
						else
							return ((DIVariableObject)((IList)field.GetValue(targetObj))[indexArray]).value;
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
						Debug.LogError("Get Object not setup properly, please setup it first");
						return;
					}
				}
				if (indexArray == -1)
				{
					if (field.FieldType == typeof(Object))
						field.SetValue(targetObj, value);
					else
						((DIVariableObject)field.GetValue(targetObj)).value = value;
				}
				else
				{
					if (field.FieldType == typeof(Object[]) || (field.FieldType == typeof(List<Object>)))
						((IList)field.GetValue(targetObj))[indexArray] = value;
					else
						((DIVariableObject)((IList)field.GetValue(targetObj))[indexArray]).value = value;
				}
			}
		}
	}
}
