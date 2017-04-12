using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Particle System/Start Emitter"), USequencerEventHideDuration, USequencerFriendlyName("Start Emitter (Legacy)")]
	public class USParticleEmitterStartEvent : USEventBase
	{
		public void Update()
		{
			if (!base.AffectedObject)
			{
				return;
			}
			ParticleSystem component = base.AffectedObject.GetComponent<ParticleSystem>();
			if (component)
			{
				base.Duration = component.duration + component.startLifetime;
			}
		}

		public override void FireEvent()
		{
			if (!base.AffectedObject)
			{
				return;
			}
			ParticleSystem component = base.AffectedObject.GetComponent<ParticleSystem>();
			if (!component)
			{
				Debug.Log("Attempting to emit particles, but the object has no particleSystem USParticleEmitterStartEvent::FireEvent");
				return;
			}
			if (!Application.isPlaying)
			{
				return;
			}
			component.Play();
		}

		public override void ProcessEvent(float deltaTime)
		{
			if (Application.isPlaying)
			{
				return;
			}
			ParticleSystem component = base.AffectedObject.GetComponent<ParticleSystem>();
			component.Simulate(deltaTime);
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
			ParticleSystem component = base.AffectedObject.GetComponent<ParticleSystem>();
			if (component)
			{
				component.Stop();
			}
		}
	}
}
