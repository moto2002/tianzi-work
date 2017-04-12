using System;
using SystemHelper.Duplicator;
using UnityDevelopment.Calculator;
using UnityEngine;

public class NcAdvanceDuplicator : NcEffectBehaviour
{
	public float m_fDuplicateTime = 0.1f;

	public bool m_bSim;

	public int m_nDuplicateCount = 3;

	public float m_fDuplicateLifeTime;

	public Vector3 m_AddStartPos = Vector3.zero;

	public Vector3 m_AccumStartRot = Vector3.zero;

	public Vector3 m_RandomRange = Vector3.zero;

	public Vector3[] m_TargetPos;

	public Vector3[] m_TargetScale;

	protected int m_nCreateCount = 1;

	protected float m_fStartTime;

	public int positionMask;

	public int scaleMask;

	public Vector3 m_AddStartScale;

	private DuplicatorBase<GameObject> duplicator;

	private TimedPulser pulser;

	private GameObject clone;

	private void Awake()
	{
		this.pulser = new TimedPulser(this.m_fDuplicateTime);
		this.duplicator = DuplicatorBase<GameObject>.create((!this.m_bSim) ? DuplicationStrategyType.SmoothGeneration : DuplicationStrategyType.InstantGeneration, this.m_nDuplicateCount - 1, new Func<GameObject>(this.generateInstance));
	}

	private void Start()
	{
		this.clone = (UnityEngine.Object.Instantiate(base.gameObject) as GameObject);
		this.clone.transform.parent = base.transform.parent;
		this.clone.transform.localScale = base.transform.localScale;
		this.clone.transform.localRotation = base.transform.localRotation;
		this.clone.transform.localPosition = base.transform.localPosition;
		this.clone.name = base.gameObject.name + " Clone ";
		this.clone.SetActive(false);
	}

	private GameObject generateInstance()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.clone) as GameObject;
		gameObject.transform.parent = base.transform.parent;
		gameObject.transform.localScale = this.clone.transform.localScale;
		gameObject.transform.localRotation = this.clone.transform.localRotation;
		gameObject.transform.localPosition = this.clone.transform.localPosition;
		UnityEngine.Object.Destroy(gameObject.GetComponent<NcAdvanceDuplicator>());
		Vector3 a = gameObject.transform.localPosition;
		switch (this.positionMask)
		{
		case 0:
			a += this.m_AddStartPos;
			break;
		case 1:
			a += this.m_AddStartPos * (float)this.m_nCreateCount;
			break;
		case 2:
			a = this.m_TargetPos[this.m_nCreateCount];
			break;
		}
		gameObject.transform.localPosition = new Vector3(UnityEngine.Random.Range(-this.m_RandomRange.x, this.m_RandomRange.x) + a.x, UnityEngine.Random.Range(-this.m_RandomRange.y, this.m_RandomRange.y) + a.y, UnityEngine.Random.Range(-this.m_RandomRange.z, this.m_RandomRange.z) + a.z);
		gameObject.transform.localRotation *= Quaternion.Euler(this.m_AccumStartRot.x * (float)this.m_nCreateCount, this.m_AccumStartRot.y * (float)this.m_nCreateCount, this.m_AccumStartRot.z * (float)this.m_nCreateCount);
		GameObject expr_1CA = gameObject;
		expr_1CA.name = expr_1CA.name + " " + this.m_nCreateCount;
		switch (this.scaleMask)
		{
		case 1:
			gameObject.transform.localScale = this.m_AddStartScale * (float)(this.m_nCreateCount + 1);
			break;
		case 2:
			gameObject.transform.localScale = this.m_TargetScale[this.m_nCreateCount];
			break;
		}
		this.m_nCreateCount++;
		gameObject.SetActive(false);
		gameObject.SetActive(true);
		return gameObject;
	}

	private void Update()
	{
		if (this.m_bSim || this.pulser.pulse())
		{
			this.duplicator.operation();
		}
	}

	private void OnDestroy()
	{
		if (this.duplicator != null)
		{
			this.duplicator.clear();
		}
		this.clone = null;
	}
}
