using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Effect Action"), USequencerFriendlyName("Effect Action")]
	public class EffectAction : ActionBase
	{
		public GameObject effectObject;

		public string effectName;

		public AttachmentPoint.Slot Slot;

		public Vector3 localPosition = Vector3.zero;

		public Vector3 localScale = Vector3.one;

		public Quaternion localRotation = Quaternion.identity;

		private GameObject prefab;

		private GameObject oGameObject;

		public Transform effectTransform;

		public override void UpdateEvent()
		{
			if (this.effectObject != null && this.effectName != this.effectObject.name)
			{
				this.effectName = this.effectObject.name;
				this.effectObject = null;
			}
			if (this.effectTransform != null)
			{
				this.localPosition = this.effectTransform.localPosition;
				this.localScale = this.effectTransform.localScale;
				this.localRotation = this.effectTransform.localRotation;
				this.effectTransform = null;
			}
		}

		public override void PrepareEvent()
		{
		}

		public override void FireEvent()
		{
			if (this.prefab != null)
			{
				Transform transform = null;
				this.oGameObject = (UnityEngine.Object.Instantiate(this.prefab) as GameObject);
				if (this.Slot == AttachmentPoint.Slot.None)
				{
					transform = base.AffectedObject.transform;
				}
				else
				{
					AttachmentPoint[] componentsInChildren = this.oGameObject.GetComponentsInChildren<AttachmentPoint>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						if (this.Slot == componentsInChildren[i].slot)
						{
							transform = componentsInChildren[i].gameObject.transform;
						}
					}
					if (transform == null)
					{
						transform = base.AffectedObject.transform;
					}
				}
				this.oGameObject.transform.parent = transform;
				this.oGameObject.transform.localPosition = this.localPosition;
				this.oGameObject.transform.localScale = this.localScale;
				this.oGameObject.transform.localRotation = this.localRotation;
			}
		}

		public override void ProcessEvent(float runningTime)
		{
		}

		public override void EndEvent()
		{
		}

		public override void StopEvent()
		{
			if (this.oGameObject != null)
			{
				UnityEngine.Object.Destroy(this.oGameObject);
				this.oGameObject = null;
			}
			this.prefab = null;
		}

		private void OnDestroy()
		{
			this.effectObject = null;
			this.oGameObject = null;
			this.effectTransform = null;
		}
	}
}
