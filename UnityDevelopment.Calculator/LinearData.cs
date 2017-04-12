using System;

namespace UnityDevelopment.Calculator
{
	public static class LinearData
	{
		public static float recurrentValidate(float content, float MAX, float MIN)
		{
			while (content > MAX)
			{
				content -= MAX - MIN + 1f;
			}
			while (content < MIN)
			{
				content += MAX - MIN + 1f;
			}
			return content;
		}

		public static float nonrecurrentValidate(float content, float MAX, float MIN)
		{
			if (content > MAX)
			{
				content = MAX;
			}
			if (content < MIN)
			{
				content = MIN;
			}
			return content;
		}

		public static bool isInRange(float content, float MAX, float MIN)
		{
			return content < MAX && content > MIN;
		}

		public static bool isInRangeWithBoundary(float content, float MAX, float MIN)
		{
			return content <= MAX && content >= MIN;
		}

		public static bool IsNullOrEmpty(string s)
		{
			if (s == null)
			{
				return true;
			}
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (c != ' ')
				{
					return false;
				}
			}
			return true;
		}
	}
}
