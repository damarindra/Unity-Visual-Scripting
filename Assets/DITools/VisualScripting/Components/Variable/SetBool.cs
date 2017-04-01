using UnityEngine;
using System.Collections;

namespace DI.VisualScripting {
	[VisualComponent("Variable")]
	public class SetBool : DIVisualComponent{

		public GetBool getBool, storeBool;

		protected override void WhatDo()
		{
			storeBool.value = getBool.value;
			Finish();
		}
	}
}
