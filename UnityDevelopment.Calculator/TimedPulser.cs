using System;

namespace UnityDevelopment.Calculator
{
	public class TimedPulser
	{
		private NcTimerTool timer = new NcTimerTool();

		private float period;

		public TimedPulser(float period)
		{
			this.period = period;
			this.timer.Start();
		}

		public bool pulse()
		{
			bool flag = this.timer.GetTime() > this.period;
			if (flag)
			{
				this.timer.Start();
			}
			return flag;
		}
	}
}
