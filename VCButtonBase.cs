using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class VCButtonBase : VCCollideableObject
{
	public delegate void VCButtonDelegate(VCButtonBase button);

	public string vcName;

	protected static Dictionary<string, VCButtonBase> _instancesByVcName;

	public bool touchMustBeginOnCollider = true;

	public bool releaseOnMoveOut = true;

	public bool anyTouchActivatesControl;

	public bool skipCollisionDetection;

	public bool debugKeyEnabled;

	public KeyCode debugTouchKey = KeyCode.A;

	public bool debugTouchKeyTogglesPress;

	protected bool _visible;

	protected bool _pressed;

	protected bool _forcePressed;

	protected bool _lastPressedState;

	protected VCTouchWrapper _touch;

	public VCButtonBase.VCButtonDelegate OnHold;

	public VCButtonBase.VCButtonDelegate OnPress;

	public VCButtonBase.VCButtonDelegate OnRelease;

	public bool Pressed
	{
		get
		{
			return this._pressed;
		}
		private set
		{
			if (this._pressed == value)
			{
				return;
			}
			this._pressed = value;
			if (this._pressed)
			{
				if (this.OnPress != null)
				{
					this.OnPress(this);
				}
			}
			else
			{
				if (this.OnRelease != null)
				{
					this.OnRelease(this);
				}
				this.HoldTime = 0f;
				this._touch = null;
			}
		}
	}

	public bool ForcePressed
	{
		get
		{
			return this._forcePressed;
		}
		set
		{
			this._forcePressed = value;
			if (this._forcePressed)
			{
				this.Pressed = true;
			}
			else
			{
				this.Pressed = !this.PressEnded;
			}
		}
	}

	public float HoldTime
	{
		get;
		private set;
	}

	protected bool PressEnded
	{
		get
		{
			return !this.ForcePressed && (this._touch == null || !this._touch.Active || (!this.anyTouchActivatesControl && this.releaseOnMoveOut && !this.Colliding(this._touch)));
		}
	}

	protected void AddInstance()
	{
		if (string.IsNullOrEmpty(this.vcName))
		{
			return;
		}
		if (VCButtonBase._instancesByVcName == null)
		{
			VCButtonBase._instancesByVcName = new Dictionary<string, VCButtonBase>();
		}
		while (VCButtonBase._instancesByVcName.ContainsKey(this.vcName))
		{
			this.vcName += "_copy";
			LogSystem.LogWarning(new object[]
			{
				"Attempting to add instance with duplicate VCName!\nVCNames must be unique -- renaming this instance to " + this.vcName
			});
		}
		VCButtonBase._instancesByVcName.Add(this.vcName, this);
	}

	public static VCButtonBase GetInstance(string vcName)
	{
		if (VCButtonBase._instancesByVcName == null || !VCButtonBase._instancesByVcName.ContainsKey(vcName))
		{
			return null;
		}
		return VCButtonBase._instancesByVcName[vcName];
	}

	protected abstract void ShowPressedState(bool pressed);

	protected abstract bool Colliding(VCTouchWrapper tw);

	public void ForceRelease()
	{
		this.Pressed = false;
	}

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
			base.gameObject.AddComponent<VCTouchController>();
		}
		this._lastPressedState = true;
		this.Pressed = false;
		this.HoldTime = 0f;
		this.AddInstance();
		return true;
	}

	protected void Update()
	{
		if (!this.skipCollisionDetection)
		{
			if (this.Pressed)
			{
				if (this.PressEnded)
				{
					this.Pressed = false;
				}
				else
				{
					this.HoldTime += Time.deltaTime;
					if (this.OnHold != null)
					{
						this.OnHold(this);
					}
				}
			}
			else
			{
				for (int i = 0; i < VCTouchController.Instance.touches.Count; i++)
				{
					VCTouchWrapper vCTouchWrapper = VCTouchController.Instance.touches[i];
					if (vCTouchWrapper.Active && (!this.touchMustBeginOnCollider || vCTouchWrapper.phase == TouchPhase.Began))
					{
						if (this.anyTouchActivatesControl || this.Colliding(vCTouchWrapper))
						{
							this._touch = vCTouchWrapper;
							this.Pressed = true;
						}
					}
				}
			}
		}
		this.UpdateVisibility();
	}

	protected virtual void UpdateVisibility()
	{
		if (this.Pressed == this._lastPressedState)
		{
			return;
		}
		this._lastPressedState = this.Pressed;
		if (this.Pressed)
		{
			this.ShowPressedState(true);
		}
		else
		{
			this.ShowPressedState(false);
		}
	}
}
