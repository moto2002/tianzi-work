using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Update Scene Sphere Action"), USequencerFriendlyName("Update Scene Sphere Action")]
	public class UpdateSceneSphereAction : ActionBase
	{
		public Vector3 postion = Vector3.zero;

		public Transform trans;

		private GameObject sphere;

		private void OnDestroy()
		{
			this.trans = null;
			this.sphere = null;
		}

		public override void UpdateEvent()
		{
			if (this.trans != null)
			{
				this.postion = this.trans.localPosition;
				this.trans = null;
			}
		}

		public override void FireEvent()
		{
			if (!Instance.Get<SequenceManager>().isEditor)
			{
				this.sphere = new GameObject("sphere");
				this.sphere.transform.parent = base.AffectedObject.transform;
				this.sphere.transform.localScale = Vector3.one;
				this.sphere.transform.localRotation = Quaternion.identity;
				this.sphere.transform.localPosition = this.postion;
			}
		}

		public override void ProcessEvent(float runningTime)
		{
			try
			{
				if (!Instance.Get<SequenceManager>().isEditor && this.sphere != null && Instance.Get<SequenceManager>().UpdateScene != null)
				{
					Instance.Get<SequenceManager>().UpdateScene(this.sphere.transform.position);
				}
			}
			catch (Exception ex)
			{
				LogSystem.LogError(new object[]
				{
					"UpdateSceneAction GameScene->UpdateView catch error",
					ex.ToString()
				});
			}
		}

		public override void EndEvent()
		{
			if (this.sphere != null)
			{
				UnityEngine.Object.Destroy(this.sphere);
			}
		}
	}
}
