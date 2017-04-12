using System;
using System.Collections.Generic;

public class PanelComparer<T> : IComparer<T> where T : UIPanel
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
			int depth = x.depth;
			int depth2 = y.depth;
			return depth.CompareTo(depth2);
		}
	}
}
