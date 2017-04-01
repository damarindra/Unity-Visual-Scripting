using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DI.VisualScripting {
	public class DIAttributeData : ScriptableObject {

		public static DIAttributeData Instance {
			get {
				if (_instance != null)
					return _instance;
				_instance = CreateInstance(typeof(DIAttributeData)) as DIAttributeData;
#if UNITY_EDITOR
				if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/DITools/Resources/GamePreferences"))
					UnityEditor.AssetDatabase.CreateFolder("Assets/DITools/Resources", "GamePreferences");
				UnityEditor.AssetDatabase.CreateAsset(_instance, "Assets/DITools/Resources/GamePreferences/VSData.asset");
				UnityEditor.AssetDatabase.SaveAssets();
				UnityEditor.AssetDatabase.Refresh();
#endif
				return _instance;
			}
		}
		static DIAttributeData _instance = null;

		public List<Type> components = new List<Type>();
		public List<string> componentPathPopup = new List<string>();
		public List<Type> componentsWithAttribute = new List<Type>();
		public List<VSCustomPropertyDrawerAttributeData> propertyDrawers = new List<VSCustomPropertyDrawerAttributeData>();

		[Serializable]
		public class VSCustomPropertyDrawerAttributeData
		{
			public VSCustomPropertyDrawerAttribute att;
			public Type type;
			public VSPropertyDrawer instance;

			public VSCustomPropertyDrawerAttributeData(VSCustomPropertyDrawerAttribute att, Type type)
			{
				this.att = att;
				this.type = type;
				instance = Activator.CreateInstance(type) as VSPropertyDrawer;
			}
		}
	}
}
