using System;
using System.Collections.Generic;
using UnityEngine;

public class LightmapCorrection
{
	private static Camera rttCamera;

	private static int size = 1024;

	private static GameObject quad;

	private static Material sntMat;

	public static List<Texture2D> maps = new List<Texture2D>();

	private static List<int> mapIndex = new List<int>();

	private static RenderTexture tagetRTT;

	public static int mapsCount
	{
		get
		{
			return LightmapCorrection.maps.Count;
		}
		set
		{
			if (value > LightmapCorrection.maps.Count)
			{
				int num = value - LightmapCorrection.maps.Count;
				for (int i = 0; i < num; i++)
				{
					Texture2D item = new Texture2D(LightmapCorrection.size, LightmapCorrection.size, TextureFormat.ARGB32, false);
					LightmapCorrection.maps.Add(item);
					LightmapCorrection.mapIndex.Add(-1);
				}
			}
		}
	}

	public static void Clear()
	{
		if (LightmapCorrection.tagetRTT != null)
		{
			LightmapCorrection.tagetRTT.Release();
		}
		LightmapCorrection.tagetRTT = null;
		LightmapCorrection.maps.Clear();
		LightmapCorrection.mapIndex.Clear();
	}

	public static Texture2D Bake(Texture2D lightmapTex, int size = 1024)
	{
		Texture2D texture2D = null;
		for (int i = 0; i < LightmapCorrection.mapIndex.Count; i++)
		{
			if (LightmapCorrection.mapIndex[i] < 0)
			{
				int value = i;
				texture2D = LightmapCorrection.maps[i];
				LightmapCorrection.mapIndex[i] = value;
				break;
			}
		}
		if (texture2D != null)
		{
			if (LightmapCorrection.sntMat == null)
			{
				LightmapCorrection.sntMat = new Material(Shader.Find("Snail/LightmapCorrection"));
			}
			LightmapCorrection.sntMat.mainTexture = lightmapTex;
			if (LightmapCorrection.tagetRTT == null)
			{
				LightmapCorrection.tagetRTT = new RenderTexture(size, size, 0, RenderTextureFormat.ARGB32);
				LightmapCorrection.tagetRTT.isPowerOfTwo = true;
				LightmapCorrection.tagetRTT.hideFlags = HideFlags.DontSave;
				LightmapCorrection.tagetRTT.format = RenderTextureFormat.ARGB32;
				LightmapCorrection.tagetRTT.depth = 0;
				LightmapCorrection.tagetRTT.useMipMap = false;
			}
			RenderTexture.active = LightmapCorrection.tagetRTT;
			Graphics.Blit(lightmapTex, LightmapCorrection.tagetRTT, LightmapCorrection.sntMat);
		}
		if (LightmapCorrection.sntMat != null)
		{
			LightmapCorrection.sntMat.mainTexture = null;
		}
		texture2D.ReadPixels(new Rect(0f, 0f, (float)size, (float)size), 0, 0);
		texture2D.Compress(true);
		texture2D.Apply();
		RenderTexture.active = null;
		return texture2D;
	}
}
