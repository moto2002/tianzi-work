using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Camera Fllow Action"), USequencerFriendlyName("Camera Fllow Action")]
	public class CameraFllowAction : ActionBase
	{
		public Vector3 distance = SequenceManager.CAMERADISTANCE;

		public Vector3 rotate = SequenceManager.CAMERAANGLE;

		public float speed = 0.03f;

		public override void ProcessEvent(float runningTime)
		{
			if (base.AffectedObject != null)
			{
				Instance.Get<SequenceManager>().FllowTarget(Camera.main.transform, base.AffectedObject.transform, this.rotate, this.distance, this.speed);
			}
		}
	}
}
