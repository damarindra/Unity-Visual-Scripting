using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace DI.VisualScripting {
	[System.Serializable]
	public class GetString : GetVariableBase{
		private FieldInfo field {
			get {
				return targetObj.GetType().GetField(fieldName);
			}
		}
		public string fieldName;
		public int indexArray = -1;
		public string exactValue;

		public string value {
			get
			{
				if (getFrom != GetFrom.Exact)
				{
					if (targetObj == null || field == null)
					{
						Debug.LogError("Get String not setup properly, please setup it first");
						return "";
					}
					if (indexArray == -1)
					{
						if (field.FieldType == typeof(string))
							return (string)field.GetValue(targetObj);
						else
							return ((DIVariableString)field.GetValue(targetObj)).value;
					}
					else {
						if (field.FieldType == typeof(string[]) || (field.FieldType == typeof(List<string>)))
							return (string)((IList)field.GetValue(targetObj))[indexArray];
						else
							return ((DIVariableString)((IList)field.GetValue(targetObj))[indexArray]).value;
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
						Debug.LogError("Get String not setup properly, please setup it first");
						return;
					}
				}
				if (indexArray == -1)
				{
					if (field.FieldType == typeof(string))
						field.SetValue(targetObj, value);
					else
						((DIVariableString)field.GetValue(targetObj)).value = value;
				}
				else
				{
					if (field.FieldType == typeof(string[]) || (field.FieldType == typeof(List<string>)))
						((IList)field.GetValue(targetObj))[indexArray] = value;
					else
						((DIVariableString)((IList)field.GetValue(targetObj))[indexArray]).value = value;
				}
			}
		}
	}
}
