using UnityEngine;
using System.Collections;

namespace DI.VisualScripting
{
	[VisualComponent("Variable")]
	public class SetString : DIVisualComponent{

		public GetString getString, setString;

		protected override void WhatDo()
		{
			setString.value = getString.value;
			Finish();
		}

	}

}
