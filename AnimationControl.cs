using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
	public static bool bEnabeld = false;
    /// <summary>
    /// 动画控制列表，包含目前所有的动画控制
    /// </summary>
	public static List<AnimationControl> ListAnimation = new List<AnimationControl>();

	public Animation mAnimation;
    /// <summary>
    /// 启用动画或禁止
    /// </summary>
    /// <param name="bValue"></param>
	public static void SetAniamtionEnabled(bool bValue)
	{
		AnimationControl.bEnabeld = bValue;
		int i = 0;
		int count = AnimationControl.ListAnimation.Count;
		while (i < count)
		{
			AnimationControl animationControl = AnimationControl.ListAnimation[i];
			if (!(animationControl == null) && !(animationControl.mAnimation == null))
			{
				if (!bValue)
				{
					AnimationControl.SetNormalized(animationControl.mAnimation, 0.1f);
				}
				animationControl.mAnimation.enabled = bValue;
			}
			i++;
		}
	}
    /// <summary>
    /// 设置动画播放时间进度
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normal"></param>
    /// <returns></returns>
	private static bool SetNormalized(Animation animation, float normal)
	{
		AnimationState aniaNamePlaying = AnimationControl.GetAniaNamePlaying(animation);
		if (aniaNamePlaying == null)
		{
			return false;
		}
		aniaNamePlaying.normalizedTime = Mathf.Clamp01(normal);
		return true;
	}
    /// <summary>
    /// 获取Animation播放剪辑状态
    /// </summary>
    /// <param name="animation"></param>
    /// <returns></returns>
	public static AnimationState GetAniaNamePlaying(Animation animation)
	{
		if (animation == null || !animation.isPlaying)
		{
			return null;
		}
		foreach (AnimationState animationState in animation)
		{
			if (!(animationState.clip == null))
			{
				if (animation.IsPlaying(animationState.clip.name))
				{
					return animationState;
				}
			}
		}
		return null;
	}
    /// <summary>
    /// 添加动画控制到列表
    /// </summary>
    /// <param name="aniaml"></param>
    /// <returns></returns>
	public static bool AddAnimation(AnimationControl aniaml)
	{
		if (aniaml == null)
		{
			return false;
		}
		if (AnimationControl.ListAnimation.Contains(aniaml))
		{
			return false;
		}
		aniaml.mAnimation.enabled = AnimationControl.bEnabeld;
		AnimationControl.ListAnimation.Add(aniaml);
		return true;
	}
    /// <summary>
    /// 删除动画控制
    /// </summary>
    /// <param name="aniaml"></param>
    /// <returns></returns>
	public static bool DelAniation(AnimationControl aniaml)
	{
		if (aniaml == null)
		{
			return false;
		}
		if (AnimationControl.ListAnimation.Contains(aniaml))
		{
			AnimationControl.ListAnimation.Remove(aniaml);
			return true;
		}
		return false;
	}

	private void Start()
	{
		this.mAnimation = base.GetComponent<Animation>();
		if (this.mAnimation == null)
		{
			UnityEngine.Object.Destroy(this);
		}
		AnimationControl.AddAnimation(this);
	}

	private void OnDestroy()
	{
		AnimationControl.DelAniation(this);
	}
}
