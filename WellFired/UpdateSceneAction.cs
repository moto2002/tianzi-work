using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Update Scene Action"), USequencerFriendlyName("Update Scene Action")]
	public class UpdateSceneAction : ActionBase
	{
		public Transform trans;

		public Vector3 postion = Vector3.zero;

		public override void UpdateEvent()
		{
			if (this.trans != null)
			{
				this.postion = this.trans.position;
				this.trans = null;
			}
		}

		public override void FireEvent()
		{
		}

		public override void ProcessEvent(float runningTime)
		{
			try
			{
				if (!Instance.Get<SequenceManager>().isEditor && Instance.Get<SequenceManager>().UpdateScene != null)
				{
					Instance.Get<SequenceManager>().UpdateScene(this.postion);
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

		private void OnDestroy()
		{
			this.trans = null;
		}
	}
}
