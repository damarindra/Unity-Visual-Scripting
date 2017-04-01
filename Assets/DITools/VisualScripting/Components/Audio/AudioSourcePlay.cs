using UnityEngine;
using System.Collections;

namespace DI.VisualScripting
{
	[VisualComponent("Audio")]
	public class AudioSourcePlay : DIVisualComponent{

		public AudioSource audioSource;
		public AudioClip clip;

		protected override void WhatDo()
		{
			if (audioSource != null) {
				audioSource.clip = clip;
				audioSource.Play();
			}
			Finish();
		}

		public override string windowName
		{
			get
			{
				return "Audio Source : Play";
			}
		}
	}

}
