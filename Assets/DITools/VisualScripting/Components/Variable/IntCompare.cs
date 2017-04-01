using UnityEngine;
using System.Collections;

namespace DI.VisualScripting {
	[VisualComponent("Variable")]
	public class IntCompare : DIVisualComponent {

		public CompareType compareType = CompareType.Equal;
		public GetInt variable1;
		public GetInt variable2;
		public DIVisualComponent trueEvent, falseEvent;

		public override string windowName
		{
			get
			{
				return "Compare Int";
			}
		}

		protected override void WhatDo()
		{
			bool result = false;
			if (compareType == CompareType.Equal)
				result = variable1.value == variable2.value;
			else if (compareType == CompareType.NotEqual)
				result = variable1.value != variable2.value;
			else if (compareType == CompareType.Greater)
				result = variable1.value > variable2.value;
			else if (compareType == CompareType.Less)
				result = variable1.value < variable2.value;
			else if (compareType == CompareType.GreaterEqual)
				result = variable1.value >= variable2.value;
			else if (compareType == CompareType.LessEqual)
				result = variable1.value <= variable2.value;

			if (result && trueEvent != null)
				trueEvent.Do();
			else if (!result && falseEvent != null)
				falseEvent.Do();

			if (next != null)
				next.Do();
		}

#if UNITY_EDITOR

		public override void LateShowWindow()
		{
			base.LateShowWindow();
			if (trueEvent != null) {
				trueEvent.ShowWindow();
				this.DrawConnectLine(.2f, trueEvent);
			}
			if (falseEvent != null) {
				falseEvent.ShowWindow();
				this.DrawConnectLine(.8f, falseEvent);
			}
		}

		public override void ConnectButton()
		{
			UnityEditor.EditorGUILayout.BeginHorizontal();
			ConnectButtonFunc(ref trueEvent, "True", "True");
			ConnectButtonFunc(ref next, "Next", "Insert");
			ConnectButtonFunc(ref falseEvent, "False", "False");
			UnityEditor.EditorGUILayout.EndHorizontal();
		}

		public override void SaveComponent(Object rootParent, DIVisualComponent previous)
		{
			base.SaveComponent(rootParent, previous);
			if (trueEvent != null)
				trueEvent.SaveComponent(rootParent, previous);
			if (falseEvent != null)
				falseEvent.SaveComponent(rootParent, previous);
		}

#endif
	}
}
