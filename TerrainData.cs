using System;
using UnityEngine;

public class TerrainData
{
    /// <summary>
    /// 高度贴图分辨率
    /// </summary>
	public int heightmapResolution = 32;
    /// <summary>
    /// 地形最大高度值
    /// </summary>
	public float maxTerrainHeight = 1000f;
    /// <summary>
    /// 地形高度图高度数组
    /// </summary>
	public float[,] heightmap;
    /// <summary>
    /// 高度图纹理
    /// </summary>
	public Texture2D _heightmapTex;
    /// <summary>
    /// 高度图更新标记
    /// </summary>
	private bool _heightmapDirty = true;
    /// <summary>
    /// 读取设置高度图更新标记
    /// </summary>
	public bool heightmapDirty
	{
		get
		{
			return this._heightmapDirty;
		}
		set
		{
			this._heightmapDirty = value;
		}
	}
    /// <summary>
    /// 获取或设置高度贴图纹理
    /// </summary>
	public Texture2D heightmapTex
	{
		get
		{
			if (this._heightmapDirty)
			{
				this.GenerateHeightMapTex();
			}
			return this._heightmapTex;
		}
		set
		{
			this._heightmapTex = value;
			int length = this.heightmap.GetLength(1);
			int length2 = this.heightmap.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					this.heightmap[j, i] = this._heightmapTex.GetPixel(j, i).r * this.maxTerrainHeight;
				}
			}
			this._heightmapDirty = true;
		}
	}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="heightmapResolution">高度贴图分辨率</param>
    /// <param name="splatmapResolution">splatmap贴图分辨率</param>
    /// <param name="spaltsmapLayers">splatmap混合层数????</param>
    /// <param name="maxTerrainHeight">地形最大高度</param>
    /// <param name="defaultTerrainHeight">地形默认高度</param>
	public TerrainData(int heightmapResolution = 480, int splatmapResolution = 480, int spaltsmapLayers = 4, float maxTerrainHeight = 1000f, float defaultTerrainHeight = 100f)
	{
		this.heightmapResolution = heightmapResolution;
		this.maxTerrainHeight = maxTerrainHeight;
		this.heightmap = new float[this.heightmapResolution, this.heightmapResolution];
		for (int i = 0; i < this.heightmapResolution; i++)
		{
			for (int j = 0; j < this.heightmapResolution; j++)
			{
				this.heightmap[i, j] = defaultTerrainHeight;
			}
		}
	}
    /// <summary>
    /// 获取指定地形顶点处的高度值
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
	public float GetHeight(int x, int z)
	{
		return this.heightmap[z, x];
	}
    /// <summary>
    /// 设置指定顶点处的高度值
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="value"></param>
	public void SetHeight(int x, int z, float value)
	{
		this.heightmap[z, x] = value;
	}
    /// <summary>
    /// 根据地形高度值生成地图高度纹理
    /// </summary>
	public void GenerateHeightMapTex()
	{
		this._heightmapTex = new Texture2D(this.heightmapResolution, this.heightmapResolution, TextureFormat.ARGB32, false);
		this._heightmapTex.wrapMode = TextureWrapMode.Clamp;
		for (int i = 0; i < this.heightmapResolution; i++)
		{
			for (int j = 0; j < this.heightmapResolution; j++)
			{
				float num = this.heightmap[j, i] / this.maxTerrainHeight;
				this._heightmapTex.SetPixel(j, i, new Color(num, num, num, 1f));
			}
		}
		this._heightmapTex.Apply();
	}
    /// <summary>
    /// 释放内存和引用
    /// </summary>
	public void Release()
	{
		this.heightmap = null;
		if (this._heightmapTex != null)
		{
			DelegateProxy.GameDestory(this._heightmapTex);
			this._heightmapTex = null;
		}
	}
    /// <summary>
    /// 获取指定范围内的高度数组
    /// </summary>
    /// <param name="xBase"></param>
    /// <param name="yBase"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
	public float[,] GetHeights(int xBase, int yBase, int width, int height)
	{
		float[,] array = new float[height, width];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				array[j, i] = this.heightmap[yBase + j, xBase + i];
			}
		}
		return array;
	}
    /// <summary>
    /// 设置一定范围内的高度值
    /// </summary>
    /// <param name="xBase"></param>
    /// <param name="yBase"></param>
    /// <param name="heights"></param>
	public void SetHeights(int xBase, int yBase, float[,] heights)
	{
		int length = heights.GetLength(1);
		int length2 = heights.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				this.heightmap[yBase + j, xBase + i] = heights[j, i];
			}
		}
		this._heightmapDirty = true;
	}
    /// <summary>
    /// 设定一定范围内的为统一高度值
    /// </summary>
    /// <param name="xBase"></param>
    /// <param name="yBase"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="value"></param>
	public void SetHeights(int xBase, int yBase, int width, int height, float value)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				this.heightmap[yBase + j, xBase + i] = value;
			}
		}
		this._heightmapDirty = true;
	}
}
