using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class VCAnalogJoystickBase : VCCollideableObject
{
	public delegate void VCJoystickDelegate(VCAnalogJoystickBase joystick);

	public string vcName;

	protected static Dictionary<string, VCAnalogJoystickBase> _instancesByVcName;

	public static List<VCTouchWrapper> touchesInUse = new List<VCTouchWrapper>();

	public GameObject movingPart;

	public GameObject basePart;

	public GameObject colliderObject;

	public bool visibleWhenNotActive = true;

	public bool positionAtTouchLocation;

	public Vector2 positionAtTouchLocationAreaMin = new Vector2(0f, 0f);

	public Vector2 positionAtTouchLocationAreaMax = new Vector2(1f, 1f);

	public bool stopDraggingOnMoveOut = true;

	public bool anyTouchActivatesControl;

	public bool measureDeltaRelativeToCenter;

	public bool hideMovingPart;

	public bool useLateUpdate;

	public bool requireExclusiveTouch;

	public float dragDeltaMagnitudeMaxPixels = 50f;

	public float tapCountResetTime = 0.2f;

	public Vector2 dragScaleFactor = new Vector2(1f, 1f);

	public Vector2 rangeMin = new Vector2(0f, 0f);

	public Vector2 rangeMax = new Vector2(1f, 1f);

	public bool debugKeysEnabled;

	public float debugTouchMovementSpeedPixels = 100f;

	public KeyCode debugTouchKey = KeyCode.LeftControl;

	public bool debugTouchKeyTogglesTouch;

	public KeyCode debugLeftKey = KeyCode.LeftArrow;

	public KeyCode debugRightKey = KeyCode.RightArrow;

	public KeyCode debugUpKey = KeyCode.UpArrow;

	public KeyCode debugDownKey = KeyCode.DownArrow;

	protected float _dragDeltaMagnitudeMaxSq;

	protected Vector3 _movingPartOrigin;

	protected Vector3 _touchOrigin;

	protected Vector2 _touchOriginScreen;

	protected bool _wasDragging;

	protected Vector2 _deltaPixels;

	protected Vector2 _movingPartOffset;

	protected bool _visible = true;

	protected bool _movingPartVisible = true;

	protected List<Behaviour> _visibleBehaviourComponents;

	protected VCTouchWrapper _touch;

	public VCAnalogJoystickBase.VCJoystickDelegate OnDoubleTap;

	private float _tapTime;

	public VCTouchWrapper TouchWrapper
	{
		get
		{
			return this._touch;
		}
	}

	public float AxisX
	{
		get
		{
			return this.RangeAdjust(this._deltaPixels.x / this.dragDeltaMagnitudeMaxPixels, this.rangeMin.x, this.rangeMax.x);
		}
	}

	public float AxisY
	{
		get
		{
			return this.RangeAdjust(this._deltaPixels.y / this.dragDeltaMagnitudeMaxPixels, this.rangeMin.y, this.rangeMax.y);
		}
	}

	public float AxisXRaw
	{
		get
		{
			return this._deltaPixels.x / this.dragDeltaMagnitudeMaxPixels;
		}
	}

	public float AxisYRaw
	{
		get
		{
			return this._deltaPixels.y / this.dragDeltaMagnitudeMaxPixels;
		}
	}

	public float MagnitudeSqr
	{
		get
		{
			return this.AxisX * this.AxisX + this.AxisY * this.AxisY;
		}
	}

	public float AngleRadians
	{
		get
		{
			return Mathf.Atan2(this.AxisY, this.AxisX);
		}
	}

	public float AngleDegrees
	{
		get
		{
			return 57.2957764f * Mathf.Atan2(this.AxisY, this.AxisX) + 180f;
		}
	}

	public int TapCount
	{
		get;
		private set;
	}

	protected float RangeX
	{
		get
		{
			return this.rangeMax.x - this.rangeMin.x;
		}
	}

	protected float RangeY
	{
		get
		{
			return this.rangeMax.y - this.rangeMin.y;
		}
	}

	protected bool Dragging
	{
		get
		{
			return this._touch != null && this._touch.Active;
		}
	}

	protected void AddInstance()
	{
		if (string.IsNullOrEmpty(this.vcName))
		{
			return;
		}
		if (VCAnalogJoystickBase._instancesByVcName == null)
		{
			VCAnalogJoystickBase._instancesByVcName = new Dictionary<string, VCAnalogJoystickBase>();
		}
		if (VCAnalogJoystickBase._instancesByVcName.ContainsKey(this.vcName))
		{
			VCAnalogJoystickBase._instancesByVcName[this.vcName] = this;
		}
		else
		{
			VCAnalogJoystickBase._instancesByVcName.Add(this.vcName, this);
		}
	}

	public static VCAnalogJoystickBase GetInstance(string vcName)
	{
		if (VCAnalogJoystickBase._instancesByVcName == null || !VCAnalogJoystickBase._instancesByVcName.ContainsKey(vcName))
		{
			return null;
		}
		return VCAnalogJoystickBase._instancesByVcName[vcName];
	}

	public static void ClearInstance()
	{
		VCAnalogJoystickBase._instancesByVcName.Clear();
	}

	protected abstract void InitOriginValues();

	protected abstract bool Colliding(VCTouchWrapper tw);

	protected abstract void ProcessTouch(VCTouchWrapper tw);

	protected abstract void SetVisible(bool visible, bool forceUpdate);

	protected void Start()
	{
		this.Init();
	}

	protected virtual bool Init()
	{
		base.useGUILayout = false;
		if (VCTouchController.Instance == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"Cannot find VCTouchController!\nVirtualControls requires a gameObject which has VCTouchController component attached in scene. Adding one for you..."
			});
			VCUtils.AddTouchController(base.gameObject);
		}
		if (!this.movingPart)
		{
			VCUtils.DestroyWithError(base.gameObject, "movingPart is null, VCAnalogJoystick requires it to be assigned to a gameObject! Destroying this control.");
			return false;
		}
		if (this.RangeX <= 0f)
		{
			VCUtils.DestroyWithError(base.gameObject, "rangeMin must be less than rangeMax!  Destroying this control.");
			return false;
		}
		if (this.RangeY <= 0f)
		{
			VCUtils.DestroyWithError(base.gameObject, "rangeMin must be less than rangeMax!  Destroying this control.");
			return false;
		}
		if (this.basePart == null)
		{
			this.basePart = base.gameObject;
		}
		if (this.colliderObject == null)
		{
			this.colliderObject = this.movingPart;
		}
		this._deltaPixels = Vector2.zero;
		this._dragDeltaMagnitudeMaxSq = this.dragDeltaMagnitudeMaxPixels * this.dragDeltaMagnitudeMaxPixels;
		base.InitCollider(this.colliderObject);
		this.InitOriginValues();
		this.TapCount = 0;
		this.AddInstance();
		return true;
	}

	protected virtual void SetPosition(GameObject go, Vector3 vec)
	{
		go.transform.position = vec;
	}

	protected virtual void LateUpdate()
	{
		if (this.useLateUpdate)
		{
			this.PerformUpdate();
		}
	}

	protected virtual void Update()
	{
		if (!this.useLateUpdate)
		{
			this.PerformUpdate();
		}
	}

	protected void PerformUpdate()
	{
		if (Time.time - this._tapTime >= this.tapCountResetTime)
		{
			this.TapCount = 0;
		}
		if (this.Dragging)
		{
			this.UpdateDelta();
			Vector3 zero = Vector3.zero;
			zero.x = this._movingPartOrigin.x + this._movingPartOffset.x;
			zero.y = this._movingPartOrigin.y + this._movingPartOffset.y;
			zero.z = this.movingPart.transform.position.z;
			this.SetPosition(this.movingPart, zero);
			this._wasDragging = true;
			if (this.stopDraggingOnMoveOut && !this.Colliding(this._touch))
			{
				this.StopDragging();
				return;
			}
		}
		else
		{
			if (this._wasDragging)
			{
				this.StopDragging();
			}
			if (VCTouchController.Instance == null || VCTouchController.Instance.touches == null)
			{
				return;
			}
			for (int i = 0; i < VCTouchController.Instance.touches.Count; i++)
			{
				VCTouchWrapper vCTouchWrapper = VCTouchController.Instance.touches[i];
				if (vCTouchWrapper.phase == TouchPhase.Began)
				{
					if (this.anyTouchActivatesControl && this.SetTouch(vCTouchWrapper))
					{
						break;
					}
					if (this.positionAtTouchLocation)
					{
						Vector2 zero2 = Vector2.zero;
						zero2.x = vCTouchWrapper.position.x / (float)ResolutionConstrain.Instance.width;
						zero2.y = vCTouchWrapper.position.y / (float)ResolutionConstrain.Instance.height;
						if (zero2.x < this.positionAtTouchLocationAreaMin.x || zero2.x > this.positionAtTouchLocationAreaMax.x)
						{
							goto IL_223;
						}
						if (zero2.y < this.positionAtTouchLocationAreaMin.y || zero2.y > this.positionAtTouchLocationAreaMax.y)
						{
							goto IL_223;
						}
						if (this.SetTouch(vCTouchWrapper))
						{
							break;
						}
					}
					if (this.Colliding(vCTouchWrapper) && this.SetTouch(vCTouchWrapper))
					{
						break;
					}
				}
				IL_223:;
			}
			if (this._touch != null && this._touch.Active)
			{
				this.ProcessTouch(this._touch);
			}
		}
		this.SetVisible(this.visibleWhenNotActive || this.Dragging, this._movingPartVisible == this.hideMovingPart);
	}

	protected virtual void UpdateDelta()
	{
		this._deltaPixels.x = (this._touch.position.x - this._touchOriginScreen.x) * this.dragScaleFactor.x;
		this._deltaPixels.y = (this._touch.position.y - this._touchOriginScreen.y) * this.dragScaleFactor.y;
		if (this._deltaPixels.sqrMagnitude > this._dragDeltaMagnitudeMaxSq)
		{
			this._deltaPixels = this._deltaPixels.normalized * this.dragDeltaMagnitudeMaxPixels;
		}
		this._movingPartOffset.x = this._deltaPixels.x;
		this._movingPartOffset.y = this._deltaPixels.y;
	}

	protected bool SetTouch(VCTouchWrapper tw)
	{
		if (this.requireExclusiveTouch && VCAnalogJoystickBase.touchesInUse.Any((VCTouchWrapper x) => x.fingerId == tw.fingerId))
		{
			return false;
		}
		this._touch = tw;
		this.TapCount++;
		if (this.TapCount == 1)
		{
			this._tapTime = Time.time;
		}
		else if (this.TapCount > 0 && this.TapCount % 2 == 0 && this.OnDoubleTap != null)
		{
			this.OnDoubleTap(this);
		}
		VCAnalogJoystickBase.touchesInUse.Add(tw);
		return true;
	}

	protected void StopDragging()
	{
		this.SetPosition(this.movingPart, this._movingPartOrigin);
		this._deltaPixels = Vector2.zero;
		this._wasDragging = false;
		VCAnalogJoystickBase.touchesInUse.Remove(this._touch);
		this._touch = null;
	}

	protected float RangeAdjust(float val, float min, float max)
	{
		float num = max - min;
		float num2 = Mathf.Abs(val);
		if (num2 < min)
		{
			return 0f;
		}
		if (num2 > max)
		{
			return 1f * VCUtils.GetSign(val);
		}
		return (num2 - min) / num * VCUtils.GetSign(val);
	}
}
