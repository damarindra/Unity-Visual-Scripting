using UnityEngine;
using System.Collections;
using System.Reflection;

namespace DI.VisualScripting
{
	[System.Serializable]
	public class GetVariableBase {
		public GetFrom getFrom;
		public GameObject target;
		public MonoBehaviour monoscript;

		public object targetObj
		{
			get
			{
				if (getFrom == GetFrom.Global)
					return DIGlobalVariable.Instance;
				else if (getFrom == GetFrom.GameObject)
					return monoscript;
				else
					return null;
			}
		}
	}
}
