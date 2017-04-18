using System;

namespace WellFired.Shared
{
	public static class PlatformSpecificFactory
	{
		private static Type cachedReflectionType;

		private static IReflectionHelper cachedReflectionHelper;

		private static Type cachedIOType;

		private static IIOHelper cachedIOHelper;

		public static IReflectionHelper ReflectionHelper
		{
			get
			{
				if (PlatformSpecificFactory.cachedReflectionType == null)
				{
					PlatformSpecificFactory.cachedReflectionType = Type.GetType("WellFired.Shared.ReflectionHelper, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
				}
				if (PlatformSpecificFactory.cachedReflectionHelper == null)
				{
					PlatformSpecificFactory.cachedReflectionHelper = (IReflectionHelper)Activator.CreateInstance(PlatformSpecificFactory.cachedReflectionType);
				}
				return PlatformSpecificFactory.cachedReflectionHelper;
			}
			private set
			{
			}
		}

		public static IIOHelper IOHelper
		{
			get
			{
				if (PlatformSpecificFactory.cachedIOType == null)
				{
					PlatformSpecificFactory.cachedIOType = Type.GetType("WellFired.Shared.IOHelper, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
				}
				if (PlatformSpecificFactory.cachedIOHelper == null)
				{
					PlatformSpecificFactory.cachedIOHelper = (IIOHelper)Activator.CreateInstance(PlatformSpecificFactory.cachedIOType);
				}
				return PlatformSpecificFactory.cachedIOHelper;
			}
			private set
			{
			}
		}
	}
}
