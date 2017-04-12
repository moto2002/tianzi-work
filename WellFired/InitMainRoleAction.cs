using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Init Main Role"), USequencerFriendlyName("Init Main Role")]
	public class InitMainRoleAction : ActionBase
	{
		private GameObject realrole;

		public bool initPos;

		public override void UpdateEvent()
		{
			base.Duration = base.Sequence.Duration;
		}

		public override void FireEvent()
		{
			if (Instance.Get<SequenceManager>().RealModel != null)
			{
				this.realrole = Instance.Get<SequenceManager>().RealModel;
				this.realrole.name = "CGMainRole";
				this.realrole.SetActive(true);
				DelegateProxy.DestroyEffect(this.realrole, LayerMask.NameToLayer("Effect"));
				NGUITools.SetLayer(this.realrole, LayerMask.NameToLayer("Default"));
				this.realrole.transform.parent = base.AffectedObject.transform.parent;
				this.realrole.transform.position = base.AffectedObject.transform.position;
				this.realrole.transform.localScale = base.AffectedObject.transform.localScale;
				this.realrole.transform.localRotation = base.AffectedObject.transform.localRotation;
				base.AffectedObject.SetActive(false);
				base.TimelineContainer.AffectedObject = this.realrole.transform;
			}
		}

		public override void ProcessEvent(float runningTime)
		{
		}

		public override void UndoEvent()
		{
			if (Instance.Get<SequenceManager>().RealModel != null)
			{
				Instance.Get<SequenceManager>().RealModel.SetActive(true);
				if (this.realrole != null)
				{
					UnityEngine.Object.Destroy(this.realrole);
				}
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

		public void ChangeShader(GameObject role)
		{
			Shader shader = Shader.Find("Snail/Bumped Specular Point Light");
			if (shader == null)
			{
				return;
			}
			Renderer[] componentsInChildren = role.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			if (componentsInChildren == null)
			{
				return;
			}
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] != null && componentsInChildren[i].material != null)
				{
					componentsInChildren[i].material.shader = shader;
				}
			}
		}

		private void OnDestroy()
		{
			this.realrole = null;
		}
	}
}
