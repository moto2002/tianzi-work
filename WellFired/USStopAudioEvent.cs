using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Audio/Stop Audio"), USequencerEventHideDuration, USequencerFriendlyName("Stop Audio")]
	public class USStopAudioEvent : USEventBase
	{
		public override void FireEvent()
		{
			if (!base.AffectedObject)
			{
				Debug.Log("USSequencer is trying to play an audio clip, but you didn't give it Audio To Play from USPlayAudioEvent::FireEvent");
				return;
			}
			AudioSource component = base.AffectedObject.GetComponent<AudioSource>();
			if (!component)
			{
				Debug.Log("USSequencer is trying to play an audio source, but the GameObject doesn't contain an AudioClip from USPlayAudioEvent::FireEvent");
				return;
			}
			component.Stop();
		}

		public override void ProcessEvent(float deltaTime)
		{
		}
	}
}
