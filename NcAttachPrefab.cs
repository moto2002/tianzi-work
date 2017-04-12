using System;
using UnityEngine;

public class NcAttachPrefab : NcEffectBehaviour
{
	public enum AttachType
	{
		Active,
		Destroy
	}

	public NcAttachPrefab.AttachType m_AttachType;

	public float m_fDelayTime;

	public float m_fRepeatTime;

	public int m_nRepeatCount;

	public GameObject m_AttachPrefab;

	public float m_fPrefabSpeed = 1f;

	public float m_fPrefabLifeTime;

	public bool m_bWorldSpace;

	public Vector3 m_AddStartPos = Vector3.zero;

	public Vector3 m_AccumStartRot = Vector3.zero;

	public Vector3 m_RandomRange = Vector3.zero;

	[HideInInspector]
	public bool m_bDetachParent;

	protected float m_fStartTime;

	protected int m_nCreateCount;

	protected bool m_bStartAttach;

	protected GameObject m_CreateGameObject;

	protected bool m_bEnabled;

	public override int GetAnimationState()
	{
		if (base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && this.m_AttachPrefab != null)
		{
			if (this.m_AttachType == NcAttachPrefab.AttachType.Active && ((this.m_nRepeatCount == 0 && this.m_nCreateCount < 1) || (0f < this.m_fRepeatTime && this.m_nRepeatCount == 0) || (0 < this.m_nRepeatCount && this.m_nCreateCount < this.m_nRepeatCount)))
			{
				return 1;
			}
			if (this.m_AttachType == NcAttachPrefab.AttachType.Destroy)
			{
				return 1;
			}
		}
		return 0;
	}

	public void CreateAttachPrefabImmediately()
	{
		this.Update();
	}

	public GameObject GetInstanceObject()
	{
		if (this.m_CreateGameObject == null)
		{
			this.CreateAttachPrefabImmediately();
		}
		return this.m_CreateGameObject;
	}

	private void Awake()
	{
		this.m_bEnabled = (base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && base.GetComponent<NcDontActive>() == null);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.m_AttachPrefab == null)
		{
			return;
		}
		if (this.m_AttachType == NcAttachPrefab.AttachType.Active)
		{
			if (!this.m_bStartAttach)
			{
				this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
				this.m_bStartAttach = true;
			}
			if (this.m_fStartTime + this.m_fDelayTime <= NcEffectBehaviour.GetEngineTime())
			{
				this.CreateAttachPrefab();
				if ((0f < this.m_fRepeatTime && this.m_nRepeatCount == 0) || this.m_nCreateCount < this.m_nRepeatCount)
				{
					this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
					this.m_fDelayTime = this.m_fRepeatTime;
				}
				else
				{
					base.enabled = false;
				}
			}
		}
	}

	private void OnDestroy()
	{
		if (this.m_bEnabled && NcEffectBehaviour.IsSafe() && this.m_AttachType == NcAttachPrefab.AttachType.Destroy && this.m_AttachPrefab != null)
		{
			this.CreateAttachPrefab();
		}
	}

	private void CreateAttachPrefab()
	{
		this.m_nCreateCount++;
		this.m_CreateGameObject = base.CreateGameObject(this.GetTargetGameObject(), (!(this.GetTargetGameObject() == base.gameObject)) ? base.transform : null, this.m_AttachPrefab);
		if (this.m_CreateGameObject == null)
		{
			return;
		}
		Vector3 position = this.m_CreateGameObject.transform.position;
		this.m_CreateGameObject.transform.position = new Vector3(UnityEngine.Random.Range(-this.m_RandomRange.x, this.m_RandomRange.x) + position.x, UnityEngine.Random.Range(-this.m_RandomRange.y, this.m_RandomRange.y) + position.y, UnityEngine.Random.Range(-this.m_RandomRange.z, this.m_RandomRange.z) + position.z);
		this.m_CreateGameObject.transform.position += this.m_AddStartPos;
		this.m_CreateGameObject.transform.localRotation *= Quaternion.Euler(this.m_AccumStartRot.x * (float)this.m_nCreateCount, this.m_AccumStartRot.y * (float)this.m_nCreateCount, this.m_AccumStartRot.z * (float)this.m_nCreateCount);
		GameObject expr_16C = this.m_CreateGameObject;
		expr_16C.name = expr_16C.name + " " + this.m_nCreateCount;
		NcEffectBehaviour.SetActiveRecursively(this.m_CreateGameObject, true);
		NcEffectBehaviour.AdjustSpeedRuntime(this.m_CreateGameObject, this.m_fPrefabSpeed);
		if (0f < this.m_fPrefabLifeTime)
		{
			NcAutoDestruct ncAutoDestruct = this.m_CreateGameObject.GetComponent<NcAutoDestruct>();
			if (ncAutoDestruct == null)
			{
				ncAutoDestruct = this.m_CreateGameObject.AddComponent<NcAutoDestruct>();
			}
			ncAutoDestruct.m_fLifeTime = this.m_fPrefabLifeTime;
		}
		if (this.m_bDetachParent)
		{
			NcDetachParent x = this.m_CreateGameObject.GetComponent<NcDetachParent>();
			if (x == null)
			{
				x = this.m_CreateGameObject.AddComponent<NcDetachParent>();
			}
		}
		if ((this.m_fRepeatTime == 0f || this.m_AttachType == NcAttachPrefab.AttachType.Destroy) && 0 < this.m_nRepeatCount && this.m_nCreateCount < this.m_nRepeatCount)
		{
			this.CreateAttachPrefab();
		}
	}

	private GameObject GetTargetGameObject()
	{
		if (this.m_bWorldSpace || this.m_AttachType == NcAttachPrefab.AttachType.Destroy)
		{
			return NcEffectBehaviour.GetRootInstanceEffect();
		}
		return base.gameObject;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDelayTime /= fSpeedRate;
		this.m_fRepeatTime /= fSpeedRate;
		this.m_fPrefabLifeTime /= fSpeedRate;
		this.m_fPrefabSpeed *= fSpeedRate;
	}

	public static void Ng_ChangeLayerWithChild(GameObject rootObj, int nLayer)
	{
		if (rootObj == null)
		{
			return;
		}
		rootObj.layer = nLayer;
		for (int i = 0; i < rootObj.transform.childCount; i++)
		{
			NcAttachPrefab.Ng_ChangeLayerWithChild(rootObj.transform.GetChild(i).gameObject, nLayer);
		}
	}
}
