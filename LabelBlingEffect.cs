using System;
using UnityEngine;

public class LabelBlingEffect : UITweener
{
	public int timers = 3;

	[Range(0f, 1f)]
	public float from = 1f;

	[Range(0f, 1f)]
	public float to = 1f;

	public Color effectColor;

	private UILabel mLabel;

	private Color originalEffectColor;

	private Color originalColor;

	private int originalTimes;

	private UILabel.Effect originalEffect;

	public UILabel cachedLabel
	{
		get
		{
			if (this.mLabel == null)
			{
				this.mLabel = base.GetComponent<UILabel>();
				if (this.mLabel == null)
				{
					this.mLabel = base.GetComponentInChildren<UILabel>();
				}
			}
			return this.mLabel;
		}
	}

	[Obsolete("Use 'value' instead")]
	public float alpha
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	public float value
	{
		get
		{
			return this.cachedLabel.effectColor.a;
		}
		set
		{
			Color color = this.cachedLabel.effectColor;
			color.a = value;
			this.cachedLabel.effectColor = color;
		}
	}

	private void Awake()
	{
		this.originalTimes = this.timers;
		this.timers *= 2;
		this.originalEffectColor = this.cachedLabel.effectColor;
		this.originalEffect = this.mLabel.effectStyle;
		this.originalColor = this.cachedLabel.color;
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Mathf.Lerp(this.from, this.to, factor);
		if (isFinished)
		{
			this.timers--;
			base.enabled = true;
			base.Toggle();
		}
		if (this.timers < 1)
		{
			base.enabled = false;
			this.StopEffect();
		}
	}

	public static TweenAlpha Begin(GameObject go, float duration, float alpha)
	{
		TweenAlpha tweenAlpha = UITweener.Begin<TweenAlpha>(go, duration);
		tweenAlpha.from = tweenAlpha.value;
		tweenAlpha.to = alpha;
		if (duration <= 0f)
		{
			tweenAlpha.Sample(1f, true);
			tweenAlpha.enabled = false;
		}
		return tweenAlpha;
	}

	public void PlayEffect()
	{
		this.cachedLabel.effectStyle = UILabel.Effect.Outline;
		this.cachedLabel.effectColor = this.effectColor;
		this.cachedLabel.color = Color.yellow;
		this.timers = this.originalTimes * 2;
		base.enabled = true;
		base.PlayForward();
	}

	public void StopEffect()
	{
		this.mLabel.effectStyle = this.originalEffect;
		this.cachedLabel.effectColor = this.originalEffectColor;
		this.cachedLabel.color = this.originalColor;
	}

	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}
}
