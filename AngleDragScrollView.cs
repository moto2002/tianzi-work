using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Angle Drag Scroll View")]
public class AngleDragScrollView : MonoBehaviour
{
	public AngleScrollView scrollView;

	[HideInInspector, SerializeField]
	private AngleScrollView draggablePanel;

	private Transform mTrans;

	private AngleScrollView mScroll;

	private bool mAutoFind;

	private void OnEnable()
	{
		this.mTrans = base.transform;
		if (this.scrollView == null && this.draggablePanel != null)
		{
			this.scrollView = this.draggablePanel;
			this.draggablePanel = null;
		}
		if (this.mAutoFind || this.mScroll == null)
		{
			this.FindScrollView();
		}
	}

	private void FindScrollView()
	{
		AngleScrollView y = NGUITools.FindInParents<AngleScrollView>(this.mTrans);
		if (this.scrollView == null)
		{
			this.scrollView = y;
			this.mAutoFind = true;
		}
		else if (this.scrollView == y)
		{
			this.mAutoFind = true;
		}
		this.mScroll = this.scrollView;
	}

	private void Start()
	{
		this.FindScrollView();
	}

	private void OnPress(bool pressed)
	{
		if (this.mAutoFind && this.mScroll != this.scrollView)
		{
			this.mScroll = this.scrollView;
			this.mAutoFind = false;
		}
		if (this.scrollView && base.enabled && NGUITools.GetActive(base.gameObject))
		{
			this.scrollView.Press(pressed);
			if (!pressed && this.mAutoFind)
			{
				this.scrollView = NGUITools.FindInParents<AngleScrollView>(this.mTrans);
				this.mScroll = this.scrollView;
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (this.scrollView && NGUITools.GetActive(this))
		{
			this.scrollView.Drag();
		}
	}

	private void OnScroll(float delta)
	{
		if (this.scrollView && NGUITools.GetActive(this))
		{
			this.scrollView.Scroll(delta);
		}
	}
}
