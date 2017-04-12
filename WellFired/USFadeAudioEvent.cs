using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Audio/Fade Audio"), USequencerEventHideDuration, USequencerFriendlyName("Fade Audio")]
	public class USFadeAudioEvent : USEventBase
	{
		private float previousVolume = 1f;

		public AnimationCurve fadeCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(1f, 0f)
		});

		public void Update()
		{
			base.Duration = (float)this.fadeCurve.length;
		}

		public override void FireEvent()
		{
			AudioSource component = base.AffectedObject.GetComponent<AudioSource>();
			if (!component)
			{
				Debug.LogWarning("Trying to fade audio on an object without an AudioSource");
				return;
			}
			this.previousVolume = component.volume;
		}

		public override void ProcessEvent(float deltaTime)
		{
			AudioSource component = base.AffectedObject.GetComponent<AudioSource>();
			if (!component)
			{
				Debug.LogWarning("Trying to fade audio on an object without an AudioSource");
				return;
			}
			component.volume = this.fadeCurve.Evaluate(deltaTime);
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			AudioSource component = base.AffectedObject.GetComponent<AudioSource>();
			if (!component)
			{
				Debug.LogWarning("Trying to fade audio on an object without an AudioSource");
				return;
			}
			component.volume = this.previousVolume;
		}
	}
}
