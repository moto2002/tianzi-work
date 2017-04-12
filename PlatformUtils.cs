using System;
using UnityEngine;

public class PlatformUtils : MonoBehaviour
{
	public delegate void InitFinishedCallBack(bool bResult);

	public delegate void CheckUpdateHandler(bool isBool);

	private static PlatformUtils instance;

	private int memorySize;

	private string TZfirstRunGame = "TZfirstRunGame";

	private string ImageQuality = "ImageQuality";

	private PlatformUtils.InitFinishedCallBack initFinishedCallBack;

	private PlatformUtils.CheckUpdateHandler checkUpdateHandler;

	private IOSInterface iOSInterface;

	private AndroidInterface androidInterface;

	public static PlatformUtils Instance
	{
		get
		{
			if (PlatformUtils.instance == null)
			{
				GameObject gameObject = new GameObject(Config.MessageName);
				PlatformUtils.instance = gameObject.AddComponent<PlatformUtils>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return PlatformUtils.instance;
		}
	}

	public void Init(PlatformUtils.InitFinishedCallBack callback)
	{
		this.initFinishedCallBack = callback;
		if (this.androidInterface == null)
		{
			this.androidInterface = base.gameObject.AddComponent<AndroidInterface>();
		}
		this.GetChannelAnySDK();
	}

	public void GetChannelAnySDK()
	{
		string callRegister = AndroidInterface.getCallRegister("getChannelAnySDK");
		this.OnGetChannelAnySDKCallBack(callRegister);
	}

	public void OnGetChannelAnySDKCallBack(string str)
	{
		Config.bANYSDK = "ANYSDK".Equals(str);
		LogSystem.LogWarning(new object[]
		{
			"OnGetChannelAnySDKCallBack::",
			str,
			" ",
			Config.bANYSDK
		});
		int isAnysdk = (!Config.bANYSDK) ? 0 : 1;
		AndroidInterface.StartInit(Config.MessageName, 0, Config.debug, isAnysdk);
		if (!Config.bANYSDK)
		{
			this.GetChannelName();
		}
	}

	public void GetChannelName()
	{
		LogSystem.LogWarning(new object[]
		{
			"GetChannelName"
		});
		AndroidInterface.GetChannelName();
	}

	public void OnGetChannelNameCallBack(string bundleIdentifier)
	{
		Config.BundleIdentifier = bundleIdentifier;
		if (string.IsNullOrEmpty(Config.BundleIdentifier))
		{
			Config.BundleIdentifier = Config.DefaultBundleIdentifier;
		}
		this.GetChannelUniqueName();
	}

	public void GetChannelUniqueName()
	{
		string channelUniqueName = AndroidInterface.GetChannelUniqueName();
		this.OnGetChannelUniqueNameCallback(channelUniqueName);
	}

	public void OnGetChannelUniqueNameCallback(string str)
	{
		Config.strChannelUniqueName = str;
		this.GetChannelAdressId();
	}

	public void GetChannelAdressId()
	{
		string channelAdressId = AndroidInterface.GetChannelAdressId();
		this.OnGetChannelAdressIdCallback(channelAdressId);
	}

	public void OnGetChannelAdressIdCallback(string str)
	{
		Config.strAdressId = str;
		Config.channelName = this.GetChannelName(Config.BundleIdentifier);
		LogSystem.LogWarning(new object[]
		{
			"getChannelName: " + Config.strChannelUniqueName,
			" ",
			Config.strAdressId,
			" ",
			Config.BundleIdentifier + " " + Config.channelName
		});
		if (string.IsNullOrEmpty(Config.channelName))
		{
			LogSystem.LogWarning(new object[]
			{
				"not in AnySDK"
			});
		}
		this.GetCurrentVersion();
	}

	public void GetCurrentVersion()
	{
		AndroidInterface.GetAppVersion();
	}

	public void currentVersionFinished(string version)
	{
		Config.version = version;
		this.U3DGetAllMemorySize();
		this.InitFinished(true);
	}

	public void U3DGetAllMemorySize()
	{
		AndroidInterface.CallRegister("getAllMemorySize", new object[0]);
	}

	public void OngetAllMemorySizeCallback(string str)
	{
		this.GetAllMemorySize(str);
	}

	public void InitFinished(bool bResult)
	{
		this.targetEffectAnalysis();
		if (this.initFinishedCallBack != null)
		{
			this.initFinishedCallBack(bResult);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void CheckUpdate(PlatformUtils.CheckUpdateHandler callback, string iosUrl, string androidUrl)
	{
		this.checkUpdateHandler = callback;
		AndroidInterface.CheckUpdate(androidUrl);
	}

	public void OnCheckUpdateCallBack(bool isHave)
	{
		if (this.checkUpdateHandler != null)
		{
			this.checkUpdateHandler(isHave);
		}
	}

	private string GetChannelName(string bundleIdentifier)
	{
		string text = "chosen.";
		int num = bundleIdentifier.IndexOf(text);
		if (num != -1)
		{
			return bundleIdentifier.Substring(num + text.Length);
		}
		return bundleIdentifier;
	}

	public void GetAllMemorySize(string allMemorySize)
	{
		this.memorySize = int.Parse(allMemorySize);
		bool flag = PlayerPrefs.GetInt(this.TZfirstRunGame, 1) == 1;
		if (flag)
		{
			PlayerPrefs.SetInt(this.TZfirstRunGame, 0);
			if (this.memorySize <= 800)
			{
				this.SetImageQuality(GameQuality.SUPER_LOW);
			}
			else if (this.memorySize <= 1900)
			{
				this.SetImageQuality(GameQuality.LOW);
			}
			else if (this.memorySize <= 2900)
			{
				this.SetImageQuality(GameQuality.MIDDLE);
			}
			else
			{
				this.SetImageQuality(GameQuality.HIGH);
			}
		}
	}

	private void SetImageQuality(GameQuality quality)
	{
		PlayerPrefs.SetInt(this.ImageQuality, (int)quality);
	}

	private void targetEffectAnalysis()
	{
	}
}
