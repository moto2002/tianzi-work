using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config
{
	public enum EnumArea
	{
		None,
		Area_China,
		Area_Taiwan,
		Area_Europe,
		Area_JSK,
		Area_SoutheastAsia
	}

	public class LanguageType
	{
		public static string LT_ChineseS = "china";

		public static string LT_ChineseT = "chinaT";

		public static string LT_English = "english";

		public static string LT_Thai = "thai";

		public static string LT_Vietnamese = "vietnamese";

		public static string LT_French = "french";

		public static string LT_German = "german";

		public static string LT_Polish = "polish";

		public static string LT_Italian = "italian";

		public static string LT_Turkish = "turkish";

		public static string LT_Russian = "russian";

		public static string LT_Korean = "korean";

		public static string LT_Japanese = "japanese";

		public static string LT_Portuguese = "portuguese";

		public static string LT_Spanish = "spanish";
	}

	public class GameAddress
	{
		public string strID = string.Empty;

		public string strName = string.Empty;

		public string strDomainAddress = string.Empty;

		public string strIPAddress = string.Empty;
	}

	public class PushServerInfo
	{
		public string strServerID = string.Empty;

		public string strServerName = string.Empty;

		public string strServerIP = string.Empty;

		public string strServerPort = string.Empty;

		public string strApiKey = string.Empty;
	}

	public class CustomInfo
	{
		public string strCustomInfoID = string.Empty;

		public string strCustomInfoVaule = string.Empty;
	}

	public class NoticeInfo
	{
		public string strText;

		public int iIndex;
	}

	public enum LoadStatus
	{
		LS_INIT,
		LS_FAILED,
		LS_SUCCUSED,
		LS_MAX
	}

	public const string ANYSDK = "ANYSDK";

	public const string USER_ACCOUNT = "useraccount";

	public const string ACCOUNT_ID = "accountid";

	public const string USER_NAME = "user_name";

	public const string SERVERID = "serverid";

	public const string GAME_ID = "game_id";

	public const string APPSTORE_URL = "appstore_url";

	private static UIFont snailFont = null;

	private static UIFont nGuiFont = null;

	public static bool bANYSDK = false;

	public static string LaunchName = "Launch";

	public static string MessageName = "MessageName";

	public static string LanguagePath = "Language/";

	public static string Language = Config.LanguageType.LT_ChineseS;

	public static bool bAppStore = false;

	public static bool bGameInitFailed = false;

	public static bool bisOpenCommentValue = true;

	public static string Appstore = "appstore";

	public static string Snail = string.Empty;

	public static string DefaultBundleIdentifier = "com.snailgames.chosen.snail";

	public static string DefaulBundleIdMac = "com.snailgames.chosen.mac";

	public static string serverName = string.Empty;

	public static string gameId = string.Empty;

	public static string accessId = string.Empty;

	public static string accessPassword = string.Empty;

	public static string accessType = string.Empty;

	public static string seed = string.Empty;

	public static string serverId = string.Empty;

	public static string pushPhoneTypeName = string.Empty;

	public static string dataCollectionUrl = string.Empty;

	public static bool isPickGuildReward = false;

	public static string errorLogUrl = string.Empty;

	public static string errorLogUrlId = string.Empty;

	public static string NativeErrorUrl = string.Empty;

	public static string appid = string.Empty;

	public static string appkey = string.Empty;

	public static string registerServerUrl = string.Empty;

	public static bool isFirstLogin = false;

	public static bool mbEnableCatcher = false;

	public static bool ErrorLogPhoneOpen = false;

	public static string FrameAddress = string.Empty;

	public static bool debug = false;

	public string versionUpdateUrl = string.Empty;

	public static string channelId = string.Empty;

	public static bool IsInstallWechat = false;

	public static string channelName = string.Empty;

	public static string cid = string.Empty;

	public static string idfa = string.Empty;

	public static string LifeCycleStr = string.Empty;

	public static string WeChatShareImgUrl = string.Empty;

	public static string strChannelUniqueName = string.Empty;

	public static string strAdressId = string.Empty;

	public static string BundleIdentifier = string.Empty;

	public static Dictionary<string, List<string>> accInfo = new Dictionary<string, List<string>>();

	public static string strShowAppAdUrl = "http;//10.203.11.48/ad/ad.json";

	public static string appStoreURL = string.Empty;

	public static string ClientInstallUrl = string.Empty;

	public static string phoneType = string.Empty;

	public static string version = string.Empty;

	public static string wxAndroidId = string.Empty;

	public static string wxiOSId = string.Empty;

	public static string pushServiceIP = string.Empty;

	public static string pushServicePOST = string.Empty;

	public static string channelAppSecret = string.Empty;

	public static string Weixin_Link_Url = string.Empty;

	public static string act_Url = string.Empty;

	public static string curreRoleName = string.Empty;

	public static string payCallBackURL = string.Empty;

	public static string strAppStoreID = string.Empty;

	public static string strNoticeName = string.Empty;

	public static string strChannelName = string.Empty;

	private static bool mbUseCacheWWW = false;

	public static int iCacheVersion = 0;

	public static string isOpenFPS = string.Empty;

	public static string IsGMOpen = string.Empty;

	public static bool bNeedDataCollect = false;

	public static bool bNeedWechatShare = false;

	public static int mQueueWaitTime = 5;

	public static string accountName = string.Empty;

	public static bool mbDynamicRes = true;

	public static bool mbMd5 = false;

	public static Dictionary<string, Config.GameAddress> mUpdaterAddress = new Dictionary<string, Config.GameAddress>();

	public static Dictionary<string, Dictionary<string, string>> mDictUpdaterGuide = new Dictionary<string, Dictionary<string, string>>();

	public static List<Dictionary<string, string>> mDictServerList = new List<Dictionary<string, string>>();

	public static List<Dictionary<string, string>> mDictAllServerList = new List<Dictionary<string, string>>();

	public static Dictionary<string, string> mWordsDict = new Dictionary<string, string>();

	public static string mstrShopData = string.Empty;

	public static string mstrServerNotice = string.Empty;

	public static Dictionary<string, Config.CustomInfo> mDictCustomInfoList = new Dictionary<string, Config.CustomInfo>();

	public static List<Config.PushServerInfo> mDictPushServerList = new List<Config.PushServerInfo>();

	public static string mstrPreSuffix = "File://";

	public static string mstrStreamSuffix = "File://";

	public static string mstrAssetBundleRootPath = string.Empty;

	public static string mstrSourceResRootPath = string.Empty;

	public static string mstrStreamResRootPath = string.Empty;

	public static string mstrMacAddress = string.Empty;

	public static string mstrVersionUseage = string.Empty;

	public static string mstrInstallationVersion = string.Empty;

	public static string mstrLocalVersion = string.Empty;

	public static int miLocalNumberVersion = 1;

	public static UIFont SnailFont
	{
		get
		{
			return Config.snailFont;
		}
		set
		{
			Config.snailFont = value;
		}
	}

	public static UIFont NGUIFont
	{
		get
		{
			return Config.nGuiFont;
		}
		set
		{
			Config.nGuiFont = value;
		}
	}

	public static bool bEditor
	{
		get
		{
			return false;
		}
	}

	public static bool bAndroid
	{
		get
		{
			return true;
		}
	}

	public static bool bIPhone
	{
		get
		{
			return false;
		}
	}

	public static bool bWin
	{
		get
		{
			return false;
		}
	}

	public static bool bMac
	{
		get
		{
			return false;
		}
	}

	public static Config.EnumArea GameArea
	{
		get;
		private set;
	}

	public static string ScreenResolution
	{
		get
		{
			return ResolutionConstrain.Instance.width + "*" + ResolutionConstrain.Instance.height;
		}
	}

	public static bool mbVerifyVersion
	{
		get
		{
			if (string.IsNullOrEmpty(Config.mstrLocalVersion))
			{
				return false;
			}
			string updaterConfig = Config.GetUpdaterConfig("VerifyVerison", "Value");
			return !string.IsNullOrEmpty(updaterConfig) && Config.mstrLocalVersion == updaterConfig;
		}
	}

	public static UIFont LoadUIFont(string strFontName)
	{
		if (Application.isPlaying)
		{
			string path = "Local/Fonts/" + strFontName;
			UnityEngine.Object @object = Resources.Load(path);
			if (@object != null)
			{
				GameObject gameObject = @object as GameObject;
				UIFont component = gameObject.GetComponent<UIFont>();
				if (component != null)
				{
					return component;
				}
			}
		}
		return null;
	}

	public static void SetUseCacheWWW(bool bUseCache)
	{
		Config.mbUseCacheWWW = bUseCache;
	}

	public static bool GetUseCacheWWW()
	{
		return Config.mbUseCacheWWW;
	}

	public static void SetCacheVersion(string strCacheVersion)
	{
		string @string = PlayerPrefs.GetString("CacheVersion", string.Empty);
		if (string.IsNullOrEmpty(@string) || strCacheVersion != @string)
		{
			PlayerPrefs.SetString("CacheVersion", strCacheVersion);
			Caching.CleanCache();
		}
		Config.iCacheVersion = 0;
		if (string.IsNullOrEmpty(strCacheVersion))
		{
			LogSystem.LogWarning(new object[]
			{
				"Version Error ",
				strCacheVersion
			});
			return;
		}
		string text = strCacheVersion.Replace(".", string.Empty);
		if (string.IsNullOrEmpty(text))
		{
			LogSystem.LogWarning(new object[]
			{
				"Version Error ",
				text
			});
			return;
		}
		Config.iCacheVersion = int.Parse(text);
	}

	public static int GetQueueWaitTime()
	{
		return Config.mQueueWaitTime;
	}

	public static void SetLanguage(string sType)
	{
		if (string.IsNullOrEmpty(sType))
		{
			sType = Config.LanguageType.LT_ChineseS;
		}
		Config.LanguagePath = Config.LanguagePath + sType + "/";
	}

	public static string GetMacAddress()
	{
		return Config.mstrMacAddress;
	}

	public static void SetMacAddress(string strMacAddress)
	{
		Config.mstrMacAddress = strMacAddress;
	}

	public static string GetPreSuffix()
	{
		return Config.mstrPreSuffix;
	}

	public static void SetPreSuffix(string strSuffix)
	{
		Config.mstrPreSuffix = strSuffix;
	}

	public static string GetStreamSuffix()
	{
		return Config.mstrStreamSuffix;
	}

	public static void SetStreamSuffix(string strSuffix)
	{
		Config.mstrStreamSuffix = strSuffix;
	}

	public static Config.GameAddress GetUpdaterAddress()
	{
		if (Config.mUpdaterAddress.Count == 0)
		{
			return null;
		}
		string key;
		if (string.IsNullOrEmpty(Config.strAdressId))
		{
			key = "QaAddr";
		}
		else
		{
			key = Config.strAdressId;
		}
		if (Config.mUpdaterAddress.ContainsKey(key))
		{
			return Config.mUpdaterAddress[key];
		}
		return null;
	}

	public static string GetUpdaterConfig(string strKey, string strName)
	{
		if (Config.mDictUpdaterGuide.ContainsKey(strKey) && Config.mDictUpdaterGuide[strKey].ContainsKey(strName))
		{
			return Config.mDictUpdaterGuide[strKey][strName];
		}
		return string.Empty;
	}

	public static string GetAssetBundleRootPath()
	{
		return Config.mstrAssetBundleRootPath;
	}

	public static void SetAssetBundleRootPath(string strRootPath)
	{
		Config.mstrAssetBundleRootPath = strRootPath;
	}

	public static string GetVersionUseage()
	{
		return Config.mstrVersionUseage;
	}

	public static void SetVersionUseage(string strValue)
	{
		Config.mstrVersionUseage = strValue;
	}

	public static string GetInstallationVersion()
	{
		return Config.mstrInstallationVersion;
	}

	public static void SetInstallationVersion(string strValue)
	{
		Config.mstrInstallationVersion = strValue;
	}

	public static string GetLocalVersion()
	{
		return Config.mstrLocalVersion;
	}

	public static void SetLocalVersion(string strLocalVersion)
	{
		Config.mstrLocalVersion = strLocalVersion;
	}

	public static int GetLocalNumberVersion()
	{
		return Config.miLocalNumberVersion;
	}

	public static void SetLocalNumberVersion(string strLocalNumberVersion)
	{
		if (string.IsNullOrEmpty(strLocalNumberVersion))
		{
			Config.miLocalNumberVersion = 0;
		}
		else
		{
			try
			{
				Config.miLocalNumberVersion = int.Parse(strLocalNumberVersion);
			}
			catch (Exception ex)
			{
				Config.miLocalNumberVersion = 0;
				LogSystem.LogError(new object[]
				{
					ex.ToString()
				});
			}
		}
	}

	public static string GetSourceRootPath()
	{
		return Config.mstrSourceResRootPath;
	}

	public static void SetSourceRootPath(string strRootPath)
	{
		Config.mstrSourceResRootPath = strRootPath;
	}

	public static string GetStreamRootPath()
	{
		return Config.mstrStreamResRootPath;
	}

	public static void SetStreamRootPath(string strRootPath)
	{
		Config.mstrStreamResRootPath = strRootPath;
	}

	public static void SetUpdaterAddress(TextAsset text)
	{
		if (text == null)
		{
			return;
		}
		XMLParser xMLParser = new XMLParser();
		XMLNode xMLNode = xMLParser.Parse(text.text);
		XMLNodeList nodeList = xMLNode.GetNodeList("Addresses>0>Address");
		if (nodeList != null)
		{
			foreach (XMLNode xMLNode2 in nodeList)
			{
				Config.GameAddress gameAddress = new Config.GameAddress();
				gameAddress.strID = xMLNode2.GetValue("@ID");
				gameAddress.strName = xMLNode2.GetValue("@Name");
				gameAddress.strDomainAddress = xMLNode2.GetValue("@DomainAddr") + "/" + Config.GetInstallationVersion();
				gameAddress.strIPAddress = xMLNode2.GetValue("@IPAddr") + "/" + Config.GetInstallationVersion();
				if (!Config.mUpdaterAddress.ContainsKey(gameAddress.strID))
				{
					Config.mUpdaterAddress.Add(gameAddress.strID, gameAddress);
				}
			}
		}
	}

	public static void SetUpdateGuideInfo(string xmlString)
	{
		if (string.IsNullOrEmpty(xmlString))
		{
			return;
		}
		int startIndex = xmlString.IndexOf('<');
		xmlString = xmlString.Substring(startIndex);
		xmlString.Trim();
		XMLParser xMLParser = new XMLParser();
		XMLNode xMLNode = xMLParser.Parse(xmlString);
		XMLNodeList xMLNodeList = (XMLNodeList)xMLNode["Resources"];
		if (xMLNodeList == null)
		{
			return;
		}
		for (int i = 0; i < xMLNodeList.Count; i++)
		{
			XMLNode xMLNode2 = xMLNodeList[i] as XMLNode;
			XMLNodeList nodeList = xMLNode2.GetNodeList("Resource");
			if (nodeList != null)
			{
				for (int j = 0; j < nodeList.Count; j++)
				{
					XMLNode xMLNode3 = nodeList[j] as XMLNode;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					string key = string.Empty;
					foreach (DictionaryEntry dictionaryEntry in xMLNode3)
					{
						if (dictionaryEntry.Value != null)
						{
							string text = dictionaryEntry.Key as string;
							if (text[0] == '@')
							{
								text = text.Substring(1);
								if (text == "ID")
								{
									key = (string)dictionaryEntry.Value;
								}
								else if (dictionary.ContainsKey(text))
								{
									dictionary[text] = (string)dictionaryEntry.Value;
								}
								else
								{
									dictionary.Add(text, (string)dictionaryEntry.Value);
								}
							}
						}
					}
					if (Config.mDictUpdaterGuide.ContainsKey(key))
					{
						Config.mDictUpdaterGuide[key] = dictionary;
					}
					else
					{
						Config.mDictUpdaterGuide.Add(key, dictionary);
					}
				}
			}
		}
	}

	public static void SetServerNotice(string xmlString)
	{
		Config.mstrServerNotice = xmlString;
	}

	public static int Compare_Index(Config.NoticeInfo aInfo, Config.NoticeInfo bInfo)
	{
		if (aInfo.iIndex > bInfo.iIndex)
		{
			return 1;
		}
		if (aInfo.iIndex < bInfo.iIndex)
		{
			return -1;
		}
		return 0;
	}

	public static void SetCustomInfoList(string xmlString)
	{
		int startIndex = xmlString.IndexOf('<');
		xmlString = xmlString.Substring(startIndex);
		xmlString.Trim();
		XMLParser xMLParser = new XMLParser();
		XMLNode xMLNode = xMLParser.Parse(xmlString);
		Config.bAppStore = (Config.channelName == Config.Appstore);
		Config.SetCustomInfo((XMLNodeList)xMLNode["Resources"]);
		Config.InitResouceInfo();
		Config.SetChannelInfo(xMLNode);
		Config.SetWechatInfo(xMLNode);
		Config.SetAccInfo(xMLNode);
	}

	private static void SetWechatInfo(XMLNode rootnode)
	{
		if (rootnode == null)
		{
			return;
		}
		XMLNodeList nodeList = rootnode.GetNodeList("Resources>0>Wechat>0>Property");
		foreach (XMLNode xMLNode in nodeList)
		{
			string value = xMLNode.GetValue("@ID");
			if (value == Config.BundleIdentifier)
			{
				Config.bNeedWechatShare = "1".Equals(xMLNode.GetValue("@WechatShare"));
				Config.wxAndroidId = xMLNode.GetValue("@wxAndroidId");
				Config.WeChatShareImgUrl = xMLNode.GetValue("@WeChatShareImgUrl");
				LogSystem.LogWarning(new object[]
				{
					"------1----wxAndroidId--------" + Config.wxAndroidId
				});
				LogSystem.LogWarning(new object[]
				{
					"------1-----bNeedWechatShare-------" + Config.bNeedWechatShare
				});
				LogSystem.LogWarning(new object[]
				{
					"------1-----WeChatShareImgUrl-------" + Config.WeChatShareImgUrl
				});
				return;
			}
		}
		LogSystem.LogWarning(new object[]
		{
			"------2----wxAndroidId--------" + Config.wxAndroidId
		});
		LogSystem.LogWarning(new object[]
		{
			"------2-----bNeedWechatShare-------" + Config.bNeedWechatShare
		});
		LogSystem.LogWarning(new object[]
		{
			"------2-----WeChatShareImgUrl-------" + Config.WeChatShareImgUrl
		});
	}

	private static void SetChannelInfo(XMLNode rootnode)
	{
		if (rootnode == null)
		{
			return;
		}
		XMLNodeList nodeList = rootnode.GetNodeList("Resources>0>Channel>0>Property");
		Config.GameArea = Config.EnumArea.None;
		LogSystem.LogWarning(new object[]
		{
			"----------strChannelUniqueName--------" + Config.strChannelUniqueName
		});
		if (string.IsNullOrEmpty(Config.strChannelUniqueName))
		{
			Config.strChannelUniqueName = "android_snail";
		}
		foreach (XMLNode xMLNode in nodeList)
		{
			string value = xMLNode.GetValue("@ID");
			if (value == Config.strChannelUniqueName)
			{
				Config.payCallBackURL = xMLNode.GetValue("@PayCallBackUrl");
				Config.ClientInstallUrl = xMLNode.GetValue("@ClientInstallUrl");
				Config.strNoticeName = xMLNode.GetValue("@NoticeName");
				Config.strChannelName = xMLNode.GetValue("@ChannelName");
				Config.bNeedDataCollect = "1".Equals(xMLNode.GetValue("@DataCollect"));
				LogSystem.LogWarning(new object[]
				{
					"------2----bNeedDataCollect--------" + Config.bNeedDataCollect
				});
				LogSystem.LogWarning(new object[]
				{
					"------2-----strID-------" + value
				});
				string value2 = xMLNode.GetValue("@Area");
				int gameArea;
				if (!string.IsNullOrEmpty(value2) && int.TryParse(value2, out gameArea))
				{
					Config.GameArea = (Config.EnumArea)gameArea;
				}
				return;
			}
		}
		LogSystem.LogWarning(new object[]
		{
			"channelName not have info ",
			Config.strChannelUniqueName
		});
		LogSystem.LogWarning(new object[]
		{
			"----1--------bNeedDataCollect------" + Config.bNeedDataCollect
		});
	}

	private static void SetAccInfo(XMLNode rootnode)
	{
		if (rootnode == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"SetAccInfo is null"
			});
			return;
		}
		XMLNodeList nodeList = rootnode.GetNodeList("Resources>0>Acc>0>Property");
		if (nodeList == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"SetAccInfo is null"
			});
			return;
		}
		foreach (XMLNode xMLNode in nodeList)
		{
			string value = xMLNode.GetValue("@No");
			if (Config.accInfo.ContainsKey(value))
			{
				Config.accInfo[value].Add(xMLNode.GetValue("@Name"));
			}
			else
			{
				List<string> list = new List<string>();
				list.Add(xMLNode.GetValue("@Name"));
				Config.accInfo.Add(value, list);
			}
		}
	}

	private static void SetCustomInfo(XMLNodeList xmlNodeList)
	{
		if (xmlNodeList == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"SetCustomInfo is null"
			});
			return;
		}
		for (int i = 0; i < xmlNodeList.Count; i++)
		{
			XMLNode xMLNode = xmlNodeList[i] as XMLNode;
			XMLNodeList nodeList = xMLNode.GetNodeList("Resource");
			if (nodeList != null)
			{
				for (int j = 0; j < nodeList.Count; j++)
				{
					XMLNode xMLNode2 = nodeList[j] as XMLNode;
					Config.CustomInfo customInfo = new Config.CustomInfo();
					foreach (DictionaryEntry dictionaryEntry in xMLNode2)
					{
						if (dictionaryEntry.Value != null)
						{
							string text = dictionaryEntry.Key as string;
							if (text[0] == '@')
							{
								text = text.Substring(1);
								if (text == "ID")
								{
									customInfo.strCustomInfoID = (dictionaryEntry.Value as string);
								}
								else if (text == "Value")
								{
									customInfo.strCustomInfoVaule = (dictionaryEntry.Value as string);
								}
							}
						}
					}
					if (Config.mDictCustomInfoList.ContainsKey(customInfo.strCustomInfoID))
					{
						Config.mDictCustomInfoList[customInfo.strCustomInfoID] = customInfo;
					}
					else
					{
						Config.mDictCustomInfoList.Add(customInfo.strCustomInfoID, customInfo);
					}
				}
			}
		}
	}

	public static string GetCustomValue(string strKey)
	{
		Config.CustomInfo customInfo;
		if (Config.mDictCustomInfoList.TryGetValue(strKey, out customInfo))
		{
			return customInfo.strCustomInfoVaule;
		}
		return string.Empty;
	}

	public static void InitResouceInfo()
	{
		if (Config.mDictCustomInfoList.ContainsKey("accessID"))
		{
			Config.accessId = Config.mDictCustomInfoList["accessID"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("accessPwd"))
		{
			Config.accessPassword = Config.mDictCustomInfoList["accessPwd"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("seed"))
		{
			Config.seed = Config.mDictCustomInfoList["seed"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("errorLogUrl"))
		{
			Config.errorLogUrl = Config.mDictCustomInfoList["errorLogUrl"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("errorLogUrlID"))
		{
			Config.errorLogUrlId = Config.mDictCustomInfoList["errorLogUrlID"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("NativeErrorUrl"))
		{
			Config.NativeErrorUrl = Config.mDictCustomInfoList["NativeErrorUrl"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("appid"))
		{
			Config.appid = Config.mDictCustomInfoList["appid"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("Weixin_Link_Url"))
		{
			Config.Weixin_Link_Url = Config.mDictCustomInfoList["Weixin_Link_Url"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("ACT_Url"))
		{
			Config.act_Url = Config.mDictCustomInfoList["ACT_Url"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("appkey"))
		{
			Config.appkey = Config.mDictCustomInfoList["appkey"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("wxiOSId"))
		{
			Config.wxiOSId = Config.mDictCustomInfoList["wxiOSId"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("dataCollectionUrl"))
		{
			Config.dataCollectionUrl = Config.mDictCustomInfoList["dataCollectionUrl"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("ErrorLogPhoneOpen"))
		{
			string strCustomInfoVaule = Config.mDictCustomInfoList["ErrorLogPhoneOpen"].strCustomInfoVaule;
			if (strCustomInfoVaule == "true")
			{
				Config.ErrorLogPhoneOpen = true;
			}
			else
			{
				Config.ErrorLogPhoneOpen = false;
			}
		}
		if (Config.mDictCustomInfoList.ContainsKey("regUrl"))
		{
			Config.registerServerUrl = Config.mDictCustomInfoList["regUrl"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("FrameAddress"))
		{
			Config.FrameAddress = Config.mDictCustomInfoList["FrameAddress"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("accessType"))
		{
			Config.accessType = Config.mDictCustomInfoList["accessType"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("gameID"))
		{
			Config.gameId = Config.mDictCustomInfoList["gameID"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("FPSOpen"))
		{
			Config.isOpenFPS = Config.mDictCustomInfoList["FPSOpen"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("GMOpen"))
		{
			Config.IsGMOpen = Config.mDictCustomInfoList["GMOpen"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("channeltype"))
		{
			Config.channelId = Config.mDictCustomInfoList["channeltype"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("PushAndroidName"))
		{
			Config.pushPhoneTypeName = Config.mDictCustomInfoList["PushAndroidName"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("version"))
		{
			Config.version = Config.mDictCustomInfoList["version"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("pushServiceIP"))
		{
			Config.pushServiceIP = Config.mDictCustomInfoList["pushServiceIP"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("pushServicePOST"))
		{
			Config.pushServicePOST = Config.mDictCustomInfoList["pushServicePOST"].strCustomInfoVaule;
		}
		if (Config.mDictCustomInfoList.ContainsKey("channelAppSecret"))
		{
			Config.channelAppSecret = Config.mDictCustomInfoList["channelAppSecret"].strCustomInfoVaule;
		}
	}

	public static void SetServerList(string xmlString)
	{
		int startIndex = xmlString.IndexOf('<');
		xmlString = xmlString.Substring(startIndex);
		xmlString.Trim();
		XMLParser xMLParser = new XMLParser();
		XMLNode xMLNode = xMLParser.Parse(xmlString);
		XMLNodeList xMLNodeList = (XMLNodeList)xMLNode["Servers"];
		if (xMLNodeList == null)
		{
			return;
		}
		for (int i = 0; i < xMLNodeList.Count; i++)
		{
			XMLNode xMLNode2 = xMLNodeList[i] as XMLNode;
			XMLNodeList nodeList = xMLNode2.GetNodeList("Server");
			if (nodeList != null)
			{
				for (int j = 0; j < nodeList.Count; j++)
				{
					XMLNode xMLNode3 = nodeList[j] as XMLNode;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					foreach (DictionaryEntry dictionaryEntry in xMLNode3)
					{
						if (dictionaryEntry.Value != null)
						{
							string text = dictionaryEntry.Key as string;
							if (text[0] == '@')
							{
								text = text.Substring(1);
								if (!dictionary.ContainsKey(text))
								{
									dictionary.Add(text, (string)dictionaryEntry.Value);
								}
								else
								{
									dictionary[text] = (string)dictionaryEntry.Value;
								}
							}
						}
					}
					Config.mDictServerList.Add(dictionary);
				}
			}
		}
	}

	public static void SetAllServerList(string xmlString)
	{
		int startIndex = xmlString.IndexOf('<');
		xmlString = xmlString.Substring(startIndex);
		xmlString.Trim();
		XMLParser xMLParser = new XMLParser();
		XMLNode xMLNode = xMLParser.Parse(xmlString);
		XMLNodeList xMLNodeList = (XMLNodeList)xMLNode["Servers"];
		if (xMLNodeList == null)
		{
			return;
		}
		for (int i = 0; i < xMLNodeList.Count; i++)
		{
			XMLNode xMLNode2 = xMLNodeList[i] as XMLNode;
			XMLNodeList nodeList = xMLNode2.GetNodeList("Server");
			if (nodeList != null)
			{
				for (int j = 0; j < nodeList.Count; j++)
				{
					XMLNode xMLNode3 = nodeList[j] as XMLNode;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					foreach (DictionaryEntry dictionaryEntry in xMLNode3)
					{
						if (dictionaryEntry.Value != null)
						{
							string text = dictionaryEntry.Key as string;
							if (text[0] == '@')
							{
								text = text.Substring(1);
								if (!dictionary.ContainsKey(text))
								{
									dictionary.Add(text, (string)dictionaryEntry.Value);
								}
								else
								{
									dictionary[text] = (string)dictionaryEntry.Value;
								}
							}
						}
					}
					Config.mDictAllServerList.Add(dictionary);
				}
			}
		}
	}

	public static void SetShopList(string xmlString)
	{
		Config.mstrShopData = xmlString;
	}

	public static void SetPushServerList(string xmlString)
	{
		int startIndex = xmlString.IndexOf('<');
		xmlString = xmlString.Substring(startIndex);
		xmlString.Trim();
		XMLParser xMLParser = new XMLParser();
		XMLNode xMLNode = xMLParser.Parse(xmlString);
		XMLNodeList xMLNodeList = (XMLNodeList)xMLNode["PushServers"];
		if (xMLNodeList == null)
		{
			return;
		}
		for (int i = 0; i < xMLNodeList.Count; i++)
		{
			XMLNode xMLNode2 = xMLNodeList[i] as XMLNode;
			XMLNodeList nodeList = xMLNode2.GetNodeList("Server");
			if (nodeList != null)
			{
				for (int j = 0; j < nodeList.Count; j++)
				{
					XMLNode xMLNode3 = nodeList[j] as XMLNode;
					Config.PushServerInfo pushServerInfo = new Config.PushServerInfo();
					foreach (DictionaryEntry dictionaryEntry in xMLNode3)
					{
						if (dictionaryEntry.Value != null)
						{
							string text = dictionaryEntry.Key as string;
							if (text[0] == '@')
							{
								text = text.Substring(1);
								if (text == "ID")
								{
									pushServerInfo.strServerID = (dictionaryEntry.Value as string);
								}
								else if (text == "Name")
								{
									pushServerInfo.strServerName = (dictionaryEntry.Value as string);
								}
								else if (text == "IP")
								{
									pushServerInfo.strServerIP = (dictionaryEntry.Value as string);
								}
								else if (text == "Port")
								{
									pushServerInfo.strServerPort = (dictionaryEntry.Value as string);
								}
								else if (text == "ApiKey")
								{
									pushServerInfo.strApiKey = (dictionaryEntry.Value as string);
								}
							}
						}
					}
					Config.mDictPushServerList.Add(pushServerInfo);
				}
			}
		}
	}

	public static Config.PushServerInfo GetPushServer()
	{
		if (Config.mDictPushServerList.Count == 0)
		{
			return null;
		}
		int index = UnityEngine.Random.Range(0, Config.mDictPushServerList.Count);
		return Config.mDictPushServerList[index];
	}

	public static void SetWordsInfo(string strWords)
	{
		if (string.IsNullOrEmpty(strWords))
		{
			return;
		}
		string[] array = strWords.Split(new string[]
		{
			"\r\n"
		}, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new string[]
			{
				"="
			}, 2, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length == 2)
			{
				if (Config.mWordsDict.ContainsKey(array2[0]))
				{
					LogSystem.Log(new object[]
					{
						"the key is echo in local file!!! please check the key = ",
						array2[0]
					});
				}
				else
				{
					array2[1] = array2[1].Replace("[n]", "\n");
					Config.mWordsDict[array2[0]] = array2[1];
				}
			}
		}
	}

	public static string GetUdpateLangage(string strKey)
	{
		if (Config.mWordsDict.ContainsKey(strKey))
		{
			return Config.mWordsDict[strKey];
		}
		return strKey;
	}

	public static void SavePlayerPrefs(string useraccount, string username, string serverid, string gameid, string appstoreUrl, string accountid)
	{
		PlayerPrefs.SetString("useraccount", useraccount);
		string value = WWW.EscapeURL(username);
		PlayerPrefs.SetString("user_name", value);
		PlayerPrefs.SetString("serverid", serverid);
		PlayerPrefs.SetString("game_id", gameid);
		PlayerPrefs.SetString("appstore_url", appstoreUrl);
		PlayerPrefs.SetString("accountid", accountid);
	}

	public static void GetPlayerPrefs(ref string useraccount, ref string username, ref string serverid, ref string gameid, ref string appstoreUrl, ref string accountid)
	{
		useraccount = PlayerPrefs.GetString("useraccount", string.Empty);
		string @string = PlayerPrefs.GetString("user_name", string.Empty);
		username = WWW.UnEscapeURL(@string);
		serverid = PlayerPrefs.GetString("serverid", string.Empty);
		gameid = PlayerPrefs.GetString("game_id", string.Empty);
		appstoreUrl = PlayerPrefs.GetString("appstore_url", string.Empty);
		accountid = PlayerPrefs.GetString("accountid", string.Empty);
	}

	public static string GetNetErrorPromp(string strKeywords)
	{
		string strKey = "CustomInfoError";
		if (strKeywords.Contains("connect to host"))
		{
			strKey = "CantConnectToHost";
		}
		else if (strKeywords.Contains("404"))
		{
			strKey = "HTTPError_404";
		}
		else if (strKeywords.Contains("403"))
		{
			strKey = "HTTPError_403";
		}
		else if (strKeywords.Contains("405"))
		{
			strKey = "HTTPError_405";
		}
		else if (strKeywords.Contains("406"))
		{
			strKey = "HTTPError_406";
		}
		else if (strKeywords.Contains("407"))
		{
			strKey = "HTTPError_407";
		}
		else if (strKeywords.Contains("410"))
		{
			strKey = "HTTPError_410";
		}
		else if (strKeywords.Contains("412"))
		{
			strKey = "HTTPError_412";
		}
		else if (strKeywords.Contains("414"))
		{
			strKey = "HTTPError_414";
		}
		else if (strKeywords.Contains("500"))
		{
			strKey = "HTTPError_500";
		}
		else if (strKeywords.Contains("501"))
		{
			strKey = "HTTPError_501";
		}
		else if (strKeywords.Contains("502"))
		{
			strKey = "HTTPError_502";
		}
		return Config.GetUdpateLangage(strKey);
	}
}
