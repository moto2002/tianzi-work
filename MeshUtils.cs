using System;
using UnityEngine;

public class MeshUtils
{
	public static Vector4 CalculaMesUV2Rect(Mesh mesh)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < mesh.uv2.Length; i++)
		{
			Vector2 vector = mesh.uv2[i];
			if (vector.x > num)
			{
				num = vector.x;
			}
			if (vector.x < num2)
			{
				num2 = vector.x;
			}
			if (vector.y > num3)
			{
				num3 = vector.y;
			}
			if (vector.y < num4)
			{
				num4 = vector.y;
			}
		}
		return new Vector4(num2, num4, num - num2, num3 - num4);
	}
}
