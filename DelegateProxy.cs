using System;
using System.Collections.Generic;
using UnityEngine;

public class DelegateProxy
{
	public delegate void PopCacheDelegateProxy(UnityEngine.Object o);

	public delegate string StringBuilderDelegateProxy(params object[] args);

	public delegate void SetObjRenderQDelegateProxy(GameObject oModel, int iLayer, int iRenderQueue);

	public delegate void GameDestroyImmediate(UnityEngine.Object obj);

	public delegate UnityEngine.Object DelegateInstantiate(UnityEngine.Object o, Vector3 postion, Quaternion rotation);

	public delegate UnityEngine.Object DelegateInstantiateNoParam(UnityEngine.Object o);

	public delegate void GameDestoryDelegateProxy(UnityEngine.Object obj);

	public delegate void GameDestoryPoolDelegateProxy(GameObject obj);

	public delegate void ShowViewDelegateProxy(string name, object arg = null);

	public delegate void DestroyViewDelegateProxy(string name);

	public delegate void LoadAssetDelegateProxy(string strFileName, AssetCallBack back);

	public delegate void UnloadAssetDelegateProxy(object[] args);

	public delegate void DestroyEffectDelegateProxy(GameObject obj, int layer);

	public delegate bool HasViewDelegateProxy(string name);

	public delegate void HideViewDelegateProxy(string name);

	public delegate void PlayUIAudioDelegateProxy(int index);

	public delegate void GetGuiCompentDelegateProxy(Dictionary<int, GameObject> lastgameObject, ref List<Component> newPanels, ref List<Component> tempPanels);

	public delegate void OnShareCallbackDelegateProxy(string result);

	public delegate void OnSetPayCallbackDelegateProxy(List<IAPProductInfo> productInfos);

	public delegate void OnVerifyTransCallbackDelegateProxy(string pid, int amount, string transactionData, string transactionIdentifier, string account, string rolename, string serverid, string accountid);

	public delegate bool OnSendMessageCallbackDelegateProxy(int iMSG, params object[] objects);

	public static DelegateProxy.PopCacheDelegateProxy PopCacheProxy;

	public static DelegateProxy.StringBuilderDelegateProxy StringBuilderPorxy;

	public static DelegateProxy.GameDestroyImmediate mGameDestroyImme;

	public static DelegateProxy.DelegateInstantiate mDelegateInstantiate;

	public static DelegateProxy.DelegateInstantiateNoParam mDelegateInstantiateNoParam;

	public static DelegateProxy.GameDestoryDelegateProxy GameDestoryProxy;

	public static DelegateProxy.GameDestoryPoolDelegateProxy GameDestoryPoolProxy;

	public static DelegateProxy.ShowViewDelegateProxy ShowViewProxy;

	public static DelegateProxy.DestroyViewDelegateProxy DestroyViewProxy;

	public static DelegateProxy.LoadAssetDelegateProxy LoadAssetProxy;

	public static DelegateProxy.UnloadAssetDelegateProxy UnloadAssetProxy;

	public static DelegateProxy.DestroyEffectDelegateProxy DestroyEffectProxy;

	public static DelegateProxy.HasViewDelegateProxy HasViewProxy;

	public static DelegateProxy.HideViewDelegateProxy HideViewProxy;

	public static DelegateProxy.PlayUIAudioDelegateProxy PlayUIAudioProxy;

	public static DelegateProxy.GetGuiCompentDelegateProxy GetGuiCompentProxy;

	public static DelegateProxy.OnShareCallbackDelegateProxy OnShareCallbackProxy;

	public static DelegateProxy.OnSetPayCallbackDelegateProxy OnSetPayCallbackProxy;

	public static DelegateProxy.OnVerifyTransCallbackDelegateProxy OnVerifyTransCallbackProxy;

	public static DelegateProxy.OnSendMessageCallbackDelegateProxy onSendMessageCallbackDelegateProxy;

	public static void PopCache(UnityEngine.Object o)
	{
		if (DelegateProxy.PopCacheProxy != null)
		{
			DelegateProxy.PopCacheProxy(o);
		}
	}

	public static string StringBuilder(params object[] args)
	{
		if (DelegateProxy.StringBuilderPorxy != null)
		{
			return DelegateProxy.StringBuilderPorxy(args);
		}
		return string.Empty;
	}

	public static void DestroyObjectImmediate(UnityEngine.Object obj)
	{
		if (DelegateProxy.mGameDestroyImme != null)
		{
			DelegateProxy.mGameDestroyImme(obj);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
	}

	public static UnityEngine.Object Instantiate(UnityEngine.Object o, Vector3 postion, Quaternion rotation)
	{
		if (DelegateProxy.mDelegateInstantiate != null)
		{
			return DelegateProxy.mDelegateInstantiate(o, postion, rotation);
		}
		return UnityEngine.Object.Instantiate(o, postion, rotation);
	}

	public static UnityEngine.Object Instantiate(UnityEngine.Object o)
	{
		if (DelegateProxy.mDelegateInstantiateNoParam != null)
		{
			return DelegateProxy.mDelegateInstantiateNoParam(o);
		}
		return UnityEngine.Object.Instantiate(o);
	}

	public static void GameDestory(UnityEngine.Object obj)
	{
		if (DelegateProxy.GameDestoryProxy != null)
		{
			DelegateProxy.GameDestoryProxy(obj);
		}
	}

	public static void GamePoolDestory(GameObject obj)
	{
		if (DelegateProxy.GameDestoryPoolProxy != null)
		{
			DelegateProxy.GameDestoryPoolProxy(obj);
		}
	}

	public static void ShowView(string name, object arg = null)
	{
		if (DelegateProxy.ShowViewProxy != null)
		{
			DelegateProxy.ShowViewProxy(name, arg);
		}
	}

	public static void DestroyView(string name)
	{
		if (DelegateProxy.DestroyViewProxy != null)
		{
			DelegateProxy.DestroyViewProxy(name);
		}
	}

	public static void LoadAsset(string strFileName, AssetCallBack callback)
	{
		if (DelegateProxy.LoadAssetProxy != null)
		{
			DelegateProxy.LoadAssetProxy(strFileName, callback);
		}
	}

	public static void UnloadAsset(object[] args)
	{
		if (DelegateProxy.UnloadAssetProxy != null)
		{
			DelegateProxy.UnloadAssetProxy(args);
		}
	}

	public static void DestroyEffect(GameObject obj, int layer)
	{
		if (DelegateProxy.DestroyEffectProxy != null)
		{
			DelegateProxy.DestroyEffectProxy(obj, layer);
		}
	}

	public static bool HasView(string name)
	{
		return DelegateProxy.HasViewProxy != null && DelegateProxy.HasViewProxy(name);
	}

	public static void HideView(string name)
	{
		if (DelegateProxy.HideViewProxy != null)
		{
			DelegateProxy.HideViewProxy(name);
		}
	}

	public static void PlayUIAudio(int index)
	{
		if (DelegateProxy.PlayUIAudioProxy != null)
		{
			DelegateProxy.PlayUIAudioProxy(index);
		}
	}

	public static void GetGuiCompent(Dictionary<int, GameObject> lastgameObject, ref List<Component> newPanels, ref List<Component> tempPanels)
	{
		if (DelegateProxy.GetGuiCompentProxy != null)
		{
			DelegateProxy.GetGuiCompentProxy(lastgameObject, ref newPanels, ref tempPanels);
		}
	}

	public static void OnShareCallback(string result)
	{
		if (DelegateProxy.OnShareCallbackProxy != null)
		{
			DelegateProxy.OnShareCallbackProxy(result);
		}
	}

	public static void OnSetPayCallback(List<IAPProductInfo> productInfos)
	{
		if (DelegateProxy.OnSetPayCallbackProxy != null)
		{
			DelegateProxy.OnSetPayCallbackProxy(productInfos);
		}
	}

	public static void OnVerifyTransCallback(string pid, int amount, string transactionData, string transactionIdentifier, string account, string rolename, string serverid, string accountid)
	{
		if (DelegateProxy.OnVerifyTransCallbackProxy != null)
		{
			DelegateProxy.OnVerifyTransCallbackProxy(pid, amount, transactionData, transactionIdentifier, account, rolename, serverid, accountid);
		}
	}

	public static void OnSendMessageCallback(int iMSG, params object[] objects)
	{
		if (DelegateProxy.onSendMessageCallbackDelegateProxy != null)
		{
			DelegateProxy.onSendMessageCallbackDelegateProxy(iMSG, objects);
		}
	}
}
