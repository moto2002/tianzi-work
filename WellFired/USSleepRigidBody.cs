using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Physics/Sleep Rigid Body"), USequencerEventHideDuration, USequencerFriendlyName("Sleep Rigid Body")]
	public class USSleepRigidBody : USEventBase
	{
		public override void FireEvent()
		{
			Rigidbody component = base.AffectedObject.GetComponent<Rigidbody>();
			if (!component)
			{
				Debug.Log("Attempting to Nullify a force on an object, but it has no rigid body from USSleepRigidBody::FireEvent");
				return;
			}
			component.Sleep();
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
			component.WakeUp();
		}
	}
}
