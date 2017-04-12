using System;
using UnityEngine;

public class VCButtonEzgui : VCButtonWithBehaviours
{
	protected override bool Init()
	{
		if (!VCPluginSettings.EzguiEnabled(base.gameObject))
		{
			return false;
		}
		if (!base.Init())
		{
			return false;
		}
		if (this.colliderObject == this.upStateObject || this.colliderObject == this.pressedStateObject)
		{
			LogSystem.LogWarning(new object[]
			{
				"VCButtonEZGUI may not behave properly when the hitTestGameObject is the same as the up ",
				"or pressedStateGameObject!  You should add a Collider to a gameObject independent from the EZGUI UI components."
			});
		}
		return true;
	}

	protected override void InitBehaviours()
	{
		this._upBehaviour = this.GetEzguiBehavior(this.upStateObject);
		this._pressedBehavior = this.GetEzguiBehavior(this.pressedStateObject);
	}

	protected override void ShowPressedState(bool pressed)
	{
		if (pressed)
		{
			if (this._upBehaviour != null)
			{
				(this._upBehaviour as SpriteRoot).Hide(true);
			}
			if (this._pressedBehavior != null)
			{
				(this._pressedBehavior as SpriteRoot).Hide(false);
			}
		}
		else
		{
			if (this._upBehaviour != null)
			{
				(this._upBehaviour as SpriteRoot).Hide(false);
			}
			if (this._pressedBehavior != null)
			{
				(this._pressedBehavior as SpriteRoot).Hide(true);
			}
		}
	}

	protected Behaviour GetEzguiBehavior(GameObject go)
	{
		if (go == null)
		{
			return null;
		}
		Behaviour component = go.GetComponent<SimpleSprite>();
		if (component == null)
		{
			component = go.GetComponent<UIButton>();
		}
		return component;
	}
}
