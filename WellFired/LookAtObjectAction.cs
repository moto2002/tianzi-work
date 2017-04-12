using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/LookAt Object Action"), USequencerFriendlyName("LookAt Object Action")]
	public class LookAtObjectAction : ActionBase
	{
		public GameObject objectToLookAt;

		public string lookTargetName = string.Empty;

		public string lookTargetNameTest = string.Empty;

		public AnimationCurve inCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});

		public AnimationCurve outCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(1f, 0f)
		});

		public float lookAtTime = 2f;

		private Quaternion sourceOrientation = Quaternion.identity;

		private Quaternion previousRotation = Quaternion.identity;

		public override void UpdateEvent()
		{
			if (this.objectToLookAt != null && string.IsNullOrEmpty(this.lookTargetName))
			{
				this.lookTargetName = this.objectToLookAt.name;
			}
			if (this.objectToLookAt != null && string.IsNullOrEmpty(this.lookTargetNameTest))
			{
				this.lookTargetNameTest = this.objectToLookAt.name;
			}
		}

		private void FindObject()
		{
			if (base.TimelineContainer != null && base.TimelineContainer.transform != null && base.TimelineContainer.transform.parent != null)
			{
				Transform transform = base.TimelineContainer.transform.parent.Find(this.lookTargetName);
				if (transform != null)
				{
					this.objectToLookAt = transform.gameObject;
					return;
				}
				transform = base.TimelineContainer.transform.parent.Find(this.lookTargetNameTest);
				if (transform != null)
				{
					this.objectToLookAt = transform.gameObject;
				}
			}
		}

		public override void FireEvent()
		{
			this.FindObject();
			if (!this.objectToLookAt)
			{
				Debug.LogWarning("The USLookAtObject event does not provice a object to look at", this);
				return;
			}
			this.previousRotation = base.AffectedObject.transform.rotation;
			this.sourceOrientation = base.AffectedObject.transform.rotation;
		}

		public override void ProcessEvent(float deltaTime)
		{
			if (!this.objectToLookAt)
			{
				Debug.LogWarning("The USLookAtObject event does not provice a object to look at", this);
				return;
			}
			float time = this.inCurve[this.inCurve.length - 1].time;
			float num = this.lookAtTime + time;
			float t = 1f;
			if (deltaTime <= time)
			{
				t = Mathf.Clamp(this.inCurve.Evaluate(deltaTime), 0f, 1f);
			}
			else if (deltaTime >= num)
			{
				t = Mathf.Clamp(this.outCurve.Evaluate(deltaTime - num), 0f, 1f);
			}
			Vector3 position = base.AffectedObject.transform.position;
			Vector3 position2 = this.objectToLookAt.transform.position;
			Vector3 forward = position2 - position;
			Quaternion to = Quaternion.LookRotation(forward);
			base.AffectedObject.transform.rotation = Quaternion.Slerp(this.sourceOrientation, to, t);
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
			base.AffectedObject.transform.rotation = this.previousRotation;
		}

		private void OnDestroy()
		{
			this.objectToLookAt = null;
		}
	}
}
