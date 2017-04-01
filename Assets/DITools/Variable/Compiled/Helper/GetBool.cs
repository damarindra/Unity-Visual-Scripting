using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace DI.VisualScripting {
	[System.Serializable]
	public class GetBool : GetVariableBase{
		private FieldInfo field {
			get {
				return targetObj.GetType().GetField(fieldName);
			}
		}
		public string fieldName;
		public int indexArray = -1;
		public bool exactValue;

		public bool value {
			get {
				if (getFrom != GetFrom.Exact)
				{
					if (targetObj == null || field == null)
					{
						Debug.LogError("Get Boolean not setup properly, please setup it first");
						return false;
					}

					if (indexArray == -1)
					{
						if (field.FieldType == typeof(bool))
							return (bool)field.GetValue(targetObj);
						else
							return ((DIVariableBool)field.GetValue(targetObj)).value;
					}
					else {
						if (field.FieldType == typeof(bool[]) || (field.FieldType == typeof(List<bool>)))
							return (bool)((IList)field.GetValue(targetObj))[indexArray];
						else
							return ((DIVariableBool)((IList)field.GetValue(targetObj))[indexArray]).value;
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
						Debug.LogError("Get Bool not setup properly, please setup it first");
						return;
					}
				}
				if (indexArray == -1)
				{
					if (field.FieldType == typeof(bool))
						field.SetValue(targetObj, value);
					else
						((DIVariableBool)field.GetValue(targetObj)).value = value;
				}
				else
				{
					if (field.FieldType == typeof(bool[]) || (field.FieldType == typeof(List<bool>)))
						((IList)field.GetValue(targetObj))[indexArray] = value;
					else
						((DIVariableBool)((IList)field.GetValue(targetObj))[indexArray]).value = value;
				}
			}
		}
	}
}
