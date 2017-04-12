using System;
using UnityEngine;

public class VCButtonGuiTexture : VCButtonWithBehaviours
{
	public Vector2 hitRectScale = new Vector2(1f, 1f);

	protected GUITexture _colliderGuiTexture;

	protected override bool Init()
	{
		this._requireCollider = false;
		if (!base.Init())
		{
			return false;
		}
		if (this._collider == null)
		{
			this._colliderGuiTexture = this.colliderObject.guiTexture;
			if (this._colliderGuiTexture == null)
			{
				VCUtils.DestroyWithError(base.gameObject, "There is no Collider attached to colliderObject, as well as no GUITexture, attach one or the other.  Destroying this control.");
				return false;
			}
		}
		return true;
	}

	protected override void InitBehaviours()
	{
		if (this.upStateObject != null)
		{
			this._upBehaviour = this.upStateObject.guiTexture;
		}
		if (this.pressedStateObject != null)
		{
			this._pressedBehavior = this.pressedStateObject.guiTexture;
		}
	}

	protected override bool Colliding(VCTouchWrapper tw)
	{
		if (this._collider != null)
		{
			return base.AABBContains(tw.position);
		}
		Rect screenRect = this._colliderGuiTexture.GetScreenRect();
		VCUtils.ScaleRect(ref screenRect, this.hitRectScale);
		return screenRect.Contains(tw.position);
	}
}
