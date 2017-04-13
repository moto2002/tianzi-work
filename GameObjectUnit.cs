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

	public bool isMainUint;

	public bool willRemoved;

	public int lightmapSize = 128;

	public GameObjectUnit.CreateInsListener createInsListener;

	public GameObjectUnit.DestroyInsListener destroyInsListener;

	public GameObjectUnit.ActiveListener activeListener;

	public float randomCode;

	public int createID;

	public string name = string.Empty;

	public bool isCollider = true;

	public GameObject ins;

	public bool isStatic = true;

	public string prePath = string.Empty;

	public Vector3 position;

	public Quaternion rotation;

	public Vector3 localScale;

	public Vector3 center;

	public Vector3 size = Vector3.zero;

	public float cullingFactor;

	private int dependResCount;

	public bool visible;

	public float near;

	public float far;

	public float viewDistance;

	public float viewAngle;

	public List<Tile> tiles = new List<Tile>();

	public Tile mainTile;

	public bool hasCollision;

	public float radius = 1f;

	public int collisionSize = 1;

	private int[,] _grids;

	public List<string> components = new List<string>();

	public List<LightmapPrototype> lightmapPrototypes = new List<LightmapPrototype>();

	public bool needScreenPoint = true;

	public Vector3 screenPoint;

	public bool mouseEnable = true;

	public int type = UnitType.UnitType_General;

	public UnitParser unitParser;

	public LoadType loadtype = LoadType.Type_Resources;

	public bool genRipple;

	protected Vector3 ripplePos;

	public int genRippleDelayTick = 50;

	public GameScene scene;

	public bool active;

	protected Vector3 scenePoint = Vector3.zero;

	public float scenePointBias = 1f;

	private int tick;

	public bool needSampleHeight = true;

	public List<Material> materials = new List<Material>();

	public int combineParentUnitID = -1;

	public List<int> combinUnitIDs = new List<int>();

	private bool readed;

	private long dataLength;

	public bool destroyed;

	public bool castShadows;

	protected bool oldCastShadows;

	private int gridCount;

	private int maxGridCount = 200;

	private Vector3 lookAtEuler = new Vector3(0f, 0f, 0f);

	public float euler;

	protected bool _rotationDirty;

	public bool lostAsset;

	private string shaderName = string.Empty;

	protected static string diffuseShaderName = "Diffuse";

	protected static string diffuseCutoutShaderName = "Transparent/Cutout/Diffuse";

	protected static string diffuseTransparentShaderName = "Transparent/Diffuse";

	protected static string snailDiffuseShaderName = "Snail/Diffuse";

	protected static string snailDiffuseCutoutShaderName = "Snail/Transparent/Cutout/Diffuse";

	protected static string snailDiffusePointShaderName = "Snail/Diffuse-PointLight";

	protected static string snailDiffusePointCutoutShaderName = "Snail/Diffuse-PointLight-Cutout";

	protected static Shader diffuseShader = Shader.Find(GameObjectUnit.diffuseShaderName);

	protected static Shader diffuseCutoutShader = Shader.Find(GameObjectUnit.diffuseCutoutShaderName);

	protected static Shader snailDiffuseShader = Shader.Find(GameObjectUnit.snailDiffuseShaderName);

	protected static Shader snailDiffuseCutoutShader = Shader.Find(GameObjectUnit.snailDiffuseCutoutShaderName);

	protected static Shader snailDiffusePointShader = Shader.Find(GameObjectUnit.snailDiffusePointShaderName);

	protected static Shader snailDiffusePointCutoutShader = Shader.Find(GameObjectUnit.snailDiffusePointCutoutShaderName);

	private UnityEngine.Object pre;

	public static GameObjectUnit.ThridPardResourManager thridPardResourManager;

	private string _bornEffectPrePath = string.Empty;

	protected GameObject bornEffect;

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

	public GameObjectUnit(int createID)
	{
		this.createID = createID;
	}

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
			this.unitParser = UnitType.GenUnitParser(this.type);
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
		if (this.cullingFactor <= 0.01f)
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
    /// 将指定网格坐标加入unit网格列表中      [x0,y0,x1,y1......]
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

	public void RotaionY(float deg, bool immediately = false)
	{
		this.rotation = Quaternion.Euler(0f, deg * 57.29578f, 0f);
		if (immediately && this.ins != null)
		{
			this.ins.transform.rotation = this.rotation;
		}
	}

	public void SetRotation(Quaternion rot)
	{
		this.rotation = rot;
		this._rotationDirty = true;
	}

	public virtual void Update()
	{
		if (this.ins != null && !GameScene.isPlaying && this.isStatic)
		{
			if (Mathf.Abs(this.ins.transform.position.x - this.position.x) > 0.01f || Mathf.Abs(this.ins.transform.position.z - this.position.z) > 0.01f)
			{
				this.ComputeTiles();
			}
			this.position = this.ins.transform.position;
			this.rotation = this.ins.transform.rotation;
			this.localScale = this.ins.transform.localScale;
			this.unitParser.Update(this.ins);
		}
		this.tick++;
	}

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

	private void Initialize()
	{
		if (this.destroyed || this.willRemoved)
		{
			return;
		}
		if (this.ins == null && this.pre != null)
		{
			if (!this.isStatic)
			{
				this.ins = (DelegateProxy.Instantiate(this.pre) as GameObject);
			}
			else
			{
				this.ins = (this.unitParser.Instantiate(this.pre) as GameObject);
			}
			if (!GameScene.isPlaying)
			{
				this.type = UnitType.GetType(this.ins.layer);
			}
			if (!this.isStatic && this.needSampleHeight)
			{
				this.position.y = this.scene.SampleHeight(this.position, true);
			}
			this.ins.transform.position = this.position;
			if (this.components != null)
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
				if (LightmapSettings.lightmaps.Length > 0)
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
				if (GameScene.isPlaying)
				{
					for (int l = 0; l < list.Count; l++)
					{
						for (int m = 0; m < list[l].materials.Length; m++)
						{
							Material material = list[l].materials[m];
							if (material != null)
							{
								if (list[l].gameObject.layer == GameLayer.Layer_Ground)
								{
									list[l].receiveShadows = true;
								}
								else
								{
									list[l].receiveShadows = false;
								}
								this.shaderName = material.shader.name;
								if (!this.scene.terrainConfig.enablePointLight)
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
					this.createInsListener();
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
			if (!GameScene.isPlaying)
			{
				this.CollectMaterials();
				this.AddMeshRenderColliders();
				if (this.cullingFactor <= 0.01f)
				{
					this.cullingFactor = this.scene.terrainConfig.defautCullingFactor;
				}
				if (this.isStatic)
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
		this.PlayBornEffect();
	}

	public static void ChangeShader(Renderer sRender)
	{
		if (sRender == null || GameScene.mainScene == null)
		{
			return;
		}
		bool flag = false;
		if (sRender.gameObject.layer == GameLayer.Layer_Ground)
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

	private void ActiveDynaUnit()
	{
		this.ins.SetActive(true);
		if (this.activeListener != null)
		{
			try
			{
				this.activeListener(true);
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

	public void Visible()
	{
		if (this.destroyed || this.willRemoved)
		{
			return;
		}
		if (!this.visible)
		{
			if (this.ins == null)
			{
				if (this.prePath != string.Empty)
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
			if (!this.isStatic && this.ins != null)
			{
				this.ActiveDynaUnit();
			}
		}
		this.visible = true;
	}

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

	public void UpdateViewRange()
	{
		this.near = this.scene.terrainConfig.unitCullingDistance * this.cullingFactor + this.scene.terrainConfig.cullingBaseDistance;
		this.far = this.near + 2f;
	}

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
					this.cullingFactor = boxCollider.bounds.size.magnitude;
				}
				if (this.cullingFactor <= 0.01f)
				{
					this.cullingFactor = this.scene.terrainConfig.defautCullingFactor;
				}
				this.UpdateViewRange();
				for (int i = 0; i < this.scene.tiles.Count; i++)
				{
					Tile tile = this.scene.tiles[i];
					if (boxCollider != null && boxCollider.bounds.Intersects(tile.bounds))
					{
						this.AddTile(tile);
					}
				}
				if (boxCollider != null)
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

	public void ClearTiles()
	{
		for (int i = 0; i < this.tiles.Count; i++)
		{
			this.tiles[i].RemoveUnit(this);
		}
		this.tiles.Clear();
	}

	public void AddTile(Tile tile)
	{
		tile.AddUnit(this);
		if (!this.tiles.Contains(tile))
		{
			this.tiles.Add(tile);
		}
	}

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
