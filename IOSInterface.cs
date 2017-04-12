using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class IOSInterface : MonoBehaviour
{
	[DllImport("__Internal")]
	public static extern void BREAK();

	[DllImport("__Internal")]
	public static extern void U3DSetCallBack(string gameObjectName);

	[DllImport("__Internal")]
	public static extern void U3DSendGameFrame(int url, string address);

	[DllImport("__Internal")]
	public static extern void U3DOpenURL(string url);

	[DllImport("__Internal")]
	public static extern void U3DCheckUpdateForUrl(string _url);

	[DllImport("__Internal")]
	public static extern void U3DSetDebugModel(bool isDebug);

	[DllImport("__Internal")]
	public static extern bool U3DGetDebugModel();

	[DllImport("__Internal")]
	public static extern void U3DInitRegisterServerAddress(string _serverAddress);

	[DllImport("__Internal")]
	public static extern void U3DShowADFromURL(string strAd);

	[DllImport("__Internal")]
	public static extern void U3DMobileRegister(string mobile, string pwd, string code, string _accessId, string _accessPwd, string _accessType, string _seed, string _gameId, string _channelID);

	[DllImport("__Internal")]
	public static extern void U3DCheckAccount(string account, string _accessId, string _accessPwd, string _accessType, string _seed);

	[DllImport("__Internal")]
	public static extern void U3DSendSMS(string mobile);

	[DllImport("__Internal")]
	public static extern void U3DAccountActive(string accout, string _actKey, string _gameId, string _serverId);

	[DllImport("__Internal")]
	public static extern void U3DDataCollectionInit(int _gameId, int _severId, string _severIp, string _account, string _roleName, string _channelID, bool isDebug);

	[DllImport("__Internal")]
	public static extern void U3DCollectLogin(long time);

	[DllImport("__Internal")]
	public static extern void U3DCollectCreateRole(int time);

	[DllImport("__Internal")]
	public static extern void U3DCollectOffLine(int time);

	[DllImport("__Internal")]
	public static extern void U3DCollectBackGround(long time);

	[DllImport("__Internal")]
	public static extern void U3DCollectActivation();

	[DllImport("__Internal")]
	public static extern void U3DCollectLoadResource(bool success);

	[DllImport("__Internal")]
	public static extern void U3DAppLuncher();

	[DllImport("__Internal")]
	public static extern void U3DAppWakeUp();

	[DllImport("__Internal")]
	public static extern void U3DLogout(long time);

	[DllImport("__Internal")]
	public static extern void U3DCustomLogoutEvent(int eventId, string _content);

	[DllImport("__Internal")]
	public static extern void U3DInitPushMsg();

	[DllImport("__Internal")]
	public static extern bool U3DPostIAPinfo(string account, string _roleName, string _gid, string _sid, string _oid, string _prodId, string _price, string _quantity, string _sign, string _address);

	[DllImport("__Internal")]
	public static extern void U3DStartANotfi(string nName, int sec, string _context);

	[DllImport("__Internal")]
	public static extern void U3DStartANotfiWithDay(string nName, string dateStr, string _context);

	[DllImport("__Internal")]
	public static extern void U3DStartANotfiWithWeekly(string nName, string dateStr, string _context);

	[DllImport("__Internal")]
	public static extern void U3DCommondRegisternew(string uname, string pwd, string _accessId, string _accessPwd, string _accessType, string _seed, string _gameId, string _channelID, string _pid, string mobile);

	[DllImport("__Internal")]
	public static extern void U3DOnekeyRegisternew(string _accessId, string _accessPwd, string _accessType, string _seed, string _gameId, string _channelID, string _pid);

	[DllImport("__Internal")]
	public static extern void U3DStartLoading(string tipTxt);

	[DllImport("__Internal")]
	public static extern void U3DStopLoading();

	[DllImport("__Internal")]
	public static extern void U3DCallTheCustomService(string tel);

	[DllImport("__Internal")]
	public static extern void U3DSendFileToServer(string fileName, string _sid, string _serverAddress, int _maxSize);

	[DllImport("__Internal")]
	public static extern void U3DSaveDataToDevice(string key, string _value, bool _isUpdate);

	[DllImport("__Internal")]
	public static extern int U3DGetUpdateStauts();

	[DllImport("__Internal")]
	public static extern void U3DgetPhoneInfo();

	[DllImport("__Internal")]
	public static extern void U3DGetDeviceToke();

	[DllImport("__Internal")]
	public static extern void U3DGetChannelName();

	[DllImport("__Internal")]
	public static extern void U3DinitExceptionCatch(string projectName, string gameId, string roleName, string address, string fileAddress, bool open);

	[DllImport("__Internal")]
	public static extern void U3DWeChatRegister(string appId);

	[DllImport("__Internal")]
	public static extern void U3DWeChatCheck();

	[DllImport("__Internal")]
	public static extern void U3DWeChatShare(string title, string content, string image, string url);

	[DllImport("__Internal")]
	public static extern void OnlyImage(string image);

	[DllImport("__Internal")]
	public static extern string U3DgetUUID();

	[DllImport("__Internal")]
	public static extern void U3DgetSceneID(string sceneID);

	[DllImport("__Internal")]
	public static extern void U3DpostUpdateNetworkInfo(string channelId, string address, string version, string type, string gameid, string parmess);

	[DllImport("__Internal")]
	public static extern string U3DPullPastPadString();

	[DllImport("__Internal")]
	public static extern void U3DPutPastPadString(string text);

	[DllImport("__Internal")]
	public static extern double U3DGetBatteryLevel();

	[DllImport("__Internal")]
	public static extern void U3DGotoRater(string appid);

	[DllImport("__Internal")]
	public static extern float U3dGetDeviceVoiceValue();

	[DllImport("__Internal")]
	public static extern void U3dSetDeviceVoiceValue(float fvalue);

	[DllImport("__Internal")]
	public static extern void U3DWChatShare(string _title, string _description, string _image, string _url);

	[DllImport("__Internal")]
	public static extern void U3DWChatShareImage(string _path, string m_server, string m_id, string _url);

	[DllImport("__Internal")]
	public static extern void U3DWChatShareFriendImage(string _path, string m_server, string m_id, string m_url);

	[DllImport("__Internal")]
	public static extern void U3DshowUserCenter();

	[DllImport("__Internal")]
	public static extern void U3DGameStart(string _channelID, string _account, string _gameId);

	[DllImport("__Internal")]
	public static extern void U3DGameEnd(string _channelID, string _account, string _gameId);

	[DllImport("__Internal")]
	public static extern void U3DCustomEvent(string eventID, string _channelID, string _account, string _gameId);

	[DllImport("__Internal")]
	public static extern void U3DsnailLevelUpload(string appid, string userid, string gamelevel, string svname, string serverid, string roleName);

	[DllImport("__Internal")]
	public static extern void U3DsnailVIPLevelUpload(string gamelevel, string serverid, string roleName);

	[DllImport("__Internal")]
	public static extern void U3DsnailGuideOver();

	[DllImport("__Internal")]
	public static extern void U3DBindMobile(string session, string aid);

	[DllImport("__Internal")]
	public static extern void U3DgetCurrentVersion();

	[DllImport("__Internal")]
	public static extern void U3DgetChannelAnySDK();

	[DllImport("__Internal")]
	public static extern void U3DgetChannelAdressId();

	[DllImport("__Internal")]
	public static extern void U3DgetChannelUniqueName();

	[DllImport("__Internal")]
	public static extern void U3DGetCID();

	[DllImport("__Internal")]
	public static extern void U3DGetIDFA();

	public void currentVersionFinished(string str)
	{
		PlatformUtils.Instance.currentVersionFinished(str);
	}

	public void OnGetChannelUniqueNameCallback(string str)
	{
		PlatformUtils.Instance.OnGetChannelUniqueNameCallback(str);
	}

	public void OnGetChannelAdressIdCallback(string str)
	{
		PlatformUtils.Instance.OnGetChannelAdressIdCallback(str);
	}

	public void OnGetChannelAnySDKCallback(string str)
	{
		PlatformUtils.Instance.OnGetChannelAnySDKCallBack(str);
	}

	public void SetChannelName(string str)
	{
		PlatformUtils.Instance.OnGetChannelNameCallBack(str);
	}

	[DllImport("__Internal")]
	public static extern void U3DshowCentificationView(string strAid);

	[DllImport("__Internal")]
	public static extern void U3DInitGameCenter();

	[DllImport("__Internal")]
	public static extern void U3DloadAchieveMents();

	[DllImport("__Internal")]
	public static extern void U3DShowGameCenterWithCatagory(string category);

	[DllImport("__Internal")]
	public static extern void U3DShowBanner(string title, string msg);

	[DllImport("__Internal")]
	public static extern void U3DReportScore(int score, string category);

	[DllImport("__Internal")]
	public static extern void U3DSubmitAllSavedScores();

	[DllImport("__Internal")]
	public static extern void U3DReportAchievement(string identifier, double percentComplete);

	[DllImport("__Internal")]
	public static extern bool U3DUserAuthenticated();

	[DllImport("__Internal")]
	public static extern bool U3DIsSupportReplay();

	[DllImport("__Internal")]
	public static extern void U3DStartRecording();

	[DllImport("__Internal")]
	public static extern void U3DStopRecording();

	[DllImport("__Internal")]
	public static extern void U3DDiscardRecording();

	[DllImport("__Internal")]
	public static extern void U3DDisplayRecordingContent();
}
