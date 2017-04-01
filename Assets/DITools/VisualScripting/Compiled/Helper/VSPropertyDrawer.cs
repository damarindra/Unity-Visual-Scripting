using UnityEngine;
using System.Collections;
using System.Reflection;

namespace DI.VisualScripting {
	public class VSPropertyDrawer {

		public VSPropertyDrawer() { }

		public virtual void PropertyDrawer(FieldInfo field, DIVisualComponent comp) {

		}
		public virtual object _PropertyDrawer(object value) {
			return value;
		}
	}
}
