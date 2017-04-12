using System;

namespace WellFired
{
	[USequencerEvent("Custom Event/Job Enable Action"), USequencerEventHideDuration, USequencerFriendlyName("Job Enable Action")]
	public class JobEnableAction : ActionBase
	{
		public enum JobType
		{
			Qiang = 1,
			Jian,
			Zhang
		}

		public bool enable;

		private bool prevEnable;

		public JobEnableAction.JobType Job = JobEnableAction.JobType.Qiang;

		public override void FireEvent()
		{
			if (Instance.Get<SequenceManager>().mainRoleJob == (int)this.Job)
			{
				this.prevEnable = base.AffectedObject.activeSelf;
				base.AffectedObject.SetActive(this.enable);
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
			if (Instance.Get<SequenceManager>().mainRoleJob == (int)this.Job)
			{
				base.AffectedObject.SetActive(this.prevEnable);
			}
		}
	}
}
