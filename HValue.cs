using System;

public class HValue
{
	public short Mark;

	public float Key;

	public float Cost;

	public short X;

	public short Y;

	public short PreX;

	public short PreY;

	public void Set(short x, short y, float cost, float key)
	{
		this.X = x;
		this.Y = y;
		this.Cost = cost;
		this.Key = key;
	}

	public void Reset()
	{
		this.Mark = -1;
		this.Cost = 1E+07f;
		this.Key = 1E+07f;
		this.X = 0;
		this.Y = 0;
		this.PreX = -1;
		this.PreY = -1;
	}
    /// <summary>
    /// 比较评估
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
	public static bool Compare(HValue left, HValue right)
	{
		if (Math.Abs(right.Key - left.Key) < 0.1f)  //key值相等   ,比较cost
		{
			return Math.Abs(right.Cost - left.Cost) >= 0.1f && left.Cost > right.Cost;
		}
		return left.Key < right.Key;
	}
}
