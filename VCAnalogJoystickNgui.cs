using System;
using System.Collections.Generic;
using UnityEngine;

public class VCAnalogJoystickNgui : VCAnalogJoystickBase
{
	protected UISprite _movingPartSprite;

	protected override bool Init()
	{
		if (!VCPluginSettings.NguiEnabled(base.gameObject))
		{
			return false;
		}
		if (!base.Init())
		{
			return false;
		}
		if (this._collider == null)
		{
			VCUtils.DestroyWithError(base.gameObject, "No collider attached to colliderGameObject!  Destroying this gameObject.");
			return false;
		}
		this._movingPartSprite = this.movingPart.GetComponent<UISprite>();
		if (this._movingPartSprite == null)
		{
			this._movingPartSprite = this.movingPart.GetComponentInChildren<UISprite>();
		}
		return true;
	}

	protected override void InitOriginValues()
	{
		this._touchOrigin = this._colliderCamera.WorldToViewportPoint(this.movingPart.transform.position);
		this._touchOriginScreen = this._colliderCamera.WorldToScreenPoint(this.movingPart.transform.position);
		this._movingPartOrigin = this.movingPart.transform.localPosition;
	}

	protected override bool Colliding(VCTouchWrapper tw)
	{
		return tw.Active && base.AABBContains(tw.position);
	}

	protected override void ProcessTouch(VCTouchWrapper tw)
	{
		if (this.measureDeltaRelativeToCenter)
		{
			this._touchOrigin = this.movingPart.transform.position;
			this._touchOriginScreen = this._colliderCamera.WorldToScreenPoint(this.movingPart.transform.position);
		}
		else
		{
			this._touchOrigin = this._colliderCamera.ScreenToWorldPoint(tw.position);
			this._touchOriginScreen.x = tw.position.x;
			this._touchOriginScreen.y = tw.position.y;
		}
		if (this.positionAtTouchLocation)
		{
			float z = this.movingPart.transform.localPosition.z;
			this.basePart.transform.position = this._touchOrigin;
			this.movingPart.transform.position = this._touchOrigin;
			this._movingPartOrigin.Set(this.movingPart.transform.localPosition.x, this.movingPart.transform.localPosition.y, z);
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
			this._visibleBehaviourComponents = new List<Behaviour>(this.basePart.GetComponentsInChildren<UISprite>());
		}
		for (int i = 0; i < this._visibleBehaviourComponents.Count; i++)
		{
			Behaviour behaviour = this._visibleBehaviourComponents[i];
			behaviour.enabled = visible;
		}
		this._movingPartVisible = (visible && !this.hideMovingPart);
		this._movingPartSprite.enabled = this._movingPartVisible;
		this._visible = visible;
	}

	protected override void SetPosition(GameObject go, Vector3 vec)
	{
		if (go != null)
		{
			go.transform.localPosition = vec;
		}
	}

	protected override void UpdateDelta()
	{
		base.UpdateDelta();
	}
}
