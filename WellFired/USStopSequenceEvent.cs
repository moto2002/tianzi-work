using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Sequence/Stop uSequence"), USequencerEventHideDuration, USequencerFriendlyName("stop uSequence")]
	public class USStopSequenceEvent : USEventBase
	{
		public USSequencer sequence;

		public override void FireEvent()
		{
			if (!this.sequence)
			{
				Debug.LogWarning("No sequence for USstopSequenceEvent : " + base.name, this);
			}
			if (this.sequence)
			{
				this.sequence.Stop();
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
		}
	}
}
