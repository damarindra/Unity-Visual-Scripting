#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace DI.VisualScripting {
	[VSCustomPropertyDrawer(typeof(GetMethod))]
	public class GetMethodPropertyDrawer : VSPropertyDrawer{

		public override void PropertyDrawer(FieldInfo field, DIVisualComponent comp)
		{
			GetMethod _method = field.GetValue(comp) as GetMethod;

			//shifting to left
			_method.bindingFlags = (BindingFlags)((int)_method.bindingFlags << 1);
			_method.bindingFlags = (System.Reflection.BindingFlags)EditorGUILayout.MaskField(new GUIContent("Binding Flags"), (int)_method.bindingFlags, System.Enum.GetNames(typeof(BindingFlags)));
			//shifting to right, to the right value
			_method.bindingFlags = (BindingFlags)((int)_method.bindingFlags >> 1);

			GameObject oldGo = _method.target;
			_method.target = EditorGUILayout.ObjectField("Target", _method.target, typeof(GameObject), true) as GameObject;
			if (oldGo != _method.target)
				_method.monoscript = null;
			if (_method.target != null) {
				//Draw Monobehaviour
				var monoBehaviours = _method.target.GetComponents<MonoBehaviour>().ToList();
				List<string> monoBehaviourNames = new List<string>();
				for (int i = 0; i < monoBehaviours.Count; i++) {
					monoBehaviourNames.Add("(" + i.ToString() + ")" + monoBehaviours[i].ToString().Replace(_method.target.name, ""));
				}
				int monoscriptIdx = monoBehaviours.IndexOf(_method.monoscript);
				if (monoscriptIdx < 0)
					monoscriptIdx = 0;
				if (monoBehaviourNames.Count == 0)
					monoBehaviourNames.Add("");
				monoscriptIdx = EditorGUILayout.Popup("Script", monoscriptIdx, monoBehaviourNames.ToArray());
				if (monoBehaviours.Count != 0)
					_method.monoscript = monoBehaviours[monoscriptIdx];
			}

			if (_method.monoscript != null) {
				//draw method
				var methodNames = _method.monoscript.GetType().GetMethods(_method.bindingFlags).Select(i => i.Name).ToList();
				int methodIdx = methodNames.IndexOf(_method.methodName);
				if (methodIdx == -1)
					methodIdx = 0;
				if (methodNames.Count == 0)
					methodNames.Add("");
				methodIdx = EditorGUILayout.Popup("Method", methodIdx, methodNames.ToArray());
				_method.methodName = methodNames[methodIdx];

			}

			field.SetValue(comp, _method);
		}
	}

}
#endif