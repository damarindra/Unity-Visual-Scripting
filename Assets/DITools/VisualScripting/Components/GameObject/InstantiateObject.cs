using UnityEngine;
using System.Collections;

namespace DI.VisualScripting {
	[VisualComponent("GameObject")]
	public class InstantiateObject : DIVisualComponent {

		public Object obj;

		protected override void WhatDo()
		{
			if (obj != null)
				Instantiate(obj);
		}
	}
}
