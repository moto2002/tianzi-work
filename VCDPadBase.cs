using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class VCDPadBase : MonoBehaviour
{
	public enum EDirection
	{
		None,
		Up,
		Down,
		Left = 4,
		Right = 8
	}

	public string vcName;

	private static Dictionary<string, VCDPadBase> _instancesByVcName;

	public VCAnalogJoystickBase joystick;

	public bool XAxisEnabled = true;

	public bool YAxisEnabled = true;

	public bool debugKeysEnabled;

	public bool debugTouchKeysTogglesPress;

	public KeyCode debugLeftKey = KeyCode.Keypad4;

	public KeyCode debugRightKey = KeyCode.Keypad6;

	public KeyCode debugUpKey = KeyCode.Keypad8;

	public KeyCode debugDownKey = KeyCode.Keypad5;

	protected int _pressedField;

	public bool Up
	{
		get
		{
			return this.Pressed(VCDPadBase.EDirection.Up);
		}
	}

	public bool Down
	{
		get
		{
			return this.Pressed(VCDPadBase.EDirection.Down);
		}
	}

	public bool Left
	{
		get
		{
			return this.Pressed(VCDPadBase.EDirection.Left);
		}
	}

	public bool Right
	{
		get
		{
			return this.Pressed(VCDPadBase.EDirection.Right);
		}
	}

	protected bool JoystickMode
	{
		get
		{
			return this.joystick != null;
		}
	}

	protected void AddInstance()
	{
		if (string.IsNullOrEmpty(this.vcName))
		{
			return;
		}
		if (VCDPadBase._instancesByVcName == null)
		{
			VCDPadBase._instancesByVcName = new Dictionary<string, VCDPadBase>();
		}
		while (VCDPadBase._instancesByVcName.ContainsKey(this.vcName))
		{
			this.vcName += "_copy";
			LogSystem.LogWarning(new object[]
			{
				"Attempting to add instance with duplicate VCName!\nVCNames must be unique -- renaming this instance to " + this.vcName
			});
		}
		VCDPadBase._instancesByVcName.Add(this.vcName, this);
	}

	public static VCDPadBase GetInstance(string vcName)
	{
		if (VCDPadBase._instancesByVcName == null || !VCDPadBase._instancesByVcName.ContainsKey(vcName))
		{
			return null;
		}
		return VCDPadBase._instancesByVcName[vcName];
	}

	protected abstract void SetPressedGraphics(VCDPadBase.EDirection dir, bool pressed);

	public bool Pressed(VCDPadBase.EDirection dir)
	{
		return (this._pressedField & (int)dir) != 0;
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
		if (this.JoystickMode && !this.joystick.measureDeltaRelativeToCenter)
		{
			LogSystem.LogWarning(new object[]
			{
				"DPad in joystickMode may not function correctly when joystick's measureDeltaRelativeToCenter is not true."
			});
		}
		this.AddInstance();
		return true;
	}

	protected virtual void Update()
	{
		if (this.JoystickMode)
		{
			this.UpdateStateJoystickMode();
		}
		else
		{
			this.UpdateStateNonJoystickMode();
		}
	}

	protected virtual void UpdateStateJoystickMode()
	{
		this.SetPressed(VCDPadBase.EDirection.Right, this.joystick.AxisX > 0f && this.XAxisEnabled);
		this.SetPressed(VCDPadBase.EDirection.Left, this.joystick.AxisX < 0f && this.XAxisEnabled);
		this.SetPressed(VCDPadBase.EDirection.Up, this.joystick.AxisY > 0f && this.YAxisEnabled);
		this.SetPressed(VCDPadBase.EDirection.Down, this.joystick.AxisY < 0f && this.YAxisEnabled);
	}

	protected virtual void UpdateStateNonJoystickMode()
	{
	}

	protected virtual void SetPressed(VCDPadBase.EDirection dir, bool pressed)
	{
		if (this.Pressed(dir) == pressed)
		{
			return;
		}
		this.SetBitfield(dir, pressed);
		this.SetPressedGraphics(dir, pressed);
	}

	protected void SetBitfield(VCDPadBase.EDirection dir, bool pressed)
	{
		this._pressedField &= (int)(~(int)dir);
		if (pressed)
		{
			this._pressedField |= (int)dir;
		}
	}

	protected VCDPadBase.EDirection GetOpposite(VCDPadBase.EDirection dir)
	{
		switch (dir)
		{
		case VCDPadBase.EDirection.Up:
			return VCDPadBase.EDirection.Down;
		case VCDPadBase.EDirection.Down:
			return VCDPadBase.EDirection.Up;
		case VCDPadBase.EDirection.Left:
			return VCDPadBase.EDirection.Right;
		case VCDPadBase.EDirection.Right:
			return VCDPadBase.EDirection.Left;
		}
		return VCDPadBase.EDirection.None;
	}
}
