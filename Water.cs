using System;
using UnityEngine;

public class Water : MonoBehaviour
{
    /// <summary>
    /// 深度贴图纹理映射
    /// </summary>
	public float[,] depthMap;

	public static Mesh mesh;
    /// <summary>
    /// 水体网格顶点数组
    /// </summary>
	public Vector3[] vertices;
    /// <summary>
    /// 水体网格法线数组
    /// </summary>
	public Vector3[] normals;
    /// <summary>
    /// 水体玩个uv数组
    /// </summary>
	public Vector2[] uvs;
    /// <summary>
    /// 水体网格数组
    /// </summary>
	public int[] triangles;
    /// <summary>
    /// 水体网格宽度
    /// </summary>
	public int segmentsW = 16;
    /// <summary>
    /// 水体网格高度
    /// </summary>
	public int segmentsH = 16;
    /// <summary>
    /// 水体材质
    /// </summary>
	private Material matrial;
    /// <summary>
    /// 水体网格渲染器
    /// </summary>
	public MeshRenderer waterRenderer;
    /// <summary>
    /// 水体采样器，估计编辑器环境使用吧
    /// </summary>
	public AmbienceSampler ambienceSampler;
    /// <summary>
    /// 水体数据对象
    /// </summary>
	public WaterData waterData;
    /// <summary>
    /// 水体shader
    /// </summary>
	private static Shader waterShader = Shader.Find("Snail/Water");

	public void Destroy()
	{
		this.waterData.Release();
		this.waterData = null;
		this.uvs = null;
		this.vertices = null;
		this.normals = null;
		this.triangles = null;
		this.waterRenderer.material = null;
		this.waterRenderer = null;
		DelegateProxy.GameDestory(this.matrial);
		this.matrial = null;
		DelegateProxy.GameDestory(Water.mesh);
		Water.mesh = null;
	}
    /// <summary>
    /// 创建水体游戏对象
    /// </summary>
    /// <returns></returns>
	public static Water CreateWaterGameObject()
	{
		GameObject gameObject = new GameObject();
		Water water = gameObject.AddComponent<Water>();
		gameObject.isStatic = false;
		if (Water.mesh == null)
		{
			water.BuildShareWaterMesh();
		}
		gameObject.AddComponent<MeshFilter>().sharedMesh = Water.mesh;
		water.waterRenderer = gameObject.AddComponent<MeshRenderer>();
		water.waterRenderer.castShadows = false;
		water.waterRenderer.receiveShadows = false;
		water.gameObject.layer = GameLayer.Layer_Water;
		water.waterData = new WaterData();
		return water;
	}
    /// <summary>
    /// 通过水体数据对象创建水体游戏对象
    /// </summary>
    /// <param name="waterData"></param>
    /// <returns></returns>
	public static Water CreateWaterGameObject(WaterData waterData)
	{
		Water water = Water.CreateWaterGameObject();
		water.waterData = waterData;
		water.BuildMaterial();
		return water;
	}
    /// <summary>
    /// 构建水体网格
    /// </summary>
	public void BuildShareWaterMesh()
	{
		this.BuildGeometry();
		this.BuildUVs();
	}
    /// <summary>
    /// 构建水体几何体信息
    /// </summary>
	public void BuildGeometry()
	{
		TerrainConfig terrainConfig = GameScene.mainScene.terrainConfig;
		if (Water.mesh == null)
		{
			Water.mesh = new Mesh();
		}
		int num = this.segmentsW;
		int num2 = this.segmentsH;
		int num3 = 0;
		int num4 = num + 1;
		int num5 = (num2 + 1) * num4;
		this.vertices = new Vector3[num5];
		this.triangles = new int[num2 * num * 6];
		this.normals = new Vector3[num5];
		num5 = 0;
		for (int i = 0; i <= num2; i++)
		{
			for (int j = 0; j <= num; j++)
			{
				float x = ((float)j / (float)num - 0.5f) * (float)terrainConfig.tileSize;
				float z = ((float)i / (float)num2 - 0.5f) * (float)terrainConfig.tileSize;
				float y = 0f;
				this.vertices[num5] = new Vector3(x, y, z);
				num5++;
				if (j != num && i != num2)
				{
					int num6 = j + i * num4;
					this.triangles[num3++] = num6;
					this.triangles[num3++] = num6 + num4;
					this.triangles[num3++] = num6 + num4 + 1;
					this.triangles[num3++] = num6;
					this.triangles[num3++] = num6 + num4 + 1;
					this.triangles[num3++] = num6 + 1;
				}
			}
		}
		Water.mesh.vertices = this.vertices;
		Water.mesh.triangles = this.triangles;
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
    /// <summary>
    /// 构建水体UV数组
    /// </summary>
	public void BuildUVs()
	{
		int num = (this.segmentsH + 1) * (this.segmentsW + 1);
		this.uvs = new Vector2[num];
		num = 0;
		for (int i = 0; i <= this.segmentsH; i++)
		{
			for (int j = 0; j <= this.segmentsW; j++)
			{
				this.uvs[num++] = new Vector2((float)j / (float)this.segmentsW, 1f - (float)i / (float)this.segmentsH);
			}
		}
		Water.mesh.uv = this.uvs;
	}
    /// <summary>
    /// 构建水体材质
    /// </summary>
	public void BuildMaterial()
	{
		TerrainConfig terrainConfig = GameScene.mainScene.terrainConfig;
		if (this.matrial == null)
		{
			this.matrial = new Material(Water.waterShader);
			base.renderer.material = this.matrial;
		}
		this.matrial.SetTexture("_DeepTex", this.waterData.depthMap);
		this.matrial.SetTexture("_BumpMap", this.waterData.bumpMap);
		this.matrial.SetFloat("_WaveScale", this.waterData.waveScale);
		this.matrial.SetTexture("_BumpMap", this.waterData.bumpMap);
		this.matrial.SetTexture("_ColorControl", this.waterData.colorControlMap);
		this.matrial.SetColor("_horizonColor", this.waterData.horizonColor);
		this.matrial.SetFloat("_RefrDistort", this.waterData.refrDistort);
		this.matrial.SetFloat("_Alpha", this.waterData.alpha);
		this.matrial.SetVector("_SunLightDir", terrainConfig.sunLightDir);
		this.matrial.SetFloat("_WaterSpecStrength", terrainConfig.waterSpecStrength);
		this.matrial.SetFloat("_WaterSpecRange", terrainConfig.waterSpecRange);
	}
    /// <summary>
    /// 更新深度映射
    /// </summary>
	public void UpdateDepthMap()
	{
		TerrainConfig terrainConfig = GameScene.mainScene.terrainConfig;
		int num = 64;
		float num2 = 0f;
		this.depthMap = new float[num, num];
		Vector3 origin = new Vector3(0f, 500f, 0f);
		Ray ray = default(Ray);
		ray.direction = Vector3.down;
		float num3 = (float)terrainConfig.tileSize / (float)num;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				origin.x = (float)i * num3 + base.transform.position.x - (float)terrainConfig.tileSize * 0.5f;
				origin.z = (float)j * num3 + base.transform.position.z - (float)terrainConfig.tileSize * 0.5f;
				ray.origin = origin;
				RaycastHit raycastHit;
				Physics.Raycast(ray, out raycastHit, 2000f, GameLayer.Mask_Ground);
				if (raycastHit.transform != null)
				{
					num2 = raycastHit.point.y;
				}
				float num4 = this.waterData.height - num2; //水底深度
				this.depthMap[i, j] = num4 / this.waterData.waterVisibleDepth;
			}
		}
		this.waterData.depthMap = new Texture2D(num, num, TextureFormat.ARGB32, false);
		this.waterData.depthMap.wrapMode = TextureWrapMode.Clamp;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				float num5 = this.depthMap[i, j];
				this.waterData.depthMap.SetPixel(i, num - j - 1, new Color(num5, num5, num5, num5));
			}
		}
		this.waterData.depthMap.Apply();
	}
    /// <summary>
    /// 判断高度是否在水下
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
	public bool Underwater(float height)
	{
		return height < this.waterData.height - this.waterData.waterDiffValue;
	}
    /// <summary>
    /// 强制更新
    /// </summary>
	public void ForcedUpdate()
	{
		this.Update();
	}

	private void Update()
	{
		if (GameScene.mainScene == null)
		{
			return;
		}
		if (!base.renderer)
		{
			return;
		}
		Material sharedMaterial = base.renderer.sharedMaterial;
		if (!sharedMaterial)
		{
			return;
		}
		float @float = sharedMaterial.GetFloat("_WaveScale");
		float time = GameScene.mainScene.time;
		Vector4 vector = this.waterData.waveSpeed * (time * @float);
		Vector4 vector2 = new Vector4(Mathf.Repeat(vector.x, 1f), Mathf.Repeat(vector.y, 1f), Mathf.Repeat(vector.z, 1f), Mathf.Repeat(vector.w, 1f));
        //计算设置水波偏移
		sharedMaterial.SetVector("_WaveOffset", vector2);
		if (!GameScene.isPlaying)
		{
			sharedMaterial.SetVector("_SunLightDir", GameScene.mainScene.terrainConfig.sunLightDir);
			sharedMaterial.SetFloat("_WaterSpecStrength", GameScene.mainScene.terrainConfig.waterSpecStrength);
			sharedMaterial.SetFloat("_WaterSpecRange", GameScene.mainScene.terrainConfig.waterSpecRange);
		}
	}
}
