using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Animation (Mecanim)/Animator/Set Layer Weight"), USequencerEventHideDuration, USequencerFriendlyName("Set Layer Weight")]
	public class USSetAnimatorLayerWeight : USEventBase
	{
		public float layerWeight = 1f;

		public int layerIndex = -1;

		private float prevLayerWeight;

		public override void FireEvent()
		{
			Animator component = base.AffectedObject.GetComponent<Animator>();
			if (!component)
			{
				Debug.LogWarning("Affected Object has no Animator component, for uSequencer Event", this);
				return;
			}
			if (this.layerIndex < 0)
			{
				Debug.LogWarning("Set Animator Layer weight, incorrect index : " + this.layerIndex);
				return;
			}
			this.prevLayerWeight = component.GetLayerWeight(this.layerIndex);
			component.SetLayerWeight(this.layerIndex, this.layerWeight);
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
			if (this.layerIndex < 0)
			{
				return;
			}
			component.SetLayerWeight(this.layerIndex, this.prevLayerWeight);
		}
	}
}
