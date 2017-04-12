using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Audio/Play Audio"), USequencerEventHideDuration, USequencerFriendlyName("Play Audio")]
	public class USPlayAudioEvent : USEventBase
	{
		public AudioClip audioClip;

		public bool loop;

		private bool wasPlaying;

		public void Update()
		{
			if (!this.loop && this.audioClip)
			{
				base.Duration = this.audioClip.length;
			}
			else
			{
				base.Duration = -1f;
			}
		}

		public override void FireEvent()
		{
			AudioSource audioSource = base.AffectedObject.GetComponent<AudioSource>();
			if (!audioSource)
			{
				audioSource = base.AffectedObject.AddComponent<AudioSource>();
				audioSource.playOnAwake = false;
			}
			if (audioSource.clip != this.audioClip)
			{
				audioSource.clip = this.audioClip;
			}
			audioSource.time = 0f;
			audioSource.loop = this.loop;
			if (!base.Sequence.IsPlaying)
			{
				return;
			}
			audioSource.Play();
		}

		public override void ProcessEvent(float deltaTime)
		{
			AudioSource audioSource = base.AffectedObject.GetComponent<AudioSource>();
			if (!audioSource)
			{
				audioSource = base.AffectedObject.AddComponent<AudioSource>();
				audioSource.playOnAwake = false;
			}
			if (audioSource.clip != this.audioClip)
			{
				audioSource.clip = this.audioClip;
			}
			if (audioSource.isPlaying)
			{
				return;
			}
			audioSource.time = deltaTime;
			if (base.Sequence.IsPlaying && !audioSource.isPlaying)
			{
				audioSource.Play();
			}
		}

		public override void ManuallySetTime(float deltaTime)
		{
			AudioSource component = base.AffectedObject.GetComponent<AudioSource>();
			if (!component)
			{
				return;
			}
			component.time = deltaTime;
		}

		public override void ResumeEvent()
		{
			AudioSource component = base.AffectedObject.GetComponent<AudioSource>();
			if (!component)
			{
				return;
			}
			component.time = base.Sequence.RunningTime - base.FireTime;
			if (this.wasPlaying)
			{
				component.Play();
			}
		}

		public override void PauseEvent()
		{
			AudioSource component = base.AffectedObject.GetComponent<AudioSource>();
			this.wasPlaying = false;
			if (component && component.isPlaying)
			{
				this.wasPlaying = true;
			}
			if (component)
			{
				component.Pause();
			}
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void EndEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			if (!base.AffectedObject)
			{
				return;
			}
			AudioSource component = base.AffectedObject.GetComponent<AudioSource>();
			if (component)
			{
				component.Stop();
			}
		}
	}
}
