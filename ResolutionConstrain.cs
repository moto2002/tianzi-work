using System;
using UnityEngine;

public class ResolutionConstrain
{
	public const float SmallRatio = 1.33333337f;

	public const double dsmallRatioPixles = 1769472.0;

	public const float BigRatio = 1.77777779f;

	public const double dbigRatioPixles = 1440000.0;

	public static int ScreenWidth;

	public static int ScreenHeight;

	private static Vector2 SmallResolution = new Vector2(1536f, 1152f);

	private static Vector2 BigResolution = new Vector2(1600f, 900f);

	private Vector2 currentResolution = Vector2.zero;

	private static ResolutionConstrain _instance;

	public static ResolutionConstrain Instance
	{
		get
		{
			if (ResolutionConstrain._instance == null)
			{
				ResolutionConstrain._instance = new ResolutionConstrain();
			}
			return ResolutionConstrain._instance;
		}
	}

	public Vector2 CurrentResolution
	{
		get
		{
			return this.currentResolution;
		}
	}

	public int width
	{
		get
		{
			return (int)this.currentResolution.x;
		}
	}

	public int height
	{
		get
		{
			return (int)this.currentResolution.y;
		}
	}

	private ResolutionConstrain()
	{
		ResolutionConstrain._instance = this;
	}

	public void init()
	{
		Resolution resolution = Screen.currentResolution;
		double num = (double)(resolution.width * resolution.height);
		float num2 = (float)resolution.width / (float)resolution.height;
		float num3 = Mathf.Abs(num2 - 1.33333337f);
		float num4 = Mathf.Abs(num2 - 1.77777779f);
		if (num3 > num4)
		{
			if (num > 1440000.0)
			{
				this.currentResolution = ResolutionConstrain.BigResolution;
				Screen.SetResolution((int)this.currentResolution.x, (int)this.currentResolution.y, true);
			}
			else
			{
				Vector2 zero = Vector2.zero;
				zero.x = (float)Screen.currentResolution.width;
				zero.y = (float)Screen.currentResolution.height;
				this.currentResolution = zero;
			}
		}
		else if (num > 1769472.0)
		{
			this.currentResolution = ResolutionConstrain.SmallResolution;
			Screen.SetResolution((int)this.currentResolution.x, (int)this.currentResolution.y, true);
		}
		else
		{
			Vector2 zero2 = Vector2.zero;
			zero2.x = (float)Screen.currentResolution.width;
			zero2.y = (float)Screen.currentResolution.height;
			this.currentResolution = zero2;
		}
	}
}
