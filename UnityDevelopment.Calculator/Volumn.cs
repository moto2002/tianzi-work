using System;

namespace UnityDevelopment.Calculator
{
	public class Volumn
	{
		private float currVol;

		private float maxVol;

		public float currentVolumn
		{
			get
			{
				return this.currVol;
			}
			set
			{
				this.currVol = LinearData.nonrecurrentValidate(value, this.maxVol, 0f);
			}
		}

		public float maxVolumn
		{
			get
			{
				return this.maxVol;
			}
			set
			{
				this.maxVol = value;
				this.currVol = LinearData.nonrecurrentValidate(this.currVol, this.maxVol, 0f);
			}
		}

		public bool isEmpty
		{
			get
			{
				return this.currVol == 0f;
			}
		}

		public bool isFull
		{
			get
			{
				return this.currVol == this.maxVol;
			}
		}

		public Volumn(float maxVolumn, bool isEmpty)
		{
			this.maxVol = maxVolumn;
			if (isEmpty)
			{
				this.currVol = 0f;
			}
			else
			{
				this.currVol = this.maxVol;
			}
		}
	}
}
