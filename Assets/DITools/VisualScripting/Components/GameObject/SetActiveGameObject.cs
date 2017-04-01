using UnityEngine;
using System.Collections;

namespace DI.VisualScripting {
	[VisualComponent("GameObject")]
	public class SetActiveGameObject : DIVisualComponent {

		public GetGameObject gameObject;
		public bool active;

		protected override void WhatDo()
		{
			if (gameObject.value != null)
				gameObject.value.SetActive(active);
			Finish();
		}
	}
}
