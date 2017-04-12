using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Sequence/Pause uSequence"), USequencerEventHideDuration, USequencerFriendlyName("Pause uSequence")]
	public class USPauseSequenceEvent : USEventBase
	{
		public USSequencer sequence;

		public override void FireEvent()
		{
			if (!this.sequence)
			{
				Debug.LogWarning("No sequence for USPauseSequenceEvent : " + base.name, this);
			}
			if (this.sequence)
			{
				this.sequence.Pause();
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
		}
	}
}
