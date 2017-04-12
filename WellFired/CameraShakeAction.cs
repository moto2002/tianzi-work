using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Camera Shake Action"), USequencerFriendlyName("Camera Shake Action")]
	public class CameraShakeAction : ActionBase
	{
		private _CameraShake shake;

		public int numberOfShakes;

		public Vector3 shakeAmount;

		public Vector3 rotateAmount;

		public float distance;

		public float speed;

		public float decay;

		public override void FireEvent()
		{
			this.shake = base.AffectedObject.GetComponent<_CameraShake>();
			if (this.shake == null)
			{
				this.shake = base.AffectedObject.AddComponent<_CameraShake>();
			}
			this.shake.DoShake(this.numberOfShakes, this.shakeAmount, this.rotateAmount, this.distance, this.speed, this.decay, 1f, false);
		}

		public override void StopEvent()
		{
			if (this.shake != null)
			{
				this.shake.DoCancelShake();
			}
		}

		public override void EndEvent()
		{
			this.StopEvent();
		}
	}
}
