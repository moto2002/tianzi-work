using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Sequence/Set Playback Rate"), USequencerEventHideDuration, USequencerFriendlyName("Set uSequence Playback Rate")]
	public class USSetPlaybackRateEvent : USEventBase
	{
		public USSequencer sequence;

		public float playbackRate = 1f;

		private float prevPlaybackRate = 1f;

		public override void FireEvent()
		{
			if (!this.sequence)
			{
				Debug.LogWarning("No sequence for USSetPlaybackRate : " + base.name, this);
			}
			if (this.sequence)
			{
				this.prevPlaybackRate = this.sequence.PlaybackRate;
				this.sequence.PlaybackRate = this.playbackRate;
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			if (this.sequence)
			{
				this.sequence.PlaybackRate = this.prevPlaybackRate;
			}
		}
	}
}
