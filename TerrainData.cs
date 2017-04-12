using System;
using UnityEngine;

public class TerrainData
{
	public int heightmapResolution = 32;

	public float maxTerrainHeight = 1000f;

	public float[,] heightmap;

	public Texture2D _heightmapTex;

	private bool _heightmapDirty = true;

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

	public float GetHeight(int x, int z)
	{
		return this.heightmap[z, x];
	}

	public void SetHeight(int x, int z, float value)
	{
		this.heightmap[z, x] = value;
	}

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

	public void Release()
	{
		this.heightmap = null;
		if (this._heightmapTex != null)
		{
			DelegateProxy.GameDestory(this._heightmapTex);
			this._heightmapTex = null;
		}
	}

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
