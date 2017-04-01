using UnityEngine;
using System.Collections;

namespace DI.VisualScripting
{
	[VisualComponent("Variable")]
	public class IntToString : DIVisualComponent {
		public GetInt getInt;
		public GetString storeString;

		protected override void WhatDo()
		{
			storeString.value = getInt.value.ToString();
			Finish();
		}
	}

}
