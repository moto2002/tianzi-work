using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Camera Switch Action"), USequencerFriendlyName("Camera Switch Action")]
	public class CameraSwitchAction : ActionBase
	{
		public Transform switchCamera;

		public Vector3 position;

		public float speed = 0.03f;

		public Vector3 rotate;

		public override void UpdateEvent()
		{
			if (this.switchCamera != null)
			{
				this.position = this.switchCamera.transform.position;
				this.rotate = this.switchCamera.transform.rotation.eulerAngles;
				this.switchCamera = null;
			}
		}

		public override void FireEvent()
		{
		}

		public override void ProcessEvent(float runningTime)
		{
			if (base.AffectedObject != null)
			{
				Instance.Get<SequenceManager>().SwitchTarget(Camera.main.transform, this.rotate, this.position, this.speed);
			}
		}

		public override void StopEvent()
		{
		}

		private void OnDestroy()
		{
			this.switchCamera = null;
		}
	}
}
