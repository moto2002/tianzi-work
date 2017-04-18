using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameScene
{
	public delegate void SceneLoadCompleBack();

	public delegate void SceneLoadingBack();
    /// <summary>
    /// 场景ID
    /// </summary>
	public int sceneID;

	public GameScene.SceneLoadCompleBack sceneLoadCompleListener;

	public GameScene.SceneLoadingBack sceneLoadingListener;
    /// <summary>
    /// 进度计数
    /// </summary>
	public float loadProgress;
    /// <summary>
    /// 场景是否加载完成
    /// </summary>
	public bool loadSceneComplate;
    /// <summary>
    /// 场景加载需要加载的unit计数tick,会记录需要加载的unit数
    /// </summary>
	private int sceneLoadUnitTick;

	public static bool isPlaying = false;
    /// <summary>
    /// 随机码
    /// </summary>
	public float randomCode;
    /// <summary>
    /// update更新频率
    /// </summary>
	private float updateInterval = 0.2f;
    /// <summary>
    /// 上次更新计时
    /// </summary>
	private float lastInterval = Time.realtimeSinceStartup;
    /// <summary>
    /// 帧计数
    /// </summary>
	private int frames;
    /// <summary>
    /// 当前的fps值
    /// </summary>
	public float fps = 30f;

	public float ms = 1f;
    /// <summary>
    /// 场景运行计时
    /// </summary>
	public float time;
    /// <summary>
    /// 分帧处理，优化性能
    /// </summary>
	private int tick;
    /// <summary>
    /// 视距Region网格的宽度,默认为1，为九宫格
    /// </summary>
	private int viewRect = 1;

	public int targetFrameRate = 25;

	private Dictionary<string, Region> regionsMap = new Dictionary<string, Region>();

	public List<Region> regions;
    /// <summary>
    /// 视点位置，也就是主角色所在的地面位置
    /// </summary>
	public Vector3 eyePos = Vector3.zero;
    /// <summary>
    /// 视点所在Region的X坐标
    /// </summary>
	public int curRegionX;
    /// <summary>
    ///  视点所在Region的Y坐标
    /// </summary>
	public int curRegionY;

	public Dictionary<int, GameObjectUnit> unitsMap;
    /// <summary>
    /// 当前的静态unit列表
    /// </summary>
	public List<GameObjectUnit> units;

	public int unitCount;

	public int unitIdCount;

	private TerrainConfig _terrainConfig;
    /// <summary>
    /// 当前场景包含的tile映射
    /// </summary>
	private Dictionary<string, Tile> tilesMap;

	public List<Tile> tiles;
    /// <summary>
    /// 当前视点所在tile
    /// </summary>
	public Tile curTile;
    /// <summary>
    /// 不剔除unit
    /// </summary>
	public static bool dontCullUnit = true;

	public static bool isEditor = false;

	private static MapPath _mapPath;

	private MapPath _saveMapPath;

	private float detailGridSize = 0.5f;

	private List<Camera> cameras = new List<Camera>();

	public GameObjectUnit mainUnit;

	public Camera mainCamera;

	private static int mainCameraCullingMask = -1;

	private static float[,] _heights;

	public static GameScene mainScene = null;
    /// <summary>
    /// GameScene栈列表
    /// </summary>
	public static List<GameScene> sceneStack = new List<GameScene>();
    /// <summary>
    /// 场景静态物体根游戏对象
    /// </summary>
	public static GameObject staticInsRoot;
    /// <summary>
    /// 场景主摄像机的RenderTexture
    /// </summary>
	public RenderTexture mainRTT;

	public static bool SampleMode = false;
    /// <summary>
    /// 是否从assetbundle加载资源
    /// </summary>
	public bool loadFromAssetBund;
    /// <summary>
    /// 场景地形数据
    /// </summary>
	public global::TerrainData terrainData;

	public bool statisticMode = true;
    /// <summary>
    /// 是否缓存模式
    /// </summary>
	public bool cacheMode;
    /// <summary>
    /// 需要预加载的资源路径列表 ---跟preloadUnitAssetTypeList中一一索引对应
    /// </summary>
	public List<string> preloadUnitAssetPathList = new List<string>();
    /// <summary>
    /// 需要预加载的资源类型列表
    /// </summary>
	public List<int> preloadUnitAssetTypeList = new List<int>();
    /// <summary>
    /// 进行预加载中的资源列表
    /// </summary>
	public List<Asset> preloadUnitAssetList = new List<Asset>();
    /// <summary>
    /// 场景静态unit单位列表
    /// </summary>
	public List<GameObjectUnit> staticUnitcCache = new List<GameObjectUnit>();
    /// <summary>
    /// 场景动态unit单位列表
    /// </summary>
	public List<DynamicUnit> dynamicUnitsCache = new List<DynamicUnit>();
    /// <summary>
    /// 当前场景unit单位映射表，包含所有加载的unit   ??????
    /// </summary>
	public Dictionary<int, GameObjectUnit> curSceneUnitsMap = new Dictionary<int, GameObjectUnit>();

	public List<ParserBase> parsers = new List<ParserBase>();
    /// <summary>
    /// 后期处理效果列表
    /// </summary>
	public List<string> postEffectsList = new List<string>();

	private Dictionary<string, byte[]> peConfig = new Dictionary<string, byte[]>();

    /// <summary>
    /// 是否启用光照贴图修正
    /// </summary>
	public static bool lightmapCorrectOn = false;

	private static Root _rootIns;
    /// <summary>
    /// 光照数据长度，读取数据时使用一下？？？？
    /// </summary>
	public long lightDataLength = -1L;

	private bool destroyed;
    /// <summary>
    /// 截屏纹理贴图
    /// </summary>
	private Texture2D screenshots;
    /// <summary>
    /// 是否启用后期处理效果
    /// </summary>
	public static bool postEffectEnable = true;
    /// <summary>
    /// 后期处理效果是否更新标记
    /// </summary>
	private bool breset;
    /// <summary>
    /// 后期处理效果配置是否已经加载完成
    /// </summary>
	private bool peConfigLoaded;
    /// <summary>
    /// 动态unit单位id开始计数
    /// </summary>
	private int dynamicUnitStartCount = 500000;
    /// <summary>
    /// 停止的非活动unit单位列表
    /// </summary>
	private List<DynamicUnit> removeDynUnits = new List<DynamicUnit>();
    /// <summary>
    /// 阳光/平行光游戏对象
    /// </summary>
	private GameObject sunLightObj;
    /// <summary>
    /// 光照贴图数量
    /// </summary>
	public int lightmapCount;
    /// <summary>
    /// 地形贴图是否加载完成
    /// </summary>
	private bool loadTerrainTextureComplate;
    /// <summary>
    /// 场景数据读取完成
    /// </summary>
	public bool readCompalte;
    /// <summary>
    /// 世界空间光源位置
    /// </summary>
	private Vector4 worldSpaceLightPosition;
    /// <summary>
    /// 之前的光照贴图修正状态--是否开启
    /// </summary>
	private bool _oldLightmapCorrectOn;
    /// <summary>
    /// 是否需要预加载
    /// </summary>
	public bool needPreload = true;
    /// <summary>
    /// 预加载是否完成
    /// </summary>
	private bool preloadComplate;
    /// <summary>
    /// 每次预加载的最大数量
    /// </summary>
	private int preloadMaxCountPer = 1;
    /// <summary>
    /// 已经预加载到的索引值
    /// </summary>
	private int loadIndex;
    /// <summary>
    /// 当前Region的注册key -- Region遍历临时使用，似乎没必要作为成员变量记录
    /// </summary>
	private string regKey = string.Empty;
    /// <summary>
    /// 观察视线方向
    /// </summary>
	public Vector3 viewDir = Vector3.zero;
    /// <summary>
    /// 当前预加载的总计数
    /// </summary>
	private int curPreLoadCount;
    /// <summary>
    /// 检测没有bake的光照贴图进行bake一次tick可以bake的数量
    /// </summary>
	private int perTerBakeCount = 1;
    /// <summary>
    /// 检测没有bake的光照贴图进行bake临时使用
    /// </summary>
	private int terBakeCount;
    /// <summary>
    /// 预加载分帧处理，用于记录的参数
    /// </summary>
	private int preloadTick;
    /// <summary>
    /// 一次预加载的间隔tick数
    /// </summary>
	private int preloadInterval = 1;
    /// <summary>
    /// 预加载的最大tick值
    /// </summary>
	private int preloadMaxTick;
    /// <summary>
    /// 预加载最大进度
    /// </summary>
	private float preloadMaxProgress = 0.8f;
    /// <summary>
    /// 一次预加载进度增加值
    /// </summary>
	private float progressInc = 0.02f;
    //加载等待tick技术
	private int loadComplateWaitTick;
    /// <summary>
    /// 场景最大的unit单位技术
    /// </summary>
	private int sceneMaxLoadUnitTick = 80;
    /// <summary>
    /// 旧的场景加载unit技术tick,也就是已经加载了的unit计数tick
    /// </summary>
	private int oldSceneLoadUnitTick;
    /// <summary>
    /// 视点的上次刷新位置
    /// </summary>
	private Vector3 lastPos;
    /// <summary>
    /// 每帧更新的tile数量
    /// </summary>
	private int visibleTilePerFrame = 1;
    /// <summary>
    /// 当前开启显示的tile计数
    /// </summary>
	private int visTileCount;
    /// <summary>
    /// 可视的动态unit计数
    /// </summary>
	public int visibleDynamicUnitCount;
    /// <summary>
    /// 可视的静态unit计数
    /// </summary>
	public int visibleStaticUnitCount;
    /// <summary>
    /// 每帧更新的可视动态unit数量计数
    /// </summary>
	private int visibleDynaUnitPerFrame;
    /// <summary>
    /// 每帧更新的可视静态unit数量计数
    /// </summary>
	private int visibleStaticUnitPerFrame;
    /// <summary>
    /// 每帧隐藏的静态unit数量限制
    /// </summary>
	private int hideStaticUnitPerFrame;
    /// <summary>
    /// 动态单位的数量上限
    /// </summary>
	public static int maxDynaUnit = 50;
    /// <summary>
    /// 可视的静态unit类型计数
    /// </summary>
	public int visibleStaticTypeCount;
    /// <summary>
    /// 当前的动态unit列表
    /// </summary>
	public List<GameObjectUnit> dynamicUnits = new List<GameObjectUnit>();

	public Dictionary<string, int> staticTypeMap = new Dictionary<string, int>();

	public int useMaterialsCount;

	public Dictionary<string, Material> materials = new Dictionary<string, Material>();

	private float dx;

	private float dz;

	private List<GameObjectUnit> shadowUnits = new List<GameObjectUnit>();
    /// <summary>
    /// 当前可以接受阴影的unit最大数量
    /// </summary>
	public int maxCastShadowsUnitCount = 8;
    /// <summary>
    /// 是否启用动态unit的动态碰撞网格功能
    /// </summary>
	public bool enableDynamicGrid;

	private string smkey = string.Empty;

	public float waterHeight;

	public static Root root
	{
		get
		{
			if (GameScene._rootIns == null)
			{
				GameScene.staticInsRoot = new GameObject("staticInsRoot");
				GameScene._rootIns = GameScene.staticInsRoot.AddComponent<Root>();
			}
			return GameScene._rootIns;
		}
	}

	public TerrainConfig terrainConfig
	{
		get
		{
			return this._terrainConfig;
		}
	}

	public MapPath mapPath
	{
		get
		{
			return GameScene._mapPath;
		}
		set
		{
			GameScene._mapPath = value;
		}
	}

	public MapPath saveMapPath
	{
		get
		{
			return this._saveMapPath;
		}
		set
		{
			this._saveMapPath = value;
		}
	}

	public float[,] heights
	{
		get
		{
			return GameScene._heights;
		}
		set
		{
			GameScene._heights = value;
		}
	}
    /// <summary>
    /// 创建GameScene，新建？？？？
    /// </summary>
    /// <param name="createNew"></param>
	public GameScene(bool createNew = false)
	{
		this.randomCode = UnityEngine.Random.value;
		this.regions = new List<Region>();
		this.regionsMap = new Dictionary<string, Region>();
		this.tilesMap = new Dictionary<string, Tile>();
		this.tiles = new List<Tile>();
		this.unitsMap = new Dictionary<int, GameObjectUnit>();
		this.units = new List<GameObjectUnit>();
		GameScene.mainScene = this;
		GameScene.isPlaying = Application.isPlaying;
		GameScene.isEditor = Application.isEditor;
		this._terrainConfig = new TerrainConfig();
		if (!GameScene.isEditor)
		{
			Terrainmapping.mapsCount = 4;
		}
		GameScene.sceneStack.Add(this);
		if (createNew)
		{
			this.readCompalte = true;
			this.loadTerrainTextureComplate = true;
		}
		this.postEffectsList.Add("TopGradualColor");
		this.postEffectsList.Add("Vignetting");
		this.postEffectsList.Add("WaterDistortion");
	}
    /// <summary>
    /// 销毁场景（主要是相关数据内存和引用等)
    /// </summary>
	public void Destroy()
	{
		if (this.mainCamera != null)
		{
			this.mainCamera.cullingMask = GameScene.mainCameraCullingMask;
		}
		this.destroyed = true;
		if (GameScene.staticInsRoot != null)
		{
			UnityEngine.Object.DestroyImmediate(GameScene.staticInsRoot);
		}
		RenderSettings.skybox = null;
		if (this.mapPath != null)
		{
			this.mapPath.Clear();
		}
		this.removeDynUnits.Clear();
		this.shadowUnits.Clear();
		int i = 0;
		if (this.units != null)
		{
			while (this.units.Count > 0)
			{
				if (this.units[0].isStatic)
				{
					if (this.units != null && this.units.Count > 0)
					{
						this.RemoveUnit(this.units[0], true, true);
					}
				}
				else if (this.units != null && this.units.Count > 0)
				{
					this.RemoveDynamicUnit(this.units[0] as DynamicUnit, true, true);
				}
			}
			this.units.Clear();
			this.units = null;
			this.dynamicUnits.Clear();
			this.dynamicUnits = null;
		}
		while (i < this.regions.Count)
		{
			this.regions[i].Destroy();
			i++;
		}
		this.regions.Clear();
		this.regions = null;
		while (this.tiles.Count > 0)
		{
			this.RemoveTile(this.tiles[0], true);
		}
		this.tiles = null;
		this.unitsMap.Clear();
		this.unitsMap = null;
		this.tilesMap.Clear();
		this.tilesMap = null;
		if (this.curSceneUnitsMap != null)
		{
			this.curSceneUnitsMap.Clear();
			this.curSceneUnitsMap = null;
		}
		if (this.saveMapPath != null)
		{
			this.saveMapPath.Clear();
			this.saveMapPath = null;
		}
		if (this.sunLightObj != null)
		{
			if (GameScene.isPlaying)
			{
				DelegateProxy.GameDestory(this.sunLightObj);
			}
			else
			{
				DelegateProxy.DestroyObjectImmediate(this.sunLightObj);
			}
		}
		this._terrainConfig = null;
		this.preloadUnitAssetPathList.Clear();
		this.preloadUnitAssetPathList = null;
		this.preloadUnitAssetList.Clear();
		this.preloadUnitAssetList = null;
		this.preloadUnitAssetTypeList.Clear();
		this.preloadUnitAssetTypeList = null;
		this.cameras.Clear();
		this.cameras = null;
		this.mainCamera = null;
		this.lightDataLength = -1L;
		this.sceneLoadCompleListener = null;
		this.sceneLoadingListener = null;
		if (this.screenshots != null)
		{
			this.screenshots = null;
		}
		LightmapSettings.lightmaps = new LightmapData[0];
		if (GameScene.mainScene == this)
		{
			GameScene.mainScene = null;
		}
		GameScene.sceneStack.Remove(this);  //作为当前的GameScene销毁时，需要从场景列表中移除
		this.postEffectsList.Clear();
		this.postEffectsList = null;
		this.parsers.Clear();
		this.parsers = null;
		this.staticUnitcCache.Clear();
		this.staticUnitcCache = null;
		this.dynamicUnitsCache.Clear();
		this.dynamicUnitsCache = null;
		LightmapSettings.lightmaps = null;
		LightmapCorrection.Clear();
		AssetLibrary.RemoveAllAsset();
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}
    /// <summary>
    /// 根据场景id获取场景对象
    /// </summary>
    /// <param name="sceneID"></param>
    /// <returns></returns>
	public static GameScene FindScene(int sceneID)
	{
		int count = GameScene.sceneStack.Count;
		for (int i = 0; i < count; i++)
		{
			if (GameScene.sceneStack[i].sceneID == sceneID)
			{
				return GameScene.sceneStack[i];
			}
		}
		return null;
	}

	public Texture2D RenderPostEffectImage(List<string> peList = null)
	{
		return null;
	}
    /// <summary>
    /// 变更后期处理效果开关状态
    /// </summary>
    /// <param name="value"></param>
	public void ActivePostEffect(bool value)
	{
		if (!GameScene.isPlaying)
		{
			return;
		}
		if (this.breset && GameScene.postEffectEnable == value)  //后期处理效果开关状态变化，才进入处理逻辑  ,否则无操作
		{
			return;
		}
		GameScene.postEffectEnable = value;
		if (this.mainCamera == null)
		{
			return;
		}
		this.breset = true;   //更新后期处理效果后的标记
		this.peConfigLoaded = true;
		for (int i = 0; i < this.postEffectsList.Count; i++)
		{
			string text = this.postEffectsList[i];
			MonoBehaviour monoBehaviour = this.mainCamera.GetComponent(text) as MonoBehaviour;
			if (monoBehaviour == null)
			{
				Type type = Type.GetType(text);
				monoBehaviour = (this.mainCamera.gameObject.AddComponent(type) as MonoBehaviour);
			}
			if (monoBehaviour != null)
			{
				if (value && this.peConfig.ContainsKey(text))      //根据开关状态，激活所有后期效果或关闭效果
				{
					monoBehaviour.enabled = true;
				}
				else
				{
					monoBehaviour.enabled = false;
				}
			}
		}
	}
    /// <summary>
    /// 加载后期处理效果配置信息
    /// </summary>
	public void LoadPostEffectConfig()
	{
		if (this.peConfigLoaded)
		{
			return;
		}
		for (int i = 0; i < this.postEffectsList.Count; i++)
		{
			string text = this.postEffectsList[i];
			PEBase pEBase = this.mainCamera.GetComponent(text) as PEBase;
			if (pEBase == null)
			{
				Type type = Type.GetType(text);
				pEBase = (this.mainCamera.gameObject.AddComponent(type) as PEBase);    //效果组件加入主摄像机游戏对象
			}
			if (pEBase != null)
			{
				pEBase.enabled = false;
				if (this.peConfig.ContainsKey(text))
				{
					byte[] array = this.peConfig[text];
					if (array != null)
					{
						MemoryStream input = new MemoryStream(array);
						BinaryReader binaryReader = new BinaryReader(input);
						pEBase.enabled = binaryReader.ReadBoolean();
						if (pEBase.enabled)
						{
							int num = binaryReader.ReadInt32();
							for (int j = 0; j < num; j++)
							{
								int num2 = binaryReader.ReadInt32();
								string propertyName = binaryReader.ReadString();
								if (num2 == 0)
								{
									pEBase.material.SetInt(propertyName, binaryReader.ReadInt32());
								}
								else if (num2 == 1)
								{
									pEBase.material.SetFloat(propertyName, binaryReader.ReadSingle());
								}
								else if (num2 == 2)
								{
									pEBase.material.SetColor(propertyName, new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle()));
								}
								else if (num2 == 3)
								{
									pEBase.material.SetVector(propertyName, new Vector4(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle()));
								}
								else if (num2 == 4)
								{
									Asset asset = AssetLibrary.Load(binaryReader.ReadString(), AssetType.Texture2D, LoadType.Type_Auto);
									if (asset != null && asset.texture2D != null)
									{
										pEBase.material.SetTexture(propertyName, asset.texture2D);
									}
								}
							}
							pEBase.LoadParams();
						}
					}
				}
				else
				{
					pEBase.enabled = false;
				}
			}
		}
		this.peConfigLoaded = true;
	}
    /// <summary>
    /// 静态物体进行静态批处理
    /// </summary>
	public void StaticBatchCombine()
	{
		StaticBatchingUtility.Combine(GameScene.staticInsRoot);
	}
    /// <summary>
    /// 首次运行 ,创建根游戏对象
    /// </summary>
	public void FirstRun()
	{
        //创建场景静态物体根对象
		if (GameScene.staticInsRoot == null)
		{
			GameScene.staticInsRoot = new GameObject("staticInsRoot");
			GameScene._rootIns = GameScene.staticInsRoot.AddComponent<Root>();
		}
	}
    /// <summary>
    /// 添加场景主摄像机
    /// </summary>
    /// <param name="camera"></param>
	public void AddCamera(Camera camera)
	{
		for (int i = 0; i < this.cameras.Count; i++)
		{
			if (this.cameras[i] == camera)
			{
				return;
			}
		}
		if (this.mainCamera == null)
		{
			this.mainCamera = camera;
		}
		camera.backgroundColor = this._terrainConfig.fogColor;
		this.cameras.Add(camera);
	}

	public bool IsValidForWalk(Vector3 worldPostion, int collisionSize)
	{
		if (GameScene._mapPath == null)
		{
			return false;
		}
		int num = Mathf.FloorToInt(worldPostion.x / this.detailGridSize);
		int num2 = Mathf.FloorToInt(worldPostion.z / this.detailGridSize);
		if (Math.Abs(num) >= GameScene._mapPath.halfWidth)
		{
			return false;
		}
		if (Math.Abs(num2) >= GameScene._mapPath.halfHeight)
		{
			return false;
		}
		int num3 = num + this.mapPath.halfWidth;
		int num4 = num2 + this.mapPath.halfHeight;
		int num5 = GameScene._mapPath.grids[num3, num4] >> collisionSize & 1;
		if (num5 == 1)
		{
			return false;
		}
		num5 = (GameScene._mapPath.grids[num3, num4] >> collisionSize - 1 & 1);
		return num5 != 1;
	}

	public bool IsValidForWalk(Vector3 worldPostion, int collisionSize, out int gridType)
	{
		int num = Mathf.FloorToInt(worldPostion.x / this.detailGridSize);
		int num2 = Mathf.FloorToInt(worldPostion.z / this.detailGridSize);
		int num3 = num + this.mapPath.halfWidth;
		int num4 = num2 + this.mapPath.halfHeight;
		int num5 = GameScene._mapPath.grids[num3, num4] >> this.mapPath.gridTypeMask;
		if (num5 > 0)
		{
			gridType = num5;
			return true;
		}
		gridType = 0;
		int num6 = GameScene._mapPath.grids[num3, num4] >> collisionSize & 1;
		if (num6 == 1)
		{
			return false;
		}
		num6 = (GameScene._mapPath.grids[num3, num4] >> collisionSize - 1 & 1);
		return num6 != 1;
	}

	public int getGridType(Vector3 worldPostion)
	{
		int num = Mathf.FloorToInt(worldPostion.x / this.detailGridSize);
		int num2 = Mathf.FloorToInt(worldPostion.z / this.detailGridSize);
		int num3 = num + this.mapPath.halfWidth;
		int num4 = num2 + this.mapPath.halfHeight;
		return GameScene._mapPath.grids[num3, num4] >> this.mapPath.gridTypeMask;
	}

	public int getGridValue(Vector3 worldPostion)
	{
		int num = Mathf.FloorToInt(worldPostion.x / this.detailGridSize);
		int num2 = Mathf.FloorToInt(worldPostion.z / this.detailGridSize);
		int num3 = num + this.mapPath.halfWidth;
		int num4 = num2 + this.mapPath.halfHeight;
		return GameScene._mapPath.grids[num3, num4];
	}

	public void UpdateWalkBlocker()
	{
		if (this.saveMapPath != null)
		{
			this.saveMapPath.PrepareForPathSearch();
		}
	}

	public void UpdateDetailMapPath()
	{
		int width = this.saveMapPath.width;
		int height = this.saveMapPath.height;
		int[,] array = new int[width * 2, height * 2];
		MapPath mapPath = new MapPath(this._terrainConfig.gridSize * 0.5f, width * 2, height * 2);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				int num = i * 2;
				int num2 = j * 2;
				int num3 = this.saveMapPath.grids[i, j];
				array[num, num2] = num3;
				array[num + 1, num2] = num3;
				array[num, num2 + 1] = num3;
				array[num + 1, num2 + 1] = num3;
			}
		}
		mapPath.grids = array;
		this.mapPath = mapPath;
		this.detailGridSize = this._terrainConfig.gridSize * 0.5f;
	}

	public GameObjectUnit CreateEmptyUnit(int createID = -1)
	{
		if (createID < 0)
		{
			this.unitIdCount++;
			createID = this.unitCount;
		}
		GameObjectUnit gameObjectUnit;
		if (this.staticUnitcCache.Count > 0)
		{
			gameObjectUnit = this.staticUnitcCache[0];
			gameObjectUnit.destroyed = false;
			this.staticUnitcCache.RemoveAt(0);
		}
		else
		{
			gameObjectUnit = new GameObjectUnit(createID);
		}
		return gameObjectUnit;
	}

	public void RemoveEmptyUnit(GameObjectUnit unit)
	{
		if (!this.staticUnitcCache.Contains(unit))
		{
			this.staticUnitcCache.Add(unit);
		}
	}
    /// <summary>
    /// 创建静态unit对象,指定对象的位置
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="prePath">资源路径</param>
    /// <param name="isStatic">是否静态</param>
    /// <returns></returns>
	public GameObjectUnit CreateUnit(Vector3 pos, string prePath, bool isStatic = true)
	{
		this.unitIdCount++;
		GameObjectUnit gameObjectUnit = GameObjectUnit.Create(this, pos, this.unitIdCount, prePath);
		gameObjectUnit.name = "Unit_" + gameObjectUnit.createID;
		gameObjectUnit.isStatic = isStatic;
		UnityEngine.Object o = Resources.Load(prePath, typeof(UnityEngine.Object));
		GameObject gameObject = DelegateProxy.Instantiate(o) as GameObject;
		gameObject.transform.position = pos;
		gameObjectUnit.localScale = gameObject.transform.localScale;
		gameObjectUnit.rotation = gameObject.transform.rotation;
		gameObjectUnit.type = UnitType.GetType(gameObject.layer);
		gameObjectUnit.unitParser = UnitType.GenUnitParser(gameObjectUnit.type);
		gameObjectUnit.unitParser.unit = gameObjectUnit;
		gameObjectUnit.ins = gameObject;
		gameObjectUnit.ComputeTiles();
		gameObjectUnit.UpdateViewRange();
		DelegateProxy.DestroyObjectImmediate(gameObject);
		this.AddUnit(gameObjectUnit);
		return gameObjectUnit;
	}
    /// <summary>
    /// 创建静态unit对象，并指定对象的自定义朝向和位置，对象属相来自配置
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="prePath"></param>
    /// <param name="rotation"></param>
    /// <param name="isStatic"></param>
    /// <param name="parser">数据解析器</param>
    /// <returns></returns>
	public GameObjectUnit CreateUnit(Vector3 pos, string prePath, Quaternion rotation, bool isStatic = true, UnitParser parser = null)
	{
		this.unitIdCount++;
		GameObjectUnit gameObjectUnit = GameObjectUnit.Create(this, pos, this.unitIdCount, prePath);
		gameObjectUnit.name = "Unit_" + gameObjectUnit.createID;
		gameObjectUnit.isStatic = isStatic;
		UnityEngine.Object o = Resources.Load(prePath, typeof(UnityEngine.Object));
		GameObject gameObject = DelegateProxy.Instantiate(o) as GameObject;
		gameObject.transform.position = pos;
		gameObjectUnit.localScale = gameObject.transform.localScale;
		gameObjectUnit.rotation = rotation;
		gameObjectUnit.type = UnitType.GetType(gameObject.layer);
		if (parser == null)
		{
			gameObjectUnit.unitParser = UnitType.GenUnitParser(gameObjectUnit.type);
		}
		else
		{
			gameObjectUnit.unitParser = parser;
		}
		gameObjectUnit.unitParser.unit = gameObjectUnit;
		gameObjectUnit.ins = gameObject;
		gameObjectUnit.ComputeTiles();
		gameObjectUnit.UpdateViewRange();
		DelegateProxy.DestroyObjectImmediate(gameObject);
		this.AddUnit(gameObjectUnit);
		return gameObjectUnit;
	}
    /// <summary>
    /// 创建动态unit,并指定类型，剔除距离等，如果动态Unit列表中有，取出重用，否则新建
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="prePath"></param>
    /// <param name="radius">碰撞计算半径？？？</param>
    /// <param name="uType"></param>
    /// <param name="dynamiCullingDistance"></param>
    /// <returns></returns>
	public DynamicUnit CreateDynamicUnit(Vector3 pos, string prePath, float radius, int uType = 0, float dynamiCullingDistance = -1f)
	{
		if (radius == 0f)
		{
			radius = 0.2f;
		}
		this.unitIdCount++;
		DynamicUnit dynamicUnit;
		if (this.dynamicUnitsCache.Count > 0)          //如果有缓存，重用缓存
		{
			dynamicUnit = this.dynamicUnitsCache[0];
			dynamicUnit.createID = this.unitIdCount + this.dynamicUnitStartCount;
			dynamicUnit.prePath = prePath;
			dynamicUnit.destroyed = false;
			dynamicUnit.isMainUint = false;
			dynamicUnit.willRemoved = false;
			dynamicUnit.scene = this;
			dynamicUnit.type = uType;
			dynamicUnit.isStatic = false;
			this.AddUnit(dynamicUnit);
			if (dynamiCullingDistance > 0f)             //有指定剔除距离，使用，否则使用配置默认的剔除距离
			{
				dynamicUnit.near = dynamiCullingDistance;
			}
			else
			{
				dynamicUnit.near = this.terrainConfig.dynamiCullingDistance;
			}
			dynamicUnit.far = dynamicUnit.near + 2f;
			this.dynamicUnitsCache.RemoveAt(0);
		}
		else
		{
			dynamicUnit = DynamicUnit.Create(this, pos, this.unitIdCount + this.dynamicUnitStartCount, prePath, radius, dynamiCullingDistance);
			dynamicUnit.isStatic = false;
			this.AddUnit(dynamicUnit);
			dynamicUnit.type = uType;
		}
		dynamicUnit.position = pos;
		dynamicUnit.position.y = this.SampleHeight(pos, true); //放到地面上
		return dynamicUnit;
	}
    /// <summary>
    /// 直接新建动态unit，指定剔除距离，类型等
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="prePath"></param>
    /// <param name="radius"></param>
    /// <param name="uType"></param>
    /// <param name="dynamiCullingDistance"></param>
    /// <returns></returns>
	public DynamicUnit CreateDynamicUnitImmediately(Vector3 pos, string prePath, float radius, int uType = 0, float dynamiCullingDistance = -1f)
	{
		DynamicUnit dynamicUnit = this.CreateDynamicUnit(pos, prePath, radius, uType, dynamiCullingDistance);
		float num = dynamicUnit.position.x - this.eyePos.x;
		float num2 = dynamicUnit.position.z - this.eyePos.z;
		dynamicUnit.viewDistance = Mathf.Sqrt(num * num + num2 * num2);
		if (dynamicUnit.position.y > 1E+08f)
		{
			dynamicUnit.position.y = this.SampleHeight(dynamicUnit.position, true);
		}
		if (dynamicUnit.viewDistance < dynamicUnit.far)   //如果在剔除距离内，激活显示
		{
			dynamicUnit.Visible();
		}
		return dynamicUnit;
	}
    /// <summary>
    /// 根据unitID在当前X宫格的所有region中查找unit对象
    /// </summary>
    /// <param name="createID"></param>
    /// <returns></returns>
	public GameObjectUnit FindUnitInRegions(int createID)
	{
		int count = this.regions.Count;
		for (int i = 0; i < count; i++)
		{
			GameObjectUnit gameObjectUnit = this.regions[i].FindUint(createID);
			if (gameObjectUnit != null)
			{
				return gameObjectUnit;
			}
		}
		return null;
	}
    /// <summary>
    /// 遍历当前的unit列表，查找指定unitID的unit对象
    /// </summary>
    /// <param name="createID"></param>
    /// <returns></returns>
	public GameObjectUnit FindUnitInTiles(int createID)
	{
		int count = this.tiles.Count;
		for (int i = 0; i < count; i++)
		{
			GameObjectUnit gameObjectUnit = this.tiles[i].FindUnit(createID);
			if (gameObjectUnit != null)
			{
				return gameObjectUnit;
			}
		}
		return null;
	}
    /// <summary>
    /// 根据unit名称查找unit对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
	public GameObjectUnit FindUnit(string name)
	{
		if (!name.Contains("Unit_"))
		{
			return null;
		}
		int key;
		if (!GameScene.isPlaying)
		{
			string[] array = name.Split(new char[]
			{
				':'
			});
			key = int.Parse(array[array.Length - 1].Replace("Unit_", string.Empty));
		}
		else
		{
			key = int.Parse(name.Replace("Unit_", string.Empty));
		}
		if (this.unitsMap.ContainsKey(key))
		{
			return this.unitsMap[key];
		}
		return null;
	}
    /// <summary>
    /// 遍历unit列表，获取第一个指定类型的unit对象
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
	public GameObjectUnit FindFirstUnit(int type)
	{
		int count = this.units.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.units[i].type == type)
			{
				return this.units[i];
			}
		}
		return null;
	}
    /// <summary>
    /// 根据id，从unit映射列表中找到指定Unit
    /// </summary>
    /// <param name="cID"></param>
    /// <returns></returns>
	public GameObjectUnit FindUnit(int cID)
	{
		if (this.unitsMap.ContainsKey(cID))
		{
			return this.unitsMap[cID];
		}
		return null;
	}
    /// <summary>
    /// 判断当前的unit映射中是否存在指定unit对象
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
	public bool ContainUnit(GameObjectUnit unit)
	{
		return this.unitsMap.ContainsKey(unit.createID);
	}
    /// <summary>
    /// 编辑器中根据unit名称删除指定unit对象
    /// </summary>
    /// <param name="name"></param>
	public void RemoveUnitInEditor(string name)
	{
		GameObjectUnit gameObjectUnit = this.FindUnit(name);
		if (gameObjectUnit == null)
		{
			return;
		}
		gameObjectUnit.ClearTiles();
		for (int i = 0; i < this.tiles.Count; i++)    //移除当前unit列表中unit中关于指定unit对象的引用
		{
			this.tiles[i].RemoveUnit(gameObjectUnit);
		}
		if (gameObjectUnit.ins != null)               //销毁Gameobject对象
		{
			DelegateProxy.DestroyObjectImmediate(gameObjectUnit.ins);
		}
		this.unitsMap.Remove(gameObjectUnit.createID);      //从映射中移除
		this.units.Remove(gameObjectUnit);                  //从列表移除
		this.unitCount--;
	}
    /// <summary>
    ///   编辑器中根据unit删除指定unit对象
    /// </summary>
    /// <param name="unit"></param>
	public void RemoveUnitInEditor(GameObjectUnit unit)
	{
		if (unit == null)
		{
			return;
		}
		unit.ClearTiles();
		for (int i = 0; i < this.tiles.Count; i++)
		{
			this.tiles[i].RemoveUnit(unit);
		}
		if (unit.ins != null)
		{
			DelegateProxy.DestroyObjectImmediate(unit.ins);
		}
		this.unitsMap.Remove(unit.createID);
		this.units.Remove(unit);
		this.unitCount--;
	}
    /// <summary>
    /// 添加unit到当前unit列表及映射
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
	public bool AddUnit(GameObjectUnit unit)
	{
		if (this.unitsMap.ContainsKey(unit.createID))
		{
			return false;
		}
		this.units.Add(unit);
		this.unitsMap.Add(unit.createID, unit);
		this.unitCount++;
		return true;
	}
    /// <summary>
    /// 移除动态unit
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="cache">是否缓存</param>
    /// <param name="immediately">是否直接销毁或者延迟销毁</param>
	public void RemoveDynamicUnit(DynamicUnit unit, bool cache = true, bool immediately = false)
	{
		if (unit == null)
		{
			return;
		}
		if (unit.mDynState == DynamicState.LINK_PARENT || unit.mDynState == DynamicState.LINK_PARENT_CHILD)
		{
			unit.RemoveAllLinkDynamic();
		}
		if (!true) //不直接销毁的话,加入待移除的unit列表
		{
			unit.Stop();
			unit.willRemoved = true;
			this.removeDynUnits.Add(unit);
		}
		else
		{
			this.mapPath.SetDynamicCollision(unit.position, unit.collisionSize, true, 1);
			unit.Invisible();
			unit.Destroy();
			this.unitsMap.Remove(unit.createID);
			this.units.Remove(unit);
			this.unitCount--;
			unit.createID = -1;
			if (cache && !this.dynamicUnitsCache.Contains(unit))
			{
				this.dynamicUnitsCache.Add(unit); //需要缓存，加入缓存动态unit列表
			}
		}
	}
    /// <summary>
    /// 移除静态unit
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="destroy">是否销毁</param>
    /// <param name="cache">是否缓存</param>
	public void RemoveUnit(GameObjectUnit unit, bool destroy = false, bool cache = false)
	{
		unit.Invisible();
		this.unitsMap.Remove(unit.createID);
		this.units.Remove(unit);
		if (destroy)
		{
			unit.Destroy();
		}
		this.unitCount--;
		if (cache && !this.staticUnitcCache.Contains(unit))
		{
			this.staticUnitcCache.Add(unit);   //加入静态unit缓存列表
		}
	}
    /// <summary>
    /// 加入tile到当前场景
    /// </summary>
    /// <param name="tile"></param>
	public void AddTile(Tile tile)
	{
		if (this.tilesMap.ContainsKey(tile.key))
		{
			return;
		}
		this.tilesMap.Add(tile.key, tile);
		this.tiles.Add(tile);
	}
    /// <summary>
    /// 判断是否包含在当前场景中
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
	public bool ContainTile(Tile tile)
	{
		return this.tilesMap.ContainsKey(tile.key);
	}
    /// <summary>
    /// 从场景移除
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="destroy">是否销毁</param>
	public void RemoveTile(Tile tile, bool destroy = false)
	{
		if (this.tilesMap.ContainsKey(tile.key))
		{
			tile.Invisible();
			if (destroy)
			{
				tile.Destroy();
			}
			this.tilesMap.Remove(tile.key);
			this.tiles.Remove(tile);
		}
	}
    /// <summary>
    /// 根据索引键获取指定的tile对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
	public Tile FindTile(string key)
	{
		if (this.tilesMap.ContainsKey(key))
		{
			return this.tilesMap[key];
		}
		return null;
	}

    /// <summary>
    /// 读取场景配置数据信息
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
	public GameScene Read(byte[] bytes)
	{
		this.sceneLoadUnitTick = 0;
		this._terrainConfig = new TerrainConfig();
		MemoryStream memoryStream = new MemoryStream(bytes);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		long position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 999999)
		{
			long num = binaryReader.ReadInt64();
			byte[] buffer = binaryReader.ReadBytes((int)num);
			MemoryStream memoryStream2 = new MemoryStream(buffer);
			MemoryStream memoryStream3 = new MemoryStream();
			StreamZip.Unzip(memoryStream2, memoryStream3);
			memoryStream = memoryStream3;
			memoryStream2.Dispose();
			binaryReader.Close();
			binaryReader = new BinaryReader(memoryStream);
			binaryReader.BaseStream.Position = 0L;
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		this.sceneID = binaryReader.ReadInt32();
		if (QualitySettings.vSyncCount == 1)
		{
			Application.targetFrameRate = 60;
		}
		else
		{
			Application.targetFrameRate = this.targetFrameRate;
		}
		this._terrainConfig.regionSize = binaryReader.ReadInt32();
		this._terrainConfig.tileSize = binaryReader.ReadInt32();
		this._terrainConfig.defaultTerrainHeight = binaryReader.ReadSingle();
		this._terrainConfig.maxTerrainHeight = binaryReader.ReadSingle();
		this._terrainConfig.cameraLookAt.x = binaryReader.ReadSingle();
		this._terrainConfig.cameraLookAt.y = binaryReader.ReadSingle();
		this._terrainConfig.cameraLookAt.z = binaryReader.ReadSingle();
		this._terrainConfig.cameraDistance = binaryReader.ReadSingle();
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10031)
		{
			this._terrainConfig.cameraRotationX = binaryReader.ReadSingle();
			this._terrainConfig.cameraRotationY = binaryReader.ReadSingle();
			this._terrainConfig.cameraRotationZ = binaryReader.ReadSingle();
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
        //根据品质，选择是否启用全局雾
		if (binaryReader.ReadInt32() == 10032)
		{
			RenderSettings.fog = binaryReader.ReadBoolean();
			int qualityLevel = QualitySettings.GetQualityLevel();
			if (qualityLevel == 1 || qualityLevel == 0)
			{
				RenderSettings.fog = false;
			}
			else
			{
				RenderSettings.fog = true;
			}
			RenderSettings.fogMode = (FogMode)binaryReader.ReadInt32();
			RenderSettings.fogColor = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			RenderSettings.fogDensity = binaryReader.ReadSingle();
			RenderSettings.fogStartDistance = binaryReader.ReadSingle();
			RenderSettings.fogEndDistance = binaryReader.ReadSingle();
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
        //读取天空盒配置
		if (binaryReader.ReadInt32() == 10012)
		{
			if (binaryReader.ReadInt32() == 1)
			{
				string path = binaryReader.ReadString();
				RenderSettings.skybox = (Resources.Load(path, typeof(Material)) as Material);
			}
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 13001)
		{
			this._terrainConfig.enableTerrain = binaryReader.ReadBoolean();
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10041)
		{
			this._terrainConfig.weather = binaryReader.ReadInt32();
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		RenderSettings.ambientLight = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
		RenderSettings.ambientLight = new Color(1f, 1f, 1f, 1f);
		this.sunLightObj = new GameObject();
		Light light = this.sunLightObj.AddComponent<Light>();
		this.sunLightObj.name = "sunLight";
		light.type = LightType.Directional;
		light.color = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
		light.transform.rotation = new Quaternion(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
		light.intensity = binaryReader.ReadSingle();
		light.shadows = LightShadows.Soft;
		light.shadowStrength = binaryReader.ReadSingle();
		light.shadowBias = binaryReader.ReadSingle();
		light.shadowSoftness = binaryReader.ReadSingle();
		light.shadowSoftnessFade = binaryReader.ReadSingle();
		light.renderMode = LightRenderMode.ForcePixel;
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10010)
		{
			this._terrainConfig.waveScale = binaryReader.ReadSingle();
			this._terrainConfig.waveSpeed = new Vector4(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			this._terrainConfig.horizonColor = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			this._terrainConfig.defaultWaterHeight = binaryReader.ReadSingle();
			this._terrainConfig.waterVisibleDepth = binaryReader.ReadSingle();
			this._terrainConfig.waterDiffValue = binaryReader.ReadSingle();
			this._terrainConfig.colorControl = AssetLibrary.Load(binaryReader.ReadString(), AssetType.Texture2D, LoadType.Type_Resources).texture2D;
			this._terrainConfig.waterBumpMap = AssetLibrary.Load(binaryReader.ReadString(), AssetType.Texture2D, LoadType.Type_Resources).texture2D;
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10017)
		{
			this._terrainConfig.waterSpecRange = binaryReader.ReadSingle();
			this._terrainConfig.waterSpecStrength = binaryReader.ReadSingle();
			this._terrainConfig.sunLightDir = new Vector4(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		this._terrainConfig.fogColor = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
		this._terrainConfig.startDistance = binaryReader.ReadSingle();
		this._terrainConfig.globalDensity = binaryReader.ReadSingle();
		this._terrainConfig.heightScale = binaryReader.ReadSingle();
		this._terrainConfig.height = binaryReader.ReadSingle();
		this._terrainConfig.fogIntensity.x = binaryReader.ReadSingle();
		this._terrainConfig.fogIntensity.y = binaryReader.ReadSingle();
		this._terrainConfig.fogIntensity.z = binaryReader.ReadSingle();
		this._terrainConfig.fogIntensity.w = binaryReader.ReadSingle();
		Shader.SetGlobalColor("_FogColor", this._terrainConfig.fogColor);
		Shader.SetGlobalVector("_FogParam", new Vector4(this._terrainConfig.startDistance, this._terrainConfig.globalDensity, this._terrainConfig.heightScale, this._terrainConfig.height));
		Shader.SetGlobalVector("_FogIntensity", this._terrainConfig.fogIntensity);
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10021)
		{
			this._terrainConfig.pointLightRangeMax = binaryReader.ReadSingle();
			this._terrainConfig.pointLightRangeMin = binaryReader.ReadSingle();
			this._terrainConfig.pointLightIntensity = binaryReader.ReadSingle();
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10051)
		{
			this._terrainConfig.enablePointLight = binaryReader.ReadBoolean();
			this._terrainConfig.enablePointLight = false;
			this._terrainConfig.pointLightColor = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10022)
		{
			this._terrainConfig.rolePointLightPostion = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			this._terrainConfig.rolePointLightColor = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			this._terrainConfig.rolePointLightRange = binaryReader.ReadSingle();
			this._terrainConfig.rolePointLightIntensity = binaryReader.ReadSingle();
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		this.peConfig.Clear();
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 70000)
		{
			int num2 = binaryReader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				string key = binaryReader.ReadString();
				int count = binaryReader.ReadInt32();
				byte[] value = binaryReader.ReadBytes(count);
				this.peConfig.Add(key, value);
			}
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 70001)
		{
			this._terrainConfig.coolColor = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
			this._terrainConfig.warmColor = new Color(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		Shader.SetGlobalVector("coolColor", this._terrainConfig.coolColor);
		Shader.SetGlobalVector("warmColor", this._terrainConfig.warmColor);
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10017)
		{
			string text = binaryReader.ReadString();
			if (!text.Contains("Terrain"))
			{
				binaryReader.ReadSingle();
				binaryReader.ReadSingle();
				binaryReader.ReadSingle();
				binaryReader.ReadSingle();
				binaryReader.ReadSingle();
				binaryReader.ReadSingle();
				binaryReader.ReadSingle();
				binaryReader.ReadInt32();
			}
			else
			{
				binaryReader.BaseStream.Position = position;
				binaryReader.ReadInt32();
			}
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		binaryReader.ReadString();
		binaryReader.ReadSingle();
		binaryReader.ReadSingle();
		binaryReader.ReadSingle();
		binaryReader.ReadSingle();
		this._terrainConfig.tileCountPerSide = this._terrainConfig.regionSize / this._terrainConfig.tileSize;
		this._terrainConfig.tileCountPerRegion = this._terrainConfig.tileCountPerSide * this._terrainConfig.tileCountPerSide;
		this.unitIdCount = binaryReader.ReadInt32();
		this._terrainConfig.tileCullingDistance = binaryReader.ReadSingle();
		this._terrainConfig.unitCullingDistance = binaryReader.ReadSingle();
		this._terrainConfig.cullingBaseDistance = binaryReader.ReadSingle();
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10018)
		{
			this._terrainConfig.dynamiCullingDistance = binaryReader.ReadSingle();
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10023)
		{
			this._terrainConfig.cullingAngleFactor = binaryReader.ReadSingle();
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10024)
		{
			this._terrainConfig.viewAngleLodFactor = binaryReader.ReadSingle();
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		if (this._terrainConfig.cullingAngleFactor < 0.001f)
		{
			this._terrainConfig.cullingAngleFactor = 3f;
		}
		if (GameScene.isPlaying)
		{
			this._terrainConfig.tileCullingDistance = 75f;
			this._terrainConfig.cullingBaseDistance = 20.5f;
			this._terrainConfig.unitCullingDistance = 0.7f;
			this._terrainConfig.dynamiCullingDistance = 16f;
		}
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 10003)
		{
			MapPath mapPath = new MapPath(this._terrainConfig.gridSize, 0, 0);
			mapPath.Read(binaryReader);
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 20003)
		{
			int num3 = binaryReader.ReadInt32();
			for (int i = 0; i < num3; i++)
			{
				this.preloadUnitAssetTypeList.Add(binaryReader.ReadInt32());
				this.preloadUnitAssetPathList.Add(binaryReader.ReadString());
			}
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		position = binaryReader.BaseStream.Position;
		if (binaryReader.ReadInt32() == 20004)
		{
			int num4 = binaryReader.ReadInt32();
			for (int i = 0; i < num4; i++)
			{
				this.preloadUnitAssetTypeList.Add(3);
				this.preloadUnitAssetPathList.Add(binaryReader.ReadString());
			}
		}
		else
		{
			binaryReader.BaseStream.Position = position;
		}
		int num5 = binaryReader.ReadInt32();
		position = binaryReader.BaseStream.Position;
		if (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
		{
			if (binaryReader.ReadInt32() == 10008)
			{
				binaryReader.ReadInt32();
			}
			else
			{
				binaryReader.BaseStream.Position = position;
			}
		}
		this.lightmapCount = num5;
		for (int i = 0; i < num5; i++)
		{
			binaryReader.ReadString();
		}
		if (num5 > 0 && !GameScene.isPlaying)
		{
			light.enabled = false;
		}
		if (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
		{
			position = binaryReader.BaseStream.Position;
			if (binaryReader.ReadInt32() == 20011)
			{
				int num6 = binaryReader.ReadInt32();
				int num7 = binaryReader.ReadInt32();
				if (this.heights == null)
				{
					this.heights = new float[num6, num7];
				}
				for (int i = 0; i < num6; i++)
				{
					for (int j = 0; j < num7; j++)
					{
						float num8 = binaryReader.ReadSingle();
						this.heights[i, j] = num8;
					}
				}
			}
			else
			{
				binaryReader.BaseStream.Position = position;
			}
		}
        //读取地形高度信息数据
		this.terrainData = new global::TerrainData(this._terrainConfig.sceneWidth, this._terrainConfig.sceneHeight, 4, this._terrainConfig.maxTerrainHeight, this._terrainConfig.defaultTerrainHeight);
		if (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
		{
			position = binaryReader.BaseStream.Position;
			if (binaryReader.ReadInt32() == 20005)
			{
				int num9 = binaryReader.ReadInt32();
				int num10 = binaryReader.ReadInt32();
				for (int i = 0; i < num9; i++)
				{
					for (int j = 0; j < num10; j++)
					{
						float num11 = binaryReader.ReadSingle();
						this.terrainData.heightmap[i, j] = num11;
					}
				}
			}
			else
			{
				binaryReader.BaseStream.Position = position;
			}
		}
		if (GameScene.isPlaying)
		{
			this.terrainData.Release();
			this.terrainData = null;
		}
		if (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
		{
			position = binaryReader.BaseStream.Position;
			if (binaryReader.ReadInt32() == 10005)
			{
				int num12 = binaryReader.ReadInt32();
				int num13 = binaryReader.ReadInt32();
				if (GameScene._mapPath == null)
				{
					GameScene._mapPath = new MapPath(this._terrainConfig.gridSize * 0.5f, num12 * 2, num13 * 2);
				}
				MapPath mapPath2 = null;
				if (Application.isEditor)
				{
					mapPath2 = new MapPath(this._terrainConfig.gridSize, num12, num13);
				}
				for (int i = 0; i < num12; i++)
				{
					for (int j = 0; j < num13; j++)
					{
						int num14 = i * 2;
						int num15 = j * 2;
						int num16 = binaryReader.ReadInt32();
						GameScene._mapPath.grids[num14, num15] = num16;
						GameScene._mapPath.grids[num14 + 1, num15] = num16;
						GameScene._mapPath.grids[num14, num15 + 1] = num16;
						GameScene._mapPath.grids[num14 + 1, num15 + 1] = num16;
						if (mapPath2 != null)
						{
							mapPath2.grids[i, j] = num16;
						}
					}
				}
				if (mapPath2 != null)
				{
					this.saveMapPath = mapPath2;
				}
				this.detailGridSize = this._terrainConfig.gridSize * 0.5f;
			}
			else
			{
				binaryReader.BaseStream.Position = position;
			}
		}
		binaryReader.Close();
		memoryStream.Flush();
		Asset asset = AssetLibrary.Load("Scenes/" + this.sceneID + "/Prefabs/Root", AssetType.GameObject, LoadType.Type_Resources);
		if (asset != null)
		{
			GameObject gameObject = asset.gameObject;
			if (gameObject != null)
			{
				GameScene.staticInsRoot = (UnityEngine.Object.Instantiate(gameObject) as GameObject);
			}
			else
			{
				LogSystem.LogWarning(new object[]
				{
					"@@@ GameObject.Instantiate Root is null:" + this.sceneID
				});
			}
			if (GameScene.staticInsRoot == null)
			{
				LogSystem.LogWarning(new object[]
				{
					"@@@ staticInsRoot is null:" + this.sceneID
				});
			}
		}
		else
		{
			LogSystem.LogWarning(new object[]
			{
				"@@@ AssetLibrary.Load Root is null:" + this.sceneID
			});
		}
		Asset asset2 = AssetLibrary.Load("Textures/Terrain", AssetType.AssetBundle, LoadType.Type_Auto);
		if (asset2.loader is ResourceLoader)
		{
			this.loadTerrainTextureComplate = true;
		}
		else if (asset2.loader is AssetBundleLoader)
		{
			asset2.loadedListener = new Asset.LoadedListener(this.LoadedTerTex);
		}
		GameScene.mainScene = this;
		this.readCompalte = true;
		return this;
	}
    /// <summary>
    /// 地形贴图资源加载完成回调
    /// </summary>
    /// <param name="asset"></param>
	public void LoadedTerTex(Asset asset)
	{
		this.loadTerrainTextureComplate = true;
	}

	public void RequestPaths(Vector3 startPoint, Vector3 endPoint, int collisionSize, out List<Vector3> paths)
	{
		if (this.mapPath != null)
		{
			this.mapPath.RequestPaths(startPoint, endPoint, collisionSize, out paths);
		}
		else
		{
			paths = new List<Vector3>();
		}
	}
    /// <summary>
    /// 刷新shader全局常量   --有待研究
    /// </summary>
	public void UpdateShaderConstant()
	{
		if (GameScene.lightmapCorrectOn != this._oldLightmapCorrectOn)
		{
			if (GameScene.lightmapCorrectOn)
			{
				Shader.EnableKeyword("LightmapCorrectOn");
				Shader.DisableKeyword("LightmapCorrectOff");
			}
			else
			{
				Shader.DisableKeyword("LightmapCorrectOn");
				Shader.EnableKeyword("LightmapCorrectOff");
			}
			this._oldLightmapCorrectOn = GameScene.lightmapCorrectOn;
		}
		Shader.SetGlobalVector("coolColor", this._terrainConfig.coolColor);
		Shader.SetGlobalVector("warmColor", this._terrainConfig.warmColor);
		this.worldSpaceLightPosition = this._terrainConfig.rolePointLightPostion + this.eyePos;
		this.worldSpaceLightPosition.w = 1f;
		Shader.SetGlobalVector("_worldSpaceLightPosition", this.worldSpaceLightPosition);
		if (Camera.main != null)
		{
			Shader.SetGlobalVector("_worldSpaceViewPos", Camera.main.transform.position);
		}
		Shader.SetGlobalVector("_lightColor", this._terrainConfig.rolePointLightColor);
		Shader.SetGlobalFloat("_lightRange", this._terrainConfig.rolePointLightRange);
		Shader.SetGlobalFloat("_lightIntensity", this._terrainConfig.rolePointLightIntensity * 100f);
	}
    /// <summary>
    /// 开始预加载需要加载的资源
    /// </summary>
	private void Preoad()
	{
		int num = 0;
		int count = this.preloadUnitAssetPathList.Count;
		for (int i = this.loadIndex; i < count; i++)
		{
			AssetType type = (AssetType)this.preloadUnitAssetTypeList[i];
			Asset item = AssetLibrary.Load(this.preloadUnitAssetPathList[i], type, LoadType.Type_Resources);
			this.preloadUnitAssetList.Add(item);
			num++;
			if (num >= this.preloadMaxCountPer) //一次预加载有最多数量限制
			{
				this.loadIndex = i + 1;
				return;
			}
		}
	}
    /// <summary>
    /// 检测是否所有待预加载的资源都加载完成
    /// </summary>
    /// <returns></returns>
	private bool CheckPreLoadComplate()
	{
		if (this.preloadUnitAssetList.Count < this.preloadUnitAssetPathList.Count)
		{
			return false;
		}
		int count = this.preloadUnitAssetList.Count;
		for (int i = 0; i < count; i++)
		{
			if (!this.preloadUnitAssetList[i].loaded)
			{
				return false;
			}
		}
		this.preloadComplate = true;
		return true;
	}
    /// <summary>
    /// 加载设置光照贴图
    /// </summary>
	public void LoadLightmap()
	{
		List<LightmapData> list = new List<LightmapData>();
		for (int i = 0; i < this.lightmapCount; i++)
		{
			string path = string.Concat(new object[]
			{
				"Scenes/",
				this.sceneID,
				"/lightmap/LightmapFar-",
				i
			});
			LightmapData lightmapData = new LightmapData();
			Asset asset;
			if (!this.loadFromAssetBund)
			{
				asset = AssetLibrary.Load(path, AssetType.Texture2D, LoadType.Type_Resources);
			}
			else
			{
				asset = AssetLibrary.Load(path, AssetType.Texture2D, LoadType.Type_AssetBundle);
			}
			if (asset.texture2D != null)
			{
				if (GameScene.lightmapCorrectOn)
				{
					LightmapCorrection.mapsCount++;
					lightmapData.lightmapFar = LightmapCorrection.Bake(asset.texture2D, 1024);
					AssetLibrary.RemoveAsset(path);
				}
				else
				{
					lightmapData.lightmapFar = asset.texture2D;
				}
				list.Add(lightmapData);
			}
		}
		LightmapSettings.lightmapsMode = LightmapsMode.Single;
		if (GameScene.lightmapCorrectOn)
		{
			LightmapCorrection.Clear();
		}
		LightmapData[] lightmaps = list.ToArray();
		LightmapSettings.lightmaps = lightmaps;
	}
    /// <summary>
    /// 预加载完成进度比率获取
    /// </summary>
    /// <returns></returns>
	private float PreLoadProgress()
	{
		int count = this.preloadUnitAssetList.Count;
		this.curPreLoadCount = 0;
		for (int i = 0; i < count; i++)
		{
			if (this.preloadUnitAssetList[i].loaded)
			{
				this.curPreLoadCount++;
			}
		}
		return (float)this.curPreLoadCount / (float)this.preloadUnitAssetPathList.Count;
	}
    /// <summary>
    /// 根据视点更新视图
    /// </summary>
    /// <param name="eyePos"></param>
	public void UpdateView(Vector3 eyePos)
	{
        //等待需要销毁，不进行更新视图逻辑
		if (this.destroyed)
		{
			return;
		}
        //场景运行中，如果场景数据信息读取未完成，不进行视图更新逻辑
		if (GameScene.isPlaying && !this.readCompalte)
		{
			return;
		}
        //地形贴图资源没有加载完成，不进行视图更新逻辑
		if (!this.loadTerrainTextureComplate)
		{
			return;
		}
		if (this.mainCamera == null)
		{
            //获取主摄像机相关信息保存
			this.mainCamera = Camera.main;
			this.mainCamera.backgroundColor = RenderSettings.fogColor;
			if (GameScene.mainCameraCullingMask < 0)
			{
				GameScene.mainCameraCullingMask = this.mainCamera.cullingMask; //临时保存遮挡剔除
			}
			this.mainCamera.farClipPlane = 160f;
		}
        //未获取到主摄像机不往下进行视图更新逻辑
		if (this.mainCamera == null)
		{
			return;
		}
		if (GameScene.isPlaying && !this.loadSceneComplate)
		{
            //没有加载完成场景，场景隐藏，颜色使用雾效颜色
			this.mainCamera.cullingMask = 0;
		}
        //场景没有加载完成，使用高帧率，完成后改为正常帧率
		if (!this.loadSceneComplate)
		{
			Application.targetFrameRate = 50;
		}
		else
		{
			Application.targetFrameRate = this.targetFrameRate;
		}
        //如果不需要预加载或者已经预加载完成
		if (!this.needPreload || this.preloadComplate)
		{
            //场景数据未加载完成
			if (!this.loadSceneComplate)
			{
                //如果保存的场景加载tick计数小于场景加载单位总tick数
				if (this.oldSceneLoadUnitTick < this.sceneLoadUnitTick)
				{
                    //加载进度信息更新
					float num = this.preloadMaxProgress + (float)this.sceneLoadUnitTick / (float)this.sceneMaxLoadUnitTick;
					if (this.loadProgress <= num)
					{
						this.loadProgress += 0.01f;
					}
					if (this.loadProgress > 1f)
					{
						this.loadProgress = 1f;
					}
					try
					{
						if (this.sceneLoadingListener != null)
						{
							this.sceneLoadingListener();
						}
					}
					catch (Exception ex)
					{
						LogSystem.LogError(new object[]
						{
							"场景加载中回调错误!错误内容:" + ex.ToString()
						});
					}
                    //保存记录场景加载unit计数tick
					this.oldSceneLoadUnitTick = this.sceneLoadUnitTick;
				}
				if (this.tick > this.sceneLoadUnitTick + 10)  //unit加载计数超过需加载数10以上
				{
					if (this.loadProgress < 1f) //加载进度没有达到100%
					{
						this.loadProgress += 0.02f;
					}
					else if (this.loadTerrainTextureComplate)  //如果达到100%，并且地形贴图加载完成
					{
						if (this.loadComplateWaitTick == 2)  //加载完成等待两个tick计数，进行资源回收
						{
							Resources.UnloadUnusedAssets();
							GC.Collect();
						}
						this.loadComplateWaitTick++;
						this.mainCamera.cullingMask = GameScene.mainCameraCullingMask; //恢复场景视图显示，但是此时有雾效
						if (this.loadComplateWaitTick > 10)    //加载完成等待超过10个tick计数
						{
							this.loadSceneComplate = true;             //判定场景加载完成
							try
							{
								this.loadProgress = 1f;
								if (this.sceneLoadCompleListener != null)
								{
									this.sceneLoadCompleListener();          //场景加载完成回调
								}
							}
							catch (Exception ex2)
							{
								LogSystem.LogError(new object[]
								{
									"场景加载完毕回调错误! 错误信息: " + ex2.ToString()
								});
							}
						}
					}
				}
			}
			GameScene.isPlaying = Application.isPlaying;  //游戏场景开始运行
			if (GameScene.isPlaying)
			{
				this.time += 0.002f;   //为什么是固定，因为专用于是对水体??????
			}
			eyePos.y = this.SampleHeight(eyePos, true);
			this.viewDir = eyePos - this.mainCamera.transform.position;
			this.viewDir.Normalize();
			if (!this.peConfigLoaded)
			{
				this.LoadPostEffectConfig();         //加载后期处理效果
			}
			this.UpdateShaderConstant();             //更新shader恒变量
			if (GameScene.isPlaying)
			{
				this.frames++;
				float num2 = Time.time;
				if (num2 > this.lastInterval + this.updateInterval)    //场景运行时，按更新时间间隔进行fps刷新
				{
					this.fps = (float)this.frames / (num2 - this.lastInterval);
					this.ms = 1000f / Mathf.Max(this.fps, 1E-05f);
					this.frames = 0;
					this.lastInterval = num2;
					if (this.fps < 5f)
					{
						this.fps = 5f;
					}
				}
			}
			else
			{
				this.fps = 30f;  //场景没有运行，fps默认30
			}
			if (this.tick == 0)   //计时开始，进行光照贴图加载，及相关参数初始化
			{
				this.LoadLightmap();
				this._oldLightmapCorrectOn = !GameScene.lightmapCorrectOn;
				this.lastInterval = Time.time;
				this.FirstRun();
				this.UpdateRegions();
				if (GameScene.isPlaying)
				{
					GameScene.dontCullUnit = false;
				}
			}
			if (this.mapPath != null)
			{
				this.mapPath.Update();
			}
			if (this.tick % 5 == 0)   //更新后期效果启用状态
			{
				if (GameScene.isEditor)
				{
					this.ActivePostEffect(GameScene.postEffectEnable);
				}
				else if (GameScene.postEffectEnable)
				{
					this.ActivePostEffect(true);
				}
				else
				{
					this.ActivePostEffect(false);
				}
			}
			this.eyePos = eyePos;
			this.UpdateTiles();
			this.UpdateUnits();
			if (GameScene.isPlaying)
			{
				if (this.tick % 5 == 0 && Vector3.Distance(eyePos, this.lastPos) > 6f)     //游戏运行时，五次tick并且视点相比上次移动超过一定距离，更新Region列表信息 ,优化性能
				{
					Tile tileAt = this.GetTileAt(eyePos);                                  //获取视点当前所在tile
					if (tileAt != this.curTile)
					{
						this.UpdateRegions();
						this.curTile = tileAt;
					}                                                                      //记录当前的视点位置
					this.lastPos = eyePos;
				}
			}
			else
			{
				this.UpdateRegions();
			}
			if (GameScene.isPlaying && this.loadSceneComplate && this.tick % 10 == 0)
			{
                //检查当前场景tile瓦片地形没有bake的进行bake处理
				this.terBakeCount = 0;
				while (this.terBakeCount < this.perTerBakeCount)
				{
					for (int i = 0; i < this.tiles.Count; i++)
					{
                        //tile的地形渲染已启用，地形贴图索引为空，视距小于指定距离，需进行bake
						if (this.tiles[i].terrain != null && this.tiles[i].terrain.terrainRenderer.enabled && this.tiles[i].terrain.terrainMapIndex < 0 && this.tiles[i].viewDistance < 33f)
						{
							this.tiles[i].terrain.Bake();
							break;
						}
					}
					this.terBakeCount++;
				}
			}
			this.tick++;
			return;
		}
        //需要加载并且没有加载完成，才进行往下进行  ,以下都是加载
		if (this.preloadTick == 0)     //首个tick,计算预加载所需的总tick数
		{
			if (this.preloadUnitAssetPathList.Count < 1)
			{
				this.preloadComplate = true;
				return;
			}
			this.preloadMaxTick = this.preloadInterval * this.preloadUnitAssetPathList.Count;
		}
		float num3 = this.PreLoadProgress() * this.preloadMaxProgress;
		if (this.loadProgress < num3)
		{
			this.loadProgress += this.progressInc;  //显示进度增长是逐步完成的
			return;
		}
		this.progressInc = 0.01f;                   //显示进度追上预加载进度，进行下一批预加载
		if (this.preloadTick % this.preloadInterval == 0) //一定间隔进行一次预加载
		{
			this.Preoad();
		}
		this.preloadTick++;
		if (this.preloadTick % 2 == 0)              //一定次数tick检测以下预加载是否完成
		{
			this.CheckPreLoadComplate();
		}
		if (this.preloadTick > this.preloadMaxTick) //如果tick达到预加载的最大tick数，默认夹杂完成
		{
			this.preloadComplate = true;
		}
	}
    /// <summary>
    /// 重置加载状态
    /// </summary>
	public void ResetLoad()
	{
		this.loadSceneComplate = false;
		this.tick = 0;
		this.sceneLoadUnitTick = 0;
		this.loadProgress = 0f;
		this.oldSceneLoadUnitTick = 0;
	}
    /// <summary>
    /// 根据视点和视距更新当前场景需要的Region网格列表 ，视点所在Region为网格中心
    /// </summary>
	private void UpdateRegions()
	{
		float num = Mathf.Abs(this.eyePos.x);
		float num2 = Mathf.Abs(this.eyePos.z);
		float num3 = num / this.eyePos.x;     //符号，+/-
		float num4 = num2 / this.eyePos.z;    //符号，+/-
        //如果视点在边界点，则使用边界Region值
		if (this.eyePos.x == 0f)
		{
			this.curRegionX = 0;
		}
		else
		{
			this.curRegionX = (int)(Mathf.Ceil(num / (float)this._terrainConfig.regionSize) * num3);
		}
		if (this.eyePos.z == 0f)
		{
			this.curRegionY = 0;
		}
		else
		{
			this.curRegionY = (int)(Mathf.Ceil(num2 / (float)this._terrainConfig.regionSize) * num4);
		}
		int num5 = this.curRegionX - this.viewRect;
		int num6 = this.curRegionX + this.viewRect;
		int num7 = this.curRegionY - this.viewRect;
		int num8 = this.curRegionY + this.viewRect;
		num5 = -1;
		num7 = -1;
		num6 = 1;
		num8 = 1;
		if (!GameScene.dontCullUnit) //如果场景没有剔除unit，清空Region列表缓存
		{
			this.regions.Clear();
		}
		for (int i = num5; i <= num6; i++)      //遍历当前视域网格
		{
			for (int j = num7; j <= num8; j++)
			{
				Region region = null;
				bool flag = this.regionsMap.ContainsKey(i + "_" + j);    //检查Region映射有无指定Region缓存
				if (flag)
				{
					region = this.regionsMap[i + "_" + j];                 //有就直接获取
				}
				if (region == null)                                        //如果映射中没有,创建
				{
					string path = string.Empty;
                    //加载资源
					Asset asset;
					if (!GameScene.isPlaying)
					{
						path = string.Concat(new object[]
						{
							"Scenes/",
							this.sceneID,
							"/",
							i,
							"_",
							j,
							"/Region"
						});
						asset = AssetLibrary.Load(path, AssetType.Region, LoadType.Type_Resources);
					}
					else
					{
						path = string.Concat(new object[]
						{
							"Scenes/",
							this.sceneID,
							"/",
							i,
							"_",
							j,
							"/Region"
						});
						if (this.loadFromAssetBund)
						{
							asset = AssetLibrary.Load(path, AssetType.Region, LoadType.Type_AssetBundle);
						}
						else
						{
							asset = AssetLibrary.Load(path, AssetType.Region, LoadType.Type_Resources);
						}
					}
                    //将Region加入队列和映射引用中
					if (asset.loaded)
					{
						region = asset.region;
						this.regions.Add(region);
						this.regionsMap.Add(region.regionX + "_" + region.regionY, region);
					}
					else if (!GameScene.isPlaying)
					{
						region = Region.Create(this, i, j);
						this.regions.Add(region);
						this.regionsMap.Add(region.regionX + "_" + region.regionY, region);
					}
				}
				else if (GameScene.dontCullUnit)  //映射中已有引用，如果不剔除unit
				{
					if (!flag)                    //映射中没有指定region引用，加入队列,意思是如果不剔除，所有的Region都要添加
					{
						this.regions.Add(region);
					}
				}
				else            //映射中有索引，如果剔除unit，并且映射中没有引用指定region，网格中的region需要添加到队列
				{
					this.regions.Add(region);
				}
			}
		}
		if (!GameScene.dontCullUnit)   //如果需要剔除，清空映射容器 ,也就是说剔除unit模式情况下，实际只在内存中保存X宫格的所有region的引用
		{
			this.regionsMap.Clear();
		}
        //将region队列中的region都加入region映射中
		for (int i = 0; i < this.regions.Count; i++)
		{
			this.regKey = this.regions[i].regionX + "_" + this.regions[i].regionY;
			if (!this.regionsMap.ContainsKey(this.regKey))
			{
				this.regionsMap.Add(this.regKey, this.regions[i]);
			}
		}
		int count = this.regions.Count;
		for (int i = 0; i < count; i++)
		{
			this.regions[i].Update(this.eyePos);   //遍历更新X宫格中的所有Region逻辑
		}
	}
    /// <summary>
    /// 更新视图属性，逐级刷新跟剔除激活相关的属性   Region -> tile  ->unit ->dynamicUnit
    /// </summary>
	public void UpdateViewRange()
	{
        //更新X宫格所有的Region视图中tile视野更新
		for (int i = 0; i < this.regions.Count; i++)
		{
			this.regions[i].UpdateViewRange();
		}
        //更新当前Region中，角色视域中的unit的激活距离和剔除距离
		for (int i = 0; i < this.units.Count; i++)
		{
			if (this.units[i].isStatic)
			{
				this.units[i].UpdateViewRange();
			}
		}
        //更新动态unit的激活距离和剔除距离
		for (int j = 0; j < this.dynamicUnits.Count; j++)
		{
			GameObjectUnit gameObjectUnit = this.dynamicUnits[j];
			gameObjectUnit.near = this.terrainConfig.dynamiCullingDistance;
			gameObjectUnit.far = gameObjectUnit.near + 2f;
		}
	}
    /// <summary>
    /// 更新tile列表
    /// </summary>
	private void UpdateTiles()
	{
		int num = this.tiles.Count;
		if (this.tick % 1 == 0)
		{
			this.visTileCount = 0;
		}
		for (int i = 0; i < num; i++)
		{
			this.dx = this.eyePos.x - this.tiles[i].position.x;
			this.dz = this.eyePos.z - this.tiles[i].position.z;
			this.tiles[i].viewDistance = Mathf.Sqrt(this.dx * this.dx + this.dz * this.dz);  //更新视距
			if (this.tiles[i].terrain != null)
			{
				if (this.tiles[i].viewDistance > 30f)   //tile视距超过一定距离关闭接收阴影,优化性能
				{
					this.tiles[i].terrain.WithoutShadow();
				}
				else
				{
					this.tiles[i].terrain.ReceiveShadow();
				}
			}
			if (this.tiles[i].viewDistance > 39f && this.tiles[i].terrain != null)  //tile视距超过一定值，可以取消光照贴图效果，优化性能
			{
				this.tiles[i].terrain.CancelBake();
			}
			if (this.tiles[i].viewDistance > this.tiles[i].far + 8f)   //tile视距比剔除距离大一定值，从场景移除tile，但是不销毁
			{
				if (!GameScene.dontCullUnit)
				{
					this.RemoveTile(this.tiles[i], false);
					num--;
					i--;
				}
			}
			else
			{
				this.tiles[i].Update(this.eyePos);
				if (!this.tiles[i].visible && this.visTileCount < this.visibleTilePerFrame)  //如果可视tile数小于每帧可显示tile上限，显示该tile
				{
					this.tiles[i].Visible();
					this.visTileCount++;
				}
			}
		}
	}
    /// <summary>
    /// 收集当前静态unit列表的资源路径列表
    /// </summary>
    /// <returns></returns>
	public List<string> CollectStaticUnitAssetPath()
	{
		List<string> list = new List<string>();
		int count = this.units.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.units[i].isStatic && !list.Contains(this.units[i].prePath) && this.units[i].ins != null)
			{
				list.Add(this.units[i].prePath);
			}
		}
		return list;
	}
    /// <summary>
    /// 收集当前tile列表的地形的splat资源路径
    /// </summary>
    /// <returns></returns>
	public List<string> CollectTerrainSplatsAssetPath()
	{
		List<string> list = new List<string>();
		int count = this.tiles.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.tiles[i].terrain != null)
			{
				Splat[] splats = this.tiles[i].terrain.terrainData.splats;
				for (int j = 0; j < splats.Length; j++)
				{
					if (splats[j] != null && !list.Contains(splats[j].key))
					{
						list.Add(splats[j].key);
					}
				}
			}
		}
		return list;
	}
    /// <summary>
    /// 更新当前unit的各种状态
    /// </summary>
	public void UpdateUnits()
	{
		int num = this.units.Count;
		this.visibleDynamicUnitCount = 0;
		this.visibleStaticUnitCount = 0;
		if (this.tick % 3 == 0)    //每3个tick进行一次初始每帧开启Unit可视计数
		{
			this.visibleDynaUnitPerFrame = 0;
		}
		this.visibleStaticUnitPerFrame = 0;
		this.hideStaticUnitPerFrame = 0;
		this.useMaterialsCount = 0;
		this.materials.Clear();
		this.visibleStaticTypeCount = 0;
		this.staticTypeMap.Clear();
		this.dynamicUnits.Clear();
		GameObjectUnit gameObjectUnit = null;                //--- 筛选的视距最小的unit
		GameObjectUnit gameObjectUnit2 = null;               //----筛选的视距最大的unit
		this.shadowUnits.Clear();
		for (int i = 0; i < num; i++)
		{
			GameObjectUnit gameObjectUnit3 = this.units[i];
			if (!GameScene.isPlaying && gameObjectUnit3.isStatic)   //游戏未运行，准备阶段更新静态unit的影响tile列表，因为静态所以基本不变
			{
				gameObjectUnit3.Update();
			}
			if (gameObjectUnit3.isStatic && (this.hideStaticUnitPerFrame < 1 || this.visibleStaticUnitPerFrame < 1)) //静态Unit,并且当前的隐藏静态unit计数没达到上限 或者可视静态unit计数没达到上限
			{
				this.dx = gameObjectUnit3.center.x - this.eyePos.x;
				this.dz = gameObjectUnit3.center.z - this.eyePos.z;
				gameObjectUnit3.viewDistance = Mathf.Sqrt(this.dx * this.dx + this.dz * this.dz);      //计算视距
                //    this.hideStaticUnitPerFrame < MAX
                if (gameObjectUnit3.visible && this.hideStaticUnitPerFrame < 1 && gameObjectUnit3.viewDistance > gameObjectUnit3.far)   //unit可视，并且unit视距大于剔除距离
				{
					if (!GameScene.dontCullUnit)    //需要剔除，则移除改unit,不destroy也不缓存
					{
						this.RemoveUnit(gameObjectUnit3, false, false);
						num--;
						i--;
						this.hideStaticUnitPerFrame++;
					}
					gameObjectUnit3.active = false;      //不激活unit
				}
                // this.visibleStaticUnitPerFrame  < MAX
                if (!gameObjectUnit3.visible && this.visibleStaticUnitPerFrame < 1 && gameObjectUnit3.viewDistance < gameObjectUnit3.near && (gameObjectUnit3.combineParentUnitID < 0 || GameScene.SampleMode))
				{
                    //不可视，没达到可视上限，视距小于激活距离
					gameObjectUnit3.active = true;     //激活unit
					Vector3 lhs = gameObjectUnit3.center - this.mainCamera.transform.position;
					lhs.Normalize();
					gameObjectUnit3.viewAngle = Mathf.Acos(Vector3.Dot(lhs, this.viewDir)) / gameObjectUnit3.cullingFactor;
					if (gameObjectUnit3.viewAngle < this.terrainConfig.cullingAngleFactor)    //在剔除视域范围内，开启可视
					{
						gameObjectUnit3.Visible();
						this.visibleStaticUnitPerFrame++;
						if (!this.loadSceneComplate) //如果没有加载完成，需要加入场景加载unit计数
						{
							this.sceneLoadUnitTick++;
						}
					}
					this.visibleStaticUnitCount++;
				}
			}
			if (!gameObjectUnit3.isStatic)          //如果是动态unit
			{
				if (gameObjectUnit3.visible)        //可视动态Unit计数
				{
					this.visibleDynamicUnitCount++;
				}
				this.dx = gameObjectUnit3.position.x - this.eyePos.x;
				this.dz = gameObjectUnit3.position.z - this.eyePos.z;
				gameObjectUnit3.viewDistance = Mathf.Sqrt(this.dx * this.dx + this.dz * this.dz);     //计算视距
				if (gameObjectUnit3.position.y > 1E+08f)
				{
					gameObjectUnit3.position.y = this.SampleHeight(gameObjectUnit3.position, true);  //计算高度坐标
				}
                //筛选视距最小的unit
				if (gameObjectUnit == null)
				{
					if (!gameObjectUnit3.visible)    //unit不可视
					{
						gameObjectUnit = gameObjectUnit3;
					}
				}
				else if (!gameObjectUnit3.visible && gameObjectUnit3.viewDistance < gameObjectUnit.viewDistance)  //对比记录的unit视距和当前unit，记录视距更小的unit
				{
					gameObjectUnit = gameObjectUnit3;
				}
                //筛选视距最大的unit的待移除unit
				if (gameObjectUnit3.willRemoved && this.removeDynUnits.Count > 0)  //unit如果待移除
				{
					if (gameObjectUnit2 == null)
					{
						gameObjectUnit2 = gameObjectUnit3;
					}
					else if (gameObjectUnit3.viewDistance > gameObjectUnit2.viewDistance)
					{
						gameObjectUnit2 = gameObjectUnit3;
					}
				}
				this.dynamicUnits.Add(gameObjectUnit3); //加入动态unit列表

                #region shadow unit 
                if (this.shadowUnits.Count < this.maxCastShadowsUnitCount || gameObjectUnit3.isMainUint) //如果当前的投影unit数未达到上限或者当前unit为主角unit，加入阴影unit列表中
				{
					this.shadowUnits.Add(gameObjectUnit3);
				}
				else      //否则，移除阴影unit列表中视距最大unit，加入当前的unit进入阴影unit列表
				{
					float viewDistance = gameObjectUnit3.viewDistance;
					int num2 = -1;
					for (int j = 0; j < this.shadowUnits.Count; j++)
					{
						if (this.shadowUnits[j].viewDistance > viewDistance)
						{
							num2 = j;
							viewDistance = this.shadowUnits[j].viewDistance;
						}
					}
					if (num2 > -1)
					{
						this.shadowUnits.RemoveAt(num2);
						this.shadowUnits.Add(gameObjectUnit3);
					}
				}
                #endregion
                if (gameObjectUnit3.viewDistance > gameObjectUnit3.far)   //unit视距大于剔除距离
				{
					if (gameObjectUnit3.willRemoved)       //如果待移除，则移除该动态unit，destroy并加入缓存
					{
						this.RemoveDynamicUnit(gameObjectUnit3 as DynamicUnit, true, true);
						num--;
					}
					else
					{
						gameObjectUnit3.Invisible();       //否则，进行隐藏
					}
				}
				if (this.mapPath != null && gameObjectUnit3.hasCollision)   //如果unit开启了碰撞 ,设置临时的动态碰撞信息，，这里似乎处理的是静止的动态unit的碰撞
				{
					this.mapPath.SetDynamicCollision(gameObjectUnit3.position, gameObjectUnit3.collisionSize, true, 1); //路径映射中移除状态信息
					gameObjectUnit3.hasCollision = false;
				}
				if (!gameObjectUnit3.willRemoved)   //如果不是待移除的unit
				{
					gameObjectUnit3.Update();       //进行了update更新，如果还在碰撞范围内
                    //如果启用unit动态碰撞网格，并且当前unit的视距小于碰撞计算范围，unit有碰撞体，静态阻塞网格为空，路径映射不为空，需要根据当前unit的当前坐标修改路径映射中当前unit相关的碰撞属性
					if (this.enableDynamicGrid && gameObjectUnit3.viewDistance < this._terrainConfig.collisionComputeRange && this.mapPath != null && gameObjectUnit3.isCollider && gameObjectUnit3.grids == null)
					{
						this.mapPath.SetDynamicCollision(gameObjectUnit3.position, gameObjectUnit3.collisionSize, false, 1);
						gameObjectUnit3.hasCollision = true;
					}
				}
			}
		}


		if (gameObjectUnit != null && gameObjectUnit.viewDistance < gameObjectUnit.near && this.visibleDynaUnitPerFrame < 1) //如果当前帧开启可视unit数量未达上限，开启筛选的视距最小的unit可视
		{
			gameObjectUnit.Visible();
			this.visibleDynaUnitPerFrame++;
		}
		if (gameObjectUnit2 != null)  //移除视距最大的unit
		{
			this.RemoveDynamicUnit(gameObjectUnit2 as DynamicUnit, true, true);
			this.removeDynUnits.Remove(gameObjectUnit2 as DynamicUnit);
		}
		for (int i = 0; i < this.dynamicUnits.Count; i++)   //所有动态unit不接受阴影
		{
			this.dynamicUnits[i].castShadows = false;
		}
		for (int i = 0; i < this.shadowUnits.Count; i++)    //接受投影的unit列表中的所有unit都接受投影
		{
			this.shadowUnits[i].castShadows = true;
		}
	}
    /// <summary>
    /// 移除待移除的动态unit
    /// </summary>
	public void RemvoeWillRemoveDynUnits()
	{
		for (int i = 0; i < this.units.Count; i++)
		{
			GameObjectUnit gameObjectUnit = this.units[i];
			if (!gameObjectUnit.isStatic && gameObjectUnit.willRemoved)
			{
				this.RemoveDynamicUnit(gameObjectUnit as DynamicUnit, true, true);
				this.removeDynUnits.Remove(gameObjectUnit as DynamicUnit);
				i--;
			}
		}
	}
    /// <summary>
    /// 获取触摸点一定范围内的一个可触摸动态unit
    /// </summary>
    /// <param name="touchRange"></param>
    /// <returns></returns>
	public GameObjectUnit GetTouchDynamicUnit(float touchRange = 700f)
	{
		int count = this.dynamicUnits.Count;
		float x = Input.mousePosition.x;
		float y = Input.mousePosition.y;
		for (int i = 0; i < count; i++)
		{
			GameObjectUnit gameObjectUnit = this.dynamicUnits[i];
			if (gameObjectUnit.visible && gameObjectUnit.mouseEnable)
			{
				float num = x - gameObjectUnit.screenPoint.x;
				float num2 = y - gameObjectUnit.screenPoint.y;
				if (num * num + num2 * num2 < touchRange)
				{
					return gameObjectUnit;
				}
			}
		}
		return null;
	}
    /// <summary>
    /// 获取指定位置所处的tile
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
	public Tile GetTileAt(Vector3 worldPosition)
	{
		int num = (int)Mathf.Floor((worldPosition.x + (float)this._terrainConfig.regionSize * 0.5f) / (float)this._terrainConfig.regionSize);
		int num2 = (int)Mathf.Floor((worldPosition.z + (float)this._terrainConfig.regionSize * 0.5f) / (float)this._terrainConfig.regionSize);
		string key = num + "_" + num2;
		Region region = null;
		if (this.regionsMap.ContainsKey(key))
		{
			region = this.regionsMap[key];
		}
		if (region == null)
		{
			return null;
		}
		return region.GetTile(worldPosition);
	}

	public Tile GetNeighborTile(Tile tile, int dirX, int dirY)
	{
		int num = tile.region.regionX;
		int num2 = tile.region.regionY;
		int tileX = tile.tileX;
		int tileY = tile.tileY;
		int num3 = tileX + dirX;
		int num4 = tileY + dirY;
		int num5 = Mathf.FloorToInt((float)this._terrainConfig.tileCountPerSide * 0.5f);
		if (num3 < -num5)
		{
			num--;
			num3 = num5;
		}
		else if (num3 > num5)
		{
			num++;
			num3 = -num5;
		}
		if (num4 < -num5)
		{
			num2--;
			num4 = num5;
		}
		else if (num4 > num5)
		{
			num2++;
			num4 = -num5;
		}
		string key = string.Concat(new object[]
		{
			num,
			"_",
			num2,
			"_",
			num3,
			"_",
			num4
		});
		if (this.tilesMap.ContainsKey(key))
		{
			return this.tilesMap[key];
		}
		return null;
	}
    /// <summary>
    /// 采样获取指定点的高度
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="interpolation">是否是交界处</param>
    /// <returns></returns>
	public float SampleHeight(Vector3 worldPosition, bool interpolation = true)
	{
		if (this.terrainConfig == null) //读不到配置，默认值
		{
			return 1E+09f;
		}
		if (this.heights != null) //高度信息已读取
		{
			int num = (int)this._terrainConfig.sceneHeightmapResolution;
            //这里的意思是说地形网格的坐标原点在中心位置？？？？？？
			float num2 = worldPosition.x + this.terrainConfig.sceneHeightmapResolution * 0.5f;
			float num3 = worldPosition.z + this.terrainConfig.sceneHeightmapResolution * 0.5f;
			float num4 = num2 % 1f;
			float num5 = num3 % 1f;
			int num6 = Mathf.FloorToInt(num2);
			int num7 = Mathf.FloorToInt(num3);
			int num8 = num6 + 1;
			int num9 = num7 + 1;
			if (num6 < 0 || num7 < 0 || num6 >= num || num7 >= num)   //超出地形，使用默认高度
			{
				return this._terrainConfig.defaultTerrainHeight;
			}
			if (num8 < 0 || num9 < 0 || num8 > num || num9 >= num)
			{
				return this._terrainConfig.defaultTerrainHeight;
			}
			float num10 = this.heights[num6, num7] * (1f - num4) + this.heights[num8, num7] * num4;
			float num11 = this.heights[num6, num9] * (1f - num4) + this.heights[num8, num9] * num4;
			return num11 * num5 + num10 * (1f - num5);
		}
		else
		{
            //Region坐标根据瓦片获取高度值????????
			int num12 = (int)Mathf.Floor((worldPosition.x + (float)this._terrainConfig.regionSize * 0.5f) / (float)this._terrainConfig.regionSize);
			int num13 = (int)Mathf.Floor((worldPosition.z + (float)this._terrainConfig.regionSize * 0.5f) / (float)this._terrainConfig.regionSize);
			Region region = null;
			this.smkey = num12 + "_" + num13;
			if (this.regionsMap.ContainsKey(this.smkey))
			{
				region = this.regionsMap[this.smkey];
			}
			if (region == null)
			{
				return 1E+09f;
			}
			Tile tile = region.GetTile(worldPosition);
			if (tile == null)
			{
				LogSystem.Log(new object[]
				{
					"SampleHeight tile is null, position-> " + worldPosition
				});
				return 1E+09f;
			}
			if (interpolation)
			{
				return tile.SampleHeightInterpolation(worldPosition);
			}
			return tile.SampleHeight(worldPosition, 0f);
		}
	}
    /// <summary>
    /// 判断是否位于水下
    /// </summary>
    /// <param name="postion">Region坐标？？？</param>
    /// <returns></returns>
	public bool Underwater(Vector3 postion)
	{
		int num = (int)Mathf.Floor((postion.x + (float)this._terrainConfig.regionSize * 0.5f) / (float)this._terrainConfig.regionSize);
		int num2 = (int)Mathf.Floor((postion.z + (float)this._terrainConfig.regionSize * 0.5f) / (float)this._terrainConfig.regionSize);
		Region region = null;
		string key = num + "_" + num2;
		if (this.regionsMap.ContainsKey(key))//获取坐标所在Region
		{
			region = this.regionsMap[key];
		}
		if (region == null)
		{
			return false;
		}
		Tile tile = region.GetTile(postion); //获取所在的Tile
		if (tile == null || tile.water == null)
		{
			return false;
		}
		if (tile.water.Underwater(postion.y))//tile的水体判断是否在水下
		{
			this.waterHeight = tile.water.waterData.height;
			return true;
		}
		return false;
	}
    /// <summary>
    /// 获取坐标处的地面类型
    /// </summary>
    /// <param name="worldPos"></param>
	public void GetGroundType(Vector3 worldPos)
	{
	}
}
