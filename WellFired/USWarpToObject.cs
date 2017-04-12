using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Transform/Warp To Object"), USequencerEventHideDuration, USequencerFriendlyName("Warp To Object")]
	public class USWarpToObject : USEventBase
	{
		public GameObject objectToWarpTo;

		public bool useObjectRotation;

		private Transform previousTransform;

		public override void FireEvent()
		{
			if (this.objectToWarpTo)
			{
				base.AffectedObject.transform.position = this.objectToWarpTo.transform.position;
				if (this.useObjectRotation)
				{
					base.AffectedObject.transform.rotation = this.objectToWarpTo.transform.rotation;
				}
			}
			else
			{
				Debug.LogError(base.AffectedObject.name + ": No Object attached to WarpToObjectSequencer Script");
			}
			this.previousTransform = base.AffectedObject.transform;
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
			if (this.previousTransform)
			{
				base.AffectedObject.transform.position = this.previousTransform.position;
				base.AffectedObject.transform.rotation = this.previousTransform.rotation;
			}
		}

		private void OnDestroy()
		{
			this.objectToWarpTo = null;
			this.previousTransform = null;
		}
	}
}
