using System;
using System.Collections.Generic;
using UnityEngine;

public class Terrainmapping
{
	private static Camera rttCamera;

	private static int size = 800;

	private static GameObject quad;

	private static Material sntMat;
    /// <summary>
    /// RenderTexture列表，数量限定
    /// </summary>
	public static List<RenderTexture> maps = new List<RenderTexture>();

	private static List<int> mapIndex = new List<int>();
    /// <summary>
    /// 地形mapping数量，创建指定数量的RenderTextuer
    /// </summary>
	public static int mapsCount
	{
		set
		{
			if (value > Terrainmapping.maps.Count)
			{
				for (int i = 0; i < value; i++)
				{
					RenderTexture renderTexture = new RenderTexture(Terrainmapping.size, Terrainmapping.size, 16);
					renderTexture.name = "Terrainmapping" + i;
					renderTexture.isPowerOfTwo = true;
					renderTexture.hideFlags = HideFlags.DontSave;
					renderTexture.format = RenderTextureFormat.RGB565;
					renderTexture.depth = 0;
					Terrainmapping.maps.Add(renderTexture);
					Terrainmapping.mapIndex.Add(-1);
				}
			}
		}
	}
    /// <summary>
    /// 取消指定的renderTexture映射索引
    /// </summary>
    /// <param name="index"></param>
	public static void Cancel(int index)
	{
		if (index >= 0)
		{
			Terrainmapping.mapIndex[index] = -1;
		}
	}
    /// <summary>
    /// 烘焙环境信息到指定RenderTextrue
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="size"></param>
    /// <returns></returns>
	public static int Bake(LODTerrain terrain, int size = 512)
	{
		RenderTexture renderTexture = null;
		int num = -1;
		for (int i = 0; i < Terrainmapping.mapIndex.Count; i++)
		{
			if (Terrainmapping.mapIndex[i] < 0)
			{
				num = i;
				renderTexture = Terrainmapping.maps[i];
				Terrainmapping.mapIndex[i] = num;
				break;
			}
		}
		if (renderTexture != null)
		{
			Terrainmapping.CreateObjects();
			Terrainmapping.UpdateCameraModes();
			Terrainmapping.rttCamera.gameObject.SetActive(true);
			Terrainmapping.rttCamera.enabled = true;
			Terrainmapping.sntMat = terrain.matrial;
			Terrainmapping.quad.renderer.lightmapIndex = terrain.renderer.lightmapIndex;
			Terrainmapping.quad.renderer.lightmapTilingOffset = terrain.renderer.lightmapTilingOffset;
			Terrainmapping.quad.renderer.material = Terrainmapping.sntMat;
			Terrainmapping.rttCamera.targetTexture = renderTexture;
			RenderTexture.active = renderTexture;
			Terrainmapping.rttCamera.Render();
			Terrainmapping.rttCamera.gameObject.SetActive(false);
			RenderTexture.active = null;
			Terrainmapping.quad.renderer.material = null;
			Terrainmapping.sntMat = null;
		}
		return num;
	}
    /// <summary>
    /// 更新采样摄像机采样模式
    /// </summary>
	private static void UpdateCameraModes()
	{
		Terrainmapping.rttCamera.backgroundColor = new Color(1f, 1f, 1f);
		Terrainmapping.rttCamera.farClipPlane = 1f;
		Terrainmapping.rttCamera.nearClipPlane = 0.1f;
		Terrainmapping.rttCamera.orthographic = true;
		Terrainmapping.rttCamera.aspect = 1f;
		Terrainmapping.rttCamera.orthographicSize = 1f;
	}
    /// <summary>
    /// 创建摄像机对象及quad对象
    /// </summary>
	private static void CreateObjects()
	{
		if (Terrainmapping.rttCamera == null)
		{
			GameObject gameObject = new GameObject("SplatsNormalmapCamera", new Type[]
			{
				typeof(Camera)
			});
			Terrainmapping.rttCamera = gameObject.camera;
			Terrainmapping.rttCamera.enabled = false;
			Terrainmapping.rttCamera.cullingMask = GameLayer.Mask_AmbienceSphere;  //渲染环境层
			Terrainmapping.rttCamera.clearFlags = CameraClearFlags.Color;          //清除颜色缓存
			gameObject.transform.position = new Vector3(0f, 0f, 100f);
		}
		if (Terrainmapping.quad == null)
		{
			Terrainmapping.quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			Terrainmapping.quad.layer = GameLayer.Layer_AmbienceSphere;
			Terrainmapping.quad.transform.parent = Terrainmapping.rttCamera.transform;
			Terrainmapping.quad.transform.localScale = new Vector3(2f, 2f, 0f);
			Terrainmapping.quad.transform.localPosition = new Vector3(0f, 0f, 1f);
		}
	}
}
