using System;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Spring Panel"), RequireComponent(typeof(UIPanel))]
public class SpringPanel : MonoBehaviour
{
	public delegate void OnFinished();

	public static SpringPanel current;

	public Vector3 target = Vector3.zero;

	public float strength = 10f;

	public SpringPanel.OnFinished onFinished;

	private UIPanel mPanel;

	private Transform mTrans;

	private float mThreshold;

	private UIScrollView mDrag;

	private void Start()
	{
		this.mPanel = base.GetComponent<UIPanel>();
		this.mDrag = base.GetComponent<UIScrollView>();
		this.mTrans = base.transform;
	}

	private void Update()
	{
		this.AdvanceTowardsPosition();
	}

	protected virtual void AdvanceTowardsPosition()
	{
		float deltaTime = RealTime.deltaTime;
		if (this.mThreshold == 0f)
		{
			this.mThreshold = (this.target - this.mTrans.localPosition).magnitude * 0.005f;
			this.mThreshold = Mathf.Max(this.mThreshold, 1E-05f);
			this.mThreshold *= this.mThreshold;
		}
		bool flag = false;
		Vector3 localPosition = this.mTrans.localPosition;
		Vector3 vector = NGUIMath.SpringLerp(this.mTrans.localPosition, this.target, this.strength, deltaTime);
		if (this.mThreshold >= (vector - this.target).sqrMagnitude)
		{
			vector = this.target;
			base.enabled = false;
			flag = true;
		}
		this.mTrans.localPosition = vector;
		Vector3 vector2 = vector - localPosition;
		Vector2 clipOffset = this.mPanel.clipOffset;
		clipOffset.x -= vector2.x;
		clipOffset.y -= vector2.y;
		this.mPanel.clipOffset = clipOffset;
		if (this.mDrag != null)
		{
			this.mDrag.UpdateScrollbars(false);
		}
		if (flag && this.onFinished != null)
		{
			SpringPanel.current = this;
			this.onFinished();
			SpringPanel.current = null;
		}
	}

	public void OnDestroy()
	{
		if (this.onFinished != null)
		{
			this.onFinished = null;
		}
	}

	public static SpringPanel Begin(GameObject go, Vector3 pos, float strength)
	{
		SpringPanel springPanel = go.GetComponent<SpringPanel>();
		if (springPanel == null)
		{
			springPanel = go.AddComponent<SpringPanel>();
		}
		springPanel.target = pos;
		springPanel.strength = strength;
		springPanel.onFinished = null;
		springPanel.mThreshold = 0f;
		springPanel.enabled = true;
		return springPanel;
	}
}
