using System;
using UnityEngine;

public class VCTouchWrapper
{
	public int fingerId;

	public TouchPhase phase;

	public Vector2 position;

	public Vector2 deltaPosition;

	public bool visited;

	public bool debugTouch;

	public bool Active
	{
		get
		{
			return this.phase == TouchPhase.Began || this.phase == TouchPhase.Moved || this.phase == TouchPhase.Stationary;
		}
	}

	public VCTouchWrapper()
	{
		this.visited = false;
		this.position = Vector2.zero;
		this.deltaPosition = Vector2.zero;
		this.phase = TouchPhase.Canceled;
		this.fingerId = -1;
	}

	public VCTouchWrapper(Vector2 position)
	{
		this.visited = true;
		this.position = position;
		this.deltaPosition = Vector2.zero;
		this.fingerId = 0;
		this.phase = TouchPhase.Began;
	}

	public VCTouchWrapper(Touch touch)
	{
		this.Set(touch);
	}

	public void Reset()
	{
		this.visited = false;
		this.position = Vector2.zero;
		this.deltaPosition = Vector2.zero;
		this.phase = TouchPhase.Ended;
		this.fingerId = -1;
		this.debugTouch = false;
	}

	public void Set(Touch touch)
	{
		this.visited = true;
		this.position = touch.position;
		this.deltaPosition = touch.deltaPosition;
		this.phase = touch.phase;
		this.fingerId = touch.fingerId;
	}
}
