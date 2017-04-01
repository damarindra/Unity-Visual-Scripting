#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace DI.VisualScripting {
	public class GetVariableBaseDrawer : VSPropertyDrawer {

		public override void PropertyDrawer(FieldInfo field, DIVisualComponent comp)
		{
			EditorGUILayout.LabelField(field.Name, EditorStyles.boldLabel);
			GetVariableBase _base = field.GetValue(comp) as GetVariableBase;
			DrawBaseVariable(_base);
		}

		//When edit this, please edit DrawBaseVariable in BuildString
		public static void DrawBaseVariable(GetVariableBase _base) {
			_base.getFrom = (GetFrom)EditorGUILayout.EnumPopup("Source", _base.getFrom);
			if (_base.getFrom == GetFrom.GameObject)
			{
				_base.target = (GameObject)EditorGUILayout.ObjectField("Target", _base.target, typeof(GameObject), true);

				if (_base.target != null)
				{
					//Show MonoScript
					var monobehaviours = _base.target.GetComponents<MonoBehaviour>().ToList();
					int monoIdx = monobehaviours.IndexOf(_base.monoscript);
					List<string> monobehaviourString = new List<string>();
					for (int i = 0; i < monobehaviours.Count; i++)
					{
						monobehaviourString.Add("(" + i.ToString() + ")" + monobehaviours[i].ToString().Replace(_base.target.name, ""));
					}
					if (monobehaviourString.Count < 0)
						monobehaviourString.Add("");
					if (monoIdx < 0)
						monoIdx = 0;
					monoIdx = EditorGUILayout.Popup("Script", monoIdx, monobehaviourString.ToArray());
					_base.monoscript = monobehaviours[monoIdx];
				}
			}
		}
	}
}
#endif