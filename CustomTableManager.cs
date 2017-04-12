using System;
using UnityEngine;

[AddComponentMenu("NGUI/Custom/TableManager")]
public class CustomTableManager : MonoBehaviour
{
	private class PressTimer
	{
		public bool press;

		public float timer;

		public void Set(bool _press, float _timer)
		{
			this.press = _press;
			this.timer = _timer;
		}
	}

	public CustomScrollView view;

	public MonoBehaviour up;

	public MonoBehaviour down;

	public float dragFower;

	private Vector3 offset;

	private CustomTableManager.PressTimer pTimer = new CustomTableManager.PressTimer();

	private Vector3 dragDelta;

	private void Awake()
	{
		if (this.up != null)
		{
			UIEventListener.Get(this.up.gameObject).onClick = new UIEventListener.VoidDelegate(this.on_click);
			UIEventListener.Get(this.up.gameObject).onPress = new UIEventListener.BoolDelegate(this.on_press);
		}
		if (this.down != null)
		{
			UIEventListener.Get(this.down.gameObject).onClick = new UIEventListener.VoidDelegate(this.on_click);
			UIEventListener.Get(this.down.gameObject).onPress = new UIEventListener.BoolDelegate(this.on_press);
		}
		if (this.dragFower == 0f)
		{
			this.dragFower = 1f;
		}
		Vector3 zero = Vector3.zero;
		zero.x = 0f;
		zero.y = this.dragFower;
		zero.z = 0f;
		this.offset = zero;
		this.pTimer.Set(false, Time.realtimeSinceStartup);
	}

	private void Update()
	{
		if (this.view.shouldMoveHorizontally || this.view.shouldMoveVertically)
		{
			this.up.gameObject.SetActive(true);
			this.down.gameObject.SetActive(true);
		}
		else
		{
			this.up.gameObject.SetActive(false);
			this.down.gameObject.SetActive(false);
		}
		if (this.view == null || !this.pTimer.press)
		{
			return;
		}
		this.view.Drag(this.dragDelta);
	}

	private void on_click(GameObject go)
	{
		if (this.view == null)
		{
			return;
		}
		if (this.pTimer.press || Time.realtimeSinceStartup - this.pTimer.timer < 0.5f)
		{
			return;
		}
		if (go.name == this.up.name)
		{
			this.view.Press(true);
			this.view.Drag(this.offset);
			this.view.Press(false);
		}
		else if (go.name == this.down.name)
		{
			this.view.Press(true);
			this.view.Drag(-this.offset);
			this.view.Press(false);
		}
	}

	private void on_press(GameObject go, bool result)
	{
		if (this.view == null)
		{
			return;
		}
		if (result)
		{
			this.view.Press(true);
			this.pTimer.Set(true, Time.realtimeSinceStartup);
			if (go.name == this.up.name)
			{
				this.dragDelta = Vector3.up * 0.1f;
			}
			else if (go.name == this.down.name)
			{
				this.dragDelta = Vector3.down * 0.1f;
			}
		}
		else
		{
			this.view.Press(false);
			this.pTimer.Set(false, Time.realtimeSinceStartup);
		}
	}
}
