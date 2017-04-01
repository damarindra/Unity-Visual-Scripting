using UnityEngine;
using System.Collections;

namespace DI.VisualScripting
{
	[VisualComponent("Misc")]
	public class DIDebugLog : DIVisualComponent {
		public string Message;

		protected override void WhatDo()
		{
			Debug.Log(Message);
		}

		public override string windowName
		{
			get
			{
				return "Debug Log"; 
			}
		}
	}
}
