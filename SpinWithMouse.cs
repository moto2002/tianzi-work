using System;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Spin With Mouse")]
public class SpinWithMouse : MonoBehaviour
{
	public delegate void OnDragHandle(Vector2 delta);

	public delegate void OnPressHandle(bool isDown);

	public Transform target;

	public float speed = 1f;

	private Transform mTrans;

	public SpinWithMouse.OnDragHandle onDrag;

	public SpinWithMouse.OnPressHandle onPress;

	private void Start()
	{
		this.mTrans = base.transform;
	}

	private void OnDrag(Vector2 delta)
	{
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
		if (this.target != null)
		{
			this.target.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * this.speed, 0f) * this.target.localRotation;
			if (this.onDrag != null)
			{
				this.onDrag(delta);
			}
		}
		else
		{
			this.mTrans.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * this.speed, 0f) * this.mTrans.localRotation;
		}
	}

	private void OnPress(bool isDown)
	{
		if (this.onPress != null)
		{
			this.onPress(isDown);
		}
	}

	private void OnDestroy()
	{
		this.onDrag = null;
		this.onPress = null;
	}
}
