using System;

public static class SpriteAnimationer
{
	public static void InitLimitLoopSrpiteAnimation(UISpriteAnimation animation, int loopTime = 1)
	{
		if (animation == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"animation is null"
			});
			return;
		}
		animation.loop = false;
		animation.gameObject.SetActive(false);
		animation.Reset(loopTime);
		animation.m_callBack = delegate
		{
			animation.Replay();
			animation.gameObject.SetActive(false);
		};
	}

	public static void PlayAnimation(UISpriteAnimation animation)
	{
		if (animation == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"animation is null"
			});
			return;
		}
		if (animation.loop)
		{
			return;
		}
		animation.gameObject.SetActive(true);
	}
}
