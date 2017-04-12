using System;
using UnityEngine;

public class Brush
{
	public int size = 32;

	public float[] strength;

	public float strengthen = 1f;

	private Texture2D m_Brush;

	public LODTerrain terrain;

	public global::TerrainData terrainData;

	public float splatMaxValue;

	public LODTerrainTool tool;

	private Projector m_BrushProjector;

	private Texture2D m_Preview;

	public float targetHeight;

	public float targetAlpha = 1f;

	public Projector previewProjector
	{
		get
		{
			return this.m_BrushProjector;
		}
	}

	private void CreatePreviewBrush()
	{
		this.m_BrushProjector = new GameObject
		{
			name = "brushProject"
		}.AddComponent<Projector>();
		this.m_BrushProjector.enabled = true;
		this.m_BrushProjector.nearClipPlane = -1000f;
		this.m_BrushProjector.farClipPlane = 1000f;
		this.m_BrushProjector.orthographic = true;
		this.m_BrushProjector.orthographicSize = 30f;
		this.m_BrushProjector.transform.Rotate(90f, 0f, 0f);
		Material material = new Material("Shader \"Hidden/Terrain Brush Preview\" {\nProperties {\n\t_MainTex (\"Main\", 2D) = \"gray\" { TexGen ObjectLinear }\n\t_CutoutTex (\"Cutout\", 2D) = \"black\" { TexGen ObjectLinear }\n}\nSubshader {\n\tZWrite Off\n\tOffset -1, -1\n\tFog { Mode Off }\n\tAlphaTest Greater 0\n\tColorMask RGB\n\tPass\n\t{\n\t\tBlend SrcAlpha OneMinusSrcAlpha\n\t\tSetTexture [_MainTex]\n\t\t{\n\t\t\tconstantColor (.2,.7,1,.5)\n\t\t\tcombine constant, texture * constant\n\t\t\tMatrix [_Projector]\n\t\t}\n\n\t\tSetTexture [_CutoutTex]\n\t\t{\n\t\t\tcombine previous, previous * texture\n\t\t\tMatrix [_Projector]\n\t\t}\n\t}\n}\n}");
		this.m_BrushProjector.material = material;
	}

	public void setBrushVisible(bool value)
	{
		if (this.m_BrushProjector != null)
		{
			this.m_BrushProjector.enabled = value;
		}
	}

	public void Dispose()
	{
		if (this.m_BrushProjector != null)
		{
			DelegateProxy.DestroyObjectImmediate(this.m_BrushProjector.material.shader);
			DelegateProxy.DestroyObjectImmediate(this.m_BrushProjector.material);
			DelegateProxy.DestroyObjectImmediate(this.m_BrushProjector.gameObject);
			this.m_BrushProjector = null;
		}
		DelegateProxy.DestroyObjectImmediate(this.m_Preview);
		this.m_Preview = null;
	}

	public bool Load(Texture2D brushTex, int size)
	{
		if (this.m_Brush == brushTex && size == this.size && this.strength != null)
		{
			return true;
		}
		if (brushTex != null)
		{
			float num = (float)size;
			this.size = size;
			this.strength = new float[this.size * this.size];
			if (this.size > 3)
			{
				for (int i = 0; i < this.size; i++)
				{
					for (int j = 0; j < this.size; j++)
					{
						this.strength[i * this.size + j] = brushTex.GetPixelBilinear(((float)j + 0.5f) / num, (float)i / num).a;
					}
				}
			}
			else
			{
				for (int k = 0; k < this.strength.Length; k++)
				{
					this.strength[k] = 1f;
				}
			}
			DelegateProxy.DestroyObjectImmediate(this.m_Preview);
			this.m_Preview = new Texture2D(this.size, this.size, TextureFormat.ARGB32, false);
			this.m_Preview.wrapMode = TextureWrapMode.Clamp;
			this.m_Preview.filterMode = FilterMode.Point;
			Color[] array = new Color[this.size * this.size];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = new Color(0f, 0f, 0f, this.strength[l]);
			}
			this.m_Preview.SetPixels(0, 0, this.size, this.size, array, 0);
			this.m_Preview.Apply();
			if (this.m_BrushProjector == null)
			{
				this.CreatePreviewBrush();
			}
			this.m_BrushProjector.orthographicSize = (float)size * 0.46875f;
			this.m_BrushProjector.material.SetTexture("_CutoutTex", this.m_Preview);
			this.m_BrushProjector.material.mainTexture = brushTex;
			this.m_Brush = brushTex;
			return true;
		}
		this.strength = new float[]
		{
			1f
		};
		this.size = 1;
		return false;
	}

	public float GetStrengthInt(int ix, int iy)
	{
		ix = Mathf.Clamp(ix, 0, this.size - 1);
		iy = Mathf.Clamp(iy, 0, this.size - 1);
		return this.strength[iy * this.size + ix];
	}

	public float GetStrength(int ix, int iy)
	{
		ix = Mathf.Clamp(ix, 0, this.size - 1);
		iy = Mathf.Clamp(this.size - iy, 0, this.size - 1);
		return this.strength[iy * this.size + ix];
	}

	public void PaintHeightmap(global::TerrainData terrainData, Vector3 worldPostion)
	{
		this.terrainData = terrainData;
		int num = Mathf.CeilToInt(worldPostion.x + (float)GameScene.mainScene.terrainConfig.sceneWidth * 0.5f);
		int num2 = Mathf.CeilToInt(worldPostion.z + (float)GameScene.mainScene.terrainConfig.sceneHeight * 0.5f);
		int num3 = this.size / 2;
		int num4 = this.size % 2;
		int num5 = Mathf.Clamp(num - num3, 0, this.terrainData.heightmapResolution);
		int num6 = Mathf.Clamp(num2 - num3, 0, this.terrainData.heightmapResolution);
		int num7 = Mathf.Clamp(num + num3 + num4, 0, this.terrainData.heightmapResolution);
		int num8 = Mathf.Clamp(num2 + num3 + num4, 0, this.terrainData.heightmapResolution);
		int num9 = num7 - num5;
		int num10 = num8 - num6;
		float[,] heights = this.terrainData.GetHeights(num5, num6, num9, num10);
		for (int i = 0; i < num10; i++)
		{
			for (int j = 0; j < num9; j++)
			{
				int ix = num5 + j - (num - num3);
				int iy = num6 + i - (num2 - num3);
				float strengthInt = this.GetStrengthInt(ix, iy);
				float num11 = heights[i, j];
				num11 = this.ApplyBrush(num11, strengthInt * this.strengthen, j + num5, i + num6);
				heights[i, j] = num11;
			}
		}
		terrainData.SetHeights(num5, num6, heights);
	}

	private float ApplyBrush(float height, float brushStrength, int x, int y)
	{
		if (this.tool == LODTerrainTool.PaintHeight)
		{
			return height + brushStrength;
		}
		if (this.tool == LODTerrainTool.SetHeight)
		{
			if (this.targetHeight > height)
			{
				height += brushStrength;
				height = Mathf.Min(height, this.targetHeight);
				return height;
			}
			height -= brushStrength;
			height = Mathf.Max(height, this.targetHeight);
			return height;
		}
		else
		{
			if (this.tool == LODTerrainTool.SmoothHeight)
			{
				return Mathf.Lerp(height, this.Smooth(x, y), brushStrength);
			}
			return height;
		}
	}

	private float Smooth(int x, int y)
	{
		float num = 0f;
		num += this.terrainData.GetHeight(x, y);
		num += this.terrainData.GetHeight(x + 1, y);
		num += this.terrainData.GetHeight(x - 1, y);
		num += this.terrainData.GetHeight(x + 1, y + 1) * 0.75f;
		num += this.terrainData.GetHeight(x - 1, y + 1) * 0.75f;
		num += this.terrainData.GetHeight(x + 1, y - 1) * 0.75f;
		num += this.terrainData.GetHeight(x - 1, y - 1) * 0.75f;
		num += this.terrainData.GetHeight(x, y + 1);
		num += this.terrainData.GetHeight(x, y - 1);
		return num / 8f;
	}

	public void PaintSplatsmap(LODTerrain terrain, Vector3 worldPos, Splat splat, int insertIndex = -1)
	{
		this.terrain = terrain;
		int num = -1;
		for (int i = 0; i < this.terrain.terrainData.splats.Length; i++)
		{
			if (this.terrain.terrainData.splats[i] == null)
			{
				this.terrain.terrainData.splats[i] = splat;
				num = i;
				break;
			}
			if (this.terrain.terrainData.splats[i].key == splat.key)
			{
				this.terrain.terrainData.splats[i] = splat;
				num = i;
				break;
			}
		}
		if (num < 0 && insertIndex >= 0)
		{
			if (insertIndex > this.terrain.terrainData.splats.Length - 1)
			{
				insertIndex = this.terrain.terrainData.splats.Length - 1;
			}
			this.terrain.terrainData.splats[insertIndex] = splat;
		}
		if (num < 0)
		{
			return;
		}
		if (num < this.terrain.terrainData.spaltsmapLayers)
		{
			int num2 = (int)(terrain.transform.position.x - worldPos.x) + 16;
			int num3 = (int)(terrain.transform.position.z - worldPos.z) + 16;
			int num4 = this.size / 2;
			float[,,] splatsmap = this.terrain.terrainData.splatsmap;
			for (int j = 0; j < 32; j++)
			{
				for (int k = 0; k < 32; k++)
				{
					int num5 = num2 - (31 - k) + num4;
					int num6 = num3 - j + num4;
					if (num5 >= 0 && num5 < this.size)
					{
						if (num6 >= 0 && num6 < this.size)
						{
							float num7 = this.GetStrength(num5, num6);
							float num8 = this.ApplyBrush(splatsmap[j, k, num], num7 * this.strengthen);
							splatsmap[j, k, num] = num8;
							this.Normalize(k, j, num, splatsmap);
						}
					}
				}
			}
			this.terrain.terrainData.SetSplasmap(0, 0, splatsmap);
		}
		else
		{
			LogSystem.Log(new object[]
			{
				"超出纹理层级。"
			});
		}
	}

	private float ApplyBrush(float value, float brushStrength)
	{
		if (this.targetAlpha > value)
		{
			value += brushStrength;
			value = Mathf.Min(value, this.targetAlpha);
			return value;
		}
		value -= brushStrength;
		value = Mathf.Max(value, this.targetAlpha);
		return value;
	}

	private void Normalize(int x, int y, int splatIndex, float[,,] alphamap)
	{
		float num = alphamap[y, x, splatIndex];
		float num2 = 0f;
		int length = alphamap.GetLength(2);
		for (int i = 0; i < length; i++)
		{
			if (i != splatIndex)
			{
				num2 += alphamap[y, x, i];
			}
		}
		if ((double)num2 > 0.01)
		{
			float num3 = (1f - num) / num2;
			for (int j = 0; j < length; j++)
			{
				if (j != splatIndex)
				{
					float num4 = alphamap[y, x, j];
					alphamap[y, x, j] = num4 * num3;
				}
			}
		}
		else
		{
			for (int k = 0; k < length; k++)
			{
				alphamap[y, x, k] = ((k == splatIndex) ? 1f : 0f);
			}
		}
	}
}
