using System;

namespace WellFired.Initialization
{
	public interface IInitializationContext
	{
		bool IsContextSetupComplete();
	}
}
