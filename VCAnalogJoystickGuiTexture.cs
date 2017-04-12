using System;
using System.Collections.Generic;
using UnityEngine;

public class VCAnalogJoystickGuiTexture : VCAnalogJoystickBase
{
	public Vector2 hitRectScale = new Vector2(1f, 1f);

	protected GUITexture _movingPartGuiTexture;

	protected override bool Init()
	{
		if (!base.Init())
		{
			return false;
		}
		this._movingPartGuiTexture = this.movingPart.guiTexture;
		return true;
	}

	protected override void InitOriginValues()
	{
		this._touchOrigin = this.movingPart.transform.position;
		Vector2 zero = Vector2.zero;
		zero.x = this._touchOrigin.x * (float)ResolutionConstrain.Instance.width;
		zero.y = this._touchOrigin.y * (float)ResolutionConstrain.Instance.height;
		this._touchOriginScreen = zero;
		this._movingPartOrigin = this.movingPart.transform.position;
	}

	protected override void UpdateDelta()
	{
		base.UpdateDelta();
		this._movingPartOffset.x = this._deltaPixels.x / (float)ResolutionConstrain.Instance.width;
		this._movingPartOffset.y = this._deltaPixels.y / (float)ResolutionConstrain.Instance.height;
	}

	protected override bool Colliding(VCTouchWrapper tw)
	{
		if (!tw.Active)
		{
			return false;
		}
		if (this._collider != null)
		{
			return base.AABBContains(tw.position);
		}
		Rect screenRect = this._movingPartGuiTexture.GetScreenRect();
		VCUtils.ScaleRect(ref screenRect, this.hitRectScale);
		return screenRect.Contains(tw.position);
	}

	protected override void ProcessTouch(VCTouchWrapper tw)
	{
		if (this.measureDeltaRelativeToCenter)
		{
			this._touchOrigin = this.movingPart.transform.position;
			this._touchOriginScreen.x = this._touchOrigin.x * (float)ResolutionConstrain.Instance.width;
			this._touchOriginScreen.y = this._touchOrigin.y * (float)ResolutionConstrain.Instance.height;
		}
		else
		{
			this._touchOrigin.x = tw.position.x / (float)ResolutionConstrain.Instance.width;
			this._touchOrigin.y = tw.position.y / (float)ResolutionConstrain.Instance.height;
			this._touchOrigin.z = this.movingPart.transform.position.z;
			this._touchOriginScreen.x = tw.position.x;
			this._touchOriginScreen.y = tw.position.y;
		}
		if (this.positionAtTouchLocation)
		{
			this._movingPartOrigin = this._touchOrigin;
			Vector3 zero = Vector3.zero;
			zero.x = this._touchOrigin.x;
			zero.y = this._touchOrigin.y;
			zero.z = this.basePart.transform.position.z;
			this.basePart.transform.position = zero;
		}
	}

	protected override void SetVisible(bool visible, bool forceUpdate)
	{
		if (!forceUpdate && this._visible == visible)
		{
			return;
		}
		if (this._visibleBehaviourComponents == null)
		{
			this._visibleBehaviourComponents = new List<Behaviour>(this.basePart.GetComponentsInChildren<GUITexture>());
		}
		foreach (Behaviour current in this._visibleBehaviourComponents)
		{
			current.enabled = visible;
		}
		this._movingPartVisible = (visible && !this.hideMovingPart);
		this.movingPart.guiTexture.enabled = this._movingPartVisible;
		this._visible = visible;
	}
}
