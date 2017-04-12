using System;
using UnityEngine;

public class WidgetHelper
{
	private static void OnWidgetLoaded(UIWidget[] spList, OnUIWidgetAtlasAllLoaded onAllLoaded, GameObject oGo, params object[] args)
	{
		if (spList == null || oGo == null)
		{
			return;
		}
		for (int i = 0; i < spList.Length; i++)
		{
			UIWidget uIWidget = spList[i];
			if (!(uIWidget == null))
			{
				if (uIWidget.CheckWaitLoadingAtlas())
				{
					return;
				}
			}
		}
		WidgetHelper.ClearSprite(spList);
		if (onAllLoaded != null)
		{
			onAllLoaded(oGo, args);
		}
	}

	private static void ClearSprite(UIWidget[] spList)
	{
		for (int i = 0; i < spList.Length; i++)
		{
			UIWidget uIWidget = spList[i];
			if (!(uIWidget == null))
			{
				uIWidget.ClearWidgetLoadInfo();
			}
		}
	}

	public static void LoadPrefabUISprite(GameObject oGo, OnUIWidgetAtlasAllLoaded onLoaded, params object[] args)
	{
		if (oGo == null)
		{
			if (onLoaded != null)
			{
				onLoaded(oGo, args);
			}
			return;
		}
		UIWidget[] componentsInChildren = oGo.GetComponentsInChildren<UIWidget>(true);
		if (componentsInChildren == null)
		{
			if (onLoaded != null)
			{
				onLoaded(oGo, args);
			}
			return;
		}
		bool flag = false;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UIWidget uIWidget = componentsInChildren[i];
			if (!(uIWidget == null))
			{
				if (uIWidget.CheckLoadAtlas(new OnUIWidgetAtlasLoaded(WidgetHelper.OnWidgetLoaded), componentsInChildren, onLoaded, oGo, args))
				{
					flag = true;
				}
			}
		}
		if (!flag && onLoaded != null)
		{
			onLoaded(oGo, args);
		}
	}
}
