using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameObjectUnit
{
	public delegate void CreateInsListener();

	public delegate void DestroyInsListener();

	public delegate void ActiveListener(bool value);

	public delegate void ThridPardResourManager(string strFileName, AssetCallBack back);
    /// <summary>
    /// 当前是作为主角的unit
    /// </summary>
	public bool isMainUint;
    /// <summary>
    /// 是否待移除
    /// </summary>
	public bool willRemoved;

	public int lightmapSize = 128;

	public GameObjectUnit.CreateInsListener createInsListener;

	public GameObjectUnit.DestroyInsListener destroyInsListener;

	public GameObjectUnit.ActiveListener activeListener;

	public float randomCode;

	public int createID;

	public string name = string.Empty;
    /// <summary>
    ///   是否有碰撞器
    /// </summary>
	public bool isCollider = true;

	public GameObject ins;

	public bool isStatic = true;
    /// <summary>
    /// 当前unit相关资源的路径
    /// </summary>
	public string prePath = string.Empty;
    /// <summary>
    /// 当前unit的位置坐标
    /// </summary>
	public Vector3 position;
    /// <summary>
    /// 当前unit的方向
    /// </summary>
	public Quaternion rotation;
    /// <summary>
    /// 当前unit的缩放率
    /// </summary>
	public Vector3 localScale;
    /// <summary>
    /// 当前unit的中心点坐标
    /// </summary>
	public Vector3 center;

	public Vector3 size = Vector3.zero;
    /// <summary>
    /// 剔除因子，用来计算剔除距离
    /// </summary>
	public float cullingFactor;
    /// <summary>
    /// 依赖的资源数量
    /// </summary>
	private int dependResCount;

	public bool visible;
    /// <summary>
    /// 近视距离，用来判断是否需要加入场景逻辑
    /// </summary>
	public float near;
    /// <summary>
    /// 剔除距离，当主角到它的视距超过时，需要从场景剔除
    /// </summary>
	public float far;
    /// <summary>
    /// 当前更新的视距
    /// </summary>
	public float viewDistance;
    /// <summary>
    /// 当前更新的视角
    /// </summary>
	public float viewAngle;
    /// <summary>
    /// 当前unit涉及的tile列表
    /// </summary>
	public List<Tile> tiles = new List<Tile>();
    /// <summary>
    /// 当前unit所在的tile
    /// </summary>
	public Tile mainTile;
    /// <summary>
    /// 是否启用碰撞
    /// </summary>
	public bool hasCollision;
    /// <summary>
    /// 预判时使用的步伐半径??????
    /// </summary>
	public float radius = 1f;
    /// <summary>
    /// 启用碰撞时的碰撞尺寸
    /// </summary>
	public int collisionSize = 1;

	private int[,] _grids;
    /// <summary>
    /// 挂载的component组件列表
    /// </summary>
	public List<string> components = new List<string>();
    /// <summary>
    /// 光照贴图原型数据列表
    /// </summary>
	public List<LightmapPrototype> lightmapPrototypes = new List<LightmapPrototype>();

	public bool needScreenPoint = true;
    /// <summary>
    /// 计算的当前unit的摄像机空间坐标
    /// </summary>
	public Vector3 screenPoint;
    /// <summary>
    /// 是否启用触摸功能
    /// </summary>
	public bool mouseEnable = true;
    /// <summary>
    /// unit类型
    /// </summary>
	public int type = UnitType.UnitType_General;
    /// <summary>
    /// unit数据解析器
    /// </summary>
	public UnitParser unitParser;
    /// <summary>
    /// 加载类型
    /// </summary>
	public LoadType loadtype = LoadType.Type_Resources;
    /// <summary>
    /// 是否生成水波纹效果
    /// </summary>
	public bool genRipple;
    /// <summary>
    /// 水波纹生成位置
    /// </summary>
	protected Vector3 ripplePos;
    /// <summary>
    /// 生成水波纹的延迟tick计数
    /// </summary>
	public int genRippleDelayTick = 50;
    /// <summary>
    /// unit当前所属场景引用
    /// </summary>
	public GameScene scene;
    /// <summary>
    /// 游戏对象是否激活
    /// </summary>
	public bool active;
    /// <summary>
    /// unit的世界位置坐标
    /// </summary>
	protected Vector3 scenePoint = Vector3.zero;
    /// <summary>
    /// 场景坐标点偏置量
    /// </summary>
	public float scenePointBias = 1f;
    /// <summary>
    /// tick计数   update调用次数?????
    /// </summary>
	private int tick;
    /// <summary>
    /// 是否需要采样计算高度
    /// </summary>
	public bool needSampleHeight = true;
    /// <summary>
    /// 材质列表
    /// </summary>
	public List<Material> materials = new List<Material>();
    /// <summary>
    /// 组合父unitID
    /// </summary>
	public int combineParentUnitID = -1;
    /// <summary>
    /// 组合子unitID列表
    /// </summary>
	public List<int> combinUnitIDs = new List<int>();
    /// <summary>
    /// 数据是否已读取
    /// </summary>
	private bool readed;
    /// <summary>
    /// unit需读取的数据长度
    /// </summary>
	private long dataLength;
    /// <summary>
    /// 是否已销毁
    /// </summary>
	public bool destroyed;
    /// <summary>
    /// 是否开启投影
    /// </summary>
	public bool castShadows;
    /// <summary>
    /// 之前是否开启投影
    /// </summary>
	protected bool oldCastShadows;
    /// <summary>
    /// unit阻塞的网格数
    /// </summary>
	private int gridCount;
    /// <summary>
    /// unit最大阻塞的网格数量
    /// </summary>
	private int maxGridCount = 200;
    /// <summary>
    /// 观察目标时计算的视线方向
    /// </summary>
	private Vector3 lookAtEuler = new Vector3(0f, 0f, 0f);
    /// <summary>
    /// 计算视线的高度值(观察的y坐标)
    /// </summary>
	public float euler;
    /// <summary>
    /// 标记是否需要更新旋转角度
    /// </summary>
	protected bool _rotationDirty;
    /// <summary>
    /// 缺失资源????????
    /// </summary>
	public bool lostAsset;
    /// <summary>
    /// 当前unit的shader名称
    /// </summary>
	private string shaderName = string.Empty;

	protected static string diffuseShaderName = "Diffuse";

	protected static string diffuseCutoutShaderName = "Transparent/Cutout/Diffuse";

	protected static string diffuseTransparentShaderName = "Transparent/Diffuse";

	protected static string snailDiffuseShaderName = "Snail/Diffuse";

	protected static string snailDiffuseCutoutShaderName = "Snail/Transparent/Cutout/Diffuse";

	protected static string snailDiffusePointShaderName = "Snail/Diffuse-PointLight";

	protected static string snailDiffusePointCutoutShaderName = "Snail/Diffuse-PointLight-Cutout";
    /// <summary>
    /// 漫反射shader，官方版本
    /// </summary>
	protected static Shader diffuseShader = Shader.Find(GameObjectUnit.diffuseShaderName);
    /// <summary>
    /// cutout镂空shader
    /// </summary>
	protected static Shader diffuseCutoutShader = Shader.Find(GameObjectUnit.diffuseCutoutShaderName);
    /// <summary>
    /// 自定义漫反射shader
    /// </summary>
	protected static Shader snailDiffuseShader = Shader.Find(GameObjectUnit.snailDiffuseShaderName);
    /// <summary>
    /// 自定义镂空漫反射shader
    /// </summary>
	protected static Shader snailDiffuseCutoutShader = Shader.Find(GameObjectUnit.snailDiffuseCutoutShaderName);
    /// <summary>
    /// 自定义漫反射点光源shader
    /// </summary>
	protected static Shader snailDiffusePointShader = Shader.Find(GameObjectUnit.snailDiffusePointShaderName);
    /// <summary>
    /// 自定义漫反射点光源镂空shader
    /// </summary>
	protected static Shader snailDiffusePointCutoutShader = Shader.Find(GameObjectUnit.snailDiffusePointCutoutShaderName);
    /// <summary>
    /// 预加载的资源游戏对象
    /// </summary>
	private UnityEngine.Object pre;

	public static GameObjectUnit.ThridPardResourManager thridPardResourManager;
    /// <summary>
    /// 出生效果预加载路径
    /// </summary>
	private string _bornEffectPrePath = string.Empty;
    /// <summary>
    /// 出生效果
    /// </summary>
	protected GameObject bornEffect;
    /// <summary>
    /// unit相关网格阻塞格子坐标数组
    /// </summary>
	public int[,] grids
	{
		get
		{
			return this._grids;
		}
		set
		{
			if (value == null)
			{
				this._grids = null;
			}
			else if (this.scene != null && this.scene.mapPath != null && value != null)
			{
				this._grids = this.scene.mapPath.CheckCustomGrids(value);
			}
			else
			{
				this._grids = value;
			}
		}
	}
    /// <summary>
    /// 获取或者设置出生效果路径，并播放
    /// </summary>
	public string bornEffectPrePath
	{
		get
		{
			return this._bornEffectPrePath;
		}
		set
		{
			this._bornEffectPrePath = value;
			this.PlayBornEffect();
		}
	}
    /// <summary>
    /// 构造一个unit游戏对象
    /// </summary>
    /// <param name="createID"></param>
	public GameObjectUnit(int createID)
	{
		this.createID = createID;
	}
    /// <summary>
    /// 组合unit
    /// </summary>
    /// <param name="units"></param>
	public void CombineUnits(List<GameObjectUnit> units)
	{
		this.combinUnitIDs.Clear();
		for (int i = 0; i < units.Count; i++)
		{
			this.combinUnitIDs.Add(units[i].createID);
			units[i].combineParentUnitID = this.createID;
			units[i].active = false;
			this.scene.RemoveUnit(units[i], false, false);
		}
	}
    /// <summary>
    /// 拆分组合的unit
    /// </summary>
    /// <returns></returns>
	public bool DeCombineUnits()
	{
		if (this.combinUnitIDs.Count < 1)
		{
			return false;
		}
		for (int i = 0; i < this.combinUnitIDs.Count; i++)
		{
			GameObjectUnit gameObjectUnit = GameScene.mainScene.FindUnitInTiles(this.combinUnitIDs[i]);
			if (gameObjectUnit == null)
			{
				return false;
			}
			gameObjectUnit.combineParentUnitID = -1;
		}
		this.combinUnitIDs.Clear();
		return true;
	}
    /// <summary>
    /// 创建unit游戏对象
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="pos"></param>
    /// <param name="createID"></param>
    /// <param name="prePath"></param>
    /// <returns></returns>
	public static GameObjectUnit Create(GameScene scene, Vector3 pos, int createID, string prePath)
	{
		GameObjectUnit gameObjectUnit = new GameObjectUnit(createID);
		gameObjectUnit.scene = scene;
		gameObjectUnit.position = pos;
		gameObjectUnit.center = pos;
		gameObjectUnit.prePath = prePath;
		gameObjectUnit.UpdateViewRange();
		return gameObjectUnit;
	}
    /// <summary>
    /// 读取unit数据信息
    /// </summary>
    /// <param name="br"></param>
    /// <param name="cID"></param>
	public void Read(BinaryReader br, int cID)
	{
		if (this.readed && this.dataLength > 0L)
		{
			br.BaseStream.Position += this.dataLength;
			return;
		}
		long num = 0L;
		this.createID = cID;
		long num2 = br.BaseStream.Position;
		long num3 = num2;
		this.prePath = br.ReadString();
		num2 = br.BaseStream.Position;
		if (br.ReadInt32() == 10009)
		{
			this.type = br.ReadInt32();
			if (!GameScene.isPlaying)
			{
			}
			this.unitParser = UnitType.GenUnitParser(this.type);  //根据不同unit类型，使用对应的解析器解析数据
			this.unitParser.unit = this;
			if (GameScene.isPlaying && this.type == UnitType.UnitType_Light && this.scene.lightDataLength > 0L)
			{
				br.BaseStream.Position += this.scene.lightDataLength;
				return;
			}
			if (this.type == UnitType.UnitType_Light)
			{
				num = br.BaseStream.Position;
			}
			this.unitParser.Read(br);
		}
		else
		{
			br.BaseStream.Position = num2;
			this.unitParser = new UnitParser();
		}
		this.dependResCount = br.ReadInt32();
		if (this.dependResCount != 0)
		{
		}
		for (int i = 0; i < this.dependResCount; i++)
		{
			br.ReadInt32();
		}
		int num4 = br.ReadInt32();
		this.lightmapPrototypes = new List<LightmapPrototype>();
		for (int i = 0; i < num4; i++)
		{
			LightmapPrototype lightmapPrototype = new LightmapPrototype();
			lightmapPrototype.rendererChildIndex = br.ReadInt32();
			lightmapPrototype.lightmapIndex = br.ReadInt32();
			num2 = br.BaseStream.Position;
			if (br.ReadInt32() == 10006)
			{
				lightmapPrototype.scale = br.ReadSingle();
			}
			else
			{
				br.BaseStream.Position = num2;
			}
			lightmapPrototype.lightmapTilingOffset.x = br.ReadSingle();
			lightmapPrototype.lightmapTilingOffset.y = br.ReadSingle();
			lightmapPrototype.lightmapTilingOffset.z = br.ReadSingle();
			lightmapPrototype.lightmapTilingOffset.w = br.ReadSingle();
			this.lightmapPrototypes.Add(lightmapPrototype);
		}
		br.ReadSingle();
		this.position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
		this.rotation = new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
		this.localScale = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
		num2 = br.BaseStream.Position;
		if (br.ReadInt32() == 20001)
		{
			this.combineParentUnitID = br.ReadInt32();
			bool flag = br.ReadBoolean();
			if (flag)
			{
				if (this.combinUnitIDs == null)
				{
					this.combinUnitIDs = new List<int>();
				}
				num4 = br.ReadInt32();
				for (int i = 0; i < num4; i++)
				{
					this.combinUnitIDs.Add(br.ReadInt32());
				}
			}
		}
		else
		{
			br.BaseStream.Position = num2;
		}
		this.cullingFactor = br.ReadSingle();
		num2 = br.BaseStream.Position;
		if (br.ReadInt32() == 40001)
		{
			this.center = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
		}
		else
		{
			this.center = this.position;
			br.BaseStream.Position = num2;
			br.ReadSingle();
			br.ReadSingle();
			br.ReadSingle();
		}
		br.ReadSingle();
		br.ReadSingle();
		if (this.cullingFactor <= 0.01f)   //太小，使用默认剔除因子
		{
			this.cullingFactor = this.scene.terrainConfig.defautCullingFactor;
		}
		this.UpdateViewRange();
		long num5 = br.BaseStream.Position;
		this.dataLength = num5 - num3;
		if (this.type == UnitType.UnitType_Light)
		{
			this.scene.lightDataLength = num5 - num;
		}
		this.readed = true;
	}
    /// <summary>
    /// 将指定阻塞网格坐标加入unit网格列表中      [x0,y0,x1,y1......]
    /// </summary>
    /// <param name="gridX"></param>
    /// <param name="gridY"></param>
	public void AppendGrid(int gridX, int gridY)
	{
		if (this.grids == null)
		{
			this.ResetGrids();
		}
		for (int i = 0; i < this.gridCount; i++)
		{
			if (this.grids[i, 0] == gridX && this.grids[i, 1] == gridY)
			{
				return;
			}
		}
		this.grids[this.gridCount, 0] = gridX;
		this.grids[this.gridCount, 1] = gridY;
		this.gridCount++;
	}
    /// <summary>
    /// 重置生成初始网格
    /// </summary>
	public void ResetGrids()
	{
		this.grids = new int[this.maxGridCount, 2];
		for (int i = 0; i < this.maxGridCount; i++)
		{
			this.grids[i, 0] = 0;
			this.grids[i, 1] = 0;
		}
	}
    /// <summary>
    /// 获取空的相关网格坐标数组
    /// </summary>
    /// <returns></returns>
	public int[,] GetCleanGrids()
	{
		int[,] array = new int[this.gridCount, 2];
		for (int i = 0; i < this.gridCount; i++)
		{
			array[i, 0] = this.grids[i, 0];
			array[i, 1] = this.grids[i, 1];
		}
		return array;
	}
    /// <summary>
    /// 朝向目标位置
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
	public float LookAt(Vector3 target)
	{
		if (Mathf.Abs(target.z - this.position.z) < 0.01f && Mathf.Abs(target.x - this.position.x) < 0.01f)
		{
			return 0f;
		}
		this.euler = -57.2957764f * Mathf.Atan2(target.z - this.position.z, target.x - this.position.x) + 90f;
		this.lookAtEuler.y = this.euler;
		this.rotation = Quaternion.Euler(this.lookAtEuler);
		return this.euler;
	}
    /// <summary>
    /// 沿着y轴旋转角度
    /// </summary>
    /// <param name="deg"></param>
    /// <param name="immediately"></param>
	public void RotaionY(float deg, bool immediately = false)
	{
		this.rotation = Quaternion.Euler(0f, deg * 57.29578f, 0f);
		if (immediately && this.ins != null)
		{
			this.ins.transform.rotation = this.rotation;
		}
	}
    /// <summary>
    /// 设置旋转角度
    /// </summary>
    /// <param name="rot"></param>
	public void SetRotation(Quaternion rot)
	{
		this.rotation = rot;
		this._rotationDirty = true;
	}
    /// <summary>
    /// 状态更新,计算阻塞的tile，更新unit的位置状态等
    /// </summary>
	public virtual void Update()
	{
		if (this.ins != null && !GameScene.isPlaying && this.isStatic) //场景没有运行时，静态unit游戏对象已建立    --这里似乎不能保证只运行一次吧，多次运行不会浪费么？？？？
		{
            //游戏对象的位置偏移保存的位置大于阈值，计算更新影响的tile列表
			if (Mathf.Abs(this.ins.transform.position.x - this.position.x) > 0.01f || Mathf.Abs(this.ins.transform.position.z - this.position.z) > 0.01f)
			{
				this.ComputeTiles();
			}
			this.position = this.ins.transform.position;
			this.rotation = this.ins.transform.rotation;
			this.localScale = this.ins.transform.localScale;
			this.unitParser.Update(this.ins);  //通过解析器解析的数据更新unit游戏对象
		}
		this.tick++;
	}
    /// <summary>
    /// 加载完成，进行初始化
    /// </summary>
    /// <param name="args"></param>
	private void OnThridPartAssetLoaded(params object[] args)
	{
		if (this.destroyed || this.willRemoved)
		{
			return;
		}
		this.pre = (args[0] as GameObject);
		this.Initialize();
		if (!this.isStatic && this.ins != null)
		{
			this.ActiveDynaUnit();
		}
	}
    /// <summary>
    /// 初始化unit场景显示游戏对象
    /// </summary>
	private void Initialize()
	{
		if (this.destroyed || this.willRemoved)    //待销毁或移除，无法初始化
		{
			return;
		}
		if (this.ins == null && this.pre != null)
		{
            //实例化gameObject
			if (!this.isStatic)     //动态unit直接instantiate
			{
				this.ins = (DelegateProxy.Instantiate(this.pre) as GameObject);
			}
			else                    //静态unit，根据数据信息解析相关属性来构建
			{
				this.ins = (this.unitParser.Instantiate(this.pre) as GameObject);
			}
			if (!GameScene.isPlaying)
			{
				this.type = UnitType.GetType(this.ins.layer); //根据layer获取unit的类型
			}
			if (!this.isStatic && this.needSampleHeight)    //非静态物体，计算unit位置点的地面高度给位置坐标y值，保证站立在地面上
			{
				this.position.y = this.scene.SampleHeight(this.position, true);
			}
			this.ins.transform.position = this.position; //设定位置
			if (this.components != null)   //挂载component，问题components从哪儿获取?????//
			{
				int count = this.components.Count;
				for (int i = 0; i < count; i++)
				{
					this.ins.AddComponent(this.components[i]);
				}
			}
			if (this.isStatic)
			{
				this.ins.transform.rotation = this.rotation;
				this.ins.transform.localScale = this.localScale;
				if (LightmapSettings.lightmaps.Length > 0) //光照贴图属性赋值
				{
					int count = this.lightmapPrototypes.Count;
					for (int j = 0; j < count; j++)
					{
						LightmapPrototype lightmapPrototype = this.lightmapPrototypes[j];
						Renderer renderer = null;
						if (lightmapPrototype.rendererChildIndex == -1)
						{
							renderer = this.ins.renderer;
						}
						else if (lightmapPrototype.rendererChildIndex < this.ins.transform.childCount)
						{
							renderer = this.ins.transform.GetChild(lightmapPrototype.rendererChildIndex).renderer;
						}
						if (renderer != null)
						{
							renderer.lightmapIndex = lightmapPrototype.lightmapIndex;
							renderer.lightmapTilingOffset = lightmapPrototype.lightmapTilingOffset;
						}
					}
				}
                //收集所有Render列表
				List<Renderer> list = new List<Renderer>();
				if (this.ins.renderer != null)
				{
					list.Add(this.ins.renderer);
				}
				for (int k = 0; k < this.ins.transform.childCount; k++)
				{
					Renderer renderer2 = this.ins.transform.GetChild(k).renderer;
					if (renderer2 != null)
					{
						list.Add(renderer2);
					}
				}
				if (GameScene.isPlaying)  //场景开始运行
				{
					for (int l = 0; l < list.Count; l++)
					{
						for (int m = 0; m < list[l].materials.Length; m++)
						{
							Material material = list[l].materials[m];
							if (material != null)
							{
								if (list[l].gameObject.layer == GameLayer.Layer_Ground)  //实时场景运行时，只有地面接受阴影
								{
									list[l].receiveShadows = true;
								}
								else
								{
									list[l].receiveShadows = false;
								}
								this.shaderName = material.shader.name;
								if (!this.scene.terrainConfig.enablePointLight)    //根据是否启用点光源，使用不同的材质shader
								{
									if (this.shaderName == GameObjectUnit.diffuseShaderName || this.shaderName == GameObjectUnit.snailDiffusePointShaderName)
									{
										material.shader = GameObjectUnit.snailDiffuseShader;
									}
									if (this.shaderName == GameObjectUnit.diffuseCutoutShaderName || this.shaderName == GameObjectUnit.snailDiffusePointCutoutShaderName)
									{
										material.shader = GameObjectUnit.snailDiffuseCutoutShader;
									}
								}
								else
								{
									if (this.shaderName == GameObjectUnit.diffuseShaderName || this.shaderName == GameObjectUnit.snailDiffuseShaderName)
									{
										material.shader = GameObjectUnit.snailDiffusePointShader;
									}
									if (this.shaderName == GameObjectUnit.diffuseCutoutShaderName || this.shaderName == GameObjectUnit.snailDiffuseCutoutShaderName)
									{
										material.shader = GameObjectUnit.snailDiffusePointCutoutShader;
									}
								}
							}
						}
					}
				}
				else
				{
                    //场景没有运行时，需要开启接受阴影和产生阴影，便于bake光照贴图
					for (int n = 0; n < list.Count; n++)
					{
						list[n].receiveShadows = true;
						list[n].castShadows = true;
						for (int num = 0; num < list[n].sharedMaterials.Length; num++)
						{
							Material material2 = list[n].sharedMaterials[num];
							if (this.scene.statisticMode)
							{
								Statistic.Push(material2, AssetType.Material);
							}
							if (material2 != null)
							{
								this.shaderName = material2.shader.name;
								if (this.shaderName == GameObjectUnit.snailDiffuseShaderName || this.shaderName == GameObjectUnit.snailDiffusePointShaderName)
								{
									material2.shader = GameObjectUnit.diffuseShader;
								}
								if (this.shaderName == GameObjectUnit.snailDiffuseCutoutShaderName || this.shaderName == GameObjectUnit.snailDiffusePointCutoutShaderName)
								{
									material2.shader = GameObjectUnit.diffuseCutoutShader;
								}
							}
						}
					}
				}
			}
			else if (this.createInsListener != null)
			{
				try
				{
					this.createInsListener();  //动态unit调用创建对象回调
				}
				catch (Exception ex)
				{
					LogSystem.LogError(new object[]
					{
						"监听创建单位函数中出错!" + ex.ToString()
					});
				}
			}
			this.Renamme();
			if (!GameScene.isPlaying)   //场景没有开始运行
			{
				this.CollectMaterials();
				this.AddMeshRenderColliders();
				if (this.cullingFactor <= 0.01f)    
				{
					this.cullingFactor = this.scene.terrainConfig.defautCullingFactor;
				}
				if (this.isStatic)     //如果是静态unit，计算更新影响的阻塞tile列表
				{
					this.ComputeTiles();
				}
			}
		}
		else
		{
			this.lostAsset = true;
			if (GameScene.isEditor)
			{
				this.ins = GameObject.CreatePrimitive(PrimitiveType.Cube);
				this.ins.transform.position = this.position;
				this.ins.transform.rotation = this.rotation;
				this.ins.transform.localScale = new Vector3(2f, 2f, 2f);
				string[] array = this.prePath.Split(new char[]
				{
					'/'
				});
				if (this.lightmapPrototypes.Count > 0 && this.ins.renderer != null)
				{
					this.ins.name = string.Concat(new object[]
					{
						array[array.Length - 1],
						":LM{",
						this.lightmapPrototypes[0].scale,
						"} : Unit_",
						this.createID
					});
				}
				else
				{
					this.ins.name = array[array.Length - 1] + ":Unit_" + this.createID;
				}
			}
		}
		this.PlayBornEffect();  //初始化完成，开始播放出生效果
	}
    /// <summary>
    /// 根据设置更换合适的shader 和设置响应的阴影设置
    /// </summary>
    /// <param name="sRender"></param>
	public static void ChangeShader(Renderer sRender)
	{
		if (sRender == null || GameScene.mainScene == null)
		{
			return;
		}
		bool flag = false;
		if (sRender.gameObject.layer == GameLayer.Layer_Ground)    //只有地面接受阴影哦
		{
			sRender.receiveShadows = true;
		}
		else
		{
			sRender.receiveShadows = false;
			if (sRender.gameObject.layer == GameLayer.Layer_UIEffect)
			{
				flag = true;
			}
		}
		for (int i = 0; i < sRender.materials.Length; i++)
		{
			Material material = sRender.materials[i];
			if (!(material == null))
			{
				string a = material.shader.name;
				if (!GameScene.mainScene.terrainConfig.enablePointLight)
				{
					if (a == GameObjectUnit.diffuseShaderName || a == GameObjectUnit.snailDiffusePointShaderName)
					{
						material.shader = GameObjectUnit.snailDiffuseShader;
					}
					if (a == GameObjectUnit.diffuseCutoutShaderName || a == GameObjectUnit.snailDiffusePointCutoutShaderName)
					{
						material.shader = GameObjectUnit.snailDiffuseCutoutShader;
					}
				}
				else
				{
					if (a == GameObjectUnit.diffuseShaderName || a == GameObjectUnit.snailDiffuseShaderName)
					{
						material.shader = GameObjectUnit.snailDiffusePointShader;
					}
					if (a == GameObjectUnit.diffuseCutoutShaderName || a == GameObjectUnit.snailDiffuseCutoutShaderName)
					{
						material.shader = GameObjectUnit.snailDiffusePointCutoutShader;
					}
				}
				if (flag)
				{
					GameObjectUnit.ChangeShader(material, new string[]
					{
						"Snail/Bumped Specular Point Light2",
						"Snail/Bumped Specular Point Light Cutout2",
						"Snail/Bumped Specular Point Light UV Anima2"
					});
				}
			}
		}
	}
    /// <summary>
    /// 更新指定材质的shader，
    /// </summary>
    /// <param name="mt"></param>
    /// <param name="args">待适配的shader列表</param>
	public static void ChangeShader(Material mt, params string[] args)
	{
		Shader[] array = new Shader[args.Length];
		for (int i = 0; i < args.Length; i++)
		{
			array[i] = Shader.Find(args[i]);
			if (array[i] == null)
			{
				LogSystem.LogWarning(new object[]
				{
					"shader not found"
				});
			}
		}
		for (int j = 0; j < args.Length; j++)
		{
			if (!(array[j] == null))
			{
				string value = args[j].Substring(0, args[j].Length - 1);
				if (mt.shader.name.Equals(value))
				{
					mt.shader = array[j];
					break;
				}
			}
		}
	}
    /// <summary>
    /// 播放出生效果
    /// </summary>
	private void PlayBornEffect()
	{
		if (this.bornEffect == null && this._bornEffectPrePath != string.Empty && this.ins != null)
		{
			GameObject gameObject = Resources.Load(this._bornEffectPrePath, typeof(GameObject)) as GameObject;
			if (gameObject != null)
			{
				this.bornEffect = (DelegateProxy.Instantiate(gameObject) as GameObject);
				this.bornEffect.transform.position = this.position;
			}
			else
			{
				LogSystem.LogWarning(new object[]
				{
					"bornEffectPrePath " + this._bornEffectPrePath + " is null."
				});
			}
		}
	}
    /// <summary>
    /// 激活动态unit
    /// </summary>
	private void ActiveDynaUnit()
	{
		this.ins.SetActive(true);
		if (this.activeListener != null)
		{
			try
			{
				this.activeListener(true);   //触发激活回调
			}
			catch (Exception ex)
			{
				LogSystem.LogError(new object[]
				{
					"监听创建单位函数中出错!" + ex.ToString()
				});
			}
		}
	}
    /// <summary>
    /// 开始显示gameobject对象
    /// </summary>
	public void Visible()
	{
		if (this.destroyed || this.willRemoved)  //销毁或待移除，没法可视
		{
			return;
		}
		if (!this.visible)
		{
			if (this.ins == null)
			{
				if (this.prePath != string.Empty)  //需要预先加载
				{
					if (GameObjectUnit.thridPardResourManager != null && this.scene.loadFromAssetBund)
					{
						if (this.pre != null)
						{
							this.Initialize();
						}
						else
						{
							GameObjectUnit.thridPardResourManager(this.prePath, new AssetCallBack(this.OnThridPartAssetLoaded));
						}
					}
					else
					{
						Asset asset = AssetLibrary.Load(this.prePath, AssetType.Prefab, LoadType.Type_Resources);
						if (!asset.loaded)
						{
							this.pre = null;
						}
						else
						{
							this.pre = asset.gameObject;
						}
						if (this.pre != null)
						{
							this.Initialize();
						}
					}
				}
			}
			else
			{
				this.ins.SetActive(true);
			}
			if (!this.isStatic && this.ins != null) //如果不是静态，激活动态unit
			{
				this.ActiveDynaUnit();
			}
		}
		this.visible = true;
	}
    /// <summary>
    /// 重命名unit游戏对象
    /// </summary>
	public void Renamme()
	{
		if (!GameScene.isPlaying)
		{
			string[] array = this.prePath.Split(new char[]
			{
				'/'
			});
			if (this.lightmapPrototypes.Count > 0 && this.ins.renderer != null)
			{
				this.ins.name = string.Concat(new object[]
				{
					array[array.Length - 1],
					":LM{",
					this.lightmapPrototypes[0].scale,
					"} : Unit_",
					this.createID
				});
			}
			else
			{
				this.ins.name = array[array.Length - 1] + ":Unit_" + this.createID;
			}
			int count = this.lightmapPrototypes.Count;
			for (int i = 0; i < count; i++)
			{
				LightmapPrototype lightmapPrototype = this.lightmapPrototypes[i];
				Renderer renderer = null;
				if (lightmapPrototype.rendererChildIndex > -1)
				{
					if (lightmapPrototype.rendererChildIndex < this.ins.transform.childCount)
					{
						renderer = this.ins.transform.GetChild(lightmapPrototype.rendererChildIndex).renderer;
					}
					if (renderer != null)
					{
						string[] array2 = renderer.gameObject.name.Split(new char[]
						{
							':'
						});
						renderer.gameObject.name = string.Concat(new object[]
						{
							array2[0],
							":LM{",
							lightmapPrototype.scale,
							"}"
						});
					}
				}
			}
		}
		else
		{
			this.ins.name = "Unit_" + this.createID;
		}
	}
    /// <summary>
    /// 收集Unit使用了的材质列表,包括光照贴图相关材质
    /// </summary>
	public void CollectMaterials()
	{
		this.materials = new List<Material>();
		if (this.ins.renderer != null && this.ins.renderer.sharedMaterial != null)
		{
			this.materials.Add(this.ins.renderer.sharedMaterial);
		}
		int count = this.lightmapPrototypes.Count;
		for (int i = 0; i < count; i++)
		{
			LightmapPrototype lightmapPrototype = this.lightmapPrototypes[i];
			Renderer renderer = null;
			if (lightmapPrototype.rendererChildIndex > -1)
			{
				if (lightmapPrototype.rendererChildIndex < this.ins.transform.childCount)
				{
					renderer = this.ins.transform.GetChild(lightmapPrototype.rendererChildIndex).renderer;
				}
				if (renderer != null && renderer.sharedMaterial != null)
				{
					this.materials.Add(renderer.sharedMaterial);
				}
			}
		}
	}
    /// <summary>
    /// 设置不可视 ,开始隐藏
    /// </summary>
	public void Invisible()
	{
		if (this.visible)
		{
			if (this.bornEffect != null)
			{
				DelegateProxy.GameDestory(this.bornEffect);
			}
			if (this.isStatic)
			{
				if (this.ins != null)
				{
					this.ins.SetActive(false);
				}
			}
			else if (this.ins != null)
			{
				this.ins.SetActive(false);
				if (this.activeListener != null)
				{
					this.activeListener(false);
				}
			}
		}
		this.visible = false;
	}
    /// <summary>
    /// 销毁unit游戏对象
    /// </summary>
	public virtual void Destroy()
	{
		if (this.destroyInsListener != null)
		{
			try
			{
				this.destroyInsListener();
			}
			catch (Exception ex)
			{
				LogSystem.LogError(new object[]
				{
					"监听创建单位函数中出错!" + ex.ToString()
				});
			}
		}
		if (this.ins != null)
		{
			if (GameScene.isPlaying)
			{
				DelegateProxy.GameDestory(this.ins);
			}
			else
			{
				DelegateProxy.DestroyObjectImmediate(this.ins);
			}
		}
		if (this.combinUnitIDs != null)
		{
			this.combinUnitIDs.Clear();
		}
		this.combineParentUnitID = -1;
		if (this.tiles != null)
		{
			for (int i = 0; i < this.tiles.Count; i++)
			{
				this.tiles[i].RemoveUnit(this);
			}
			this.tiles.Clear();
		}
		if (this.materials != null)
		{
			this.materials.Clear();
			this.materials = null;
		}
		this.active = false;
		this.grids = null;
		this.pre = null;
		this.ins = null;
		if (this.unitParser != null)
		{
			this.unitParser.Destroy();
			this.unitParser = null;
		}
		if (this.lightmapPrototypes != null)
		{
			this.lightmapPrototypes.Clear();
		}
		this.bornEffectPrePath = string.Empty;
		this.bornEffect = null;
		this.dataLength = -1L;
		this.prePath = string.Empty;
		this.createInsListener = null;
		this.destroyInsListener = null;
		this.activeListener = null;
		this.visible = false;
		this.destroyed = true;
		this.scene = null;
		this.readed = false;
	}
    /// <summary>
    /// 计算当前unit的剔除距离和进入距离
    /// </summary>
	public void UpdateViewRange()
	{
		this.near = this.scene.terrainConfig.unitCullingDistance * this.cullingFactor + this.scene.terrainConfig.cullingBaseDistance;
		this.far = this.near + 2f;
	}
    /// <summary>
    /// 更新计算阻塞tile列表
    /// </summary>
	public void ComputeTiles()
	{
		this.ClearTiles();
		this.mainTile = this.scene.GetTileAt(this.position);
		if (this.mainTile != null)
		{
			this.AddTile(this.mainTile);
			if (this.ins != null)
			{
				BoxCollider boxCollider = this.ComputeBounds();
				if (boxCollider != null)
				{
					this.cullingFactor = boxCollider.bounds.size.magnitude;  //根据碰撞体的,包围盒对角线长度做剔除因子
				}
				if (this.cullingFactor <= 0.01f)
				{
					this.cullingFactor = this.scene.terrainConfig.defautCullingFactor;
				}
				this.UpdateViewRange();
				for (int i = 0; i < this.scene.tiles.Count; i++)   //遍历当前场景的所有tile
				{
					Tile tile = this.scene.tiles[i];
					if (boxCollider != null && boxCollider.bounds.Intersects(tile.bounds))  //unit有碰撞，通过包围盒检测是否包含到该tile中去
					{
						this.AddTile(tile);
					}
				}
				if (boxCollider != null)     //使用完了就销毁，也就是说实际游戏运行中，不进行实际碰撞处理,依赖于服务器吧
				{
					DelegateProxy.DestroyObjectImmediate(boxCollider);
				}
			}
		}
		if (this.tiles.Count < 1)
		{
			Debug.Log("该单位没有所属切片, 无法进行存储! 单位编号[" + this.createID + "]");
		}
	}
    /// <summary>
    /// 清除相关tile列表，同时从这些tile移除当前unit的应用
    /// </summary>
	public void ClearTiles()
	{
		for (int i = 0; i < this.tiles.Count; i++)
		{
			this.tiles[i].RemoveUnit(this);
		}
		this.tiles.Clear();
	}
    /// <summary>
    /// 添加tile到当前unit影响的tile列表
    /// </summary>
    /// <param name="tile"></param>
	public void AddTile(Tile tile)
	{
		tile.AddUnit(this);
		if (!this.tiles.Contains(tile))
		{
			this.tiles.Add(tile);
		}
	}
    /// <summary>
    /// 给unit下的所有网格组件添加MeshCollider碰撞组件
    /// </summary>
	private void AddMeshRenderColliders()
	{
		if (this.ins != null)
		{
			if (this.ins.transform.renderer && this.ins.GetComponent<MeshCollider>() == null)
			{
				this.ins.AddComponent<MeshCollider>();
			}
			int childCount = this.ins.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Renderer renderer = this.ins.transform.GetChild(i).renderer;
				if (renderer != null && renderer.gameObject.GetComponent<MeshCollider>() == null)
				{
					renderer.gameObject.AddComponent<MeshCollider>();
				}
			}
		}
	}
    /// <summary>
    /// 根据网格渲染组件，计算unit的包围盒范围，并添加碰撞体
    /// </summary>
    /// <returns></returns>
	public BoxCollider ComputeBounds()
	{
		if (this.ins == null)
		{
			return null;
		}
		MeshRenderer[] componentsInChildren = this.ins.GetComponentsInChildren<MeshRenderer>();
		if (componentsInChildren.Length < 1)
		{
			return null;
		}
		Bounds bounds = default(Bounds);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Bounds bounds2 = componentsInChildren[i].bounds;
			if (i == 0)
			{
				bounds = bounds2;
			}
			if (i > 0)
			{
				bounds.Encapsulate(bounds2);
			}
		}
		Vector3 min = this.ins.transform.InverseTransformPoint(bounds.min);
		Vector3 max = this.ins.transform.InverseTransformPoint(bounds.max);
		bounds.min = min;
		bounds.max = max;
		BoxCollider boxCollider = this.ins.AddComponent<BoxCollider>();
		boxCollider.center = bounds.center;
		boxCollider.size = bounds.size;
		this.center = this.ins.transform.localToWorldMatrix.MultiplyPoint3x4(bounds.center);
		return boxCollider;
	}
}
