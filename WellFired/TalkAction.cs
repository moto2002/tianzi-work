using System;

namespace WellFired
{
	[USequencerEvent("Custom Event/Talk Action"), USequencerFriendlyName("Talk Action")]
	public class TalkAction : ActionBase
	{
		public string talkId;

		public override void FireEvent()
		{
			base.FireEvent();
			if (Instance.Get<SequenceManager>().isPlaying && !string.IsNullOrEmpty(this.talkId) && Instance.Get<SequenceManager>().ShowDailogPanel != null)
			{
				Instance.Get<SequenceManager>().ShowDailogPanel(this.talkId);
			}
		}

		public override void StopEvent()
		{
			if (Instance.Get<SequenceManager>().HideDailogPanel != null)
			{
				Instance.Get<SequenceManager>().HideDailogPanel();
			}
		}
	}
}
