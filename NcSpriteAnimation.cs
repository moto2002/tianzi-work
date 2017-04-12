using System;
using UnityEngine;

public class NcSpriteAnimation : NcEffectAniBehaviour
{
	public enum TEXTURE_TYPE
	{
		TileTexture,
		TrimTexture,
		SpriteFactory
	}

	public enum PLAYMODE
	{
		DEFAULT,
		INVERSE,
		PINGPONG,
		RANDOM,
		SELECT
	}

	public NcSpriteAnimation.TEXTURE_TYPE m_TextureType;

	public NcSpriteAnimation.PLAYMODE m_PlayMode;

	public float m_fDelayTime;

	private bool m_fDelayTimeComplete;

	public int m_nStartFrame;

	public int m_nFrameCount;

	public int m_nSelectFrame;

	public bool m_bLoop = true;

	public bool m_bAutoDestruct;

	public float m_fFps = 10f;

	public int m_nTilingX = 2;

	public int m_nTilingY = 2;

	public GameObject m_NcSpriteFactoryPrefab;

	protected NcSpriteFactory m_NcSpriteFactoryCom;

	public NcSpriteFactory.NcFrameInfo[] m_NcSpriteFrameInfos;

	public float m_fUvScale = 1f;

	public int m_nSpriteFactoryIndex;

	public NcSpriteFactory.MESH_TYPE m_MeshType;

	public NcSpriteFactory.ALIGN_TYPE m_AlignType = NcSpriteFactory.ALIGN_TYPE.CENTER;

	protected bool m_bCreateBuiltInPlane;

	[HideInInspector]
	public bool m_bBuildSpriteObj;

	[HideInInspector]
	public bool m_bNeedRebuildAlphaChannel;

	[HideInInspector]
	public AnimationCurve m_curveAlphaWeight;

	protected Vector2 m_size;

	protected Renderer m_Renderer;

	protected float m_fStartTime;

	protected int m_nLastIndex = -999;

	protected Vector2[] m_MeshUVsByTileTexture;

	private bool isStarted;

	public override int GetAnimationState()
	{
		if (!base.enabled || !NcEffectBehaviour.IsActive(base.gameObject))
		{
			return -1;
		}
		if (this.m_fStartTime == 0f || !base.IsEndAnimation())
		{
			return 1;
		}
		return 0;
	}

	public float GetDurationTime()
	{
		return (float)((this.m_PlayMode != NcSpriteAnimation.PLAYMODE.PINGPONG) ? this.m_nFrameCount : (this.m_nFrameCount * 2 - 1)) / this.m_fFps;
	}

	public int GetShowIndex()
	{
		return this.m_nLastIndex + this.m_nStartFrame;
	}

	public override void ResetAnimation()
	{
		if (this.isStarted)
		{
			base.ResetAnimation();
		}
		base.gameObject.SetActive(true);
		if (this.isStarted)
		{
			this.m_fDelayTimeComplete = false;
			this.m_nLastIndex = -1;
			if (!base.enabled)
			{
				base.enabled = true;
			}
			this.Start();
		}
	}

	public void SetSelectFrame(int nSelFrame)
	{
		this.m_nSelectFrame = nSelFrame;
		this.SetIndex(this.m_nSelectFrame);
	}

	public int GetMaxFrameCount()
	{
		if (this.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
		{
			return this.m_nTilingX * this.m_nTilingY;
		}
		return this.m_NcSpriteFrameInfos.Length;
	}

	public int GetValidFrameCount()
	{
		if (this.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
		{
			return this.m_nTilingX * this.m_nTilingY - this.m_nStartFrame;
		}
		return this.m_NcSpriteFrameInfos.Length - this.m_nStartFrame;
	}

	private void Awake()
	{
		if (this.m_NcSpriteFactoryPrefab == null && base.gameObject.GetComponent<NcSpriteFactory>() != null)
		{
			this.m_NcSpriteFactoryPrefab = base.gameObject;
		}
		if (this.m_NcSpriteFactoryPrefab && this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null)
		{
			this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		}
		if (this.m_MeshFilter == null && base.gameObject.GetComponent<MeshFilter>() != null)
		{
			this.m_MeshFilter = base.gameObject.GetComponent<MeshFilter>();
		}
	}

	private void Start()
	{
		this.isStarted = true;
		this.m_size = new Vector2(1f / (float)this.m_nTilingX, 1f / (float)this.m_nTilingY);
		this.m_Renderer = base.renderer;
		this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
		this.m_nFrameCount = ((this.m_nFrameCount > 0) ? this.m_nFrameCount : (this.m_nTilingX * this.m_nTilingY));
		if (this.m_Renderer == null)
		{
			base.enabled = false;
			return;
		}
		if (this.m_PlayMode == NcSpriteAnimation.PLAYMODE.SELECT)
		{
			this.SetIndex(this.m_nSelectFrame);
		}
		else
		{
			if (0f < this.m_fDelayTime)
			{
				this.m_Renderer.enabled = false;
				return;
			}
			if (this.m_PlayMode == NcSpriteAnimation.PLAYMODE.RANDOM)
			{
				this.SetIndex(UnityEngine.Random.Range(0, this.m_nFrameCount - 1));
			}
			else
			{
				base.InitAnimationTimer();
				this.SetIndex(0);
			}
		}
	}

	private void Update()
	{
		if (this.m_PlayMode == NcSpriteAnimation.PLAYMODE.SELECT)
		{
			return;
		}
		if (this.m_Renderer == null || this.m_nTilingX * this.m_nTilingY == 0)
		{
			return;
		}
		if (!this.m_fDelayTimeComplete)
		{
			if (NcEffectBehaviour.GetEngineTime() < this.m_fStartTime + this.m_fDelayTime)
			{
				return;
			}
			this.m_fDelayTimeComplete = true;
			base.InitAnimationTimer();
			this.m_Renderer.enabled = true;
		}
		if (this.m_PlayMode != NcSpriteAnimation.PLAYMODE.RANDOM)
		{
			int num = (int)(this.m_Timer.GetTime() * this.m_fFps);
			if (num == 0 && this.m_NcSpriteFactoryCom != null)
			{
				this.m_NcSpriteFactoryCom.OnAnimationStartFrame(this);
			}
			if (this.m_NcSpriteFactoryCom != null && this.m_nFrameCount <= 0)
			{
				this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, 0);
			}
			else
			{
				if (((this.m_PlayMode != NcSpriteAnimation.PLAYMODE.PINGPONG) ? this.m_nFrameCount : (this.m_nFrameCount * 2 - 1)) <= num)
				{
					if (!this.m_bLoop)
					{
						if (this.m_NcSpriteFactoryCom != null && this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, 1))
						{
							return;
						}
						base.enabled = false;
						base.OnEndAnimation();
						if (this.m_bAutoDestruct)
						{
							base.gameObject.SetActive(false);
						}
						return;
					}
					else if (this.m_PlayMode == NcSpriteAnimation.PLAYMODE.PINGPONG)
					{
						if (this.m_NcSpriteFactoryCom != null && num % (this.m_nFrameCount * 2 - 2) == 1 && this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, num / (this.m_nFrameCount * 2 - 1)))
						{
							return;
						}
					}
					else if (this.m_NcSpriteFactoryCom != null && num % this.m_nFrameCount == 0 && this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, num / this.m_nFrameCount))
					{
						return;
					}
				}
				this.SetIndex(num);
			}
		}
	}

	public void SetSpriteFactoryIndex(int nSpriteFactoryIndex, bool bRunImmediate)
	{
		if (this.m_NcSpriteFactoryCom == null)
		{
			if (!this.m_NcSpriteFactoryPrefab || !(this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null))
			{
				return;
			}
			this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		}
		NcSpriteFactory.NcSpriteNode spriteNode = this.m_NcSpriteFactoryCom.GetSpriteNode(nSpriteFactoryIndex);
		this.m_bBuildSpriteObj = false;
		this.m_bAutoDestruct = false;
		this.m_fUvScale = this.m_NcSpriteFactoryCom.m_fUvScale;
		this.m_nSpriteFactoryIndex = nSpriteFactoryIndex;
		this.m_nStartFrame = 0;
		this.m_nFrameCount = spriteNode.m_nFrameCount;
		this.m_fFps = spriteNode.m_fFps;
		this.m_bLoop = spriteNode.m_bLoop;
		this.m_NcSpriteFrameInfos = spriteNode.m_FrameInfos;
	}

	private void SetIndex(int nIndex)
	{
		if (this.m_Renderer != null)
		{
			int num = nIndex;
			int nLoopCount = nIndex / this.m_nFrameCount;
			switch (this.m_PlayMode)
			{
			case NcSpriteAnimation.PLAYMODE.DEFAULT:
				num = nIndex % this.m_nFrameCount;
				break;
			case NcSpriteAnimation.PLAYMODE.INVERSE:
				num = this.m_nFrameCount - num % this.m_nFrameCount - 1;
				break;
			case NcSpriteAnimation.PLAYMODE.PINGPONG:
				nLoopCount = num / (this.m_nFrameCount * 2 - ((num != 0) ? 2 : 1));
				num %= this.m_nFrameCount * 2 - ((num != 0) ? 2 : 1);
				if (this.m_nFrameCount <= num)
				{
					num = this.m_nFrameCount - num % this.m_nFrameCount - 2;
				}
				break;
			case NcSpriteAnimation.PLAYMODE.SELECT:
				num = nIndex % this.m_nFrameCount;
				break;
			}
			if (num == this.m_nLastIndex)
			{
				return;
			}
			if (this.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
			{
				int num2 = (num + this.m_nStartFrame) % this.m_nTilingX;
				int num3 = (num + this.m_nStartFrame) / this.m_nTilingX;
				Vector2 mainTextureOffset = new Vector2((float)num2 * this.m_size.x, 1f - this.m_size.y - (float)num3 * this.m_size.y);
				if (!this.UpdateMeshUVsByTileTexture(new Rect(mainTextureOffset.x, mainTextureOffset.y, this.m_size.x, this.m_size.y)))
				{
					this.m_Renderer.material.mainTextureOffset = mainTextureOffset;
					this.m_Renderer.material.mainTextureScale = this.m_size;
				}
			}
			else if (this.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TrimTexture)
			{
				this.UpdateSpriteTexture(num, true);
			}
			else if (this.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.SpriteFactory)
			{
				this.UpdateFactoryTexture(num, true);
			}
			if (this.m_NcSpriteFactoryCom != null)
			{
				this.m_NcSpriteFactoryCom.OnAnimationChangingFrame(this, this.m_nLastIndex, num, nLoopCount);
			}
			this.m_nLastIndex = num;
		}
	}

	private void UpdateSpriteTexture(int nSelIndex, bool bShowEffect)
	{
		nSelIndex += this.m_nStartFrame;
		if (this.m_NcSpriteFrameInfos == null || nSelIndex < 0 || this.m_NcSpriteFrameInfos.Length <= nSelIndex)
		{
			return;
		}
		this.CreateBuiltInPlane(nSelIndex);
		this.UpdateBuiltInPlane(nSelIndex);
	}

	private void UpdateFactoryTexture(int nSelIndex, bool bShowEffect)
	{
		nSelIndex += this.m_nStartFrame;
		if (this.m_NcSpriteFrameInfos == null || nSelIndex < 0 || this.m_NcSpriteFrameInfos.Length <= nSelIndex)
		{
			return;
		}
		if (!this.UpdateFactoryMaterial())
		{
			return;
		}
		if (!this.m_NcSpriteFactoryCom.IsValidFactory())
		{
			return;
		}
		this.CreateBuiltInPlane(nSelIndex);
		this.UpdateBuiltInPlane(nSelIndex);
	}

	public bool UpdateFactoryMaterial()
	{
		if (this.m_NcSpriteFactoryPrefab == null)
		{
			return false;
		}
		if (this.m_NcSpriteFactoryPrefab.renderer == null || this.m_NcSpriteFactoryPrefab.renderer.sharedMaterial == null || this.m_NcSpriteFactoryPrefab.renderer.sharedMaterial.mainTexture == null)
		{
			return false;
		}
		if (base.renderer == null)
		{
			return false;
		}
		if (this.m_NcSpriteFactoryCom == null)
		{
			return false;
		}
		if (this.m_nSpriteFactoryIndex < 0 || this.m_NcSpriteFactoryCom.GetSpriteNodeCount() <= this.m_nSpriteFactoryIndex)
		{
			return false;
		}
		if (this.m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation)
		{
			return false;
		}
		base.renderer.sharedMaterial = this.m_NcSpriteFactoryPrefab.renderer.sharedMaterial;
		return true;
	}

	private void CreateBuiltInPlane(int nSelIndex)
	{
		if (this.m_bCreateBuiltInPlane)
		{
			return;
		}
		this.m_bCreateBuiltInPlane = true;
		if (this.m_MeshFilter == null)
		{
			if (base.gameObject.GetComponent<MeshFilter>() != null)
			{
				this.m_MeshFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			else
			{
				this.m_MeshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
		}
		NcSpriteFactory.CreatePlane(this.m_MeshFilter, this.m_fUvScale, this.m_NcSpriteFrameInfos[nSelIndex], this.m_AlignType, this.m_MeshType);
	}

	private void UpdateBuiltInPlane(int nSelIndex)
	{
		NcSpriteFactory.UpdatePlane(this.m_MeshFilter, this.m_fUvScale, this.m_NcSpriteFrameInfos[nSelIndex], this.m_AlignType);
		NcSpriteFactory.UpdateMeshUVs(this.m_MeshFilter, this.m_NcSpriteFrameInfos[nSelIndex].m_TextureUvOffset);
	}

	private bool UpdateMeshUVsByTileTexture(Rect uv)
	{
		if (this.m_MeshFilter != null && this.m_MeshUVsByTileTexture == null)
		{
			return false;
		}
		if (this.m_MeshFilter == null)
		{
			this.m_MeshFilter = (MeshFilter)base.gameObject.GetComponent(typeof(MeshFilter));
		}
		if (this.m_MeshFilter == null || this.m_MeshFilter.sharedMesh == null)
		{
			return false;
		}
		if (this.m_MeshUVsByTileTexture == null)
		{
			for (int i = 0; i < this.m_MeshFilter.sharedMesh.uv.Length; i++)
			{
				if (this.m_MeshFilter.sharedMesh.uv[i].x != 0f && this.m_MeshFilter.sharedMesh.uv[i].x != 1f)
				{
					return false;
				}
				if (this.m_MeshFilter.sharedMesh.uv[i].y != 0f && this.m_MeshFilter.sharedMesh.uv[i].y != 1f)
				{
					return false;
				}
			}
			this.m_MeshUVsByTileTexture = this.m_MeshFilter.sharedMesh.uv;
		}
		Vector2[] array = new Vector2[this.m_MeshUVsByTileTexture.Length];
		for (int j = 0; j < this.m_MeshUVsByTileTexture.Length; j++)
		{
			if (this.m_MeshUVsByTileTexture[j].x == 0f)
			{
				array[j].x = uv.x;
			}
			if (this.m_MeshUVsByTileTexture[j].y == 0f)
			{
				array[j].y = uv.y;
			}
			if (this.m_MeshUVsByTileTexture[j].x == 1f)
			{
				array[j].x = uv.x + uv.width;
			}
			if (this.m_MeshUVsByTileTexture[j].y == 1f)
			{
				array[j].y = uv.y + uv.height;
			}
		}
		this.m_MeshFilter.mesh.uv = array;
		return true;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDelayTime *= fSpeedRate;
		this.m_fFps *= fSpeedRate;
	}
}
