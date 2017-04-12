using System;

namespace SystemHelper.Duplicator
{
	internal class SmoothDuplicator<T> : DuplicatorBase<T> where T : class
	{
		public SmoothDuplicator(int quantity, Func<T> generator) : base(quantity, generator)
		{
		}

		public override bool operation()
		{
			if (this.currentQuantity >= this.targetQuantity)
			{
				return false;
			}
			T t = this.generator();
			if (t != null && !t.Equals(null))
			{
				this.instances.Add(t);
				this.currentQuantity++;
				return true;
			}
			return false;
		}
	}
}
