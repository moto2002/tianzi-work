using System;
using UnityEngine;

public class AnimationAsset : ScriptableObject
{
	public AnimationClip mainClip;

	public AnimationClip[] animClip;

	private void OnDestroy()
	{
		this.mainClip = null;
		for (int i = 0; i < this.animClip.Length; i++)
		{
			this.animClip[i] = null;
		}
	}
}
