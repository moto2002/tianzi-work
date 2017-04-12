using System;
using UnityEngine;

public class OrderButton : UIButton
{
	private UILabel label;

	public static OrderButton Current;

	public string Text
	{
		get
		{
			if (this.label != null)
			{
				return this.label.originalText;
			}
			return string.Empty;
		}
	}

	protected override void OnInit()
	{
		base.OnInit();
		Transform transform = base.transform.FindChild("Content");
		if (transform != null)
		{
			this.label = transform.GetComponent<UILabel>();
		}
	}

	public new void SetState(UIButtonColor.State state, bool immediate)
	{
		base.SetState(state, immediate);
		if (this.label != null)
		{
			switch (state)
			{
			case UIButtonColor.State.Normal:
				if (this.label.gameObject.activeSelf)
				{
					this.label.gameObject.SetActive(false);
				}
				break;
			case UIButtonColor.State.Hover:
				if (!this.label.gameObject.activeSelf)
				{
					this.label.gameObject.SetActive(true);
				}
				break;
			case UIButtonColor.State.Pressed:
				if (!this.label.gameObject.activeSelf)
				{
					this.label.gameObject.SetActive(true);
				}
				break;
			case UIButtonColor.State.Disabled:
				if (this.label.gameObject.activeSelf)
				{
					this.label.gameObject.SetActive(false);
				}
				break;
			}
		}
	}
}
