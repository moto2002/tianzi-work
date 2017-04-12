using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Sequence/Stop And Skip"), USequencerEventHideDuration, USequencerFriendlyName("Stop and Skip sequencer")]
	public class USStopAndSkipToTimeSequenceEvent : USEventBase
	{
		[SerializeField]
		private USSequencer sequence;

		[SerializeField]
		private float timeToSkipTo;

		public override void FireEvent()
		{
			if (!this.sequence)
			{
				Debug.LogWarning("No sequence for USstopSequenceEvent : " + base.name, this);
			}
			if (this.sequence)
			{
				this.sequence.Stop();
				this.sequence.SkipTimelineTo(this.timeToSkipTo);
				this.sequence.UpdateSequencer(0f);
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
		}
	}
}
