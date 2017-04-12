using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectLevel : MonoBehaviour
{
	public static GameQuality iEffectLevel = GameQuality.LOW;

	private static List<EffectLevel> mGoList = new List<EffectLevel>();

	public bool mbHigh;

	public bool mbMiddle;

	public bool mbLow;

	public bool mbMinimalism;

	protected bool mbCurActive;

	public static void SetEffectLevel(GameQuality level)
	{
		if (EffectLevel.iEffectLevel != level)
		{
			EffectLevel.iEffectLevel = level;
			EffectLevel.NoticeLevelChange(EffectLevel.iEffectLevel);
		}
	}

	private static void NoticeLevelChange(GameQuality level)
	{
		for (int i = 0; i < EffectLevel.mGoList.Count; i++)
		{
			EffectLevel effectLevel = EffectLevel.mGoList[i];
			if (effectLevel != null)
			{
				effectLevel.LevelChange(level);
			}
		}
	}

	public static void AddGoList(EffectLevel eLevel)
	{
		if (eLevel != null)
		{
			EffectLevel.mGoList.Add(eLevel);
		}
	}

	public static void ClearList()
	{
		EffectLevel.mGoList.Clear();
	}

	public static void RemoveGoList(EffectLevel eLevel)
	{
		if (eLevel != null && EffectLevel.mGoList.Contains(eLevel))
		{
			EffectLevel.mGoList.Remove(eLevel);
		}
	}

	private void Start()
	{
		EffectLevel.AddGoList(this);
		this.StartELevel();
		this.LevelChange(EffectLevel.iEffectLevel);
	}

	private void Destroy()
	{
		this.DestoryELevel();
		EffectLevel.RemoveGoList(this);
	}

	protected virtual void StartELevel()
	{
	}

	protected virtual void LevelChange(GameQuality level)
	{
		bool flag = false;
		switch (level)
		{
		case GameQuality.SUPER_LOW:
			if (this.mbMinimalism)
			{
				flag = true;
			}
			break;
		case GameQuality.LOW:
			if (this.mbLow)
			{
				flag = true;
			}
			break;
		case GameQuality.MIDDLE:
			if (this.mbMiddle)
			{
				flag = true;
			}
			break;
		case GameQuality.HIGH:
			if (this.mbHigh)
			{
				flag = true;
			}
			break;
		}
		this.mbCurActive = flag;
		this.SetActive(this.mbCurActive);
	}

	protected virtual void SetActive(bool bShow)
	{
	}

	protected virtual void DestoryELevel()
	{
	}
}
