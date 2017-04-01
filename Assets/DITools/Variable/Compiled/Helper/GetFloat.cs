using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace DI.VisualScripting {
	[System.Serializable]
	public class GetFloat : GetVariableBase{
		private FieldInfo field {
			get {
				return targetObj.GetType().GetField(fieldName);
			}
		}
		public string fieldName;
		public int indexArray = -1;
		public float exactValue;

		public float value {
			get
			{
				if (getFrom != GetFrom.Exact)
				{
					if (targetObj == null || field == null)
					{
						Debug.LogError("Get Float not setup properly, please setup it first");
						return 0;
					}
					if (indexArray == -1)
					{
						if (field.FieldType == typeof(float))
							return (float)field.GetValue(targetObj);
						else
							return ((DIVariableFloat)field.GetValue(targetObj)).value;
					}
					else {
						if (field.FieldType == typeof(float[]) || (field.FieldType == typeof(List<float>)))
							return (float)((IList)field.GetValue(targetObj))[0];
						else
							return ((DIVariableFloat)((IList)field.GetValue(targetObj))[indexArray]).value;
					}
				}
				else {
					return exactValue;
				}
			}
			set
			{
				if (getFrom != GetFrom.Exact) {
					if (targetObj == null || field == null)
					{
						Debug.LogError("Get Float not setup properly, please setup it first");
						return;
					}
				}
				if (indexArray == -1)
				{
					if (field.FieldType == typeof(float))
						field.SetValue(targetObj, value);
					else
						((DIVariableFloat)field.GetValue(targetObj)).value = value;
				}
				else
				{
					if (field.FieldType == typeof(float[]) || (field.FieldType == typeof(List<float>)))
						((IList)field.GetValue(targetObj))[indexArray] = value;
					else
						((DIVariableFloat)((IList)field.GetValue(targetObj))[indexArray]).value = value;
				}
			}
		}
	}
}
