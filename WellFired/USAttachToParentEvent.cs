using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Attach/Attach To Parent"), USequencerEventHideDuration, USequencerFriendlyName("Attach Object To Parent")]
	public class USAttachToParentEvent : USEventBase
	{
		public Transform parentObject;

		private Transform originalParent;

		public override void FireEvent()
		{
			if (!this.parentObject)
			{
				Debug.Log("USAttachEvent has been asked to attach an object, but it hasn't been given a parent from USAttachEvent::FireEvent");
				return;
			}
			this.originalParent = base.AffectedObject.transform.parent;
			base.AffectedObject.transform.parent = this.parentObject;
		}

		private void OnDestroy()
		{
			this.parentObject = null;
			this.originalParent = null;
		}

		public override void ProcessEvent(float deltaTime)
		{
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			if (!base.AffectedObject)
			{
				return;
			}
			base.AffectedObject.transform.parent = this.originalParent;
		}
	}
}
