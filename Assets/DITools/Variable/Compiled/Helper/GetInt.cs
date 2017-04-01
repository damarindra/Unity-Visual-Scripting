using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace DI.VisualScripting {
	[System.Serializable]
	public class GetInt : GetVariableBase{
		private FieldInfo field {
			get {
				return targetObj.GetType().GetField(fieldName);
			}
		}
		public string fieldName;
		public int indexArray = -1;
		public int exactValue;

		public int value {
			get {
				if (getFrom != GetFrom.Exact)
				{
					if (targetObj == null || field == null)
					{
						Debug.LogError("Get Int not setup properly, please setup it first");
						return 0;
					}
					if (indexArray == -1)
					{
						if (field.FieldType == typeof(int))
							return (int)field.GetValue(targetObj);
						else
							return ((DIVariableInt)field.GetValue(targetObj)).value;
					}
					else {
						if (field.FieldType == typeof(int[]) || (field.FieldType == typeof(List<int>)))
							return (int)((IList)field.GetValue(targetObj))[indexArray];
						else
							return ((DIVariableInt)((IList)field.GetValue(targetObj))[indexArray]).value;
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
						Debug.LogError("Get Int not setup properly, please setup it first");
						return;
					}
				}
				if (indexArray == -1)
				{
					if (field.FieldType == typeof(int))
						field.SetValue(targetObj, value);
					else
						((DIVariableInt)field.GetValue(targetObj)).value = value;
				}
				else
				{
					if (field.FieldType == typeof(int[]) || (field.FieldType == typeof(List<int>)))
						((IList)field.GetValue(targetObj))[indexArray] = value;
					else
						((DIVariableInt)((IList)field.GetValue(targetObj))[indexArray]).value = value;
				}
			}
		}
	}
}
