using UnityEngine;
using System.Collections;

namespace DI.VisualScripting
{
	[VisualComponent("Audio")]
	public class AudioSourceStop : DIVisualComponent{

		public AudioSource audioSource;

		protected override void WhatDo()
		{
			if (audioSource != null) {
				audioSource.Stop();
			}
			Finish();
		}

		public override string windowName
		{
			get
			{
				return "Audio Source : Stop";
			}
		}
	}

}
