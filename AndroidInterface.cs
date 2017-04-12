using LitJson;
using System;
using UnityEngine;

public class AndroidInterface : MonoBehaviour
{
	public const string SDK_JAVA_CLASS_UPDATE = "com.snailgame.anysdk.MainActivity";

	public static void CallRegister(string methods, params object[] objs)
	{
		try
		{
			LogSystem.LogWarning(new object[]
			{
				"CallRegister . "
			});
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.snailgame.anysdk.MainActivity"))
			{
				androidJavaClass.CallStatic(methods, objs);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static string getCallRegister(string methods)
	{
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.snailgame.anysdk.MainActivity"))
			{
				return androidJavaClass.CallStatic<string>(methods, new object[0]);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return null;
	}

	public static void StartInit(string objName, int screenOrientation, bool isDebug, int isAnysdk)
	{
		AndroidInterface.CallRegister("init", new object[]
		{
			objName,
			screenOrientation,
			isDebug,
			isAnysdk
		});
	}

	public static void GetChannelName()
	{
		AndroidInterface.CallRegister("getChannelName", new object[0]);
	}

	public static string GetChannelUniqueName()
	{
		return AndroidInterface.getCallRegister("getChannelUniqueName");
	}

	public static string GetChannelAdressId()
	{
		return AndroidInterface.getCallRegister("getChannelAdressId");
	}

	public static void GetAppVersion()
	{
		AndroidInterface.CallRegister("getAppVersion", new object[0]);
	}

	public static void CheckUpdate(string androidUrl)
	{
		AndroidInterface.CallRegister("checkUpdate", new object[]
		{
			androidUrl
		});
	}

	public void OngetAllMemorySizeCallback(string str)
	{
		PlatformUtils.Instance.OngetAllMemorySizeCallback(str);
	}

	public void OngetChannelNameCallback(string channelName)
	{
		PlatformUtils.Instance.OnGetChannelNameCallBack(channelName);
	}

	public void OngetAppVersionCallback(string version)
	{
		PlatformUtils.Instance.currentVersionFinished(version);
	}

	public void OnonInitFinishedCallback(string json)
	{
		JsonData jsonData = JsonMapper.ToObject(json);
		string strA = jsonData["data"].ToString();
		if (string.Compare(strA, "0") != 0)
		{
			PlatformUtils.Instance.InitFinished(false);
		}
		else if (Config.bANYSDK)
		{
			PlatformUtils.Instance.GetChannelName();
		}
	}
}
