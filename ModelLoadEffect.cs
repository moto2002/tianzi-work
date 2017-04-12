using System;
using System.Collections.Generic;
using UnityEngine;

public class ModelLoadEffect : EffectLevel
{
	public delegate void LoadFuncDelegate(string strFileName, AssetCallBack callback);

	public static ModelLoadEffect.LoadFuncDelegate LoadFunc = null;

	public Action<GameObject> LoadModelComplete;

	public string mstrPath = string.Empty;

	public Vector3 mvecPos = Vector3.zero;

	public Vector3 mvecRot = Vector3.zero;

	public Vector3 mvecScale = Vector3.one;

	public int miRenderQ = 3050;

	private GameObject mGoEffect;

	private bool bLoaded;

	private GameObject mGo;

	private UnityEngine.Object mPrefab;

	private string timerKey;

	public static List<int> instanceIDList = new List<int>();

	private int instanceID;

	private bool mbValidate;

	private bool mbHasLoaded;

	public Action gameObjectActiveCallBack;

	public GameObject GoEffect
	{
		get
		{
			return this.mGoEffect;
		}
	}

	private void Awake()
	{
		this.mGo = base.gameObject;
		this.instanceID = base.GetInstanceID();
	}

	private void OnEnable()
	{
		if (!this.mbHasLoaded)
		{
			return;
		}
		if (!this.CheckEffectValidate())
		{
			this.mbValidate = true;
			if (!string.IsNullOrEmpty(this.mstrPath) && ModelLoadEffect.LoadFunc != null)
			{
				ModelLoadEffect.LoadFunc(this.mstrPath, new AssetCallBack(this.OnValidateLoadComplete));
			}
		}
	}

	private void OnDestroy()
	{
		TimerManager.DestroyTimer(this.timerKey);
		if (this.gameObjectActiveCallBack != null)
		{
			this.gameObjectActiveCallBack = null;
		}
		this.LoadModelComplete = null;
	}

	private void DelayLevelChange()
	{
		if (base.enabled)
		{
			this.LevelChange(EffectLevel.iEffectLevel);
		}
	}

	private void OnDespawned()
	{
		this.mGoEffect = null;
		this.bLoaded = false;
		if (ModelLoadEffect.instanceIDList.Contains(this.instanceID))
		{
			ModelLoadEffect.instanceIDList.Remove(this.instanceID);
		}
		this.LoadModelComplete = null;
	}

	private bool CheckEffectValidate()
	{
		if (this.mGoEffect == null)
		{
			return this.mbValidate;
		}
		return this.mGoEffect.transform.parent == this.mGo.transform;
	}

	protected override void SetActive(bool bShow)
	{
		if (bShow)
		{
			if (this.mGoEffect != null)
			{
				this.mGoEffect.transform.parent = base.transform;
				this.mGoEffect.transform.localPosition = this.mvecPos;
				this.mGoEffect.transform.localRotation = Quaternion.Euler(this.mvecRot);
				this.mGoEffect.transform.localScale = this.mvecScale;
				this.mGoEffect.SetActive(true);
				if (this.gameObjectActiveCallBack != null)
				{
					this.gameObjectActiveCallBack();
				}
			}
			else if (!string.IsNullOrEmpty(this.mstrPath) && ModelLoadEffect.LoadFunc != null && !this.bLoaded)
			{
				this.bLoaded = true;
				if (!ModelLoadEffect.instanceIDList.Contains(this.instanceID))
				{
					ModelLoadEffect.instanceIDList.Add(this.instanceID);
					ModelLoadEffect.LoadFunc(this.mstrPath, new AssetCallBack(this.ResLoadComplete));
				}
			}
		}
		else if (this.mGoEffect != null)
		{
			this.mGoEffect.SetActive(false);
			this.mGoEffect.transform.parent = base.transform;
			this.mGoEffect.transform.localPosition = this.mvecPos;
			this.mGoEffect.transform.localRotation = Quaternion.Euler(this.mvecRot);
			this.mGoEffect.transform.localScale = this.mvecScale;
			if (this.gameObjectActiveCallBack != null)
			{
				this.gameObjectActiveCallBack();
			}
		}
	}

	protected override void DestoryELevel()
	{
		this.mGoEffect = null;
		this.mGo = null;
		this.gameObjectActiveCallBack = null;
		this.LoadModelComplete = null;
		this.mPrefab = null;
	}

	private void OnValidateLoadComplete(params object[] args)
	{
		if (this.mGo == null)
		{
			return;
		}
		this.mPrefab = (args[0] as UnityEngine.Object);
		if (this.mPrefab != null)
		{
			GameObject gameObject = DelegateProxy.Instantiate(this.mPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			if (gameObject != null)
			{
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = this.mvecPos;
				gameObject.transform.localRotation = Quaternion.Euler(this.mvecRot);
				gameObject.transform.localScale = this.mvecScale;
				this.mGoEffect = gameObject;
				NGUITools.SetLayer(this.mGoEffect, this.mGo.layer);
				this.mGoEffect.SetActive(this.mbCurActive);
				this.mbValidate = false;
			}
		}
		if (this.LoadModelComplete != null)
		{
			this.LoadModelComplete(this.mGoEffect);
		}
		if (this.gameObjectActiveCallBack != null)
		{
			this.gameObjectActiveCallBack();
		}
	}

	private void ResLoadComplete(params object[] args)
	{
		if (this.mGo == null)
		{
			return;
		}
		if (!base.enabled)
		{
			this.mGoEffect = null;
			this.bLoaded = false;
			return;
		}
		this.mPrefab = (args[0] as UnityEngine.Object);
		this.DisPlayModelEffect(this.mPrefab, out this.mGoEffect);
		if (this.LoadModelComplete != null)
		{
			this.LoadModelComplete(this.mGoEffect);
		}
		if (this.gameObjectActiveCallBack != null)
		{
			this.gameObjectActiveCallBack();
		}
	}

	private bool DisPlayModelEffect(UnityEngine.Object prefab, out GameObject oEffect)
	{
		if (prefab == null)
		{
			oEffect = null;
			return false;
		}
		oEffect = (DelegateProxy.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject);
		if (oEffect != null)
		{
			oEffect.transform.parent = base.transform;
			oEffect.transform.localPosition = this.mvecPos;
			oEffect.transform.localRotation = Quaternion.Euler(this.mvecRot);
			oEffect.transform.localScale = this.mvecScale;
			NGUITools.SetLayer(oEffect, this.mGo.layer);
			oEffect.SetActive(this.mbCurActive);
			this.mbHasLoaded = true;
			return true;
		}
		return false;
	}
}
