using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace DI.VisualScripting {
	[System.Serializable]
	public class GetVector2 : GetVariableBase{
		private FieldInfo field {
			get {
				return targetObj.GetType().GetField(fieldName);
			}
		}
		public string fieldName;
		public int indexArray = -1;
		public Vector2 exactValue;

		public Vector2 value {
			get {
				if (getFrom != GetFrom.Exact)
				{
					if (targetObj == null || field == null)
					{
						Debug.LogError("Get Vector2 not setup properly, please setup it first");
						return Vector2.zero;
					}

					if (indexArray == -1)
					{
						if (field.FieldType == typeof(Vector2))
							return (Vector2)field.GetValue(targetObj);
						else
							return ((DIVariableVector2)field.GetValue(targetObj)).value;
					}
					else {
						if (field.FieldType == typeof(Vector2[]) || (field.FieldType == typeof(List<Vector2>)))
							return (Vector2)((IList)field.GetValue(targetObj))[indexArray];
						else
							return ((DIVariableVector2)((IList)field.GetValue(targetObj))[indexArray]).value;
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
						Debug.LogError("Get Vector2 not setup properly, please setup it first");
						return;
					}
				}
				if (indexArray == -1)
				{
					if (field.FieldType == typeof(Vector2))
						field.SetValue(targetObj, value);
					else
						((DIVariableVector2)field.GetValue(targetObj)).value = value;
				}
				else
				{
					if (field.FieldType == typeof(Vector2[]) || (field.FieldType == typeof(List<Vector2>)))
						((IList)field.GetValue(targetObj))[indexArray] = value;
					else
						((DIVariableVector2)((IList)field.GetValue(targetObj))[indexArray]).value = value;
				}
			}
		}
	}
}
