using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Render/Toggle Mesh Renderer"), USequencerEventHideDuration, USequencerFriendlyName("Toggle Mesh Renderer")]
	public class USMeshRenderDisable : USEventBase
	{
		public bool enable;

		private bool previousEnable;

		public override void FireEvent()
		{
			if (!base.AffectedObject)
			{
				return;
			}
			MeshRenderer component = base.AffectedObject.GetComponent<MeshRenderer>();
			if (!component)
			{
				Debug.LogWarning("You didn't add a Mesh Renderer to the Affected Object", base.AffectedObject);
				return;
			}
			this.previousEnable = component.enabled;
			component.enabled = this.enable;
		}

		public override void ProcessEvent(float deltaTime)
		{
		}

		public override void EndEvent()
		{
			this.UndoEvent();
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
			MeshRenderer component = base.AffectedObject.GetComponent<MeshRenderer>();
			if (!component)
			{
				Debug.LogWarning("You didn't add a Mesh Renderer to the Affected Object", base.AffectedObject);
				return;
			}
			component.enabled = this.previousEnable;
		}
	}
}
