using System;
using UnityEngine;

public class VCButtonWithBehaviours : VCButtonBase
{
	public GameObject upStateObject;

	public GameObject pressedStateObject;

	public GameObject colliderObject;

	protected Behaviour _upBehaviour;

	protected Behaviour _pressedBehavior;

	protected bool _requireCollider = true;

	protected override bool Init()
	{
		if (!base.Init())
		{
			return false;
		}
		this.InitGameObjects();
		this.InitBehaviours();
		return true;
	}

	protected void InitGameObjects()
	{
		if (this.upStateObject == null && this.pressedStateObject == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"No up or pressed state GameObjects specified! Setting upStateObject to this.gameObject."
			});
			this.upStateObject = base.gameObject;
		}
		if (this.colliderObject == null)
		{
			GameObject arg_73_1;
			if ((arg_73_1 = this.upStateObject) == null)
			{
				arg_73_1 = (this.pressedStateObject ?? base.gameObject);
			}
			this.colliderObject = arg_73_1;
		}
		base.InitCollider(this.colliderObject);
		if (this._requireCollider && this._collider == null)
		{
			VCUtils.DestroyWithError(base.gameObject, "colliderObject must have a Collider component!  Destroying this control.");
			return;
		}
	}

	protected virtual void InitBehaviours()
	{
	}

	protected override bool Colliding(VCTouchWrapper tw)
	{
		return base.AABBContains(tw.position);
	}

	protected override void ShowPressedState(bool pressed)
	{
		if (pressed)
		{
			if (this._upBehaviour != null)
			{
				this._upBehaviour.enabled = false;
			}
			if (this._pressedBehavior != null)
			{
				this._pressedBehavior.enabled = true;
			}
		}
		else
		{
			if (this._upBehaviour != null)
			{
				this._upBehaviour.enabled = true;
			}
			if (this._pressedBehavior != null)
			{
				this._pressedBehavior.enabled = false;
			}
		}
	}
}
