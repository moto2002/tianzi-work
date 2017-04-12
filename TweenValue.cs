using System;
using UnityEngine;

public class TweenValue : UITweener
{
	public float from;

	public float to = 1f;

	public bool updateTable;

	private static TweenValueAdapter mTarget;

	public float value
	{
		get
		{
			return TweenValue.mTarget.value;
		}
		set
		{
			TweenValue.mTarget.value = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Mathf.Lerp(this.from, this.to, factor);
	}

	public static TweenValue Begin(GameObject widget, TweenValueAdapter target, float duration, float value)
	{
		TweenValue.mTarget = target;
		TweenValue tweenValue = UITweener.Begin<TweenValue>(widget, duration);
		tweenValue.from = target.value;
		tweenValue.to = value;
		if (duration <= 0f)
		{
			tweenValue.Sample(1f, true);
			tweenValue.enabled = false;
		}
		return tweenValue;
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		this.value = this.from;
	}

	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		this.value = this.to;
	}
}
