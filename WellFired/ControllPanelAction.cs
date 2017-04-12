using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Controll Panel Action"), USequencerFriendlyName("Controll Panel Action")]
	public class ControllPanelAction : ActionBase
	{
		public bool isCustomTime;

		public override void UpdateEvent()
		{
			if (!this.isCustomTime)
			{
				base.FireTime = 1f;
				base.Duration = base.Sequence.Duration - 1f;
			}
		}

		public override void FireEvent()
		{
			base.FireEvent();
			if (Application.isPlaying && Instance.Get<SequenceManager>().ShowControllPanel != null)
			{
				Instance.Get<SequenceManager>().ShowControllPanel();
			}
		}

		public override void EndEvent()
		{
			this.StopEvent();
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			if (Application.isPlaying && Instance.Get<SequenceManager>().HideControllPanel != null)
			{
				Instance.Get<SequenceManager>().HideControllPanel();
			}
		}
	}
}
