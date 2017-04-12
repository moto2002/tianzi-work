using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Recording/Stop Recording"), USequencerEventHideDuration, USequencerFriendlyName("Stop Recording")]
	public class USStopRecordingEvent : USEventBase
	{
		public override void FireEvent()
		{
			if (!Application.isPlaying)
			{
				Debug.Log("Recording events only work when in play mode");
				return;
			}
			USRuntimeUtility.StopRecordingSequence();
		}

		public override void ProcessEvent(float deltaTime)
		{
		}
	}
}
