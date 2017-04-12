using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Animation (Mecanim)/Animator/Toggle Stabalize Feet"), USequencerEventHideDuration, USequencerFriendlyName("Toggle Stabalize Feet")]
	public class USToggleAnimatorStabalizeFeet : USEventBase
	{
		public bool stabalizeFeet = true;

		private bool prevStabalizeFeet;

		public override void FireEvent()
		{
			Animator component = base.AffectedObject.GetComponent<Animator>();
			if (!component)
			{
				Debug.LogWarning("Affected Object has no Animator component, for uSequencer Event", this);
				return;
			}
			this.prevStabalizeFeet = component.stabilizeFeet;
			component.stabilizeFeet = this.stabalizeFeet;
		}

		public override void ProcessEvent(float runningTime)
		{
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			Animator component = base.AffectedObject.GetComponent<Animator>();
			if (!component)
			{
				return;
			}
			component.stabilizeFeet = this.prevStabalizeFeet;
		}
	}
}
