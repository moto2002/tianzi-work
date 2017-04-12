using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/StateSprite")]
public class StateSprite : UIButtonColor
{
	public static StateSprite current;

	public bool dragHighlight;

	public string hoverSprite;

	public string pressedSprite;

	public string disabledSprite;

	public bool pixelSnap;

	private string mNormalSprite;

	private UISprite mSprite;

	public override bool isEnabled
	{
		get
		{
			if (!base.enabled)
			{
				return false;
			}
			Collider collider = base.collider;
			return collider && collider.enabled;
		}
		set
		{
			Collider collider = base.collider;
			if (collider != null)
			{
				collider.enabled = value;
				this.SetState((!value) ? UIButtonColor.State.Disabled : UIButtonColor.State.Normal, false);
			}
			else
			{
				base.enabled = value;
			}
		}
	}

	public string normalSprite
	{
		get
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			return this.mNormalSprite;
		}
		set
		{
			this.mNormalSprite = value;
			if (this.mState == UIButtonColor.State.Normal)
			{
				this.SetSprite(value);
			}
		}
	}

	protected override void OnInit()
	{
		base.OnInit();
		this.mSprite = (this.mWidget as UISprite);
		if (this.mSprite != null)
		{
			this.mNormalSprite = this.mSprite.spriteName;
		}
	}

	protected override void OnEnable()
	{
		if (this.isEnabled)
		{
			if (this.mInitDone)
			{
				if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
				{
					this.OnHover(UICamera.selectedObject == base.gameObject);
				}
				else if (UICamera.currentScheme == UICamera.ControlScheme.Mouse)
				{
					this.OnHover(UICamera.hoveredObject == base.gameObject);
				}
				else
				{
					this.SetState(UIButtonColor.State.Normal, false);
				}
			}
		}
		else
		{
			this.SetState(UIButtonColor.State.Disabled, true);
		}
	}

	protected override void OnSelect(bool isSelected)
	{
	}

	protected override void OnPress(bool isPressed)
	{
	}

	protected override void OnHover(bool isOver)
	{
	}

	protected override void OnDragOver()
	{
	}

	protected override void OnDragOut()
	{
	}

	protected virtual void OnClick()
	{
	}

	protected override void SetState(UIButtonColor.State state, bool immediate)
	{
		base.SetState(state, immediate);
		switch (state)
		{
		case UIButtonColor.State.Normal:
			this.SetSprite(this.mNormalSprite);
			break;
		case UIButtonColor.State.Hover:
			this.SetSprite(this.hoverSprite);
			break;
		case UIButtonColor.State.Pressed:
			this.SetSprite(this.pressedSprite);
			break;
		case UIButtonColor.State.Disabled:
			this.SetSprite(this.disabledSprite);
			break;
		}
	}

	public void SetButtonState(UIButtonColor.State state)
	{
		base.SetState(state, false);
		switch (state)
		{
		case UIButtonColor.State.Normal:
			this.SetSprite(this.mNormalSprite);
			break;
		case UIButtonColor.State.Hover:
			this.SetSprite(this.hoverSprite);
			break;
		case UIButtonColor.State.Pressed:
			this.SetSprite(this.pressedSprite);
			break;
		case UIButtonColor.State.Disabled:
			this.SetSprite(this.disabledSprite);
			break;
		}
	}

	public void SetSpriteName(string sp)
	{
		if (this.mSprite != null && !string.IsNullOrEmpty(sp) && this.mSprite.spriteName != sp)
		{
			this.mSprite.spriteName = sp;
			if (this.pixelSnap)
			{
				this.mSprite.MakePixelPerfect();
			}
		}
	}

	protected void SetSprite(string sp)
	{
		if (this.mSprite != null && !string.IsNullOrEmpty(sp) && this.mSprite.spriteName != sp)
		{
			this.mSprite.spriteName = sp;
			if (this.pixelSnap)
			{
				this.mSprite.MakePixelPerfect();
			}
		}
	}
}
