using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/UIButtonEx")]
public class UIButtonEx : UIButton
{
	public GameObject mUIForeSprite;

	public string mstrNormalName;

	public string mstrPressName;

	public string mstrDisableName;

	protected override void SetState(UIButtonColor.State state, bool immediate)
	{
		base.SetState(state, immediate);
		if (this.mUIForeSprite != null)
		{
			UISprite component = this.mUIForeSprite.GetComponent<UISprite>();
			if (component != null)
			{
				if (string.IsNullOrEmpty(this.mstrPressName))
				{
					this.mstrPressName = this.mstrNormalName;
				}
				if (string.IsNullOrEmpty(this.mstrDisableName))
				{
					this.mstrDisableName = this.mstrNormalName;
				}
				switch (state)
				{
				case UIButtonColor.State.Normal:
					component.spriteName = this.mstrNormalName;
					break;
				case UIButtonColor.State.Hover:
					component.spriteName = this.mstrPressName;
					break;
				case UIButtonColor.State.Pressed:
					component.spriteName = this.mstrPressName;
					break;
				case UIButtonColor.State.Disabled:
					component.spriteName = this.mstrDisableName;
					break;
				}
			}
		}
	}

	private void OnDestroy()
	{
		this.mUIForeSprite = null;
	}
}
