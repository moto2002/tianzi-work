using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Animation (Legacy)/Blend Animation"), USequencerFriendlyName("Blend Animation (Legacy)")]
	public class USBlendAnimEvent : USEventBase
	{
		public AnimationClip animationClipSource;

		public AnimationClip animationClipDest;

		public float blendPoint = 1f;

		public void Update()
		{
			if (base.Duration < 0f)
			{
				base.Duration = 2f;
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
			component.wrapMode = WrapMode.Loop;
			component.Play(this.animationClipSource.name);
		}

		public override void ProcessEvent(float deltaTime)
		{
			Animation animation = base.AffectedObject.GetComponent<Animation>();
			if (!animation)
			{
				LogSystem.LogWarning(new object[]
				{
					string.Concat(new object[]
					{
						"Trying to play an animation : ",
						this.animationClipSource.name,
						" but : ",
						base.AffectedObject,
						" doesn't have an animation component, we will add one, this time, though you should add it manually"
					})
				});
				animation = base.AffectedObject.AddComponent<Animation>();
			}
			if (animation[this.animationClipSource.name] == null)
			{
				LogSystem.LogWarning(new object[]
				{
					"Trying to play an animation : " + this.animationClipSource.name + " but it isn't in the animation list. I will add it, this time, though you should add it manually."
				});
				animation.AddClip(this.animationClipSource, this.animationClipSource.name);
			}
			if (animation[this.animationClipDest.name] == null)
			{
				LogSystem.LogWarning(new object[]
				{
					"Trying to play an animation : " + this.animationClipDest.name + " but it isn't in the animation list. I will add it, this time, though you should add it manually."
				});
				animation.AddClip(this.animationClipDest, this.animationClipDest.name);
			}
			if (deltaTime < this.blendPoint)
			{
				animation.CrossFade(this.animationClipSource.name);
			}
			else
			{
				animation.CrossFade(this.animationClipDest.name);
			}
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
