using System;

namespace WellFired
{
	[USequencerEvent("Debug/Log Message"), USequencerEventHideDuration, USequencerFriendlyName("Debug Message")]
	public class USMessageEvent : USEventBase
	{
		public string message = "Default Message";

		public override void FireEvent()
		{
		}

		public override void ProcessEvent(float deltaTime)
		{
		}
	}
}
