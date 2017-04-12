using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Recording/Start Recording"), USequencerEventHideDuration, USequencerFriendlyName("Start Recording")]
	public class USStartRecordingEvent : USEventBase
	{
		public override void FireEvent()
		{
			if (!Application.isPlaying)
			{
				Debug.Log("Recording events only work when in play mode");
				return;
			}
			USRuntimeUtility.StartRecordingSequence(base.Sequence, USRecordRuntimePreferences.CapturePath, USRecord.GetFramerate(), USRecord.GetUpscaleAmount());
		}

		public override void ProcessEvent(float deltaTime)
		{
		}
	}
}
