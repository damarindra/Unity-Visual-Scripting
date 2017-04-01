using UnityEngine;
using System.Collections;

namespace DI.VisualScripting {
	[VisualComponent("Variable")]
	public class FloatToString : DIVisualComponent{

		public GetFloat getFloat;
		public GetString getString;
		public string stringFormat;

		protected override void WhatDo()
		{
			getString.value = getFloat.value.ToString(stringFormat);
			Finish();
		}

	}
}
