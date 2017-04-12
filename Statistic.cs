using System;
using System.Collections.Generic;
using UnityEngine;

public class Statistic
{
	private static List<Material> matList = new List<Material>();

	public static List<Material> materials
	{
		get
		{
			return Statistic.matList;
		}
	}

	public static void Push(UnityEngine.Object asset, AssetType type)
	{
		if (type == AssetType.Material)
		{
			Material item = asset as Material;
			if (!Statistic.matList.Contains(item))
			{
				Statistic.matList.Add(item);
			}
		}
	}

	public static int Useage(AssetType type)
	{
		if (type == AssetType.Material)
		{
			return Statistic.matList.Count;
		}
		return 0;
	}

	public static void Clear()
	{
		Statistic.matList = new List<Material>();
	}
}
