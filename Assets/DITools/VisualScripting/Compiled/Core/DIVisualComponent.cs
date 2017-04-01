using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace DI.VisualScripting {
	[System.Serializable]
	public class DIVisualComponent : ScriptableObject{

		public DIVisualComponent next;
		public DIVisualComponent previous;
		protected bool isRunning = false;

		public void Do() {
			isRunning = true;
			WhatDo();
		}
		protected virtual void WhatDo() {

		}

		protected void Finish() {
			//set current execution to next
			if (next != null) {
				next.Do();
			}
			isRunning = false;
		}
		public virtual string windowName { get { return "Component"; } }

		#region WINDOW
		[HideInInspector]
		public Rect position = new Rect(0, 0, 200, 45);
		public delegate void OnRemoved();
		public OnRemoved onBeforeRemoved;
		[HideInInspector]
		public bool withoutComponentType = false;
		[HideInInspector]
		private DIVisualComponent _tempReplaceComp;
		[HideInInspector]
		public bool isMinimize = false;
		[HideInInspector]
		public bool isRemoveAction = false;
		[HideInInspector]
		public bool isRoot = false;

		void ComponentTypePopup()
		{
			_tempReplaceComp = this.ComponentChooser();
			if (_tempReplaceComp != null)
			{
				if (_tempReplaceComp.GetType() != GetType())
					DIVisualScriptingData.atTheEnd += changeThisComponentWithDestroy;
			}

		}

		/// <summary>
		/// Build Window only works on Editor, so be carefull or your project cannot build
		/// </summary>
		public void BuildWindow(int id)
		{
			if (isMinimize)
			{
				GUILayout.Space(13);
				GUI.DragWindow();
				return;
			}
			//position.size = new Vector2(200, 62);
			if (!withoutComponentType)
				ComponentTypePopup();

			BuildWindow();
			ConnectButton();

			GUI.DragWindow();
		}

		/// <summary>
		/// Change the current component with _tempReplaceComp and delete existing.
		/// </summary>
		public void changeThisComponentWithDestroy()
		{
			changeThisComponent(_tempReplaceComp);
			ScriptableObject.DestroyImmediate(this, true);
		}

		/// <summary>
		/// Change the current component without delete existing
		/// </summary>
		public void changeThisComponent(DIVisualComponent withThis)
		{
			withThis.position = position;

			//Set next Component neighbor
			if (next != null)
			{
				withThis.next = next;
				next.previous = withThis;
			}

			//if previous not null, set the previous component next to new component
			if (previous != null)
			{
				withThis.previous = previous;
				if (previous.next == this)
					previous.next = withThis;
				else
				{
					//use refelection to get real field name, because on previous component not always connected to next, like DICompare, can connected to true event
					foreach (FieldInfo info in previous.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
					{
						//looping with fieldinfo of previous component
						//if fieldInfo is DIVisualComponent
						if (info.FieldType == typeof(DIVisualComponent) || info.FieldType.IsSubclassOf(typeof(DIVisualComponent)))
						{
							//Cast as DIVC
							DIVisualComponent target = info.GetValue(previous) as DIVisualComponent;
							//Check if target is same with this component
							if (target == this)
							{
								//true means FieldInfo connected with this
								//so we need to set the field info with the newComp
								info.SetValue(previous, withThis);
								break;
							}
						}
						else if (info.FieldType == typeof(List<DIVisualComponent>))
						{
							List<DIVisualComponent> comps = info.GetValue(previous) as List<DIVisualComponent>;
							for (int i = 0; i < comps.Count; i++)
							{
								if (comps[i] == this)
								{
									comps[i] = withThis;
									break;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Find child and destroy child and component parameter
		/// </summary>
		/// <param name="component">parent, will be destroy with all child connected</param>
		public void FindChildAndDestroy() {
			if(next != null)
				next.FindChildAndDestroy();
			RemoveComponent();
		}
		#endregion
		#region VIRTUAL WINDOW
		/// <summary>
		/// This method only works at Unity Editor, warp it with define UnityEditor
		/// <para/>#if UNITY_EDITOR
		/// <para/>BuildWindow(){}
		/// <para/>#endif
		/// </summary>
		public virtual void BuildWindow()
		{
			//check if this type is not EQUAL DIVisualComponent
			if (GetType() != typeof(DIVisualComponent))
				DrawDefaultWindow();
		}
		public virtual void ConnectButton()
		{
			ConnectButtonFunc(ref next, "Next Component", "Insert Component");
		}
		/// <summary>
		/// Create button to connect other Visual Component
		/// </summary>
		/// <param name="comp">Component will be connected as child, null to make creation mode, not null to insert mode</param>
		/// <param name="nextStr">Next button name</param>
		/// <param name="insertStr">Insert button name</param>
		protected void ConnectButtonFunc(ref DIVisualComponent comp, string nextStr, string insertStr, GUIStyle style = null)
		{
			if (style == null)
				style = new GUIStyle(GUI.skin.button);
			if (comp == null)
			{
				if (GUILayout.Button(nextStr, style))
				{
					//DIVisualScriptingWindow.Window.inspectTarget.start.components.Add(DIVisualComponent.CreateInstance<DIVisualComponent>());
					comp = (DIVisualComponent.CreateInstance<DIVisualComponent>());
					comp.previous = this;
					comp.position = position;
					comp.position.position += Vector2.up * (position.height + 45);
				}
			}
			else
			{
				if (GUILayout.Button(insertStr, style))
				{
					//DIVisualScriptingWindow.Window.inspectTarget.start.components.Add(DIVisualComponent.CreateInstance<DIVisualComponent>());
					DIVisualComponent _backupNext = comp;
					comp = (DIVisualComponent.CreateInstance<DIVisualComponent>());
					comp.previous = this;
					comp.position = position;
					comp.position.position += Vector2.up * (position.height + 45);
					comp.next = _backupNext;
					comp.next.previous = comp;
					if (_backupNext.position.y == comp.position.y)
					{
						_backupNext.position.y += 40;
					}
				}
			}
		}
		public void RemoveAction(int id)
		{
			if (isRoot)
			{
				DIComponentHelper.EGLLabel("Remove include child?", DIComponentHelper.boldLabel);
				DIComponentHelper.EGLBeginHorizontal();
				if (GUILayout.Button("With Childs"))
				{
					DIVisualScriptingData.atTheEnd += OnRemoveButton;
					DIVisualScriptingData.atTheEnd += (() => {
						FindChildAndDestroy();
					});
				}
				if (GUILayout.Button("No"))
					isRemoveAction = false;
				DIComponentHelper.EGLEndHorizontal();
			}
			else
			{
				DIComponentHelper.EGLLabel("Remove component?", DIComponentHelper.boldLabel);
				DIComponentHelper.EGLBeginHorizontal();
				if (GUILayout.Button("Yes"))
				{
					if (next != null && previous != null)
					{
						next.previous = previous;
						previous.next = next;
					}
					DIVisualScriptingData.atTheEnd += (() => { DestroyImmediate(this, true); });
				}
				if (GUILayout.Button("With Childs"))
				{
					DIVisualScriptingData.atTheEnd += OnRemoveButton;
					DIVisualScriptingData.atTheEnd += (() => {
						FindChildAndDestroy();
					});
				}
				if (GUILayout.Button("No"))
					isRemoveAction = false;

				DIComponentHelper.EGLEndHorizontal();
			}

			GUI.DragWindow();
		}
		/// <summary>
		/// Func look like listener when remove button clicked
		/// </summary>
		public virtual void OnRemoveButton() {

		}
		public virtual void RemoveComponent() {
			if (onBeforeRemoved != null)
				onBeforeRemoved();
#if UNITY_EDITOR
			//UnityEditor.Undo.DestroyObjectImmediate(this);
#endif
			ScriptableObject.DestroyImmediate(this, true);
		}
		/// <summary>
		/// Save Component to disk or scene
		/// </summary>
		/// <param name="rootParent">Root, can be .asset / prefab / null</param>
		/// <param name="previous">fill only if you want to duplicate mode</param>
		public virtual void SaveComponent(UnityEngine.Object rootParent, DIVisualComponent previous) {
			this._SaveComponent(rootParent, previous);	
		}
		public virtual void LateShowWindow()
		{
			if (next != null) {
				Vector2 fromPos = position.position + Vector2.right * position.width / 2 + Vector2.up * position.height;
				this.DrawConnectLine(fromPos, next);
			}
		}
		#endregion

		#region Drawer Property
		protected void DrawDefaultWindow(bool ignoreNotSupportedType = false) {
			FieldInfo[] infos = DIComponentHelper.GatherFieldInfos(this);
			for (int i = 0; i < infos.Length; i++) {
				DIComponentHelper.DrawField(infos[i], this, ignoreNotSupportedType);
			}
		}
		#endregion
	}
}
