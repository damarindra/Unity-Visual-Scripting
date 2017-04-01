using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DI.VisualScripting {
	[VisualComponent("Misc")]
	public class DIBranch : DIVisualComponent {

		public List<DIVisualComponent> splitedComponent = new List<DIVisualComponent>();

		protected override void WhatDo()
		{
			if (next != null)
				next.Do();
			foreach (DIVisualComponent splitComp in splitedComponent)
				splitComp.Do();
		}

		public override string windowName
		{
			get
			{
				return "Branch";
			}
		}

#if UNITY_EDITOR
		public override void BuildWindow()
		{
			if (onBeforeRemoved == null) {
				onBeforeRemoved = (() => {
					for (int i = 0; i < splitedComponent.Count; i++)
					{
						if (splitedComponent[i] != null)
							splitedComponent[i].FindChildAndDestroy();
					}
				});
			}

			UnityEditor.EditorGUILayout.BeginHorizontal();
			GUI.enabled = false;
			int count = next != null ? 1 + splitedComponent.Count : splitedComponent.Count;
			UnityEditor.EditorGUILayout.IntField("Count", count);
			GUI.enabled = true;
			if (GUILayout.Button("+")) {
				if (next == null)
					next = AddBranch();
				else
					splitedComponent.Add(AddBranch());
			}
			if (GUILayout.Button("-")) {
				if (splitedComponent.Count == 0)
					next.FindChildAndDestroy();
				else
					RemoveBranch();
			}
			UnityEditor.EditorGUILayout.EndHorizontal();
			
			GUI.DragWindow();
		}

		public override void LateShowWindow()
		{
			if(next != null)
				this.DrawConnectLine((1) / ((float)splitedComponent.Count + 2), next);
			for (int i = 0; i < splitedComponent.Count; i++)
			{
				if (splitedComponent[i] != null)
				{
					splitedComponent[i].ShowWindow();
					this.DrawConnectLine((i + 2) / ((float)splitedComponent.Count + 2), splitedComponent[i]);
				}
				else {
					RemoveBranch(i);
				}
			}
		}
		public override void ConnectButton()
		{	}
		public override DIVisualComponent SaveComponent(UnityEngine.Object rootParent, DIVisualComponent previous = null, bool autoAssignNextComponentOfPrevious = true)
		{
			DIBranch _branch = base.SaveComponent(rootParent, previous, autoAssignNextComponentOfPrevious) as DIBranch;
			for (int i = 0; i < splitedComponent.Count; i++)
			{
				if (splitedComponent[i] != null)
				{
					//splitedComponent[i].SaveComponent(rootParent, this);
					_branch.splitedComponent[i] = splitedComponent[i].SaveComponent(rootParent, _branch, false);
				}
				else
				{
					RemoveBranch(i);
				}
			}
			return _branch;
		}
		DIVisualComponent AddBranch() {
			DIVisualComponent comp = (DIVisualComponent.CreateInstance<DIVisualComponent>());
			comp.previous = this;
			comp.position = position;
			comp.position.position += Vector2.up * (position.height + 45);
			return comp;
		}

		void RemoveBranch() {
			DIVisualComponent removedComp = splitedComponent[splitedComponent.Count - 1];
			if (removedComp != null)
				removedComp.FindChildAndDestroy();
			splitedComponent.RemoveAt(splitedComponent.Count - 1);
		}
		void RemoveBranch(int index)
		{
			DIVisualComponent removedComp = splitedComponent[index];
			if (removedComp != null)
				removedComp.FindChildAndDestroy();
			splitedComponent.RemoveAt(index);
		}
#endif

	}
}
