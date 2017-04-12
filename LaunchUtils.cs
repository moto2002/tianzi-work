using System;
using UnityEngine;

public class LaunchUtils
{
	public static void OnErrorQuit()
	{
		Application.Quit();
	}

	public static void DestroyLaunch(GameObject obj)
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
		DelegateProxy.DestroyObjectImmediate(obj);
	}
}
