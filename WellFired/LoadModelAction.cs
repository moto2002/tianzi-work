using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Load Mode Action"), USequencerFriendlyName("Load Mode Action")]
	public class LoadModelAction : ActionBase
	{
		public GameObject tempModel;

		private GameObject realModel;

		public string ModelName;

		public string modelPath;

		public bool enable;

		public override void UpdateEvent()
		{
			if (this.tempModel != null)
			{
			}
		}

		public override void FireEvent()
		{
			if (Instance.Get<SequenceManager>().isEditor)
			{
				GameObject gameObject = Resources.Load(this.modelPath) as GameObject;
				this.OnLoadModelComplete(new object[]
				{
					gameObject
				});
			}
			else
			{
				Instance.Get<SequenceManager>().LoadAsset(this.modelPath, new AssetCallBack(this.OnLoadModelComplete));
			}
		}

		private void OnLoadModelComplete(object[] args)
		{
			if (args != null)
			{
				GameObject gameObject = args[0] as GameObject;
				if (gameObject != null)
				{
					this.realModel = (UnityEngine.Object.Instantiate(gameObject) as GameObject);
					this.realModel.name = this.ModelName;
					if (base.AffectedObject != null)
					{
						Vector3 localPosition = base.AffectedObject.transform.localPosition;
						Vector3 localScale = base.AffectedObject.transform.localScale;
						Quaternion localRotation = base.AffectedObject.transform.localRotation;
						if (!Instance.Get<SequenceManager>().isEditor)
						{
							this.realModel.transform.parent = base.TimelineContainer.AffectedObject.parent;
						}
						NGUITools.SetLayer(this.realModel, LayerMask.NameToLayer("Default"));
						this.realModel.transform.localPosition = localPosition;
						this.realModel.transform.localScale = localScale;
						this.realModel.transform.localRotation = localRotation;
						base.TimelineContainer.AffectedObject = this.realModel.transform;
						base.AffectedObject.SetActive(this.enable);
					}
				}
			}
		}

		public override void ProcessEvent(float runningTime)
		{
		}

		public override void UndoEvent()
		{
			if (this.realModel != null)
			{
				this.realModel.SetActive(false);
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
	}
}
