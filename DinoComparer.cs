using System;
using System.Collections.Generic;
using UnityEngine;

public class DinoComparer<T> : IComparer<T> where T : UnityEngine.Object
{
	public int Compare(T x, T y)
	{
		if (x == null)
		{
			if (y == null)
			{
				return 0;
			}
			return -1;
		}
		else
		{
			if (y == null)
			{
				return 1;
			}
			string name = x.name;
			string name2 = y.name;
			int num = name.Length.CompareTo(name2.Length);
			if (num != 0)
			{
				return num;
			}
			return name.CompareTo(name2);
		}
	}
}
