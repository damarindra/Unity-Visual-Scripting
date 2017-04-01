using UnityEngine;
using System.Collections;

namespace DI.VisualScripting
{
	[VisualComponent("Variable")]
	public class SetInt : DIVisualComponent {
		public GetInt getInt, setInt;

		protected override void WhatDo()
		{
			setInt.value = getInt.value;
			Finish();
		}
	}

}
