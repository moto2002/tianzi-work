using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Render/Change Objects Color"), USequencerEventHideDuration, USequencerFriendlyName("Change Color")]
	public class USChangeColor : USEventBase
	{
		public Color newColor;

		private Color previousColor;

		public override void FireEvent()
		{
			if (!base.AffectedObject)
			{
				return;
			}
			if (!Application.isPlaying && Application.isEditor)
			{
				this.previousColor = base.AffectedObject.renderer.sharedMaterial.color;
				base.AffectedObject.renderer.sharedMaterial.color = this.newColor;
			}
			else
			{
				this.previousColor = base.AffectedObject.renderer.material.color;
				base.AffectedObject.renderer.material.color = this.newColor;
			}
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
			if (!Application.isPlaying && Application.isEditor)
			{
				base.AffectedObject.renderer.sharedMaterial.color = this.previousColor;
			}
			else
			{
				base.AffectedObject.renderer.material.color = this.previousColor;
			}
		}
	}
}
