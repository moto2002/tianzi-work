using System;
using System.Collections.Generic;
using UnityEngine;

public class DynamicUnit : GameObjectUnit
{
	public delegate void MoveListener(bool flag);

	public delegate void PathNextPointListener(Vector3 nextPathPoint);

	public delegate void PathInterruptedListener(Vector3 postion);

	public delegate void PathEndListener();
    /// <summary>
    /// 移动状态事件侦听回调，如开始移动，停止移动
    /// </summary>
	public DynamicUnit.MoveListener moveListener;
    /// <summary>
    /// 进入下个路径点事件侦听回调
    /// </summary>
	public DynamicUnit.PathNextPointListener pathNextPointListener;
    /// <summary>
    /// 路径中断侦听事件回调
    /// </summary>
	public DynamicUnit.PathInterruptedListener pathInterruptedListener;
    /// <summary>
    /// 路径移动结束侦听事件回调
    /// </summary>
	public DynamicUnit.PathEndListener pathEndListener;
    /// <summary>
    /// 路径移动是否中断
    /// </summary>
	private bool pathInterrupted;
    /// <summary>
    /// 路径移动中断的情况下是否继续
    /// </summary>
	public bool continuWithInterr = true;
    /// <summary>
    /// 是否位于地面上
    /// </summary>
	public bool isGrounded;
    /// <summary>
    /// 当前的移动的速度
    /// </summary>
	public float speed;
    /// <summary>
    /// 是否开始移动中
    /// </summary>
	public bool moving;
    /// <summary>
    /// 开始移动标记
    /// </summary>
	public int _startMove;
    /// <summary>
    /// 停止移动标记
    /// </summary>
	public int _endMove;

	private float mfCurEaseTime;

	private float mfEaseTime = 2f;

	public Vector3 start;
    /// <summary>
    /// unit移动的目标位置
    /// </summary>
	public Vector3 target;

	private Vector3 step;

	private Vector3 normalStep;

	private float distance;

	private Vector3 nextPostion = Vector3.zero;

	public bool isBreak;

	public bool dontComputeCollision = true;

	private bool autoComputeDynCollision = true;

	public int tick;
    /// <summary>
    /// 是否在水下
    /// </summary>
	public bool underwater;

	public DynamicState mDynState;

	public List<DynamicLinkUnit> linkUnits = new List<DynamicLinkUnit>();

	private Material mainUnitMat;

	protected static string mainUnitShaderName = "Snail/Bumped Specular Point Light ZTest";

	protected static string mainUnitOriShaderName = "Snail/Bumped Specular Point Light";

	protected static Shader mainUnitShader = Shader.Find(DynamicUnit.mainUnitShaderName);

	private List<Material> oriMats;

	private List<Renderer> renderers;

	private Dictionary<string, Material> replaceMats;

	private bool doneOccEffect;

	private float moveDistance;

	private float smHeight;

	private Vector3 pathFindTarget = Vector3.zero;

	private List<Vector3> paths = new List<Vector3>();

	public int delayTick;

	public int curDelayTick;

	private bool pathFindEnd = true;

	private string pfWrongTip = "注意：【失败原因1】：寻路移动失败! 请相关策划查询目标点配置是否正确  【失败原因2】有人站在你的格子上与你人物有重叠或者一群怪物围着你,寻路目标点->";
    /// <summary>
    /// 移动类型 0 --  指定速度移动到目标位置   1 -- 固定时间内移动到目标位置
    /// </summary>
	private int move_type = -1;

	private Quaternion targetRotation;

	private Vector3 lookAtEuler = new Vector3(0f, 0f, 0f);

	private Vector3 newTarget = Vector3.zero;

	private Vector3 from;

	private Vector3 lastTarget;

	private Vector3 dir;

	private Vector3 tryStep;

	private Vector3 nextTarget;

	private bool outObstacles;

	public Vector3 tryMoveTarget = Vector3.zero;
    /// <summary>
    /// 网格类型
    /// </summary>
	public int gridType;

	public int MoveType
	{
		get
		{
			return this.move_type;
		}
	}

	public bool startMove
	{
		get
		{
			return this._startMove > 0;
		}
	}

	public bool endMove
	{
		get
		{
			return this._endMove > 0;
		}
	}

	public DynamicUnit(int createID) : base(createID)
	{
		this.isStatic = false;
	}
    
    /// <summary>
    /// 销毁动态unit游戏对象,清理各种数据及引用
    /// </summary>
	public override void Destroy()
	{
		if (this.renderers != null)
		{
			for (int i = 0; i < this.renderers.Count; i++)
			{
				if (this.renderers[i] != null && this.oriMats[i] != null)
				{
					this.renderers[i].sharedMaterial = this.oriMats[i];
				}
			}
		}
		if (this.replaceMats != null)  //清理替换材质列表
		{
			this.replaceMats.Clear();
			this.replaceMats = null;
		}
		if (this.oriMats != null)
		{
			this.oriMats.Clear();
			this.oriMats = null;
		}
		if (this.renderers != null)    //清理渲染器列表
		{
			this.renderers.Clear();
			this.renderers = null;
		}
		this._endMove = 1;
		this._startMove = 0;
		this.moving = false;
		if (this.paths != null)
		{
			this.pathFindEnd = true;
			this.paths.Clear();
		}
		this.isMainUint = false;
		this.genRipple = false;
		this.moveListener = null;
		this.pathNextPointListener = null;
		this.pathInterruptedListener = null;
		this.pathEndListener = null;
		base.Destroy();
	}
    /// <summary>
    /// 作为主角unit
    /// </summary>
	public void AsMainUint()
	{
		this.scene.mainUnit = this;
		this.isMainUint = true;
		this.isCollider = false;
		this.dontComputeCollision = false;
		if (this.isMainUint && GameScene.isPlaying)
		{
			this.genRipple = true;
		}
		this.doneOccEffect = true;
	}
    /// <summary>
    /// 设置碰撞
    /// </summary>
    /// <param name="value">是否引擎内置碰撞</param>
	public void SetCollision(bool value)
	{
		if (!value)
		{
			this.scene.mapPath.SetDynamicCollision(this.position, this.collisionSize, true, 1);
			this.hasCollision = false;
			this.isCollider = false;
		}
		else
		{
			this.scene.mapPath.SetDynamicCollision(this.position, this.collisionSize, false, 1);
			this.hasCollision = true;
			this.isCollider = true;
		}
	}
    /// <summary>
    /// 使用引擎碰撞器碰撞还是自己的碰撞计算算法
    /// </summary>
    /// <param name="value"></param>
	public void SetCustomCollision(bool value)
	{
		if (base.grids != null)
		{
			if (!value)
			{
				if (this.scene != null && this.scene.mapPath != null)
				{
					this.scene.mapPath.SetDynamicCollision(this.position, base.grids, true, 0);
				}
				this.isCollider = false;
			}
			else
			{
				if (this.scene != null && this.scene.mapPath != null)
				{
					this.scene.mapPath.SetDynamicCollision(this.position, base.grids, false, 0);
				}
				this.isCollider = true;
			}
		}
	}
    /// <summary>
    /// 创建动态Unit
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="pos"></param>
    /// <param name="createID"></param>
    /// <param name="prePath"></param>
    /// <param name="radius"></param>
    /// <param name="dynamiCullingDistance"></param>
    /// <returns></returns>
	public static DynamicUnit Create(GameScene scene, Vector3 pos, int createID, string prePath, float radius, float dynamiCullingDistance = -1f)
	{
		DynamicUnit dynamicUnit = new DynamicUnit(createID);
		dynamicUnit.scene = scene;
		dynamicUnit.position = pos;
		dynamicUnit.prePath = prePath;
		dynamicUnit.radius = radius;
		if (dynamiCullingDistance > 0f)
		{
			dynamicUnit.near = dynamiCullingDistance;
		}
		else
		{
			dynamicUnit.near = scene.terrainConfig.dynamiCullingDistance;
		}
		dynamicUnit.far = dynamicUnit.near + 2f;
		return dynamicUnit;
	}
    /// <summary>
    /// 添加设置动态unit到关联子unit列表
    /// </summary>
    /// <param name="position"></param>
    /// <param name="orient"></param>
    /// <param name="unit"></param>
	public void SetDynamicLink(Vector3 position, float orient, DynamicUnit unit)
	{
		if (unit == null)
		{
			return;
		}
		if (this.destroyed || unit.destroyed || this.scene.mapPath == null)
		{
			return;
		}
		if (this.mDynState == DynamicState.LINK_CHILD)
		{
			this.mDynState = DynamicState.LINK_PARENT_CHILD;
		}
		else if (this.mDynState == DynamicState.NULL)
		{
			this.mDynState = DynamicState.LINK_PARENT;
		}
		if (this.linkUnits == null)
		{
			this.linkUnits = new List<DynamicLinkUnit>();
		}
		int count = this.linkUnits.Count;
		DynamicLinkUnit dynamicLinkUnit = null;
		for (int i = 0; i < count; i++)
		{
			if (this.linkUnits[i] != null && this.linkUnits[i].mDynamic != null && this.linkUnits[i].mDynamic == unit)
			{
				dynamicLinkUnit = this.linkUnits[i];
				break;
			}
		}
		if (dynamicLinkUnit == null)
		{
			dynamicLinkUnit = new DynamicLinkUnit(unit);
			this.linkUnits.Add(dynamicLinkUnit);
		}
		else
		{
			dynamicLinkUnit.Init();
		}
		if (unit.mDynState == DynamicState.LINK_PARENT)
		{
			unit.mDynState = DynamicState.LINK_PARENT_CHILD;
		}
		else
		{
			unit.mDynState = DynamicState.LINK_CHILD;
		}
		Vector3 eulerAngles = this.rotation.eulerAngles;
		dynamicLinkUnit.SetPositionAndOrient(position, orient);
		Vector3 vector = this.position + dynamicLinkUnit.GetPosition(eulerAngles.y);
		float orient2 = eulerAngles.y * 0.0174532924f + orient;
		unit.OnLinkLocation(vector, orient2);  //子Unit放置到位置
	}
    /// <summary>
    /// 移除指定的子unit
    /// </summary>
    /// <param name="unit"></param>
	public void RemoveLinkDynamic(DynamicUnit unit)
	{
		int count = this.linkUnits.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.linkUnits[i] != null && this.linkUnits[i].mDynamic != null && this.linkUnits[i].mDynamic == unit)
			{
				this.linkUnits[i].Remove();
				this.linkUnits.RemoveAt(i);
				break;
			}
		}
		if (this.linkUnits.Count == 0)
		{
			this.mDynState = DynamicState.NULL;
		}
	}
    /// <summary>
    /// 移除关联的所有子unit
    /// </summary>
	public void RemoveAllLinkDynamic()
	{
		this.mDynState = DynamicState.NULL;
		int count = this.linkUnits.Count;
		for (int i = count - 1; i >= 0; i--)
		{
			if (this.linkUnits[i] != null)
			{
				this.linkUnits[i].Remove();
			}
			this.linkUnits.RemoveAt(i);
		}
	}
    /// <summary>
    /// 设置动态unit动态剔除距离及激活距离
    /// </summary>
    /// <param name="dynamiCullingDistance"></param>
	public void SetCullingDistance(float dynamiCullingDistance)
	{
		if (dynamiCullingDistance > 0f)
		{
			this.near = dynamiCullingDistance;
		}
		else
		{
			this.near = this.scene.terrainConfig.dynamiCullingDistance;
		}
		this.far = this.near + 2f;
	}
    /// <summary>
    /// 更新接口
    /// </summary>
	public override void Update()
	{
		if (this.ins != null)
		{
			if (this.doneOccEffect && this.tick % 40 == 0) //更新材质效果
			{
				if (this.oriMats == null)
				{
					this.oriMats = new List<Material>();
				}
				if (this.renderers == null)
				{
					this.renderers = new List<Renderer>();
				}
				if (this.replaceMats == null)
				{
					this.replaceMats = new Dictionary<string, Material>();
				}
				MeshRenderer[] componentsInChildren = this.ins.GetComponentsInChildren<MeshRenderer>();
				SkinnedMeshRenderer[] componentsInChildren2 = this.ins.GetComponentsInChildren<SkinnedMeshRenderer>();
				if (!this.underwater)
				{
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						MeshRenderer meshRenderer = componentsInChildren[i];
						if (meshRenderer.sharedMaterial != null && meshRenderer.sharedMaterial.shader.name == DynamicUnit.mainUnitOriShaderName)
						{
							if (!this.replaceMats.ContainsKey(meshRenderer.sharedMaterial.name))
							{
								this.oriMats.Add(meshRenderer.sharedMaterial);
								this.renderers.Add(meshRenderer);
								Material material = new Material(DynamicUnit.mainUnitShader);
								material.mainTexture = meshRenderer.sharedMaterial.mainTexture;
								material.color = meshRenderer.sharedMaterial.color;
								material.name = meshRenderer.sharedMaterial.name;
								meshRenderer.sharedMaterial = material;
								this.replaceMats.Add(material.name, material);
							}
							else
							{
								meshRenderer.sharedMaterial = this.replaceMats[meshRenderer.sharedMaterial.name];
							}
						}
					}
					for (int i = 0; i < componentsInChildren2.Length; i++)
					{
						SkinnedMeshRenderer skinnedMeshRenderer = componentsInChildren2[i];
						if (skinnedMeshRenderer.sharedMaterial != null && skinnedMeshRenderer.sharedMaterial.shader.name == DynamicUnit.mainUnitOriShaderName)
						{
							if (!this.replaceMats.ContainsKey(skinnedMeshRenderer.sharedMaterial.name))
							{
								this.oriMats.Add(skinnedMeshRenderer.sharedMaterial);
								this.renderers.Add(skinnedMeshRenderer);
								Material material2 = new Material(DynamicUnit.mainUnitShader);
								material2.mainTexture = skinnedMeshRenderer.sharedMaterial.mainTexture;
								material2.color = skinnedMeshRenderer.sharedMaterial.color;
								material2.name = skinnedMeshRenderer.sharedMaterial.name;
								skinnedMeshRenderer.sharedMaterial = material2;
								this.replaceMats.Add(material2.name, material2);
							}
							else
							{
								skinnedMeshRenderer.sharedMaterial = this.replaceMats[skinnedMeshRenderer.sharedMaterial.name];
							}
						}
					}
				}
				else
				{
					for (int i = 0; i < this.renderers.Count; i++)
					{
						if (this.renderers[i] != null && this.oriMats[i] != null)
						{
							this.renderers[i].sharedMaterial = this.oriMats[i];
						}
					}
				}
			}
			if (this.tick % 10 == 0 && this.oldCastShadows != this.castShadows)  //更新接受阴影设置
			{
				MeshRenderer[] componentsInChildren3 = this.ins.GetComponentsInChildren<MeshRenderer>();
				SkinnedMeshRenderer[] componentsInChildren4 = this.ins.GetComponentsInChildren<SkinnedMeshRenderer>();
				string a = string.Empty;
				if (componentsInChildren3 != null)
				{
					for (int i = 0; i < componentsInChildren3.Length; i++)
					{
						if (componentsInChildren3[i] != null)
						{
							if (componentsInChildren3[i].sharedMaterial != null)
							{
								a = componentsInChildren3[i].sharedMaterial.shader.name;
							}
							if (a == DynamicUnit.mainUnitOriShaderName || a == DynamicUnit.mainUnitOriShaderName)
							{
								componentsInChildren3[i].castShadows = this.castShadows;
							}
						}
					}
				}
				if (componentsInChildren4 != null)
				{
					for (int i = 0; i < componentsInChildren4.Length; i++)
					{
						if (componentsInChildren4[i] != null)
						{
							if (componentsInChildren4[i].sharedMaterial != null)
							{
								a = componentsInChildren4[i].sharedMaterial.shader.name;
							}
							if (a == DynamicUnit.mainUnitOriShaderName || a == DynamicUnit.mainUnitOriShaderName)
							{
								componentsInChildren4[i].castShadows = this.castShadows;
							}
						}
					}
				}
				this.oldCastShadows = this.castShadows;
			}
			if (Mathf.Abs(this.ins.transform.position.x - this.position.x) > 0.01f || Mathf.Abs(this.ins.transform.position.z - this.position.z) > 0.01f)  //位置移动，更新相关
			{
				if (this.needSampleHeight)
				{
					this.smHeight = this.scene.SampleHeight(this.position, true);
					this.position.y = this.smHeight;
				}
				this.ins.transform.position = this.position;
				if ((this.needScreenPoint || this.mouseEnable) && this.scene.mainCamera)
				{
					this.scenePoint.x = this.position.x;
					this.scenePoint.z = this.position.z;
					this.scenePoint.y = this.position.y + this.scenePointBias;
					this.screenPoint = this.scene.mainCamera.WorldToScreenPoint(this.scenePoint);
				}
				if (this.genRipple && this.tick % 4 == 0)     //是否有水波纹效果
				{
					if (this.scene.Underwater(this.position))      //是否水下
					{
						this.ripplePos.x = this.position.x;
						this.ripplePos.z = this.position.z;
						this.ripplePos.y = this.scene.waterHeight;
						Ripple.CreateRippleGameObject(this.ripplePos);
						this.underwater = true;
					}
					else
					{
						this.underwater = false;
					}
				}
			}
			if (this.isMainUint)
			{
				this.smHeight = this.scene.SampleHeight(this.position, true);
				this.position.y = this.smHeight;
				this.ins.transform.position = this.position;
			}
			if (Mathf.Abs(this.ins.transform.rotation.x - this.rotation.x) > 0.01f || Mathf.Abs(this.ins.transform.rotation.y - this.rotation.y) > 0.01f || Mathf.Abs(this.ins.transform.rotation.z - this.rotation.z) > 0.01f)
			{
				this._rotationDirty = true;
			}
			if (this._rotationDirty)
			{
				this.ins.transform.rotation = this.rotation;
				this._rotationDirty = false;
			}
		}
		if (this.genRipple && this.tick % this.genRippleDelayTick == 0)
		{
			if (this.scene.Underwater(this.position))
			{
				this.ripplePos.x = this.position.x;
				this.ripplePos.z = this.position.z;
				this.ripplePos.y = this.scene.waterHeight;
				Ripple.CreateRippleGameObject(this.ripplePos);
				this.underwater = true;
			}
			else
			{
				this.underwater = false;
			}
		}
		if (this.bornEffect != null)  //出生特效位置跟随
		{
			this.bornEffect.transform.position = this.position;
		}
		if (this.mDynState == DynamicState.LINK_PARENT || this.mDynState == DynamicState.LINK_PARENT_CHILD)
		{
			for (int j = 0; j < this.linkUnits.Count; j++)   //相关连子unit更新
			{
				if (this.linkUnits[j] != null)
				{
					this.linkUnits[j].Update(this.position, this.rotation);
				}
			}
		}
		if (this.pathInterrupted)     //如果路径中断
		{
			this.curDelayTick--;
			if (this.curDelayTick < 1 && this.scene.mapPath.pathFindEnd)
			{
				this.FindPathMove(this.pathFindTarget, this.speed, this.continuWithInterr, this.delayTick, true);
			}
		}
		if (this._startMove > 0)
		{
			this._startMove--;
		}
		if (this._endMove > 0)
		{
			this._endMove--;
		}
		if (this.move_type == 0)
		{
			this.UpdateMove();
		}
		else if (this.move_type == 1)
		{
			this.UpdateForce();
		}
		this.tick++;
	}
    /// <summary>
    /// 搜索移动路径
    /// </summary>
    /// <param name="pTarget"></param>
    /// <param name="speed"></param>
    /// <param name="continuWithInterr"></param>
    /// <param name="delayTick"></param>
    /// <param name="sysInvoke"></param>
	public void FindPathMove(Vector3 pTarget, float speed, bool continuWithInterr, int delayTick = 0, bool sysInvoke = false)
	{
		if (!sysInvoke)
		{
			this.Stop();
		}
		this.target = this.position;
		float num = GameScene.mainScene.SampleHeight(pTarget, true);
		if (num < 10f)   //目标点高度小于10，停止寻路
		{
			this.Stop();
			if (GameScene.isEditor)
			{
				LogSystem.LogWarning(new object[]
				{
					string.Concat(new object[]
					{
						this.pfWrongTip,
						this.target,
						"; 失败场景ID:",
						GameScene.mainScene.sceneID
					})
				});
			}
			return;
		}
		this.pathInterrupted = false;
		if (!GameScene.mainScene.IsValidForWalk(pTarget, this.collisionSize)) //目标点不可通行，停止
		{
			float num2 = pTarget.x - this.position.x;
			float num3 = pTarget.z - this.position.z;
			if (Mathf.Sqrt(num2 * num2 + num3 * num3) < 4f)    //碰撞范围内
			{
				this.pathInterrupted = false;
			}
			this.Stop();
			if (GameScene.isEditor)
			{
				LogSystem.LogWarning(new object[]
				{
					string.Concat(new object[]
					{
						this.pfWrongTip,
						this.target,
						"; 失败场景ID:",
						GameScene.mainScene.sceneID
					})
				});
			}
			return;
		}
		this.pathFindEnd = false;
		this.delayTick = delayTick;
		this.curDelayTick = delayTick;
		this.continuWithInterr = continuWithInterr;
		this.speed = speed;
		this.pathFindTarget = pTarget;
		this.scene.mapPath.RequestPaths(this.position, pTarget, this.collisionSize, new MapPath.PathFindEndBack(this.OnPathFindEnd), 8000);
	}
    /// <summary>
    /// 寻路结束回调方法
    /// </summary>
    /// <param name="paths">搜索获取的路径</param>
	public void OnPathFindEnd(List<Vector3> paths)
	{
		this.paths = paths;
		if (paths.Count < 1)
		{
			this.Stop();
			return;
		}
		this.pathInterrupted = false;    //移动开始，标记持续路径运动
		this.Move(paths[0], this.speed);
		if (paths.Count > 1)
		{
			paths.RemoveAt(0);
		}
	}
    /// <summary>
    /// 以指定速度，移动到目标点
    /// </summary>
    /// <param name="target"></param>
    /// <param name="speed"></param>
	public void Move(Vector3 target, float speed)
	{
		if (this.destroyed)
		{
			return;
		}
		this.move_type = 0;
		this._startMove = 1;
		this._endMove = 0;
		this.moving = true;
		this.target = target;
		this.target.y = this.position.y;   //高度应该跟地形高度有关吧?????
		this.start = this.position;
		this.speed = speed;
		this.distance = Vector3.Distance(this.start, this.target);
		this.normalStep = (this.target - this.start).normalized;      //归一化速度方向
		this.step = this.normalStep * (1f / this.scene.fps) * speed;  //每一帧移动的距离
		if (this.moveListener != null)
		{
			this.moveListener(true);
		}
		float y = -57.2957764f * Mathf.Atan2(target.z - this.position.z, target.x - this.position.x) + 90f;
		this.lookAtEuler.y = y;
		this.targetRotation = Quaternion.Euler(this.lookAtEuler);
	}
    /// <summary>
    /// 直接按一定速度移动到指定目标
    /// </summary>
    /// <param name="pTarget"></param>
    /// <param name="speed"></param>
	public void MoveImmediately(Vector3 pTarget, float speed)
	{
		this.Move(pTarget, speed);
		this.paths.RemoveAt(0);
		this.UpdateMove();
	}
    /// <summary>
    /// 移动更新接口,会持续触发
    /// </summary>
	public void UpdateMove()
	{
		this.isBreak = false;
		if (this.moving)
		{
			this.rotation = Quaternion.Lerp(this.rotation, this.targetRotation, 0.35f);
			this._rotationDirty = true;
			this.step = this.normalStep * (1f / this.scene.fps) * this.speed; //单步移动量
			this.start.y = this.position.y;
			this.moveDistance = Vector3.Distance(this.start, this.position);
			if (Mathf.Abs(this.moveDistance - this.distance) < 0.01f || this.moveDistance > this.distance)
			{
				if (this.paths != null && this.paths.Count > 0)
				{
					if (this.pathNextPointListener != null)
					{
						this.pathNextPointListener(this.paths[0]);
					}
					this.MoveImmediately(this.paths[0], this.speed);
				}
				else
				{
					this.position = this.target;
					if (!this.pathFindEnd && this.pathEndListener != null)
					{
						this.pathEndListener();
					}
					this.Stop();
				}
			}
			else if (!this.dontComputeCollision && !this.outObstacles)  //如果没有计算碰撞，或者障碍外部
			{
				this.nextPostion = this.position + this.step;
				if (!this.scene.IsValidForWalk(this.nextPostion, this.collisionSize)) //判断下一个移动点位置是否有效移动点
				{
					if (!this.pathFindEnd)
					{
						if (this.scene.IsValidForWalk(this.nextPostion, 0))
						{
							this.position += this.step;
							return;
						}
						if (this.pathInterruptedListener != null)
						{
							this.pathInterruptedListener(this.position);
						}
						this.pathInterrupted = true;  //如果不可通行，路径中断
						if (this.continuWithInterr && this.delayTick == 0)     //如果中断继续,继续寻路
						{
							this.FindPathMove(this.pathFindTarget, this.speed, this.continuWithInterr, 0, true);
							return;
						}
					}
					this.isBreak = true;
					this.Stop();
				}
				else
				{
					this.position += this.step;  //可通行，继续移动
				}
			}
			else
			{
				this.position += this.step;
			}
		}
	}
    /// <summary>
    /// 指定时间内移动到目标位置
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="time"></param>
    /// <param name="size"></param>
	public void EaseMove(Vector3 dest, float time, int size)
	{
		if (this.destroyed)
		{
			return;
		}
		this.move_type = 1;
		this.target = dest;
		this.start = this.position;
		this.mfEaseTime = time;
		this.scene.mapPath.SetDynamicCollision(this.position, size, true, 1);
	}
    /// <summary>
    /// 更新向目标点强制移动
    /// </summary>
	public void UpdateForce()
	{
		this.mfCurEaseTime += Time.deltaTime;
		float num = Vector3.Distance(this.position, this.target);
		if (num > 0.03f)
		{
			this.position = Vector3.Lerp(this.position, this.target, this.mfCurEaseTime / this.mfEaseTime);
		}
		else
		{
			this.position = this.target;
			this.move_type = -1;
			this.mfEaseTime = 0f;
			this.mfCurEaseTime = 0f;
		}
	}
    /// <summary>
    /// 停止移动，清除路径信息
    /// </summary>
	public void Stop()
	{
		this._endMove = 1;
		this._startMove = 0;
		this.moving = false;
		this.target = this.position;
		if (this.paths != null)
		{
			this.pathFindEnd = true;
			this.paths.Clear();
		}
		if (this.moveListener != null)
		{
			this.moveListener(false);
		}
	}
    /// <summary>
    /// 设置位置到指定位置，及指定偏角
    /// </summary>
    /// <param name="target"></param>
    /// <param name="orient"></param>
	public void OnLocation(Vector3 target, float orient)
	{
		if (this.destroyed || this.scene.mapPath == null)
		{
			return;
		}
		this.scene.mapPath.SetDynamicCollision(this.position, this.collisionSize, true, 1);  //修改该位置相关的网格碰撞状态信息
		this.target = target;
		Quaternion rotation = Quaternion.Euler(0f, orient / 0.0174532924f, 0f);
		this.rotation = rotation;
		this.position = this.target;
	}

	public void OnLinkLocation(Vector3 target, float orient)
	{
		if (this.destroyed || this.scene.mapPath == null)
		{
			return;
		}
		this.target = target;
		Quaternion rotation = Quaternion.Euler(0f, orient / 0.0174532924f, 0f);
		this.rotation = rotation;
		this.position = this.target;
	}

	public bool TryLocationAt(Vector3 postion, out Vector3 target)
	{
		target = Vector3.zero;
		return false;
	}

	public Vector3 TryMove(Vector3 target, out bool isBlocked, bool doneAvoid = true)
	{
		if (this.autoComputeDynCollision)   //自动计算碰撞
		{
			this.dontComputeCollision = false;
		}
		if (this.dontComputeCollision)
		{
			isBlocked = false;
			return target;
		}
		this.outObstacles = false;
		this.from = this.position;
		float num = Vector3.Distance(this.from, target);
		if (num < 0.01f)
		{
			isBlocked = false;
			return target;
		}
		this.lastTarget = this.from;
		this.dir = (target - this.from).normalized;
		this.tryStep = this.dir * this.radius;
		this.nextTarget = this.from + this.tryStep;
		if (!this.scene.IsValidForWalk(this.position, this.collisionSize))
		{
			if (num < 5f)
			{
				num = 5f;
			}
			target = this.from + this.dir * num;
			while (this.scene.getGridType(this.nextTarget) >= 1 || this.scene.IsValidForWalk(this.nextTarget, 0))
			{
				if (this.scene.IsValidForWalk(this.nextTarget, this.collisionSize))
				{
					this.outObstacles = true;
					this.lastTarget = this.nextTarget;
				}
				else
				{
					float num2 = Vector3.Distance(this.from, this.nextTarget);
					if (num2 < num)
					{
						this.lastTarget = this.nextTarget;
						this.nextTarget += this.tryStep;
						continue;
					}
					this.lastTarget = this.position;
				}
				IL_1A6:
				isBlocked = true;
				return this.lastTarget;
			}
			goto IL_1A6;
		}
		if (doneAvoid && !this.scene.IsValidForWalk(this.from + this.dir * 0.3f, this.collisionSize))
		{
			isBlocked = true;
			target.y = this.position.y;
			float num3 = -57.2957764f * Mathf.Atan2(target.z - this.position.z, target.x - this.position.x) + 90f;
			int num4 = this.Sign((double)Mathf.Sin(num3 * 0.0174532924f));
			int num5 = this.Sign((double)Mathf.Cos(num3 * 0.0174532924f));
			this.newTarget.x = this.position.x;
			this.newTarget.z = this.position.z + (float)num5 * 1f;
			if (this.scene.IsValidForWalk(this.newTarget, this.collisionSize))
			{
				return this.newTarget;
			}
			this.newTarget.x = this.position.x + (float)num4 * 1f;
			this.newTarget.z = this.position.z;
			this.newTarget.y = this.position.y;
			if (this.scene.IsValidForWalk(this.newTarget, this.collisionSize))
			{
				return this.newTarget;
			}
		}
		while (this.scene.IsValidForWalk(this.nextTarget, this.collisionSize))
		{
			float num6 = Vector3.Distance(this.from, this.nextTarget);
			if (num6 >= num)
			{
				isBlocked = false;
				return this.nextTarget;
			}
			this.lastTarget = this.nextTarget;
			this.nextTarget += this.tryStep;
		}
		isBlocked = true;
		return this.lastTarget;
	}
    /// <summary>
    /// 符号判定
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
	private int Sign(double value)
	{
		if (Math.Abs(value) < 0.0010000000474974513)
		{
			return 0;
		}
		if (value > 0.0)
		{
			return 1;
		}
		if (value < 0.0)
		{
			return -1;
		}
		return 0;
	}
    /// <summary>
    /// 尝试移动到目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
	public bool TryMove(Vector3 target)
	{
		if (this.autoComputeDynCollision)
		{
			this.dontComputeCollision = true;
		}
		this.from = this.position;
		this.lastTarget = this.from;
		this.dir = (target - this.from).normalized;
		this.tryStep = this.dir * this.radius;
		float num = Vector3.Distance(this.from, target);
		if (num < 0.01f)
		{
			this.tryMoveTarget = this.lastTarget;
			return false;
		}
		this.nextTarget = this.from + this.tryStep;
		while (this.scene.IsValidForWalk(this.nextTarget, this.collisionSize, out this.gridType))
		{
			float num2 = Vector3.Distance(this.from, this.nextTarget);
			if (num2 >= num)
			{
				this.tryMoveTarget = target;
				return false;
			}
			this.lastTarget = this.nextTarget;
			this.nextTarget += this.tryStep;
		}
		this.tryMoveTarget = this.lastTarget;
		return true;
	}
}
