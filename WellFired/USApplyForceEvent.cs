using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Physics/Apply Force"), USequencerEventHideDuration, USequencerFriendlyName("Apply Force")]
	public class USApplyForceEvent : USEventBase
	{
		public Vector3 direction = Vector3.up;

		public float strength = 1f;

		public ForceMode type = ForceMode.Impulse;

		private Transform previousTransform;

		private void OnDestroy()
		{
			this.previousTransform = null;
		}

		public override void FireEvent()
		{
			Rigidbody component = base.AffectedObject.GetComponent<Rigidbody>();
			if (!component)
			{
				Debug.Log("Attempting to apply an impulse to an object, but it has no rigid body from USequencerApplyImpulseEvent::FireEvent");
				return;
			}
			component.AddForceAtPosition(this.direction * this.strength, base.transform.position, this.type);
			this.previousTransform = component.transform;
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
			Rigidbody component = base.AffectedObject.GetComponent<Rigidbody>();
			if (!component)
			{
				return;
			}
			component.Sleep();
			if (this.previousTransform)
			{
				base.AffectedObject.transform.position = this.previousTransform.position;
				base.AffectedObject.transform.rotation = this.previousTransform.rotation;
			}
		}
	}
}
