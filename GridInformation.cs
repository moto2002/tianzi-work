using System;
using System.Collections.Generic;

public class GridInformation
{
	private int MaxDynamicCollisizeSize;

	private Dictionary<int, bool> _WalkBlocker;

	public bool IsTree
	{
		get
		{
			return (this.Data & 1) != 0;
		}
	}

	public int Data
	{
		get;
		set;
	}

	public GridInformation()
	{
		this.Data = 0;
	}

	public bool IsWalkBlocker(int collisionSize)
	{
		return this._WalkBlocker[collisionSize];
	}

	public void SetWalkBlocker(int collisionSize, bool blocker)
	{
		this._WalkBlocker[collisionSize] = blocker;
	}

	public void UpdateWalkBlocker()
	{
		if (this._WalkBlocker == null)
		{
			this._WalkBlocker = new Dictionary<int, bool>();
			for (int i = 0; i <= this.MaxDynamicCollisizeSize; i++)
			{
				this._WalkBlocker.Add(i, false);
			}
		}
		for (int j = 0; j <= 0; j++)
		{
			bool value = false;
			if ((this.Data & 2) != 0)
			{
				value = true;
			}
			if ((this.Data & 16) != 0)
			{
				value = true;
			}
			if ((this.Data & 1) != 0)
			{
				value = true;
			}
			this._WalkBlocker[j] = value;
		}
	}
}
