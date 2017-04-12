using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Tile
{
	public string key = string.Empty;

	public int tileX;

	public int tileY;

	public float viewDistance;

	public Vector3 position = Vector3.zero;
    /// <summary>
    /// tile剔除距离
    /// </summary>
	public float far;

	private Dictionary<int, GameObjectUnit> unitsMap = new Dictionary<int, GameObjectUnit>();

	public List<GameObjectUnit> units = new List<GameObjectUnit>();

	public int unitCount;

	public Region region;

	public string path = string.Empty;

	public bool visible;
    /// <summary>
    /// tile包围盒
    /// </summary>
	public Bounds bounds;

	public float[,] heights;

	public int[,] grids;
    /// <summary>
    /// 左边的tile
    /// </summary>
	public Tile left;
    /// <summary>
    /// 右边的tile
    /// </summary>
	public Tile right;
    /// <summary>
    /// 上方的tile
    /// </summary>
	public Tile top;
    /// <summary>
    /// 下方的tile
    /// </summary>
	public Tile bot;
    /// <summary>
    /// 左上角的tile
    /// </summary>
	public Tile top_left;
    /// <summary>
    /// 右上角的tile
    /// </summary>
	public Tile top_right;
    /// <summary>
    /// 左下角的tile
    /// </summary>
	public Tile bot_left;
    /// <summary>
    /// 右下角的tile
    /// </summary>
	public Tile bot_right;
    /// <summary>
    /// 当前tile的地形对象
    /// </summary>
	public LODTerrain terrain;

	public LightmapPrototype _lightmapPrototype = new LightmapPrototype();
    /// <summary>
    /// tile的水体对象
    /// </summary>
	public Water water;
    /// <summary>
    /// 水体数据
    /// </summary>
	private WaterData waterData;

	private int tick;

	public bool preload;

	public GameScene scene;

	private float dx;

	private float dz;

	private int unitInd;
    /// <summary>
    /// 检查运算的临时计数
    /// </summary>
	private int computeCount;
    /// <summary>
    /// 一次检查运算的最大个数
    /// </summary>
	private int maxComputeCount = 5;
    /// <summary>
    /// 上次检查运算的最后索引记录
    /// </summary>
	private int lastInd;

	private GameObject gridDataGO;

	private Mesh gridDataMesh = new Mesh();

	private Shader gridDataShader;

	private Material gridDataMat;

	public LightmapPrototype lightmapPrototype
	{
		get
		{
			GameObject gameObject = GameObject.Find(this.key);
			if (gameObject != null)
			{
				this.terrain = gameObject.GetComponent<LODTerrain>();
			}
			if (this.terrain != null)
			{
				this._lightmapPrototype.lightmapIndex = this.terrain.terrainRenderer.lightmapIndex;
				this._lightmapPrototype.lightmapTilingOffset = this.terrain.terrainRenderer.lightmapTilingOffset;
			}
			return this._lightmapPrototype;
		}
	}

	public Tile(Region region)
	{
		this.region = region;
		this.scene = region.scene;
	}

	public void AddUnit(GameObjectUnit unit)
	{
		if (this.unitsMap.ContainsKey(unit.createID))
		{
			return;
		}
		this.unitsMap.Add(unit.createID, unit);
		this.units.Add(unit);
		this.unitCount++;
	}

	public void RemoveUnit(GameObjectUnit unit)
	{
		if (this.unitsMap.ContainsKey(unit.createID))
		{
			this.unitsMap.Remove(unit.createID);
			this.units.Remove(unit);
			this.unitCount--;
		}
	}

	public GameObjectUnit FindUnit(int createID)
	{
		if (this.unitsMap.ContainsKey(createID))
		{
			return this.unitsMap[createID];
		}
		return null;
	}

	public bool ContainUnit(GameObjectUnit unit)
	{
		return this.unitsMap.ContainsKey(unit.createID);
	}

	public static Tile Create(Region region, int tileX, int tileY)
	{
		TerrainConfig terrainConfig = region.scene.terrainConfig;
		Tile tile = new Tile(region);
		tile.tileX = tileX;
		tile.tileY = tileY;
		tile.key = string.Concat(new object[]
		{
			region.regionX,
			"_",
			region.regionY,
			"_",
			tileX,
			"_",
			tileY
		});
		tile.unitCount = 0;
		tile.position.x = (float)(tileX * terrainConfig.tileSize) + region.actualX;
		tile.position.y = terrainConfig.defaultTerrainHeight;
		tile.position.z = (float)(tileY * terrainConfig.tileSize) + region.actualY;
		tile.far = terrainConfig.tileCullingDistance;
		tile.bounds = default(Bounds);
		tile.bounds.min = new Vector3(tile.position.x - (float)terrainConfig.tileSize * 0.5f, 0f, tile.position.z - (float)terrainConfig.tileSize * 0.5f);
		tile.bounds.max = new Vector3(tile.position.z + (float)terrainConfig.tileSize * 0.5f, 300f, tile.position.z + (float)terrainConfig.tileSize * 0.5f);
		tile.path = string.Concat(new object[]
		{
			"Scenes/",
			region.scene.sceneID,
			"/",
			region.regionX,
			"_",
			region.regionY,
			"/",
			tileX,
			"_",
			tileY
		});
		return tile;
	}

	public void UpdateViewRange()
	{
		this.far = this.region.scene.terrainConfig.tileCullingDistance;
		for (int i = 0; i < this.units.Count; i++)
		{
			this.units[i].UpdateViewRange();
		}
	}
    /// <summary>
    /// 读取瓦片数据信息
    /// </summary>
    /// <param name="br"></param>
	public void Read(BinaryReader br)
	{
		TerrainConfig terrainConfig = this.region.scene.terrainConfig;
		this.tileX = br.ReadInt32();
		this.tileY = br.ReadInt32();
		this.key = string.Concat(new object[]
		{
			this.region.regionX,
			"_",
			this.region.regionY,
			"_",
			this.tileX,
			"_",
			this.tileY
		});
		this.far = terrainConfig.tileCullingDistance;
		br.ReadSingle();
		br.ReadSingle();
		br.ReadSingle();
		bool flag = br.ReadBoolean();
		long num = br.BaseStream.Position;
		int num2 = terrainConfig.heightmapResolution;
		if (br.ReadInt32() == 10001)
		{
			num2 = terrainConfig.heightmapResolution;
		}
		else
		{
			num2 = terrainConfig.heightmapResolution + 1;
			br.BaseStream.Position = num;
		}
		if (GameScene.isPlaying)
		{
			br.BaseStream.Position += 8196L;
		}
		else
		{
			if (flag)
			{
				for (int i = 0; i < num2; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						this.heights = new float[num2, num2];
						this.heights[j, i] = br.ReadSingle();
					}
				}
			}
			num = br.BaseStream.Position;
			if (br.ReadInt32() == 10002)
			{
				for (int i = 0; i < num2; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						this.grids = new int[num2, num2];
						this.grids[j, i] = br.ReadInt32();
					}
				}
			}
			else
			{
				br.BaseStream.Position = num;
			}
		}
		this.position.x = (float)(this.tileX * terrainConfig.tileSize) + this.region.actualX;
		this.position.y = terrainConfig.defaultTerrainHeight;
		this.position.z = (float)(this.tileY * terrainConfig.tileSize) + this.region.actualY;
		int num3 = br.ReadInt32();
		int num4 = 0;
		if (num3 > 0)
		{
            //循环读取unit对象
			while (br.ReadString() == "GameObject")
			{
				int num5 = br.ReadInt32();
				GameObjectUnit gameObjectUnit = null;
				if (this.scene.curSceneUnitsMap.ContainsKey(num5))
				{
					gameObjectUnit = this.scene.curSceneUnitsMap[num5];
				}
				if (gameObjectUnit == null)
				{
					gameObjectUnit = this.scene.CreateEmptyUnit(num5);
				}
				gameObjectUnit.scene = this.region.scene;
				gameObjectUnit.Read(br, num5);
				if (GameScene.SampleMode)
				{
					this.AddUnit(gameObjectUnit);
					gameObjectUnit.tiles.Add(this);
					if (!this.scene.curSceneUnitsMap.ContainsKey(num5))
					{
						this.scene.curSceneUnitsMap.Add(num5, gameObjectUnit);
					}
				}
				else if ((gameObjectUnit.combineParentUnitID < 0 && gameObjectUnit.type != UnitType.UnitType_Light) || !GameScene.isPlaying)
				{
					this.AddUnit(gameObjectUnit);
					gameObjectUnit.tiles.Add(this);
					if (!this.scene.curSceneUnitsMap.ContainsKey(num5))
					{
						this.scene.curSceneUnitsMap.Add(num5, gameObjectUnit);
					}
				}
				else if (gameObjectUnit != null)
				{
					gameObjectUnit.Destroy();
					this.scene.RemoveEmptyUnit(gameObjectUnit);
				}
				num4++;
				if (num4 >= num3)
				{
					break;
				}
			}
		}
        //读取水体数据
		if (br.BaseStream.Position < br.BaseStream.Length)
		{
			num = br.BaseStream.Position;
			if (br.ReadInt32() == 10011)
			{
				if (br.ReadInt32() == 1)
				{
					this.waterData = new WaterData();
					this.waterData.Read(br);
				}
			}
			else
			{
				br.BaseStream.Position = num;
			}
		}
        //读取光照贴图原型数据属性
		bool flag2 = br.ReadBoolean();
		if (flag2)
		{
			this._lightmapPrototype.lightmapIndex = br.ReadInt32();
			num = br.BaseStream.Position;
			if (br.ReadInt32() == 10007)
			{
				this._lightmapPrototype.scale = br.ReadSingle();
			}
			else
			{
				br.BaseStream.Position = num;
			}
			this._lightmapPrototype.lightmapTilingOffset.x = br.ReadSingle();
			this._lightmapPrototype.lightmapTilingOffset.y = br.ReadSingle();
			this._lightmapPrototype.lightmapTilingOffset.z = br.ReadSingle();
			this._lightmapPrototype.lightmapTilingOffset.w = br.ReadSingle();
		}
        //瓦片包围盒
		this.bounds = default(Bounds);
		this.bounds.min = new Vector3(this.position.x - (float)terrainConfig.tileSize * 0.5f, 0f, this.position.z - (float)terrainConfig.tileSize * 0.5f);
		this.bounds.max = new Vector3(this.position.x + (float)terrainConfig.tileSize * 0.5f, 300f, this.position.z + (float)terrainConfig.tileSize * 0.5f);
        //tile资源路径
        this.path = string.Concat(new object[]
		{
			"Scenes/",
			this.region.scene.sceneID,
			"/",
			this.region.regionX,
			"_",
			this.region.regionY,
			"/",
			this.tileX,
			"_",
			this.tileY
		});
	}
    /// <summary>
    /// 根据视点更新瓦片状态
    /// </summary>
    /// <param name="eyePos"></param>
	public void Update(Vector3 eyePos)
	{
		if (this.units == null || this.units.Count < 1)   //瓦片上没有unit，不进行更新
		{
			return;
		}
		if (!GameScene.isPlaying && this.terrain != null)     //当场景没有开始，地形对象非空时 ,更新下周围9宫格tile状态
		{
			if (this.left == null)
			{
				this.left = this.scene.GetNeighborTile(this, -1, 0);
			}
			if (this.right == null)
			{
				this.right = this.scene.GetNeighborTile(this, 1, 0);
			}
			if (this.top == null)
			{
				this.top = this.scene.GetNeighborTile(this, 0, 1);
			}
			if (this.bot == null)
			{
				this.bot = this.scene.GetNeighborTile(this, 0, -1);
			}
			if (this.top_left == null)
			{
				this.top_left = this.scene.GetNeighborTile(this, -1, 1);
			}
			if (this.top_right == null)
			{
				this.top_right = this.scene.GetNeighborTile(this, 1, 1);
			}
			if (this.bot_left == null)
			{
				this.bot_left = this.scene.GetNeighborTile(this, -1, -1);
			}
			if (this.bot_right == null)
			{
				this.bot_right = this.scene.GetNeighborTile(this, 1, -1);
			}
			if (this.terrain.left == null && this.left != null)
			{
				this.terrain.left = this.left.terrain;
			}
			if (this.terrain.right == null && this.right != null)
			{
				this.terrain.right = this.right.terrain;
			}
			if (this.terrain.top == null && this.top != null)
			{
				this.terrain.top = this.top.terrain;
			}
			if (this.terrain.bot == null && this.bot != null)
			{
				this.terrain.bot = this.bot.terrain;
			}
			if (this.terrain.top_left == null && this.top_left != null)
			{
				this.terrain.top_left = this.top_left.terrain;
			}
			if (this.terrain.top_right == null && this.top_right != null)
			{
				this.terrain.top_right = this.top_right.terrain;
			}
			if (this.terrain.bot_left == null && this.bot_left != null)
			{
				this.terrain.bot_left = this.bot_left.terrain;
			}
			if (this.terrain.bot_right == null && this.bot_right != null)
			{
				this.terrain.bot_right = this.bot_right.terrain;
			}
		}
		if (!GameScene.isPlaying && this.water != null)   //场景没有运行，但是水体已创建，强制更新
		{
			this.water.ForcedUpdate();
		}
		if (this.tick % 2 == 0) //两次进行一次
		{
			this.computeCount = 0;
			int num = this.unitCount - 1;
			this.unitInd = this.lastInd;
			while (this.unitInd < this.unitCount)
			{
				if (this.computeCount > this.maxComputeCount)
				{
					this.lastInd = this.unitInd;
					return;
				}
				GameObjectUnit gameObjectUnit = this.units[this.unitInd];
				if (!gameObjectUnit.active && (gameObjectUnit.combineParentUnitID < 0 || GameScene.SampleMode))   //如果unit没有激活并且组合父unitid为空或者游戏场景为采用模式
				{
					this.dx = gameObjectUnit.center.x - eyePos.x;
					this.dz = gameObjectUnit.center.z - eyePos.z;
					gameObjectUnit.viewDistance = Mathf.Sqrt(this.dx * this.dx + this.dz * this.dz);
					if (gameObjectUnit.viewDistance < gameObjectUnit.near)     //小于近视距离，加入场景tile列表
					{
						this.scene.AddUnit(gameObjectUnit);
					}
				}
				if (this.unitInd >= num)
				{
					this.lastInd = 0;
				}
				this.computeCount++;
				this.unitInd++;
			}
		}
		this.tick++;
	}

	public void Visible()
	{
		if (LightmapSettings.lightmaps.Length > 0 && (this._lightmapPrototype.lightmapIndex == 255 || this._lightmapPrototype.lightmapIndex == -1))
		{
			this.visible = true;
			return;
		}
		if (!this.visible)
		{
			if (this.waterData != null && this.water == null)
			{
				this.water = Water.CreateWaterGameObject(this.waterData);
				this.water.gameObject.name = "water" + this.key;
				this.water.transform.position = new Vector3(this.position.x, this.water.waterData.height, this.position.z);
			}
			TerrainConfig terrainConfig = this.region.scene.terrainConfig;
			if (terrainConfig.enableTerrain)
			{
				if (this.terrain == null)
				{
					Asset asset;
					if (this.region.scene.loadFromAssetBund)
					{
						asset = AssetLibrary.Load(this.path + "_terrainData", AssetType.Terrain, LoadType.Type_AssetBundle);
					}
					else
					{
						asset = AssetLibrary.Load(this.path + "_terrainData", AssetType.Terrain, LoadType.Type_Resources);
					}
					if (asset != null)
					{
						if (asset.loaded)
						{
							this.OnTerrainLoadCompate(asset);
						}
						else
						{
							asset.loadedListener = new Asset.LoadedListener(this.OnTerrainLoadCompate);
						}
						if (!GameScene.isPlaying && !asset.loaded)
						{
							this.OnTerrainLoadCompate(asset);
						}
					}
				}
				else
				{
					this.terrain.gameObject.SetActive(true);
				}
				if (this.water != null)
				{
					this.water.gameObject.SetActive(true);
				}
			}
		}
		this.visible = true;
	}

	private void OnTerrainLoadCompate(Asset asset)
	{
		if (asset.loaded)
		{
			this.terrain = asset.terrain;
			this.terrain.name = this.key;
			this.terrain.transform.position = new Vector3(this.position.x, 0f, this.position.z);
			this.terrain.gameObject.layer = GameLayer.Layer_Ground;
			if (this._lightmapPrototype.lightmapIndex >= 0)
			{
				this.terrain.GetComponent<Renderer>().lightmapIndex = this._lightmapPrototype.lightmapIndex;
				this.terrain.GetComponent<Renderer>().lightmapTilingOffset = this._lightmapPrototype.lightmapTilingOffset;
			}
			this.terrain.splatsMapPath = string.Concat(new object[]
			{
				"Scenes/",
				this.region.scene.sceneID,
				"/",
				this.region.regionX,
				"_",
				this.region.regionY,
				"/",
				this.tileX,
				"_",
				this.tileY,
				"Splats"
			});
			if (!GameScene.isPlaying)
			{
				this.terrain.BuildMaterial(null);
			}
		}
		else
		{
			LODTerrainData terrainData = new LODTerrainData();
			this.terrain = LODTerrain.CreateTerrainGameObject(terrainData, false);
			this.terrain.name = this.key;
			this.terrain.transform.position = new Vector3(this.position.x, 0f, this.position.z);
			this.terrain.gameObject.layer = GameLayer.Layer_Ground;
			this.terrain.Init();
		}
		if (!GameScene.isPlaying && this.terrain != null)
		{
			this.terrain.gameObject.AddComponent<MeshCollider>();
		}
	}

	public void Invisible()
	{
		if (this.visible)
		{
			if (this.terrain != null)
			{
				this.terrain.CancelBake();
				this.terrain.gameObject.SetActive(false);
			}
			if (this.water != null)
			{
				this.water.gameObject.SetActive(false);
			}
		}
		this.visible = false;
	}

	public void Destroy()
	{
		if (GameScene.isPlaying)
		{
			if (this.waterData != null)
			{
				this.waterData.Release();
			}
			if (this.terrain != null)
			{
				this.terrain.Destroy();
				DelegateProxy.GameDestory(this.terrain);
				DelegateProxy.GameDestory(this.terrain.gameObject);
				this.terrain = null;
			}
			if (this.water != null)
			{
				this.water.Destroy();
				DelegateProxy.GameDestory(this.water);
				DelegateProxy.GameDestory(this.water.gameObject);
				this.water = null;
			}
		}
		else
		{
			if (this.terrain != null)
			{
				DelegateProxy.DestroyObjectImmediate(this.terrain.gameObject);
			}
			if (this.water != null)
			{
				DelegateProxy.DestroyObjectImmediate(this.water.gameObject);
			}
		}
		if (this.units != null)
		{
			while (this.units.Count > 0)
			{
				GameObjectUnit gameObjectUnit = this.units[0];
				if (this.scene.ContainUnit(gameObjectUnit))
				{
					this.scene.RemoveUnit(gameObjectUnit, true, true);
				}
				else
				{
					gameObjectUnit.Destroy();
					this.scene.RemoveEmptyUnit(gameObjectUnit);
				}
			}
			this.units.Clear();
			this.units = null;
			this.unitsMap.Clear();
			this.unitsMap = null;
		}
		this.left = null;
		this.right = null;
		this.top = null;
		this.bot = null;
		this.top_left = null;
		this.top_right = null;
		this.bot_left = null;
		this.bot_right = null;
		this.region = null;
		this.scene = null;
		this._lightmapPrototype = null;
		this.waterData = null;
		this.heights = new float[0, 0];
		this.grids = new int[0, 0];
		this.heights = null;
		this.grids = null;
	}

	public void RemoveWater()
	{
		if (this.water != null)
		{
			if (GameScene.isPlaying)
			{
				DelegateProxy.GameDestory(this.water.gameObject);
			}
			else
			{
				DelegateProxy.DestroyObjectImmediate(this.water.gameObject);
			}
			this.water = null;
			this.waterData = null;
			if (this.terrain != null)
			{
				this.terrain.hasWater = false;
				this.terrain.BuildMaterial(null);
			}
		}
	}

	public void ComputeHeights(int resolution, int mask, int occlusionMask, GameObjectUnit selectObject = null)
	{
		TerrainConfig terrainConfig = this.region.scene.terrainConfig;
		int mask_Ground = GameLayer.Mask_Ground;
		Ray ray = default(Ray);
		Vector3 vector = new Vector3(0f, 500f, 0f);
		Vector3 vector2 = new Vector3(1f, 500f, 0f);
		Vector3 vector3 = new Vector3(0f, 500f, 1f);
		Vector3 vector4 = new Vector3(1f, 500f, 1f);
		Vector3 vector5 = new Vector3(0.5f, 500f, 0.5f);
		Vector3 origin = new Vector3(-0.5f, 500f, -0.5f);
		Vector3[] array = new Vector3[]
		{
			vector,
			vector2,
			vector3,
			vector4,
			vector5
		};
		ray.direction = Vector3.down;
		float num = (float)terrainConfig.tileSize / (float)resolution;
		this.heights = new float[resolution, resolution];
		this.grids = new int[resolution, resolution];
		for (int i = 0; i < resolution; i++)
		{
			for (int j = 0; j < resolution; j++)
			{
				this.heights[i, j] = 0f;
				float num2 = 100000f;
				float num3 = -100000f;
				int num4 = 0;
				bool flag = false;
				for (int k = 0; k < array.Length; k++)
				{
					origin.x = (float)i * num + this.position.x + array[k].x - (float)terrainConfig.tileSize * 0.5f;
					origin.z = (float)j * num + this.position.z + array[k].z - (float)terrainConfig.tileSize * 0.5f;
					ray.origin = origin;
					RaycastHit raycastHit;
					Physics.Raycast(ray, out raycastHit, 2000f, mask);
					if (raycastHit.transform != null)
					{
						if (num4 == 0 && (1 << raycastHit.transform.gameObject.layer & occlusionMask) >= 1)
						{
							num4 = 1;
						}
						if (num2 > raycastHit.point.y)
						{
							num2 = raycastHit.point.y;
						}
						if (num3 < raycastHit.point.y)
						{
							num3 = raycastHit.point.y;
						}
						if (selectObject != null && selectObject.ins != null && raycastHit.transform.gameObject.name == selectObject.ins.name)
						{
							flag = true;
						}
					}
				}
				this.grids[i, j] = num4;
				if (num4 == 0)
				{
					if (num3 - num2 > terrainConfig.blockHeight)
					{
						this.grids[i, j] = 1;
					}
					else
					{
						this.grids[i, j] = 0;
					}
				}
				this.heights[i, j] = 0f;
				num2 = 100000f;
				num3 = -100000f;
				for (int l = 0; l < array.Length; l++)
				{
					origin.x = (float)i * num + this.position.x + array[l].x - (float)terrainConfig.tileSize * 0.5f;
					origin.z = (float)j * num + this.position.z + array[l].z - (float)terrainConfig.tileSize * 0.5f;
					ray.origin = origin;
					RaycastHit raycastHit;
					Physics.Raycast(ray, out raycastHit, 2000f, mask_Ground);
					if (raycastHit.transform != null)
					{
						if (num2 > raycastHit.point.y)
						{
							num2 = raycastHit.point.y;
						}
						if (num3 < raycastHit.point.y)
						{
							num3 = raycastHit.point.y;
						}
					}
				}
				if (num3 > terrainConfig.maxReachTerrainHeight)
				{
					this.grids[i, j] = 1;
				}
				this.heights[i, j] = num3;
				if (flag && this.grids[i, j] == 1)
				{
					float num5 = (float)i * num + this.position.x + array[4].x - (float)terrainConfig.tileSize * 0.5f;
					float num6 = (float)j * num + this.position.z + array[4].z - (float)terrainConfig.tileSize * 0.5f;
					int gridX = Mathf.FloorToInt(num5 / terrainConfig.gridSize);
					int gridY = Mathf.FloorToInt(num6 / terrainConfig.gridSize);
					selectObject.AppendGrid(gridX, gridY);
				}
			}
		}
	}

	public float SampleHeightInterpolation(Vector3 worldPosition)
	{
		TerrainConfig terrainConfig = this.region.scene.terrainConfig;
		if (this.heights == null)
		{
			return terrainConfig.defaultTerrainHeight;
		}
		float num = worldPosition.x - this.position.x + (float)terrainConfig.tileSize * 0.5f;
		float num2 = worldPosition.z - this.position.z + (float)terrainConfig.tileSize * 0.5f;
		float num3 = (float)terrainConfig.tileSize / (float)terrainConfig.heightmapResolution;
		float num4 = num % num3 / num3;
		float num5 = num2 % num3 / num3;
		int num6 = Mathf.FloorToInt(num / num3);
		int num7 = Mathf.FloorToInt(num2 / num3);
		num6 = Math.Max(0, num6);
		num7 = Math.Max(0, num7);
		int num8 = num6;
		int num9 = num7;
		if (num8 < terrainConfig.heightmapResolution - 1)
		{
			num8++;
		}
		if (num9 < terrainConfig.heightmapResolution - 1)
		{
			num9++;
		}
		float num10 = this.heights[num6, num7] * (1f - num4) + this.heights[num8, num7] * num4;
		float num11 = this.heights[num6, num9] * (1f - num4) + this.heights[num8, num9] * num4;
		return num11 * num5 + num10 * (1f - num5);
	}

	public float SampleHeight(Vector3 worldPosition, float curHeight = 0f)
	{
		TerrainConfig terrainConfig = this.region.scene.terrainConfig;
		if (this.heights == null)
		{
			return terrainConfig.defaultTerrainHeight;
		}
		float num = worldPosition.x - this.position.x + (float)terrainConfig.tileSize * 0.5f;
		float num2 = worldPosition.z - this.position.z + (float)terrainConfig.tileSize * 0.5f;
		float num3 = (float)terrainConfig.tileSize / (float)terrainConfig.heightmapResolution;
		int num4 = (int)Mathf.Floor(num / num3);
		int num5 = (int)Mathf.Floor(num2 / num3);
		return this.heights[num4, num5];
	}

	public void DrawGridData()
	{
		TerrainConfig terrainConfig = this.region.scene.terrainConfig;
		if (this.gridDataGO == null)
		{
			this.gridDataGO = new GameObject();
			this.gridDataGO.name = "GridData_" + this.key;
			this.gridDataGO.AddComponent<MeshFilter>();
			this.gridDataGO.AddComponent<MeshRenderer>();
			this.gridDataGO.transform.position = new Vector3(this.position.x, 0f, this.position.z);
			this.gridDataShader = Shader.Find("Snail/Grid");
			this.gridDataMat = new Material(this.gridDataShader);
			string text = "Textures/Shader/Grid";
			this.gridDataMat.mainTexture = (Resources.Load(text, typeof(UnityEngine.Object)) as Texture2D);
			this.gridDataGO.GetComponent<Renderer>().material = this.gridDataMat;
		}
		int heightmapResolution = terrainConfig.heightmapResolution;
		int num = heightmapResolution;
		int num2 = heightmapResolution;
		int num3 = num * num2;
		int num4 = num3 * 4;
		int num5 = num3 * 2;
		Vector3[] array = new Vector3[num4];
		Vector2[] array2 = new Vector2[num4];
		int[] array3 = new int[num5 * 3];
		int num6 = 0;
		float gridSize = terrainConfig.gridSize;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				bool flag = this.grids[i, j] > 0;
				float num7 = (float)i;
				float num8 = (float)j;
				float num9 = num7 * terrainConfig.gridSize - (float)terrainConfig.tileSize * 0.5f;
				float num10 = num8 * terrainConfig.gridSize - (float)terrainConfig.tileSize * 0.5f;
				float y;
				if (!flag)
				{
					y = -1000f;
				}
				else
				{
					y = this.region.scene.SampleHeight(new Vector3(num9 + this.position.x, 100f, num10 + this.position.z), true) + 0.1f;
				}
				array[num6] = new Vector3(num9 + this.position.x, y, num10 + this.position.z)
				{
					x = num9,
					y = y,
					z = num10
				};
				Vector3 v = new Vector2(0f, 0f);
				array2[num6] = v;
				num6++;
				array[num6] = new Vector3(num9 + this.position.x + gridSize, y, num10 + this.position.z)
				{
					x = num9 + gridSize,
					y = y,
					z = num10
				};
				Vector3 v2 = new Vector2(1f, 0f);
				array2[num6] = v2;
				num6++;
				array[num6] = new Vector3(num9 + this.position.x, y, num10 + this.position.z + gridSize)
				{
					x = num9,
					y = y,
					z = num10 + gridSize
				};
				Vector3 v3 = new Vector2(0f, 1f);
				array2[num6] = v3;
				num6++;
				array[num6] = new Vector3(num9 + this.position.x + gridSize, y, num10 + this.position.z + gridSize)
				{
					x = num9 + gridSize,
					y = y,
					z = num10 + gridSize
				};
				Vector3 v4 = new Vector2(1f, 1f);
				array2[num6] = v4;
				num6++;
			}
		}
		num6 = 0;
		for (int i = 0; i < num3; i++)
		{
			array3[num6 * 6] = i * 4;
			array3[num6 * 6 + 1] = i * 4 + 2;
			array3[num6 * 6 + 2] = i * 4 + 3;
			array3[num6 * 6 + 3] = i * 4;
			array3[num6 * 6 + 4] = i * 4 + 3;
			array3[num6 * 6 + 5] = i * 4 + 1;
			num6++;
		}
		this.gridDataMesh.vertices = array;
		this.gridDataMesh.uv = array2;
		this.gridDataMesh.triangles = array3;
		this.gridDataGO.GetComponent<MeshFilter>().mesh = this.gridDataMesh;
	}
}
