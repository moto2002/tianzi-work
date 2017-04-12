using System;
using System.Collections.Generic;
using UnityEngine;

public class SplatsNormalmap
{
	private Camera rttCamera;

	public RenderTexture rtt;

	private int size = 2048;

	private GameObject quad;

	private Material sntMat;

	private static Dictionary<string, GameObject> quads = new Dictionary<string, GameObject>();

	private string key = string.Empty;

	private static float tick = 0f;

	private Texture2D tex;

	public SplatsNormalmap(string key)
	{
		this.key = key;
	}

	public void Real()
	{
		this.rttCamera.enabled = false;
		this.rttCamera.gameObject.SetActive(false);
	}

	public Texture Bake(LODTerrainData terrainData, int size = 2048)
	{
		this.size = size;
		this.CreateObjects();
		this.UpdateCameraModes();
		SplatsNormalmap.quads[this.key].SetActive(true);
		this.rttCamera.gameObject.SetActive(true);
		this.rttCamera.enabled = true;
		TerrainConfig terrainConfig = GameScene.mainScene.terrainConfig;
		this.sntMat.SetTexture("_Control", terrainData.splatsmapTex);
		for (int i = 0; i < terrainData.spaltsmapLayers; i++)
		{
			Splat splat = terrainData.splats[i];
			if (splat != null)
			{
				this.sntMat.SetTexture("_Splat" + i, splat.normalMap);
				this.sntMat.SetTextureScale("_Splat" + i, new Vector2(splat.tilingOffset.x, splat.tilingOffset.y));
				this.sntMat.SetTextureOffset("_Splat" + i, Vector2.zero);
			}
			else
			{
				splat = terrainData.splats[0];
				this.sntMat.SetTexture("_Splat" + i, terrainConfig.baseSplat.normalMap);
				this.sntMat.SetTextureScale("_Splat" + i, new Vector2(splat.tilingOffset.x, splat.tilingOffset.y));
				this.sntMat.SetTextureOffset("_Splat" + i, Vector2.zero);
			}
		}
		this.rttCamera.targetTexture = this.rtt;
		RenderTexture.active = this.rtt;
		this.tex.ReadPixels(new Rect(0f, 0f, (float)size, (float)size), 0, 0);
		this.tex.Apply();
		return this.tex;
	}

	private void UpdateCameraModes()
	{
		this.rttCamera.backgroundColor = new Color(1f, 1f, 1f);
		this.rttCamera.farClipPlane = 1f;
		this.rttCamera.nearClipPlane = 0.1f;
		this.rttCamera.orthographic = true;
		this.rttCamera.aspect = 1f;
		this.rttCamera.orthographicSize = 1f;
	}

	private void CreateObjects()
	{
		if (this.rtt == null)
		{
			this.rtt = new RenderTexture(this.size, this.size, 24);
			this.rtt.name = "SplatsNormalmapRTT";
			this.rtt.isPowerOfTwo = true;
			this.rtt.hideFlags = HideFlags.DontSave;
		}
		if (this.rttCamera == null)
		{
			GameObject gameObject = new GameObject("SplatsNormalmapCamera", new Type[]
			{
				typeof(Camera),
				typeof(Skybox)
			});
			Transform arg_B4_0 = gameObject.transform;
			float arg_AF_0 = 0f;
			float arg_AF_1 = 0f;
			float expr_A3 = SplatsNormalmap.tick;
			SplatsNormalmap.tick = expr_A3 + 1f;
			arg_B4_0.position = new Vector3(arg_AF_0, arg_AF_1, expr_A3);
			this.rttCamera = gameObject.camera;
			this.rttCamera.enabled = false;
			this.rttCamera.gameObject.AddComponent("FlareLayer");
		}
		if (this.quad == null)
		{
			this.quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			this.quad.transform.parent = this.rttCamera.transform;
			this.quad.transform.localScale = new Vector3(2f, 2f, 0f);
			this.quad.transform.localPosition = new Vector3(0f, 0f, 1f);
			this.sntMat = new Material(Shader.Find("Snail/Terrain-Splats-Bump-Texture"));
			this.quad.renderer.material = this.sntMat;
			this.quad.SetActive(false);
			SplatsNormalmap.quads.Add(this.key, this.quad);
		}
		if (this.tex == null)
		{
			this.tex = new Texture2D(this.size, this.size, TextureFormat.RGB24, false);
		}
	}
}
