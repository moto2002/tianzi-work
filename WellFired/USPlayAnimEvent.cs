using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Animation (Legacy)/Play Animation"), USequencerFriendlyName("Play Animation (Legacy)")]
	public class USPlayAnimEvent : USEventBase
	{
		private AnimationProxy animationprox;

		[SerializeField]
		public string animationName;

		public AnimationClip animationClip;

		public WrapMode wrapMode;

		public float playbackSpeed = 1f;

		private AnimationClip mAnimationClip;

		public void Update()
		{
			if (this.wrapMode != WrapMode.Loop && this.animationClip)
			{
				base.Duration = this.animationClip.length / this.playbackSpeed;
			}
			if (this.animationClip != null)
			{
				if (!string.IsNullOrEmpty(this.animationClip.name))
				{
					this.animationName = string.Format("{0}", this.animationClip.name);
				}
				this.animationClip = null;
			}
		}

		public override void FireEvent()
		{
			if (Instance.Get<SequenceManager>().isEditor)
			{
				this.OnEditorFireEvent();
			}
			else
			{
				this.OnGameFireEvent();
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
			if (Instance.Get<SequenceManager>().isEditor)
			{
				this.OnEditorProcessEvent(deltaTime);
			}
		}

		public override void StopEvent()
		{
			if (!Instance.Get<SequenceManager>().isEditor)
			{
				this.OnGameStopEvent();
			}
		}

		private void OnEditorFireEvent()
		{
			if (base.AffectedObject == null)
			{
				return;
			}
			if (!base.AffectedObject.gameObject.activeSelf)
			{
				base.AffectedObject.gameObject.SetActive(true);
			}
			this.animationprox = base.AffectedObject.GetComponentInChildren<AnimationProxy>();
			if (this.animationprox == null)
			{
				LogSystem.LogError(new object[]
				{
					"UIPlayAnimEvent.OnEditorFireEvent() ->",
					base.AffectedObject.name,
					": not found animation proxy!"
				});
				return;
			}
			string editorAnimationPath = this.animationprox.GetEditorAnimationPath(this.animationName);
			this.mAnimationClip = (Resources.Load(editorAnimationPath) as AnimationClip);
			if (!this.mAnimationClip)
			{
				Debug.Log("Attempting to play an animation on a GameObject but you haven't given the event an animation clip from USPlayAnimEvent::FireEvent");
				return;
			}
			Animation animation = base.AffectedObject.GetComponent<Animation>();
			if (!animation)
			{
				animation = base.AffectedObject.AddComponent<Animation>();
			}
			animation.playAutomatically = false;
			animation.clip = this.mAnimationClip;
			animation.AddClip(this.mAnimationClip, this.animationName);
			animation.wrapMode = this.wrapMode;
			animation.Play(this.mAnimationClip.name);
			AnimationState animationState = animation[this.mAnimationClip.name];
			if (!animationState)
			{
				return;
			}
			animationState.speed = this.playbackSpeed;
		}

		private void OnEditorProcessEvent(float deltaTime)
		{
			Animation animation = base.AffectedObject.GetComponent<Animation>();
			if (!this.mAnimationClip)
			{
				return;
			}
			if (!animation)
			{
				LogSystem.LogWarning(new object[]
				{
					string.Concat(new object[]
					{
						"Trying to play an animation : ",
						this.mAnimationClip.name,
						" but : ",
						base.AffectedObject,
						" doesn't have an animation component, we will add one, this time, though you should add it manually"
					})
				});
				animation = base.AffectedObject.AddComponent<Animation>();
			}
			if (animation[this.mAnimationClip.name] == null)
			{
				LogSystem.LogWarning(new object[]
				{
					"Trying to play an animation : " + this.mAnimationClip.name + " but it isn't in the animation list. I will add it, this time, though you should add it manually."
				});
				animation.AddClip(this.mAnimationClip, this.mAnimationClip.name);
			}
			AnimationState animationState = animation[this.mAnimationClip.name];
			if (!animation.IsPlaying(this.mAnimationClip.name))
			{
				animation.wrapMode = this.wrapMode;
				animation.Play(this.mAnimationClip.name);
			}
			animationState.speed = this.playbackSpeed;
			animationState.time = deltaTime * this.playbackSpeed;
			animationState.enabled = true;
			animation.Sample();
			animationState.enabled = false;
		}

		private void OnGameFireEvent()
		{
			if (!base.AffectedObject.gameObject.activeSelf)
			{
				base.AffectedObject.gameObject.SetActive(true);
			}
			this.animationprox = base.AffectedObject.GetComponentInChildren<AnimationProxy>();
			if (this.animationprox == null)
			{
				LogSystem.LogError(new object[]
				{
					"UIPlayAnimEvent.OnGameFireEvent() ->",
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

		private void OnGameProcessEvent(float deltaTime)
		{
		}

		private void OnGameStopEvent()
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
