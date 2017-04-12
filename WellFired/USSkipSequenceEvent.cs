using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Sequence/Skip uSequence"), USequencerEventHideDuration, USequencerFriendlyName("Skip uSequence")]
	public class USSkipSequenceEvent : USEventBase
	{
		public USSequencer sequence;

		public bool skipToEnd = true;

		public float skipToTime = -1f;

		public override void FireEvent()
		{
			if (!this.sequence)
			{
				Debug.LogWarning("No sequence for USSkipSequenceEvent : " + base.name, this);
				return;
			}
			if (!this.skipToEnd && this.skipToTime < 0f && this.skipToTime > this.sequence.Duration)
			{
				Debug.LogWarning("You haven't set the properties correctly on the Sequence for this USSkipSequenceEvent, either the skipToTime is invalid, or you haven't flagged it to skip to the end", this);
				return;
			}
			if (this.skipToEnd)
			{
				this.sequence.SkipTimelineTo(this.sequence.Duration);
			}
			else
			{
				this.sequence.SkipTimelineTo(this.skipToTime);
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
		}
	}
}
