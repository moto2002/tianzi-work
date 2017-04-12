using System;
using System.Collections.Generic;
using UnityEngine;

public class VCAnalogJoystickEzgui : VCAnalogJoystickBase
{
	protected Behaviour _movingPartBehaviorComponent;

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
		if (this.colliderObject == this.movingPart)
		{
			LogSystem.LogWarning(new object[]
			{
				"VCAnalogJoystickEzgui may not behave properly when the colliderObject is the same as the movingPart! ",
				"You should add a Collider to a gameObject independent from the EZGUI UI components."
			});
		}
		this._movingPartBehaviorComponent = this.GetEzguiBehavior(this.movingPart);
		if (this._movingPartBehaviorComponent == null)
		{
			VCUtils.DestroyWithError(base.gameObject, "Cannot find a SimpleSprite or UIButton component on movingPart.  Destroying this control.");
			return false;
		}
		if (this._collider == null)
		{
			VCUtils.DestroyWithError(base.gameObject, "No collider attached to colliderGameObject!  Destroying this control.");
			return false;
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
			this._touchOrigin.Set(this._touchOrigin.x, this._touchOrigin.y, this.basePart.transform.position.z);
			this._touchOriginScreen.x = tw.position.x;
			this._touchOriginScreen.y = tw.position.y;
		}
		if (this.positionAtTouchLocation)
		{
			float z = this.movingPart.transform.localPosition.z;
			this.basePart.transform.localPosition = this._touchOrigin;
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
			this._visibleBehaviourComponents = new List<Behaviour>(this.basePart.GetComponentsInChildren<SimpleSprite>());
			this._visibleBehaviourComponents.AddRange(this.basePart.GetComponentsInChildren<UIButton>());
		}
		foreach (Behaviour current in this._visibleBehaviourComponents)
		{
			(current as SpriteRoot).Hide(!visible);
		}
		this._movingPartVisible = (visible && !this.hideMovingPart);
		(this._movingPartBehaviorComponent as SpriteRoot).Hide(!this._movingPartVisible);
		this._visible = visible;
	}

	protected override void SetPosition(GameObject go, Vector3 vec)
	{
		go.transform.localPosition = vec;
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
