using UnityEngine;
using System.Collections;

namespace DI.VisualScripting
{
	[VisualComponent("Variable")]
	public class SetFloat : DIVisualComponent {

		public GetFloat getFloat, setFloat;

		protected override void WhatDo()
		{
			setFloat.value = getFloat.value;
			Finish();
		}

	}

}
