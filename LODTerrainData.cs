using System;
using System.IO;
using UnityEngine;

public class LODTerrainData
{
	public int heightmapResolution = 32;

	public int splatmapResolution = 64;

	public Vector3 size = Vector3.zero;

	public Splat[] splats;

	public int segmentsW = 16;

	public int segmentsH = 16;

	public float[,] heightmap;

	public float[,,] splatsmap;

	private static float[,,] defaultSplatsmap;

	public int spaltsmapLayers;

	public Texture2D _heightmapTex;

	public Texture2D _splatsmapTex;

	private bool _heightmapDirty;

	private bool _splatsmapDirty;

	public Vector3[] vertices;

	public Vector3[] normals;

	public Vector4[] tangents;

	public Vector2[] uvs;

	public int[] triangles;

	public TerrainConfig terrainConfig;

	private WaterData waterData;

	public static Vector3[] temporaryVertices;

	public int splatsCount
	{
		get
		{
			for (int i = 0; i < this.spaltsmapLayers; i++)
			{
				if (this.splats[i] == null)
				{
					return i;
				}
			}
			return this.spaltsmapLayers;
		}
	}

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

	public bool splatsmapDirty
	{
		get
		{
			return this._splatsmapDirty;
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
	}

	public Texture2D splatsmapTex
	{
		get
		{
			if (this._splatsmapDirty)
			{
				this.GenerateSplatsmapTex();
			}
			return this._splatsmapTex;
		}
	}

	public LODTerrainData()
	{
		this.terrainConfig = GameScene.mainScene.terrainConfig;
		this.heightmapResolution = this.terrainConfig.heightmapResolution;
		this.splatmapResolution = this.terrainConfig.splatmapResolution;
		this.spaltsmapLayers = this.terrainConfig.spaltsmapLayers;
		this.size.x = (float)this.terrainConfig.tileSize;
		this.size.z = (float)this.terrainConfig.tileSize;
		this.size.y = this.terrainConfig.maxTerrainHeight;
		this.splats = new Splat[this.spaltsmapLayers];
		this.splats[0] = this.terrainConfig.baseSplat;
		if (!GameScene.isPlaying)
		{
			this.heightmap = new float[this.heightmapResolution, this.heightmapResolution];
			for (int i = 0; i < this.heightmapResolution; i++)
			{
				for (int j = 0; j < this.heightmapResolution; j++)
				{
					this.heightmap[i, j] = this.terrainConfig.defaultTerrainHeight;
				}
			}
			if (this.splatsmap == null)
			{
				this.splatsmap = new float[this.splatmapResolution, this.splatmapResolution, this.spaltsmapLayers];
				for (int i = 0; i < this.splatmapResolution; i++)
				{
					for (int j = 0; j < this.splatmapResolution; j++)
					{
						this.splatsmap[i, j, 0] = 1f;
						for (int k = 1; k < this.spaltsmapLayers; k++)
						{
							this.splatsmap[i, j, k] = 0f;
						}
					}
				}
			}
			this.GenerateSplatsmapTex();
		}
	}

	public void Release()
	{
		this.vertices = null;
		this.triangles = null;
		this.normals = null;
		this.heightmap = null;
		this.splatsmap = null;
	}

	public void Read(byte[] bytes)
	{
		MemoryStream memoryStream = new MemoryStream(bytes);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		long position = binaryReader.BaseStream.Position;
		if (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length && binaryReader.ReadInt32() == 10016)
		{
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				Splat splat = new Splat();
				string text = binaryReader.ReadString();
				splat.texture = AssetLibrary.Load(text, AssetType.Texture2D, LoadType.Type_Auto).texture2D;
				splat.key = text;
				splat.tilingOffset.x = binaryReader.ReadSingle();
				splat.tilingOffset.y = binaryReader.ReadSingle();
				splat.tilingOffset.z = binaryReader.ReadSingle();
				splat.tilingOffset.w = binaryReader.ReadSingle();
				this.splats[i] = splat;
			}
			int num2 = binaryReader.ReadInt32();
			if (GameScene.isPlaying)
			{
				if (LODTerrainData.temporaryVertices == null)
				{
					LODTerrainData.temporaryVertices = new Vector3[num2];
					for (int i = 0; i < 289; i++)
					{
						LODTerrainData.temporaryVertices[i] = Vector3.zero;
					}
				}
				for (int i = 0; i < num2; i++)
				{
					LODTerrainData.temporaryVertices[i].x = binaryReader.ReadSingle();
					LODTerrainData.temporaryVertices[i].y = binaryReader.ReadSingle();
					LODTerrainData.temporaryVertices[i].z = binaryReader.ReadSingle();
				}
				this.vertices = LODTerrainData.temporaryVertices;
			}
			else
			{
				this.vertices = new Vector3[num2];
				for (int i = 0; i < num2; i++)
				{
					Vector3 zero = Vector3.zero;
					zero.x = binaryReader.ReadSingle();
					zero.y = binaryReader.ReadSingle();
					zero.z = binaryReader.ReadSingle();
					this.vertices[i] = zero;
				}
			}
			if (GameScene.isPlaying)
			{
				binaryReader.Close();
				memoryStream.Flush();
				return;
			}
			int num3 = binaryReader.ReadInt32();
			this.triangles = new int[num3];
			for (int i = 0; i < num3; i++)
			{
				this.triangles[i] = binaryReader.ReadInt32();
			}
			int num4 = binaryReader.ReadInt32();
			this.uvs = new Vector2[num4];
			for (int i = 0; i < num4; i++)
			{
				Vector2 zero2 = Vector2.zero;
				zero2.x = binaryReader.ReadSingle();
				zero2.y = binaryReader.ReadSingle();
				this.uvs[i] = zero2;
			}
			int num5 = binaryReader.ReadInt32();
			this.normals = new Vector3[num5];
			for (int i = 0; i < num5; i++)
			{
				Vector3 zero3 = Vector3.zero;
				zero3.x = binaryReader.ReadSingle();
				zero3.y = binaryReader.ReadSingle();
				zero3.z = binaryReader.ReadSingle();
				this.normals[i] = zero3;
			}
			int num6 = binaryReader.ReadInt32();
			this.tangents = new Vector4[num6];
			for (int i = 0; i < num6; i++)
			{
				Vector4 zero4 = Vector4.zero;
				zero4.x = binaryReader.ReadSingle();
				zero4.y = binaryReader.ReadSingle();
				zero4.z = binaryReader.ReadSingle();
				zero4.w = binaryReader.ReadSingle();
				this.tangents[i] = zero4;
			}
		}
		for (int i = 0; i < this.heightmapResolution; i++)
		{
			for (int j = 0; j < this.heightmapResolution; j++)
			{
				this.heightmap[j, i] = binaryReader.ReadSingle();
			}
		}
		int num7 = 0;
		position = binaryReader.BaseStream.Position;
		if (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
		{
			if (binaryReader.ReadInt32() == 10013)
			{
				num7 = this.spaltsmapLayers;
			}
			else
			{
				binaryReader.BaseStream.Position = position;
			}
		}
		for (int i = 0; i < this.splatmapResolution; i++)
		{
			for (int j = 0; j < this.splatmapResolution; j++)
			{
				for (int k = 0; k < num7; k++)
				{
					this.splatsmap[j, i, k] = binaryReader.ReadSingle();
				}
			}
		}
		this.GenerateSplatsmapTex();
	}

	public void GenerateHeightMapTex()
	{
		this._heightmapTex = new Texture2D(this.heightmapResolution, this.heightmapResolution, TextureFormat.ARGB32, false);
		this._heightmapTex.wrapMode = TextureWrapMode.Clamp;
		for (int i = 0; i < this.heightmapResolution; i++)
		{
			for (int j = 0; j < this.heightmapResolution; j++)
			{
				float num = this.heightmap[j, i] / this.size.y;
				this._heightmapTex.SetPixel(j, i, new Color(num, num, num, 1f));
			}
		}
		this._heightmapTex.Apply();
	}

	public void GenerateSplatsmapTex()
	{
		float[,,] array = this.splatsmap;
		this._splatsmapTex = new Texture2D(this.splatmapResolution, this.splatmapResolution, TextureFormat.ARGB32, false);
		this._splatsmapTex.wrapMode = TextureWrapMode.Clamp;
		for (int i = 0; i < this.splatmapResolution; i++)
		{
			for (int j = 0; j < this.splatmapResolution; j++)
			{
				Color color = new Color(0f, 0f, 0f, 0f);
				color.r = array[j, i, 0];
				if (this.terrainConfig.spaltsmapLayers > 1)
				{
					color.g = array[j, i, 1];
				}
				if (this.terrainConfig.spaltsmapLayers > 2)
				{
					color.b = array[j, i, 2];
				}
				if (this.terrainConfig.spaltsmapLayers > 3)
				{
					color.a = array[j, i, 3];
				}
				this._splatsmapTex.SetPixel(i, j, color);
			}
		}
		this._splatsmapTex.Apply();
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

	public float GetHeight(int x, int z)
	{
		return this.heightmap[z, x];
	}

	public float[,,] GetSplatsmap(int xBase, int yBase, int width, int height)
	{
		float[,,] array = new float[height, width, this.spaltsmapLayers];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < this.spaltsmapLayers; k++)
				{
					array[j, i, k] = this.splatsmap[yBase + j, xBase + i, k];
				}
			}
		}
		return array;
	}

	public void SetSplasmap(int xBase, int yBase, float[,,] map)
	{
		int length = map.GetLength(1);
		int length2 = map.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				for (int k = 0; k < this.spaltsmapLayers; k++)
				{
					this.splatsmap[yBase + j, xBase + i, k] = map[j, i, k];
				}
			}
		}
		this._splatsmapDirty = true;
	}
}
