using UnityEngine;
using System.Collections;
using System;

namespace DI.VisualScripting {
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class VSCustomPropertyDrawerAttribute : Attribute{

		public Type type;

		public VSCustomPropertyDrawerAttribute(Type type) {
			this.type = type;
		}
	}
}
