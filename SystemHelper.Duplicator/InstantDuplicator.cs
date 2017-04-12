using System;

namespace SystemHelper.Duplicator
{
	internal class InstantDuplicator<T> : DuplicatorBase<T> where T : class
	{
		internal InstantDuplicator(int quantity, Func<T> generator) : base(quantity, generator)
		{
		}

		public override bool operation()
		{
			while (this.currentQuantity < this.targetQuantity)
			{
				T t = this.generator();
				if (t == null || t.Equals(null))
				{
					return false;
				}
				this.instances.Add(t);
				this.currentQuantity++;
			}
			return true;
		}
	}
}
