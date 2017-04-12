using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Play Animation Action"), USequencerFriendlyName("Play MainRole Animation Action")]
	public class PlayAnimAction : ActionBase
	{
		[SerializeField]
		public string animationName;

		public WrapMode wrapMode;

		public float playbackSpeed = 1f;

		private AnimationProxy animationprox;

		public AnimationClip animationClip;

		public new void Update()
		{
			if (this.wrapMode != WrapMode.Loop && this.animationClip)
			{
				base.Duration = this.animationClip.length / this.playbackSpeed;
				if (this.animationClip != null)
				{
					if (!string.IsNullOrEmpty(this.animationClip.name))
					{
						this.animationName = string.Format("{0}", this.animationClip.name);
					}
					this.animationClip = null;
				}
			}
		}

		public override void FireEvent()
		{
			this.animationprox = base.AffectedObject.GetComponentInChildren<AnimationProxy>();
			if (this.animationprox == null)
			{
				LogSystem.LogWarning(new object[]
				{
					"PlayAnimAction ->",
					base.AffectedObject.name,
					": not found animation proxy!"
				});
				return;
			}
			if (Instance.Get<SequenceManager>().PlayActionFunc != null)
			{
				this.animationprox.mMainClip.strName = this.animationName;
				this.animationprox.mMainClip.strPath = this.animationprox.GetAnimationPath(this.animationName);
				Instance.Get<SequenceManager>().PlayActionFunc(this.animationprox, this.animationName, this.playbackSpeed);
			}
		}

		public override void EndEvent()
		{
			this.StopEvent();
		}

		public override void StopEvent()
		{
			this.animationClip = null;
			this.animationprox = null;
		}
	}
}
