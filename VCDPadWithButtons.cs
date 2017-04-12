using System;
using UnityEngine;

public class VCDPadWithButtons : VCDPadBase
{
	public GameObject leftVCButtonObject;

	public GameObject rightVCButtonObject;

	public GameObject upVCButtonObject;

	public GameObject downVCButtonObject;

	public VCButtonBase LeftButton
	{
		get;
		private set;
	}

	public VCButtonBase RightButton
	{
		get;
		private set;
	}

	public VCButtonBase UpButton
	{
		get;
		private set;
	}

	public VCButtonBase DownButton
	{
		get;
		private set;
	}

	protected override bool Init()
	{
		if (!base.Init())
		{
			return false;
		}
		if (this.leftVCButtonObject)
		{
			this.LeftButton = this.leftVCButtonObject.GetComponent<VCButtonBase>();
		}
		if (this.rightVCButtonObject)
		{
			this.RightButton = this.rightVCButtonObject.GetComponent<VCButtonBase>();
		}
		if (this.upVCButtonObject)
		{
			this.UpButton = this.upVCButtonObject.GetComponent<VCButtonBase>();
		}
		if (this.downVCButtonObject)
		{
			this.DownButton = this.downVCButtonObject.GetComponent<VCButtonBase>();
		}
		this.SetPressedGraphics(VCDPadBase.EDirection.Left, false);
		this.SetPressedGraphics(VCDPadBase.EDirection.Right, false);
		this.SetPressedGraphics(VCDPadBase.EDirection.Up, false);
		this.SetPressedGraphics(VCDPadBase.EDirection.Down, false);
		if (base.JoystickMode)
		{
			if (this.LeftButton && !this.LeftButton.skipCollisionDetection)
			{
				LogSystem.LogWarning(new object[]
				{
					"When DPad is in JoystickMode, Buttons should have skipCollisionDetection set to true.  Setting it automatically for LeftButton"
				});
				this.LeftButton.skipCollisionDetection = true;
			}
			if (this.RightButton && !this.RightButton.skipCollisionDetection)
			{
				LogSystem.LogWarning(new object[]
				{
					"When DPad is in JoystickMode, Buttons should have skipCollisionDetection set to true.  Setting it automatically for RightButton"
				});
				this.RightButton.skipCollisionDetection = true;
			}
			if (this.DownButton && !this.DownButton.skipCollisionDetection)
			{
				LogSystem.LogWarning(new object[]
				{
					"When DPad is in JoystickMode, Buttons should have skipCollisionDetection set to true.  Setting it automatically for DownButton"
				});
				this.DownButton.skipCollisionDetection = true;
			}
			if (this.UpButton && !this.UpButton.skipCollisionDetection)
			{
				LogSystem.LogWarning(new object[]
				{
					"When DPad is in JoystickMode, Buttons should have skipCollisionDetection set to true.  Setting it automatically for UpButton"
				});
				this.UpButton.skipCollisionDetection = true;
			}
		}
		return true;
	}

	protected override void SetPressedGraphics(VCDPadBase.EDirection dir, bool pressed)
	{
		if (!base.JoystickMode)
		{
			return;
		}
		if (dir == VCDPadBase.EDirection.Left && this.LeftButton != null)
		{
			this.LeftButton.ForcePressed = pressed;
		}
		if (dir == VCDPadBase.EDirection.Right && this.RightButton != null)
		{
			this.RightButton.ForcePressed = pressed;
		}
		if (dir == VCDPadBase.EDirection.Up && this.UpButton != null)
		{
			this.UpButton.ForcePressed = pressed;
		}
		if (dir == VCDPadBase.EDirection.Down && this.DownButton != null)
		{
			this.DownButton.ForcePressed = pressed;
		}
	}

	protected override void UpdateStateNonJoystickMode()
	{
		if (this.XAxisEnabled)
		{
			this.SetPressed(VCDPadBase.EDirection.Left, this.ButtonExistsAndIsPressed(this.LeftButton));
			this.SetPressed(VCDPadBase.EDirection.Right, this.ButtonExistsAndIsPressed(this.RightButton));
		}
		if (this.YAxisEnabled)
		{
			this.SetPressed(VCDPadBase.EDirection.Up, this.ButtonExistsAndIsPressed(this.UpButton));
			this.SetPressed(VCDPadBase.EDirection.Down, this.ButtonExistsAndIsPressed(this.DownButton));
		}
	}

	protected bool ButtonExistsAndIsPressed(VCButtonBase button)
	{
		return !(button == null) && button.Pressed;
	}
}
