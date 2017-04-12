using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Particle System/Stop Emitter"), USequencerEventHideDuration, USequencerFriendlyName("Stop Emitter (Legacy)")]
	public class USParticleEmitterStopEvent : USEventBase
	{
		public override void FireEvent()
		{
			ParticleSystem component = base.AffectedObject.GetComponent<ParticleSystem>();
			if (!component)
			{
				Debug.Log("Attempting to emit particles, but the object has no particleSystem USParticleEmitterStartEvent::FireEvent");
				return;
			}
			component.Stop();
		}

		public override void ProcessEvent(float deltaTime)
		{
		}
	}
}
