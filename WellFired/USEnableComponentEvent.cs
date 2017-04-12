using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Object/Toggle Component"), USequencerEventHideDuration, USequencerFriendlyName("Toggle Component")]
	public class USEnableComponentEvent : USEventBase
	{
		public bool enableComponent;

		private bool prevEnable;

		[HideInInspector, SerializeField]
		private string componentName;

		public string ComponentName
		{
			get
			{
				return this.componentName;
			}
			set
			{
				this.componentName = value;
			}
		}

		public override void FireEvent()
		{
			Behaviour behaviour = base.AffectedObject.GetComponent(this.ComponentName) as Behaviour;
			if (!behaviour)
			{
				return;
			}
			this.prevEnable = behaviour.enabled;
			behaviour.enabled = this.enableComponent;
		}

		public override void ProcessEvent(float deltaTime)
		{
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
			Behaviour behaviour = base.AffectedObject.GetComponent(this.ComponentName) as Behaviour;
			if (!behaviour)
			{
				return;
			}
			behaviour.enabled = this.prevEnable;
		}
	}
}
