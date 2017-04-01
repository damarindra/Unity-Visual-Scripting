using UnityEngine;
using System.Collections;
using System;

namespace DI.VisualScripting{
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class VisualComponentAttribute : PropertyAttribute {
		public string path;
		
		public VisualComponentAttribute(string path){
			this.path = path;
		}
	}
}
