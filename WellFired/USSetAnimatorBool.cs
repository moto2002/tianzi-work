using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Animation (Mecanim)/Animator/Set Value/Bool"), USequencerEventHideDuration, USequencerFriendlyName("Set Mecanim Bool")]
	internal class USSetAnimatorBool : USEventBase
	{
		public string valueName = string.Empty;

		public bool Value = true;

		private bool prevValue;

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
			this.prevValue = component.GetBool(this.hash);
			component.SetBool(this.hash, this.Value);
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
			this.prevValue = component.GetBool(this.hash);
			component.SetBool(this.hash, this.Value);
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
			component.SetBool(this.hash, this.prevValue);
		}
	}
}
