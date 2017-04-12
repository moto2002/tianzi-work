using System;

public class VCButtonNgui : VCButtonWithBehaviours
{
	protected override bool Init()
	{
		return VCPluginSettings.NguiEnabled(base.gameObject) && base.Init();
	}

	protected override void InitBehaviours()
	{
		if (this.upStateObject != null)
		{
			this._upBehaviour = this.upStateObject.GetComponent<UISprite>();
		}
		if (this.pressedStateObject != null)
		{
			this._pressedBehavior = this.pressedStateObject.GetComponent<UISprite>();
		}
	}

	protected override void ShowPressedState(bool pressed)
	{
		base.ShowPressedState(pressed);
		if (base.Pressed)
		{
			UISprite uISprite = this._pressedBehavior as UISprite;
			if (uISprite != null && uISprite.panel.widgetsAreStatic)
			{
				uISprite.panel.Refresh();
			}
		}
		else
		{
			UISprite uISprite2 = this._upBehaviour as UISprite;
			if (uISprite2 != null && uISprite2.panel.widgetsAreStatic)
			{
				uISprite2.panel.Refresh();
			}
		}
	}

	protected override bool Colliding(VCTouchWrapper tw)
	{
		return base.AABBContains(tw.position);
	}
}
