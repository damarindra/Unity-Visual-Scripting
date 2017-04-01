using UnityEngine;
using System.Collections;

namespace DI.VisualScripting
{
	[VisualComponent("GameObject")]
	public class InstantiateGameObject : DIVisualComponent {
		public GetGameObject gameObject;
		public GetVector3 _position, rotationEuler;

		protected override void WhatDo()
		{
			if (gameObject.value != null)
				Instantiate(gameObject.value, _position.value, Quaternion.Euler(rotationEuler.value));
			Finish();
		}
	}
}
