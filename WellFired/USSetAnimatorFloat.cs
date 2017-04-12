using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Animation (Mecanim)/Animator/Set Value/Float"), USequencerEventHideDuration, USequencerFriendlyName("Set Mecanim Float")]
	public class USSetAnimatorFloat : USEventBase
	{
		public string valueName = string.Empty;

		public float Value;

		private float prevValue;

		private int hash;

		public override void FireEvent()
		{
			Animator component = base.AffectedObject.GetComponent<Animator>();
			if (!component)
			{
				Debug.LogWarning("Affected Object has no Animator component, for uSequencer Event", this);
				return;
			}
			if (this.valueName.Length == 0)
			{
				Debug.LogWarning("Invalid name passed to the uSequencer Event Set Float", this);
				return;
			}
			this.hash = Animator.StringToHash(this.valueName);
			this.prevValue = component.GetFloat(this.hash);
			component.SetFloat(this.hash, this.Value);
		}

		public override void ProcessEvent(float runningTime)
		{
			Animator component = base.AffectedObject.GetComponent<Animator>();
			if (!component)
			{
				Debug.LogWarning("Affected Object has no Animator component, for uSequencer Event", this);
				return;
			}
			if (this.valueName.Length == 0)
			{
				Debug.LogWarning("Invalid name passed to the uSequencer Event Set Float", this);
				return;
			}
			this.hash = Animator.StringToHash(this.valueName);
			this.prevValue = component.GetFloat(this.hash);
			component.SetFloat(this.hash, this.Value);
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
			if (this.valueName.Length == 0)
			{
				return;
			}
			component.SetFloat(this.hash, this.prevValue);
		}
	}
}
