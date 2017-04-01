using UnityEngine;
using System.Collections;

namespace DI.VisualScripting {
	[VisualComponent("Variable")]
	public class BoolCompare : DIVisualComponent {

		public GetBool getBool;
		public DIVisualComponent trueEvent, falseEvent;

		public override string windowName
		{
			get
			{
				return "Compare Bool";
			}
		}

		protected override void WhatDo()
		{
			if (getBool.value && trueEvent != null)
				trueEvent.Do();
			else if (!getBool.value && falseEvent != null)
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

		public override DIVisualComponent SaveComponent(Object rootParent, DIVisualComponent previous, bool autoAssignNextComponentOfPrevious = true)
		{
			BoolCompare result = base.SaveComponent(rootParent, previous, autoAssignNextComponentOfPrevious) as BoolCompare;
			if (trueEvent != null)
				result.trueEvent = trueEvent.SaveComponent(rootParent, result, false);
			if (falseEvent != null)
				result.falseEvent = falseEvent.SaveComponent(rootParent, result, false);
			return result;
		}

#endif
	}
}
