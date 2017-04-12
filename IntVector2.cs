using System;

public class IntVector2
{
	public int x;

	public int y;

	public IntVector2(int x = 0, int y = 0)
	{
		this.x = x;
		this.y = y;
	}

	public static bool operator ==(IntVector2 p1, IntVector2 p2)
	{
		return (p2 != null || p1 == null) && ((p2 == null && p1 == null) || (p1.x == p2.x && p1.y == p2.y));
	}

	public static bool operator !=(IntVector2 p1, IntVector2 p2)
	{
		return (p2 == null && p1 != null) || ((p2 != null || p1 != null) && (p1.x != p2.x || p1.y != p2.y));
	}
}
