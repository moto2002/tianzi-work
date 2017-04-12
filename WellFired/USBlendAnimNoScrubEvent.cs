using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Animation (Legacy)/Blend Animation No Scrub"), USequencerEventHideDuration, USequencerFriendlyName("Blend Animation No Scrub (Legacy)")]
	public class USBlendAnimNoScrubEvent : USEventBase
	{
		public AnimationClip blendedAnimation;

		public void Update()
		{
			if (base.Duration < 0f)
			{
				base.Duration = this.blendedAnimation.length;
			}
		}

		public override void FireEvent()
		{
			Animation component = base.AffectedObject.GetComponent<Animation>();
			if (!component)
			{
				Debug.Log("Attempting to play an animation on a GameObject without an Animation Component from USPlayAnimEvent.FireEvent");
				return;
			}
			component[this.blendedAnimation.name].wrapMode = WrapMode.Once;
			component[this.blendedAnimation.name].layer = 1;
		}

		public override void ProcessEvent(float deltaTime)
		{
			base.animation.CrossFade(this.blendedAnimation.name);
		}

		public override void StopEvent()
		{
			if (!base.AffectedObject)
			{
				return;
			}
			Animation component = base.AffectedObject.GetComponent<Animation>();
			if (component)
			{
				component.Stop();
			}
		}
	}
}
