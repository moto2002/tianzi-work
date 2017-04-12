using System;
using UnityEngine;

namespace WellFired
{
	[ExecuteInEditMode, USequencerEvent("Camera/Set Culling Mask"), USequencerEventHideDuration, USequencerFriendlyName("Set Culling Mask")]
	public class USCameraSetCullingMask : USEventBase
	{
		[SerializeField]
		private LayerMask newLayerMask;

		private int prevLayerMask;

		private Camera cameraToAffect;

		public override void FireEvent()
		{
			if (base.AffectedObject != null)
			{
				this.cameraToAffect = base.AffectedObject.GetComponent<Camera>();
			}
			if (this.cameraToAffect)
			{
				this.prevLayerMask = this.cameraToAffect.cullingMask;
				this.cameraToAffect.cullingMask = this.newLayerMask;
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
		}

		public override void EndEvent()
		{
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			if (this.cameraToAffect)
			{
				this.cameraToAffect.cullingMask = this.prevLayerMask;
			}
		}
	}
}
