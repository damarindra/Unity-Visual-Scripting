using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace DI.VisualScripting {
	[System.Serializable]
	public class GetVector3 : GetVariableBase{
		private FieldInfo field {
			get {
				return targetObj.GetType().GetField(fieldName);
			}
		}
		public string fieldName;
		public int indexArray = -1;
		public Vector3 exactValue;

		public Vector3 value {
			get {
				if (getFrom != GetFrom.Exact)
				{
					if (targetObj == null || field == null)
					{
						Debug.LogError("Get Vector3 not setup properly, please setup it first");
						return Vector2.zero;
					}

					if (indexArray == -1)
					{
						if (field.FieldType == typeof(Vector3))
							return (Vector3)field.GetValue(targetObj);
						else
							return ((DIVariableVector3)field.GetValue(targetObj)).value;
					}
					else {
						if (field.FieldType == typeof(Vector3[]) || (field.FieldType == typeof(List<Vector3>)))
							return (Vector3)((IList)field.GetValue(targetObj))[indexArray];
						else
							return ((DIVariableVector3)((IList)field.GetValue(targetObj))[indexArray]).value;
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
						Debug.LogError("Get Vector3 not setup properly, please setup it first");
						return;
					}
				}
				if (indexArray == -1)
				{
					if (field.FieldType == typeof(Vector3))
						field.SetValue(targetObj, value);
					else
						((DIVariableVector3)field.GetValue(targetObj)).value = value;
				}
				else
				{
					if (field.FieldType == typeof(Vector3[]) || (field.FieldType == typeof(List<Vector3>)))
						((IList)field.GetValue(targetObj))[indexArray] = value;
					else
						((DIVariableVector3)((IList)field.GetValue(targetObj))[indexArray]).value = value;
				}
			}
		}
	}
}
