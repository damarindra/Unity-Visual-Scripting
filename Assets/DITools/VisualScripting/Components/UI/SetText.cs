using UnityEngine;
using System.Collections;

namespace DI.VisualScripting {
	[VisualComponent("UI")]
	public class SetText : DIVisualComponent {

		public GetString getString;
		public UnityEngine.UI.Text text;

		protected override void WhatDo()
		{
			if (text != null)
				text.text = getString.value;
			Finish();
		}

		public override string windowName
		{
			get
			{
				return "Set Text";
			}
		}
	}
}
