using System;
using UnityEngine;

namespace WellFired
{
	public abstract class ActionBase : USEventBase
	{
		private bool isPrepare;

		public virtual float PrepareTime
		{
			get
			{
				return base.FireTime - 3f;
			}
		}

		public void Update()
		{
			if (Application.isPlaying && !this.isPrepare && base.Sequence.RunningTime >= this.PrepareTime)
			{
				this.PrepareEvent();
				this.isPrepare = true;
			}
			base.Duration = Mathf.Max(1E-05f, base.Duration);
			this.UpdateEvent();
		}

		public virtual void UpdateEvent()
		{
		}

		public virtual void PrepareEvent()
		{
		}

		public override void FireEvent()
		{
		}

		public override void ProcessEvent(float runningTime)
		{
		}

		public override void EndEvent()
		{
		}
	}
}
