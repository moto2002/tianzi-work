using System;

namespace WellFired.Shared
{
	public static class DoubleEasing
	{
		public delegate double EasingFunction(double currentTime, double startingValue, double finalValue, double duration);

		public static DoubleEasing.EasingFunction GetEasingFunctionFor(Easing.EasingType easingType)
		{
			switch (easingType)
			{
			case Easing.EasingType.Linear:
				return new DoubleEasing.EasingFunction(DoubleEasing.Linear);
			case Easing.EasingType.QuadraticEaseOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuadraticEaseOut);
			case Easing.EasingType.QuadraticEaseIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuadraticEaseIn);
			case Easing.EasingType.QuadraticEaseInOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuadraticEaseInOut);
			case Easing.EasingType.QuadraticEaseOutIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuadraticEaseOutIn);
			case Easing.EasingType.SineEaseOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.SineEaseOut);
			case Easing.EasingType.SineEaseIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.SineEaseIn);
			case Easing.EasingType.SineEaseInOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.SineEaseInOut);
			case Easing.EasingType.SineEaseOutIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.SineEaseOutIn);
			case Easing.EasingType.ExponentialEaseOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.ExponentialEaseOut);
			case Easing.EasingType.ExponentialEaseIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.ExponentialEaseIn);
			case Easing.EasingType.ExponentialEaseInOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.ExponentialEaseInOut);
			case Easing.EasingType.ExponentialEaseOutIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.ExponentialEaseOutIn);
			case Easing.EasingType.CirclicEaseOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.CirclicEaseOut);
			case Easing.EasingType.CirclicEaseIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.CirclicEaseIn);
			case Easing.EasingType.CirclicEaseInOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.CirclicEaseInOut);
			case Easing.EasingType.CirclicEaseOutIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.CirclicEaseOutIn);
			case Easing.EasingType.CubicEaseOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.CubicEaseOut);
			case Easing.EasingType.CubicEaseIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.CubicEaseIn);
			case Easing.EasingType.CubicEaseInOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.CubicEaseInOut);
			case Easing.EasingType.CubicEaseOutIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.CubicEaseOutIn);
			case Easing.EasingType.QuarticEaseOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuarticEaseOut);
			case Easing.EasingType.QuarticEaseIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuarticEaseIn);
			case Easing.EasingType.QuarticEaseInOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuarticEaseInOut);
			case Easing.EasingType.QuarticEaseOutIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuarticEaseOutIn);
			case Easing.EasingType.QuinticEaseOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuinticEaseOut);
			case Easing.EasingType.QuinticEaseIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuinticEaseIn);
			case Easing.EasingType.QuinticEaseInOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuinticEaseInOut);
			case Easing.EasingType.QuinticEaseOutIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.QuinticEaseOutIn);
			case Easing.EasingType.ElasticEaseOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.ElasticEaseOut);
			case Easing.EasingType.ElasticEaseIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.ElasticEaseIn);
			case Easing.EasingType.ElasticEaseInOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.ElasticEaseInOut);
			case Easing.EasingType.ElasticEaseOutIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.ElasticEaseOutIn);
			case Easing.EasingType.BounceEaseOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.BounceEaseOut);
			case Easing.EasingType.BounceEaseIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.BounceEaseIn);
			case Easing.EasingType.BounceEaseInOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.BounceEaseInOut);
			case Easing.EasingType.BounceEaseOutIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.BounceEaseOutIn);
			case Easing.EasingType.BackEaseOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.BackEaseOut);
			case Easing.EasingType.BackEaseIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.BackEaseIn);
			case Easing.EasingType.BackEaseInOut:
				return new DoubleEasing.EasingFunction(DoubleEasing.BackEaseInOut);
			case Easing.EasingType.BackEaseOutIn:
				return new DoubleEasing.EasingFunction(DoubleEasing.BackEaseOutIn);
			default:
				throw new Exception("Easing type not implemented");
			}
		}

		public static double Linear(double t, double b, double c, double d)
		{
			return c * t / d + b;
		}

		public static double ExponentialEaseOut(double t, double b, double c, double d)
		{
			return (t != d) ? (c * (-Math.Pow(2.0, -10.0 * t / d) + 1.0) + b) : (b + c);
		}

		public static double ExponentialEaseIn(double t, double b, double c, double d)
		{
			return (t != 0.0) ? (c * Math.Pow(2.0, 10.0 * (t / d - 1.0)) + b) : b;
		}

		public static double ExponentialEaseInOut(double t, double b, double c, double d)
		{
			if (t == 0.0)
			{
				return b;
			}
			if (t == d)
			{
				return b + c;
			}
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * Math.Pow(2.0, 10.0 * (t - 1.0)) + b;
			}
			return c / 2.0 * (-Math.Pow(2.0, -10.0 * (t -= 1.0)) + 2.0) + b;
		}

		public static double ExponentialEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return DoubleEasing.ExponentialEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return DoubleEasing.ExponentialEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		public static double CirclicEaseOut(double t, double b, double c, double d)
		{
			return c * Math.Sqrt(1.0 - (t = t / d - 1.0) * t) + b;
		}

		public static double CirclicEaseIn(double t, double b, double c, double d)
		{
			return -c * (Math.Sqrt(1.0 - (t /= d) * t) - 1.0) + b;
		}

		public static double CirclicEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return -c / 2.0 * (Math.Sqrt(1.0 - t * t) - 1.0) + b;
			}
			return c / 2.0 * (Math.Sqrt(1.0 - (t -= 2.0) * t) + 1.0) + b;
		}

		public static double CirclicEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return DoubleEasing.CirclicEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return DoubleEasing.CirclicEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		public static double QuadraticEaseOut(double t, double b, double c, double d)
		{
			return -c * (t /= d) * (t - 2.0) + b;
		}

		public static double QuadraticEaseIn(double t, double b, double c, double d)
		{
			return c * (t /= d) * t + b;
		}

		public static double QuadraticEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * t * t + b;
			}
			return -c / 2.0 * ((t -= 1.0) * (t - 2.0) - 1.0) + b;
		}

		public static double QuadraticEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return DoubleEasing.QuadraticEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return DoubleEasing.QuadraticEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		public static double SineEaseOut(double t, double b, double c, double d)
		{
			return c * Math.Sin(t / d * 1.5707963267948966) + b;
		}

		public static double SineEaseIn(double t, double b, double c, double d)
		{
			return -c * Math.Cos(t / d * 1.5707963267948966) + c + b;
		}

		public static double SineEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * Math.Sin(3.1415926535897931 * t / 2.0) + b;
			}
			return -c / 2.0 * (Math.Cos(3.1415926535897931 * (t -= 1.0) / 2.0) - 2.0) + b;
		}

		public static double SineEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return DoubleEasing.SineEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return DoubleEasing.SineEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		public static double CubicEaseOut(double t, double b, double c, double d)
		{
			return c * ((t = t / d - 1.0) * t * t + 1.0) + b;
		}

		public static double CubicEaseIn(double t, double b, double c, double d)
		{
			return c * (t /= d) * t * t + b;
		}

		public static double CubicEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * t * t * t + b;
			}
			return c / 2.0 * ((t -= 2.0) * t * t + 2.0) + b;
		}

		public static double CubicEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return DoubleEasing.CubicEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return DoubleEasing.CubicEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		public static double QuarticEaseOut(double t, double b, double c, double d)
		{
			return -c * ((t = t / d - 1.0) * t * t * t - 1.0) + b;
		}

		public static double QuarticEaseIn(double t, double b, double c, double d)
		{
			return c * (t /= d) * t * t * t + b;
		}

		public static double QuarticEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * t * t * t * t + b;
			}
			return -c / 2.0 * ((t -= 2.0) * t * t * t - 2.0) + b;
		}

		public static double QuarticEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return DoubleEasing.QuarticEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return DoubleEasing.QuarticEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		public static double QuinticEaseOut(double t, double b, double c, double d)
		{
			return c * ((t = t / d - 1.0) * t * t * t * t + 1.0) + b;
		}

		public static double QuinticEaseIn(double t, double b, double c, double d)
		{
			return c * (t /= d) * t * t * t * t + b;
		}

		public static double QuinticEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * t * t * t * t * t + b;
			}
			return c / 2.0 * ((t -= 2.0) * t * t * t * t + 2.0) + b;
		}

		public static double QuinticEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return DoubleEasing.QuinticEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return DoubleEasing.QuinticEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		public static double ElasticEaseOut(double t, double b, double c, double d)
		{
			if ((t /= d) == 1.0)
			{
				return b + c;
			}
			double num = d * 0.3;
			double num2 = num / 4.0;
			return c * Math.Pow(2.0, -10.0 * t) * Math.Sin((t * d - num2) * 6.2831853071795862 / num) + c + b;
		}

		public static double ElasticEaseIn(double t, double b, double c, double d)
		{
			if ((t /= d) == 1.0)
			{
				return b + c;
			}
			double num = d * 0.3;
			double num2 = num / 4.0;
			return -(c * Math.Pow(2.0, 10.0 * (t -= 1.0)) * Math.Sin((t * d - num2) * 6.2831853071795862 / num)) + b;
		}

		public static double ElasticEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) == 2.0)
			{
				return b + c;
			}
			double num = d * 0.44999999999999996;
			double num2 = num / 4.0;
			if (t < 1.0)
			{
				return -0.5 * (c * Math.Pow(2.0, 10.0 * (t -= 1.0)) * Math.Sin((t * d - num2) * 6.2831853071795862 / num)) + b;
			}
			return c * Math.Pow(2.0, -10.0 * (t -= 1.0)) * Math.Sin((t * d - num2) * 6.2831853071795862 / num) * 0.5 + c + b;
		}

		public static double ElasticEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return DoubleEasing.ElasticEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return DoubleEasing.ElasticEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		public static double BounceEaseOut(double t, double b, double c, double d)
		{
			if ((t /= d) < 0.36363636363636365)
			{
				return c * (7.5625 * t * t) + b;
			}
			if (t < 0.72727272727272729)
			{
				return c * (7.5625 * (t -= 0.54545454545454541) * t + 0.75) + b;
			}
			if (t < 0.90909090909090906)
			{
				return c * (7.5625 * (t -= 0.81818181818181823) * t + 0.9375) + b;
			}
			return c * (7.5625 * (t -= 0.95454545454545459) * t + 0.984375) + b;
		}

		public static double BounceEaseIn(double t, double b, double c, double d)
		{
			return c - DoubleEasing.BounceEaseOut(d - t, 0.0, c, d) + b;
		}

		public static double BounceEaseInOut(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return DoubleEasing.BounceEaseIn(t * 2.0, 0.0, c, d) * 0.5 + b;
			}
			return DoubleEasing.BounceEaseOut(t * 2.0 - d, 0.0, c, d) * 0.5 + c * 0.5 + b;
		}

		public static double BounceEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return DoubleEasing.BounceEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return DoubleEasing.BounceEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		public static double BackEaseOut(double t, double b, double c, double d)
		{
			return c * ((t = t / d - 1.0) * t * (2.70158 * t + 1.70158) + 1.0) + b;
		}

		public static double BackEaseIn(double t, double b, double c, double d)
		{
			return c * (t /= d) * t * (2.70158 * t - 1.70158) + b;
		}

		public static double BackEaseInOut(double t, double b, double c, double d)
		{
			double num = 1.70158;
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * (t * t * (((num *= 1.525) + 1.0) * t - num)) + b;
			}
			return c / 2.0 * ((t -= 2.0) * t * (((num *= 1.525) + 1.0) * t + num) + 2.0) + b;
		}

		public static double BackEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return DoubleEasing.BackEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return DoubleEasing.BackEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}
	}
}
