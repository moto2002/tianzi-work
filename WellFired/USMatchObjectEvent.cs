using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Transform/Match Objects Orientation"), USequencerFriendlyName("Match Objects Orientation")]
	public class USMatchObjectEvent : USEventBase
	{
		public GameObject objectToMatch;

		public AnimationCurve inCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});

		private Quaternion sourceRotation = Quaternion.identity;

		private Vector3 sourcePosition = Vector3.zero;

		public override void FireEvent()
		{
			if (!this.objectToMatch)
			{
				Debug.LogWarning("The USMatchObjectEvent event does not provice a object to match", this);
				return;
			}
			this.sourceRotation = base.AffectedObject.transform.rotation;
			this.sourcePosition = base.AffectedObject.transform.position;
		}

		public override void ProcessEvent(float deltaTime)
		{
			if (!this.objectToMatch)
			{
				Debug.LogWarning("The USMatchObjectEvent event does not provice a object to look at", this);
				return;
			}
			float t = Mathf.Clamp(this.inCurve.Evaluate(deltaTime), 0f, 1f);
			Vector3 position = this.objectToMatch.transform.position;
			Quaternion rotation = this.objectToMatch.transform.rotation;
			base.AffectedObject.transform.rotation = Quaternion.Slerp(this.sourceRotation, rotation, t);
			base.AffectedObject.transform.position = Vector3.Slerp(this.sourcePosition, position, t);
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
			base.AffectedObject.transform.rotation = this.sourceRotation;
			base.AffectedObject.transform.position = this.sourcePosition;
		}
	}
}
