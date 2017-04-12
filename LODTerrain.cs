using System;
using System.Collections.Generic;
using UnityEngine;

public class LODTerrain : MonoBehaviour
{
	public bool hasWater;

	public LODTerrain left;

	public LODTerrain right;

	public LODTerrain top;

	public LODTerrain bot;

	public LODTerrain top_left;

	public LODTerrain top_right;

	public LODTerrain bot_left;

	public LODTerrain bot_right;

	public LODTerrainData terrainData;

	public Mesh mesh;

	public Vector3[] vertices;

	public Vector3[] normals;

	public Vector2[] uvs;

	public Vector4[] tangents;

	public int[] triangles;

	public Material matrial;

	public MeshRenderer terrainRenderer;

	public TerrainConfig terrainConfig;

	private int tick;

	public static Vector3[] shareVertices;

	public static int[] shareTriangles;

	public static Vector2[] shareUVS;

	public string splatsMapPath = string.Empty;

	private Shader bakeShader = Shader.Find("Snail/TerrainBake");

	private Shader bakeNoShadowShader = Shader.Find("Snail/TerrainBakeNoShadow");

	private Shader terrainMobileShader = Shader.Find("Snail/TerrainMobile");

	private Shader terrainMobileWithPointLightShader = Shader.Find("Snail/TerrainMobileWithPointLight");

	private Shader realLightShader = Shader.Find("Snail/TerrainSplat4");

	public int terrainMapIndex = -1;

	private Material bakeMat;

	private Vector3 nCenter;

	private static Dictionary<string, Vector3> sideNormals = new Dictionary<string, Vector3>();

	private float[] _faceNormals;

	private float[] _faceWeights;

	private float[] _faceTangents;

	public void Init()
	{
		this.BuildGeometry();
		this.BuildTriangles();
		this.BuildUVs();
		this.BuildNormals();
		this.BuildTangents();
		this.BuildMaterial(null);
	}

	private void Update()
	{
		this.tick++;
		if (this.tick == 3 && this.uvs == null)
		{
			this.BuildUVs();
		}
		if (this.tick == 4 && this.triangles == null)
		{
			this.BuildTriangles();
		}
		if (this.tick == 5 && this.matrial == null)
		{
			this.BuildMaterial(null);
		}
		if (this.tick == 6)
		{
			this.terrainRenderer.enabled = true;
		}
	}

	public static LODTerrain CreateTerrainGameObject(LODTerrainData terrainData, bool useTrrainData = false)
	{
		GameObject gameObject = new GameObject();
		LODTerrain lODTerrain = gameObject.AddComponent<LODTerrain>();
		gameObject.isStatic = true;
		lODTerrain.terrainData = terrainData;
		lODTerrain.terrainConfig = GameScene.mainScene.terrainConfig;
		lODTerrain.mesh = new Mesh();
		if (GameScene.isPlaying)
		{
			lODTerrain.mesh.vertices = terrainData.vertices;
		}
		else
		{
			if (LODTerrain.shareVertices == null)
			{
				LODTerrain.BuildShareVertices();
			}
			if (terrainData.vertices != null)
			{
				lODTerrain.vertices = terrainData.vertices;
			}
			else
			{
				lODTerrain.vertices = LODTerrain.shareVertices;
			}
			lODTerrain.normals = terrainData.normals;
			lODTerrain.tangents = terrainData.tangents;
			lODTerrain.mesh.vertices = lODTerrain.vertices;
			lODTerrain.BuildUVs();
			lODTerrain.mesh.normals = lODTerrain.normals;
			lODTerrain.mesh.tangents = lODTerrain.tangents;
			lODTerrain.BuildTriangles();
		}
		gameObject.AddComponent<MeshFilter>().mesh = lODTerrain.mesh;
		lODTerrain.terrainRenderer = gameObject.AddComponent<MeshRenderer>();
		if (GameScene.isPlaying)
		{
			lODTerrain.terrainRenderer.enabled = false;
			lODTerrain.terrainRenderer.receiveShadows = true;
			lODTerrain.terrainRenderer.castShadows = false;
		}
		return lODTerrain;
	}

	public void Destroy()
	{
		this.terrainData.Release();
		this.terrainData = null;
		this.uvs = null;
		this.vertices = null;
		this.normals = null;
		this.triangles = null;
		this.tangents = null;
		DelegateProxy.GameDestory(this.matrial);
		this.matrial = null;
		DelegateProxy.GameDestory(this.mesh);
		this.mesh = null;
	}

	public void CopyTo(global::TerrainData target)
	{
		float num = 0.53125f;
		for (int i = 0; i < 32; i++)
		{
			for (int j = 0; j < 32; j++)
			{
				int num2 = (int)((float)i * num);
				int num3 = (int)((float)j * num);
				this.terrainData.heightmap[j, i] = this.vertices[num3 * 17 + num2].y;
			}
		}
		int xBase = (int)base.transform.position.x - 16 + this.terrainConfig.sceneWidth / 2;
		int yBase = (int)base.transform.position.z - 16 + this.terrainConfig.sceneHeight / 2;
		target.SetHeights(xBase, yBase, this.terrainData.heightmap);
	}

	public void BuildGeometry()
	{
		if (this.mesh == null)
		{
			this.mesh = new Mesh();
		}
		int segmentsW = this.terrainData.segmentsW;
		int segmentsH = this.terrainData.segmentsH;
		int num = segmentsW + 1;
		int num2 = (segmentsH + 1) * num;
		this.vertices = new Vector3[num2];
		num2 = 0;
		for (int i = 0; i <= segmentsH; i++)
		{
			for (int j = 0; j <= segmentsW; j++)
			{
				float num3 = ((float)j / (float)segmentsW - 0.5f) * this.terrainData.size.x;
				float num4 = ((float)i / (float)segmentsH - 0.5f) * this.terrainData.size.z;
				float height = GameScene.mainScene.terrainData.GetHeight(Mathf.FloorToInt(num3 + base.transform.position.x + 240f), Mathf.FloorToInt(num4 + base.transform.position.z + 240f));
				this.vertices[num2] = new Vector3(num3, height, num4);
				num2++;
			}
		}
		this.mesh.vertices = this.vertices;
		if (Application.isEditor && base.gameObject != null)
		{
			MeshCollider component = base.gameObject.GetComponent<MeshCollider>();
			if (component != null)
			{
				DelegateProxy.DestroyObjectImmediate(component);
				base.gameObject.AddComponent<MeshCollider>();
			}
		}
	}

	private static void BuildShareVertices()
	{
		int num = 16;
		int num2 = 16;
		int num3 = (num2 + 1) * (num + 1);
		LODTerrain.shareVertices = new Vector3[num3];
		num3 = 0;
		for (int i = 0; i <= num2; i++)
		{
			for (int j = 0; j <= num; j++)
			{
				float x = ((float)j / (float)num - 0.5f) * 32f;
				float z = ((float)i / (float)num2 - 0.5f) * 32f;
				float defaultTerrainHeight = GameScene.mainScene.terrainConfig.defaultTerrainHeight;
				LODTerrain.shareVertices[num3] = new Vector3(x, defaultTerrainHeight, z);
				num3++;
			}
		}
	}

	public void BuildTriangles()
	{
		if (LODTerrain.shareTriangles == null)
		{
			LODTerrain.BuildShareTriangles();
		}
		this.triangles = LODTerrain.shareTriangles;
		this.mesh.triangles = this.triangles;
	}

	private static void BuildShareTriangles()
	{
		int num = 16;
		int num2 = 16;
		int num3 = 0;
		int num4 = num + 1;
		int[] array = new int[num2 * num * 6];
		for (int i = 0; i <= num2; i++)
		{
			for (int j = 0; j <= num; j++)
			{
				if (j != num && i != num2)
				{
					int num5 = j + i * num4;
					array[num3++] = num5;
					array[num3++] = num5 + num4;
					array[num3++] = num5 + num4 + 1;
					array[num3++] = num5;
					array[num3++] = num5 + num4 + 1;
					array[num3++] = num5 + 1;
				}
			}
		}
		LODTerrain.shareTriangles = array;
	}

	public void BuildUVs()
	{
		if (LODTerrain.shareUVS == null)
		{
			LODTerrain.BuildShareUVS();
		}
		this.mesh.uv = LODTerrain.shareUVS;
		this.uvs = LODTerrain.shareUVS;
	}

	private static void BuildShareUVS()
	{
		int num = 16;
		int num2 = 16;
		int num3 = (num + 1) * (num2 + 1);
		Vector2[] array = new Vector2[num3];
		num3 = 0;
		for (int i = 0; i <= num; i++)
		{
			for (int j = 0; j <= num2; j++)
			{
				array[num3++] = new Vector2((float)j / (float)num2, 1f - (float)i / (float)num);
			}
		}
		LODTerrain.shareUVS = array;
	}

	public void BuildMaterial(WaterData waterData = null)
	{
		if (LightmapSettings.lightmaps.Length > 0 || GameScene.isPlaying)
		{
			if (!this.terrainConfig.enablePointLight)
			{
				this.matrial = new Material(this.terrainMobileShader);
			}
			else
			{
				this.matrial = new Material(this.terrainMobileWithPointLightShader);
			}
		}
		else
		{
			this.matrial = new Material(this.realLightShader);
		}
		Texture2D texture;
		if (GameScene.isPlaying)
		{
			texture = AssetLibrary.Load(this.splatsMapPath, AssetType.Texture2D, LoadType.Type_Auto).texture2D;
		}
		else
		{
			texture = this.terrainData.splatsmapTex;
		}
		this.matrial.SetTexture("_Control", texture);
		for (int i = 0; i < this.terrainData.spaltsmapLayers; i++)
		{
			Splat splat = this.terrainData.splats[i];
			if (splat != null)
			{
				this.matrial.SetTexture("_Splat" + i, splat.texture);
				this.matrial.SetTextureScale("_Splat" + i, new Vector2(splat.tilingOffset.x, splat.tilingOffset.y));
				this.matrial.SetTextureOffset("_Splat" + i, Vector2.zero);
			}
			else
			{
				splat = this.terrainData.splats[0];
				this.matrial.SetTexture("_Splat" + i, this.terrainConfig.baseSplat.texture);
				this.matrial.SetTextureScale("_Splat" + i, new Vector2(splat.tilingOffset.x, splat.tilingOffset.y));
				this.matrial.SetTextureOffset("_Splat" + i, Vector2.zero);
			}
		}
		base.renderer.material = this.matrial;
	}

	public void WithoutShadow()
	{
		this.terrainRenderer.receiveShadows = false;
		if (this.bakeMat != null)
		{
			this.bakeMat.shader = this.bakeNoShadowShader;
		}
	}

	public void ReceiveShadow()
	{
		this.terrainRenderer.receiveShadows = true;
		if (this.bakeMat != null)
		{
			this.bakeMat.shader = this.bakeShader;
		}
	}

	public void Bake()
	{
		this.terrainMapIndex = Terrainmapping.Bake(this, 1024);
		if (this.terrainMapIndex > -1)
		{
			this.bakeMat = new Material(this.bakeShader);
			this.bakeMat.SetTexture("_MainTex", Terrainmapping.maps[this.terrainMapIndex]);
			base.renderer.material = this.bakeMat;
		}
	}

	public void CancelBake()
	{
		if (this.terrainMapIndex >= 0)
		{
			Terrainmapping.Cancel(this.terrainMapIndex);
			base.renderer.material = this.matrial;
			this.terrainMapIndex = -1;
		}
	}

	public void BuildNormals()
	{
		this.UpdateFaceNormals();
		this.UpdateVertexNormals(-1f);
		this.mesh.normals = this.normals;
	}

	public void BuildNormals(Vector3 center, float range)
	{
		this.nCenter = center;
		this.UpdateFaceNormals();
		this.UpdateVertexNormals(range);
		this.mesh.normals = this.normals;
	}

	public void BuildTangents()
	{
		this.UpdateFaceTangents();
		this.UpdateVertexTangents();
		this.mesh.tangents = this.tangents;
	}

	public static void ClearSideNormals()
	{
		LODTerrain.sideNormals.Clear();
	}

	public bool HasSideNormal(float x, float z)
	{
		string key = x + "_" + z;
		return LODTerrain.sideNormals.ContainsKey(key);
	}

	public void AddSideNormal(float x, float z, Vector3 normal)
	{
		string key = x + "_" + z;
		if (LODTerrain.sideNormals.ContainsKey(key))
		{
			return;
		}
		LODTerrain.sideNormals[key] = normal;
	}

	public Vector3 GetSideNormal(float x, float z)
	{
		string key = x + "_" + z;
		if (LODTerrain.sideNormals.ContainsKey(key))
		{
			return LODTerrain.sideNormals[key];
		}
		return Vector3.zero;
	}

	private void UpdateFaceNormals()
	{
		int i = 0;
		int num = 0;
		int num2 = 0;
		int num3 = this.triangles.Length;
		this._faceNormals = new float[num3];
		this._faceWeights = new float[num3 / 3];
		while (i < num3)
		{
			int num4 = this.triangles[i++];
			float x = this.vertices[num4].x;
			float y = this.vertices[num4].y;
			float z = this.vertices[num4].z;
			num4 = this.triangles[i++];
			float x2 = this.vertices[num4].x;
			float y2 = this.vertices[num4].y;
			float z2 = this.vertices[num4].z;
			num4 = this.triangles[i++];
			float x3 = this.vertices[num4].x;
			float y3 = this.vertices[num4].y;
			float z3 = this.vertices[num4].z;
			float num5 = x3 - x;
			float num6 = y3 - y;
			float num7 = z3 - z;
			float num8 = x2 - x;
			float num9 = y2 - y;
			float num10 = z2 - z;
			float num11 = num7 * num9 - num6 * num10;
			float num12 = num5 * num10 - num7 * num8;
			float num13 = num6 * num8 - num5 * num9;
			float num14 = Mathf.Sqrt(num11 * num11 + num12 * num12 + num13 * num13);
			float num15 = num14 * 10000f;
			if (num15 < 1f)
			{
				num15 = 1f;
			}
			this._faceWeights[num2++] = num15;
			num14 = 1f / num14;
			this._faceNormals[num++] = num11 * num14;
			this._faceNormals[num++] = num12 * num14;
			this._faceNormals[num++] = num13 * num14;
		}
	}

	private bool InRange(int ind, float range)
	{
		Vector3 v = this.vertices[ind];
		v = base.transform.localToWorldMatrix.MultiplyPoint3x4(v);
		if (range > 0f)
		{
			float num = v.x - this.nCenter.x;
			float num2 = v.z - this.nCenter.z;
			float num3 = Mathf.Sqrt(num * num + num2 * num2);
			if (num3 > range)
			{
				return false;
			}
		}
		return true;
	}

	protected void UpdateVertexNormals(float range = -1f)
	{
		int i = 0;
		int num = 0;
		int num2 = 1;
		int num3 = 2;
		int num4 = this.vertices.Length;
		if (this.normals == null)
		{
			this.normals = new Vector3[num4];
			while (i < num4)
			{
				this.normals[i] = new Vector3(0f, 0f, 0f);
				i++;
			}
		}
		int j = 0;
		int num5 = 0;
		int num6 = this.triangles.Length;
		while (j < num6)
		{
			float num7 = this._faceWeights[num5++];
			int num8 = this.triangles[j++];
			if (this.InRange(num8, range))
			{
				Vector3[] expr_B0_cp_0 = this.normals;
				int expr_B0_cp_1 = num8;
				expr_B0_cp_0[expr_B0_cp_1].x = expr_B0_cp_0[expr_B0_cp_1].x + this._faceNormals[num] * num7;
				Vector3[] expr_D4_cp_0 = this.normals;
				int expr_D4_cp_1 = num8;
				expr_D4_cp_0[expr_D4_cp_1].y = expr_D4_cp_0[expr_D4_cp_1].y + this._faceNormals[num2] * num7;
				Vector3[] expr_F8_cp_0 = this.normals;
				int expr_F8_cp_1 = num8;
				expr_F8_cp_0[expr_F8_cp_1].z = expr_F8_cp_0[expr_F8_cp_1].z + this._faceNormals[num3] * num7;
			}
			num8 = this.triangles[j++];
			if (this.InRange(num8, range))
			{
				Vector3[] expr_13A_cp_0 = this.normals;
				int expr_13A_cp_1 = num8;
				expr_13A_cp_0[expr_13A_cp_1].x = expr_13A_cp_0[expr_13A_cp_1].x + this._faceNormals[num] * num7;
				Vector3[] expr_15E_cp_0 = this.normals;
				int expr_15E_cp_1 = num8;
				expr_15E_cp_0[expr_15E_cp_1].y = expr_15E_cp_0[expr_15E_cp_1].y + this._faceNormals[num2] * num7;
				Vector3[] expr_182_cp_0 = this.normals;
				int expr_182_cp_1 = num8;
				expr_182_cp_0[expr_182_cp_1].z = expr_182_cp_0[expr_182_cp_1].z + this._faceNormals[num3] * num7;
			}
			num8 = this.triangles[j++];
			if (this.InRange(num8, range))
			{
				Vector3[] expr_1C4_cp_0 = this.normals;
				int expr_1C4_cp_1 = num8;
				expr_1C4_cp_0[expr_1C4_cp_1].x = expr_1C4_cp_0[expr_1C4_cp_1].x + this._faceNormals[num] * num7;
				Vector3[] expr_1E8_cp_0 = this.normals;
				int expr_1E8_cp_1 = num8;
				expr_1E8_cp_0[expr_1E8_cp_1].y = expr_1E8_cp_0[expr_1E8_cp_1].y + this._faceNormals[num2] * num7;
				Vector3[] expr_20C_cp_0 = this.normals;
				int expr_20C_cp_1 = num8;
				expr_20C_cp_0[expr_20C_cp_1].z = expr_20C_cp_0[expr_20C_cp_1].z + this._faceNormals[num3] * num7;
			}
			num += 3;
			num2 += 3;
			num3 += 3;
		}
		for (i = 0; i < num4; i++)
		{
			Vector3 v = this.vertices[i];
			if (Mathf.Abs(v.x) > 15f || Mathf.Abs(v.z) > 15f)
			{
				v = base.transform.localToWorldMatrix.MultiplyPoint3x4(v);
				if (range > 0f)
				{
					float num9 = v.x - this.nCenter.x;
					float num10 = v.z - this.nCenter.z;
					float num11 = Mathf.Sqrt(num9 * num9 + num10 * num10);
					if (num11 > range)
					{
						i++;
						continue;
					}
				}
				if (!this.HasSideNormal(v.x, v.z))
				{
					float x = this.normals[i].x;
					float y = this.normals[i].y;
					float z = this.normals[i].z;
					float num12 = 1f / Mathf.Sqrt(x * x + y * y + z * z);
					this.normals[i].x = x * num12;
					this.normals[i].y = y * num12;
					this.normals[i].z = z * num12;
					this.AddSideNormal(v.x, v.z, this.normals[i]);
				}
				else
				{
					this.normals[i].x = this.GetSideNormal(v.x, v.z).x;
					this.normals[i].y = this.GetSideNormal(v.x, v.z).y;
					this.normals[i].z = this.GetSideNormal(v.x, v.z).z;
				}
			}
			else
			{
				v = base.transform.localToWorldMatrix.MultiplyPoint3x4(v);
				if (range > 0f)
				{
					float num13 = v.x - this.nCenter.x;
					float num14 = v.z - this.nCenter.z;
					float num15 = Mathf.Sqrt(num13 * num13 + num14 * num14);
					if (num15 > range)
					{
						i++;
						continue;
					}
				}
				float x2 = this.normals[i].x;
				float y2 = this.normals[i].y;
				float z2 = this.normals[i].z;
				float num16 = 1f / Mathf.Sqrt(x2 * x2 + y2 * y2 + z2 * z2);
				this.normals[i].x = x2 * num16;
				this.normals[i].y = y2 * num16;
				this.normals[i].z = z2 * num16;
			}
		}
	}

	protected void UpdateFaceTangents()
	{
		int i = 0;
		int num = this.triangles.Length;
		this._faceTangents = new float[num];
		while (i < num)
		{
			int num2 = this.triangles[i];
			int num3 = this.triangles[i + 1];
			int num4 = this.triangles[i + 2];
			float y = this.uvs[num2].y;
			float num5 = this.uvs[num3].y - y;
			float num6 = this.uvs[num4].y - y;
			float x = this.vertices[num2].x;
			float y2 = this.vertices[num2].y;
			float z = this.vertices[num2].z;
			float num7 = this.vertices[num3].x - x;
			float num8 = this.vertices[num3].y - y2;
			float num9 = this.vertices[num3].z - z;
			float num10 = this.vertices[num4].x - x;
			float num11 = this.vertices[num4].y - y2;
			float num12 = this.vertices[num4].z - z;
			float num13 = num6 * num7 - num5 * num10;
			float num14 = num6 * num8 - num5 * num11;
			float num15 = num6 * num9 - num5 * num12;
			float num16 = 1f / Mathf.Sqrt(num13 * num13 + num14 * num14 + num15 * num15);
			this._faceTangents[i++] = num16 * num13;
			this._faceTangents[i++] = num16 * num14;
			this._faceTangents[i++] = num16 * num15;
		}
	}

	protected void UpdateVertexTangents()
	{
		int i = 0;
		int num = 0;
		int num2 = this.vertices.Length;
		this.tangents = new Vector4[num2];
		while (i < num2)
		{
			this.tangents[i] = new Vector4(0f, 0f, 0f, -1f);
			i++;
		}
		int num3 = this.triangles.Length;
		int num4 = 0;
		int num5 = 1;
		int num6 = 2;
		i = 0;
		while (i < num3)
		{
			float num7 = this._faceWeights[num++];
			int num8 = this.triangles[i++];
			Vector4[] expr_95_cp_0 = this.tangents;
			int expr_95_cp_1 = num8;
			expr_95_cp_0[expr_95_cp_1].x = expr_95_cp_0[expr_95_cp_1].x + this._faceTangents[num4] * num7;
			Vector4[] expr_BA_cp_0 = this.tangents;
			int expr_BA_cp_1 = num8;
			expr_BA_cp_0[expr_BA_cp_1].y = expr_BA_cp_0[expr_BA_cp_1].y + this._faceTangents[num5] * num7;
			Vector4[] expr_DF_cp_0 = this.tangents;
			int expr_DF_cp_1 = num8;
			expr_DF_cp_0[expr_DF_cp_1].z = expr_DF_cp_0[expr_DF_cp_1].z + this._faceTangents[num6] * num7;
			num8 = this.triangles[i++];
			Vector4[] expr_112_cp_0 = this.tangents;
			int expr_112_cp_1 = num8;
			expr_112_cp_0[expr_112_cp_1].x = expr_112_cp_0[expr_112_cp_1].x + this._faceTangents[num4] * num7;
			Vector4[] expr_137_cp_0 = this.tangents;
			int expr_137_cp_1 = num8;
			expr_137_cp_0[expr_137_cp_1].y = expr_137_cp_0[expr_137_cp_1].y + this._faceTangents[num5] * num7;
			Vector4[] expr_15C_cp_0 = this.tangents;
			int expr_15C_cp_1 = num8;
			expr_15C_cp_0[expr_15C_cp_1].z = expr_15C_cp_0[expr_15C_cp_1].z + this._faceTangents[num6] * num7;
			num8 = this.triangles[i++];
			Vector4[] expr_18F_cp_0 = this.tangents;
			int expr_18F_cp_1 = num8;
			expr_18F_cp_0[expr_18F_cp_1].x = expr_18F_cp_0[expr_18F_cp_1].x + this._faceTangents[num4] * num7;
			Vector4[] expr_1B4_cp_0 = this.tangents;
			int expr_1B4_cp_1 = num8;
			expr_1B4_cp_0[expr_1B4_cp_1].y = expr_1B4_cp_0[expr_1B4_cp_1].y + this._faceTangents[num5] * num7;
			Vector4[] expr_1D9_cp_0 = this.tangents;
			int expr_1D9_cp_1 = num8;
			expr_1D9_cp_0[expr_1D9_cp_1].z = expr_1D9_cp_0[expr_1D9_cp_1].z + this._faceTangents[num6] * num7;
			num4 += 3;
			num5 += 3;
			num6 += 3;
		}
		for (i = 0; i < num2; i++)
		{
			float x = this.tangents[i].x;
			float y = this.tangents[i].y;
			float z = this.tangents[i].z;
			float num9 = 1f / Mathf.Sqrt(x * x + y * y + z * z);
			this.tangents[i].x = x * num9;
			this.tangents[i].y = y * num9;
			this.tangents[i].z = z * num9;
		}
	}
}
