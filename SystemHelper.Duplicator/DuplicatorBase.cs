using System;
using System.Collections.Generic;

namespace SystemHelper.Duplicator
{
	public abstract class DuplicatorBase<T> where T : class
	{
		protected Func<T> generator;

		protected int targetQuantity = 10;

		protected int currentQuantity;

		protected List<T> instances = new List<T>();

		public List<T> Instances
		{
			get
			{
				return this.instances;
			}
		}

		public bool IsFinished
		{
			get
			{
				return this.currentQuantity == this.targetQuantity;
			}
		}

		internal DuplicatorBase(int quantity, Func<T> generator)
		{
			this.generator = generator;
			this.targetQuantity = quantity;
		}

		public abstract bool operation();

		public static DuplicatorBase<T> create(DuplicationStrategyType duplicationStrategy, int quantity, Func<T> generator)
		{
			if (duplicationStrategy == DuplicationStrategyType.InstantGeneration)
			{
				return new InstantDuplicator<T>(quantity, generator);
			}
			if (duplicationStrategy != DuplicationStrategyType.SmoothGeneration)
			{
				return null;
			}
			return new SmoothDuplicator<T>(quantity, generator);
		}

		public void clear()
		{
			this.instances.Clear();
			this.currentQuantity = 0;
		}
	}
}
