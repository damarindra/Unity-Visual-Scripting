using UnityEngine;
using System.Collections;
using System.Reflection;

namespace DI.VisualScripting
{
	[System.Serializable]
	public class GetMethod {
		public GameObject target;
		public MonoBehaviour monoscript;
		public string methodName;
		public BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

		public MethodInfo methodInfo {
			get {
				if (monoscript != null)
					return monoscript.GetType().GetMethod(methodName);
				return null;
			}
		}
		
	}

}
