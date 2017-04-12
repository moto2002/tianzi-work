using System;
using UnityEngine;

public class NcBillboard : NcEffectBehaviour
{
	public enum AXIS_TYPE
	{
		AXIS_FORWARD,
		AXIS_BACK,
		AXIS_RIGHT,
		AXIS_LEFT,
		AXIS_UP,
		AXIS_DOWN
	}

	public enum ROTATION
	{
		NONE,
		RND,
		ROTATE
	}

	public enum AXIS
	{
		X,
		Y,
		Z
	}

	private Camera mainCamera;

	private Transform mainCameraTransform;

	private Transform thisTransform;

	public bool m_bCameraLookAt;

	public bool m_bFixedObjectUp;

	public bool m_bFixedStand;

	public NcBillboard.AXIS_TYPE m_FrontAxis;

	public NcBillboard.ROTATION m_RatationMode;

	public NcBillboard.AXIS m_RatationAxis = NcBillboard.AXIS.Z;

	public float m_fRotationValue = 180f;

	protected float m_fRndValue;

	protected float m_fTotalRotationValue;

	protected Quaternion m_qOiginal;

	private void Awake()
	{
		this.mainCamera = Camera.main;
		this.mainCameraTransform = ((!(this.mainCamera == null)) ? this.mainCamera.transform : null);
		this.thisTransform = base.transform;
	}

	private void OnEnable()
	{
		this.UpdateBillboard();
	}

	public void UpdateBillboard()
	{
		this.m_fRndValue = UnityEngine.Random.Range(0f, 360f);
		if (base.enabled)
		{
			this.Update();
		}
	}

	private void Start()
	{
		this.m_qOiginal = base.transform.rotation;
	}

	private void Update()
	{
		if (this.mainCamera == null || this.mainCameraTransform == null)
		{
			return;
		}
		Vector3 worldUp;
		if (this.m_bFixedObjectUp)
		{
			worldUp = base.transform.up;
		}
		else
		{
			worldUp = this.mainCameraTransform.rotation * Vector3.up;
		}
		if (this.m_bCameraLookAt)
		{
			this.thisTransform.LookAt(this.mainCameraTransform, worldUp);
		}
		else
		{
			this.thisTransform.LookAt(this.thisTransform.position + this.mainCameraTransform.rotation * Vector3.back, worldUp);
		}
		switch (this.m_FrontAxis)
		{
		case NcBillboard.AXIS_TYPE.AXIS_BACK:
			this.thisTransform.Rotate(this.thisTransform.up, 180f, Space.World);
			break;
		case NcBillboard.AXIS_TYPE.AXIS_RIGHT:
			this.thisTransform.Rotate(this.thisTransform.up, 270f, Space.World);
			break;
		case NcBillboard.AXIS_TYPE.AXIS_LEFT:
			this.thisTransform.Rotate(this.thisTransform.up, 90f, Space.World);
			break;
		case NcBillboard.AXIS_TYPE.AXIS_UP:
			this.thisTransform.Rotate(this.thisTransform.right, 90f, Space.World);
			break;
		case NcBillboard.AXIS_TYPE.AXIS_DOWN:
			this.thisTransform.Rotate(this.thisTransform.right, 270f, Space.World);
			break;
		}
		if (this.m_bFixedStand)
		{
			this.thisTransform.rotation = Quaternion.Euler(new Vector3(0f, this.thisTransform.rotation.eulerAngles.y, this.thisTransform.rotation.eulerAngles.z));
		}
		if (this.m_RatationMode == NcBillboard.ROTATION.RND)
		{
			this.thisTransform.localRotation *= Quaternion.Euler((this.m_RatationAxis != NcBillboard.AXIS.X) ? 0f : this.m_fRndValue, (this.m_RatationAxis != NcBillboard.AXIS.Y) ? 0f : this.m_fRndValue, (this.m_RatationAxis != NcBillboard.AXIS.Z) ? 0f : this.m_fRndValue);
		}
		if (this.m_RatationMode == NcBillboard.ROTATION.ROTATE)
		{
			float num = this.m_fTotalRotationValue + NcEffectBehaviour.GetEngineDeltaTime() * this.m_fRotationValue;
			this.thisTransform.Rotate((this.m_RatationAxis != NcBillboard.AXIS.X) ? 0f : num, (this.m_RatationAxis != NcBillboard.AXIS.Y) ? 0f : num, (this.m_RatationAxis != NcBillboard.AXIS.Z) ? 0f : num, Space.Self);
			this.m_fTotalRotationValue = num;
		}
	}

	private void OnDestroy()
	{
		this.mainCamera = null;
		this.mainCameraTransform = null;
		this.thisTransform = null;
	}
}
