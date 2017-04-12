using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Transform/Parent and reset Transform"), USequencerEventHideDuration, USequencerFriendlyName("Parent and reset Transform")]
	public class USParentAndResetObjectEvent : USEventBase
	{
		public Transform parent;

		public Transform child;

		private Transform previousParent;

		private Vector3 previousPosition;

		private Quaternion previousRotation;

		public override void FireEvent()
		{
			this.previousParent = this.child.parent;
			this.previousPosition = this.child.localPosition;
			this.previousRotation = this.child.localRotation;
			this.child.parent = this.parent;
			this.child.localPosition = Vector3.zero;
			this.child.localRotation = Quaternion.identity;
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
			this.child.parent = this.previousParent;
			this.child.localPosition = this.previousPosition;
			this.child.localRotation = this.previousRotation;
		}

		private void OnDestroy()
		{
			this.parent = null;
			this.child = null;
			this.previousParent = null;
		}
	}
}
