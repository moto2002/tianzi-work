using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Audio/Pause Or Resume Audio"), USequencerEventHideDuration, USequencerFriendlyName("Pause Or Resume Audio")]
	public class USPauseResumeAudioEvent : USEventBase
	{
		public bool pause = true;

		public override void FireEvent()
		{
			if (!base.AffectedObject)
			{
				Debug.Log("USSequencer is trying to play an audio clip, but you didn't give it Audio To Play from USPauseAudioEvent::FireEvent");
				return;
			}
			AudioSource component = base.AffectedObject.GetComponent<AudioSource>();
			if (!component)
			{
				Debug.Log("USSequencer is trying to play an audio source, but the GameObject doesn't contain an AudioClip from USPauseAudioEvent::FireEvent");
				return;
			}
			if (this.pause)
			{
				component.Pause();
			}
			if (!this.pause)
			{
				component.Play();
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
			AudioSource component = base.AffectedObject.GetComponent<AudioSource>();
			if (!component)
			{
				Debug.Log("USSequencer is trying to play an audio source, but the GameObject doesn't contain an AudioClip from USPauseAudioEvent::FireEvent");
				return;
			}
			if (component.isPlaying)
			{
				return;
			}
		}
	}
}
