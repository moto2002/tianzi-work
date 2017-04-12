using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Init Camera Position Action"), USequencerFriendlyName("Init Camera Position Action")]
	public class InitCameraPositionAction : ActionBase
	{
		public override void UpdateEvent()
		{
			base.Duration = 0.1f;
		}

		public override void FireEvent()
		{
			base.FireEvent();
			Transform transform = Instance.Get<SequenceManager>().Camera.transform;
			base.AffectedObject.transform.position = transform.position;
			base.AffectedObject.transform.rotation = transform.rotation;
			base.AffectedObject.SetActive(true);
		}
	}
}
