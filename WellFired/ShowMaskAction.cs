using System;

namespace WellFired
{
	[USequencerEvent("Custom Event/Show Mask Action"), USequencerFriendlyName("Show Mask Action")]
	public class ShowMaskAction : ActionBase
	{
		public override void FireEvent()
		{
			if (Instance.Get<SequenceManager>().ShowMaskPanel != null)
			{
				Instance.Get<SequenceManager>().ShowMaskPanel();
			}
		}
	}
}
