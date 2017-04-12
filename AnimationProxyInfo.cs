using System;
using UnityEngine;

public class AnimationProxyInfo : ScriptableObject
{
	[SerializeField]
	public AnimationProxy.AnimationInfo[] mAnimations;

	[SerializeField]
	public AnimationProxy.AnimationInfo mMainClip;

	public string aniID;
}
