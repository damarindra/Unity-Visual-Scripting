using UnityEngine;
using System.Collections;

namespace DI.VisualScripting
{
	[VisualComponent("Script")]
	public class CallMethod : DIVisualComponent{

		public GetMethod getMethod;

		protected override void WhatDo()
		{
			if (getMethod.methodInfo != null)
				getMethod.methodInfo.Invoke(getMethod.monoscript, null);
			Finish();
		}
	}

}
