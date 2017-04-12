using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideUI : MonoBehaviour
{
	private const float ORIGIN_UI_HEIGHT = 720f;

	private const float ORIGIN_UI_WIDTH = 1280f;

	private const float miThumbWidth = 886f;

	public const int miBtnLeftPosX = -131;

	public const int miBtnRightPosX = 135;

	public const int miBtnCenterPosX = -5;

	public const int miBtnPosY = -135;

	private const float defualtMin = -488f;

	private const int width = -873;

	private static GuideUI Instance;

	private Camera uiCamera;

	private GameObject moMessageBox;

	private GameObject uiMessageBox;

	private EventDelegate.Callback mOkCallback;

	private EventDelegate.Callback mCancelCallback;

	private UISprite mSpLeft;

	private UISprite mSpRight;

	private UILabel mlblRate;

	private UISprite mSpIcon;

	private Dictionary<string, Dictionary<string, string>> mConfigs = new Dictionary<string, Dictionary<string, string>>();

	private GameObject uiLoading;

	private GameObject muiBackPanel;

	private GameObject muiEffect;

	private UILabel m_UIProgressLabel;

	private UILabel m_UIProgressRate;

	private UISprite m_UIProgressThumb;

	private UISprite m_UIProgressEffect;

	private UILabel mlblDestVersion;

	private UILabel mlblSourceVersion;

	private int lastLoadSize;

	private float lastTime;

	private string strUpdateText = string.Empty;

	public static GameObject StaticShowMBox(int iType, string strInfo, EventDelegate.Callback onMok, EventDelegate.Callback onMCancel, string strTitle = "")
	{
		if (GuideUI.Instance != null)
		{
			return GuideUI.Instance.ShowMBox(iType, strInfo, onMok, onMCancel, strTitle);
		}
		return null;
	}

	private void ParseEffectConfig(string strContent)
	{
		if (string.IsNullOrEmpty(strContent))
		{
			return;
		}
		XMLParser xMLParser = new XMLParser();
		XMLNode xMLNode = xMLParser.Parse(strContent);
		XMLNodeList nodeList = xMLNode.GetNodeList("Configs>0>Config");
		if (nodeList != null)
		{
			for (int i = 0; i < nodeList.Count; i++)
			{
				XMLNode xMLNode2 = nodeList[i] as XMLNode;
				string key = string.Empty;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (DictionaryEntry dictionaryEntry in xMLNode2)
				{
					if (dictionaryEntry.Value != null)
					{
						string text = dictionaryEntry.Key as string;
						if (text[0] == '@')
						{
							text = text.Replace("@", string.Empty);
							if (text == "ID")
							{
								key = (string)dictionaryEntry.Value;
							}
							if (!dictionary.ContainsKey(text))
							{
								dictionary.Add(text, (string)dictionaryEntry.Value);
							}
						}
					}
				}
				if (!this.mConfigs.ContainsKey(key))
				{
					this.mConfigs.Add(key, dictionary);
				}
			}
		}
	}

	public string GetConfig(string strSection, string strKey)
	{
		Dictionary<string, string> dictionary = null;
		if (this.mConfigs.TryGetValue(strSection, out dictionary) && dictionary.ContainsKey(strKey))
		{
			return dictionary[strKey];
		}
		return string.Empty;
	}

	public float GetConfigToFloat(string strSection, string strKey)
	{
		float result = 0f;
		Dictionary<string, string> dictionary = null;
		if (this.mConfigs.TryGetValue(strSection, out dictionary) && dictionary.ContainsKey(strKey) && float.TryParse(dictionary[strKey], out result))
		{
			return result;
		}
		return result;
	}

	private void Awake()
	{
		GuideUI.Instance = this;
		GameObject gameObject = GameObject.FindWithTag("UICamera");
		if (gameObject == null)
		{
			return;
		}
		this.uiCamera = gameObject.GetComponent<Camera>();
		if (this.uiCamera == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"UICamera is not find"
			});
			return;
		}
		if (this.uiCamera.gameObject.GetComponent<UICamera>() == null)
		{
			this.uiCamera.gameObject.AddComponent<UICamera>();
		}
		if ((float)ResolutionConstrain.Instance.width * 1f / (float)ResolutionConstrain.Instance.height >= 1.77777779f)
		{
			this.uiCamera.orthographicSize = 720f / (float)ResolutionConstrain.Instance.height;
		}
		else
		{
			this.uiCamera.orthographicSize = 1280f / (float)ResolutionConstrain.Instance.width;
		}
	}

	private void Start()
	{
		TextAsset textAsset = Resources.Load("Local/Config/GameConfig") as TextAsset;
		if (textAsset != null)
		{
			this.ParseEffectConfig(textAsset.text);
		}
		this.CreateBackPanel();
		this.ShowInfo(Config.GetUdpateLangage("GameInit"));
	}

	public GameObject ShowMBox(int iType, string strInfo, EventDelegate.Callback onMok, EventDelegate.Callback onMCancel, string strTitle = "")
	{
		if (strInfo == null)
		{
			return null;
		}
		if (this.moMessageBox == null)
		{
			this.moMessageBox = (Resources.Load("Local/Prefabs/UI/MessageBox") as GameObject);
		}
		if (this.moMessageBox == null)
		{
			return null;
		}
		if (this.uiMessageBox == null)
		{
			this.uiMessageBox = (UnityEngine.Object.Instantiate(this.moMessageBox, Vector3.zero, Quaternion.identity) as GameObject);
			this.uiMessageBox.transform.parent = this.uiCamera.transform;
			this.uiMessageBox.transform.localPosition = Vector3.zero;
			this.uiMessageBox.transform.localScale = Vector3.one;
		}
		Transform transform = this.uiMessageBox.transform.FindChild("title");
		if (transform != null)
		{
			UILabel component = transform.GetComponent<UILabel>();
			string text = strTitle;
			if (string.IsNullOrEmpty(text))
			{
				text = Config.GetUdpateLangage("title");
			}
			component.text = text;
		}
		Transform transform2 = this.uiMessageBox.transform.FindChild("content");
		if (transform2 != null)
		{
			UILabel component2 = transform2.GetComponent<UILabel>();
			component2.text = strInfo.Replace("[n]", "\n");
		}
		this.mOkCallback = onMok;
		this.mCancelCallback = onMCancel;
		switch (iType)
		{
		case 1:
		{
			Transform transform3 = this.uiMessageBox.transform.FindChild("sure");
			Vector3 zero = Vector3.zero;
			zero.x = -5f;
			zero.y = -135f;
			transform3.transform.localPosition = zero;
			if (transform3)
			{
				Transform transform4 = transform3.transform.FindChild("Label");
				if (transform4)
				{
					UILabel component3 = transform4.GetComponent<UILabel>();
					component3.text = Config.GetUdpateLangage("SureBtn");
				}
				UIEventListener.Get(transform3.gameObject).onClick = new UIEventListener.VoidDelegate(this.OkEventHandler);
				transform3.gameObject.SetActive(true);
			}
			this.ShowEffect(false);
			break;
		}
		case 2:
		{
			Transform transform5 = this.uiMessageBox.transform.FindChild("sure");
			Transform transform6 = this.uiMessageBox.transform.FindChild("cancel");
			if (transform5)
			{
				Vector3 zero2 = Vector3.zero;
				zero2.x = -131f;
				zero2.y = -135f;
				transform5.localPosition = zero2;
				UILabel component4 = transform5.transform.FindChild("Label").GetComponent<UILabel>();
				if (component4 != null)
				{
					component4.text = Config.GetUdpateLangage("SureBtn");
				}
				UIEventListener.Get(transform5.gameObject).onClick = new UIEventListener.VoidDelegate(this.OkEventHandler);
				transform5.gameObject.SetActive(true);
			}
			if (transform6)
			{
				Vector3 zero3 = Vector3.zero;
				zero3.x = 135f;
				zero3.y = -135f;
				transform6.localPosition = zero3;
				UILabel component5 = transform6.transform.FindChild("Label").GetComponent<UILabel>();
				if (component5 != null)
				{
					component5.text = Config.GetUdpateLangage("CancleBtn");
				}
				UIEventListener.Get(transform6.gameObject).onClick = new UIEventListener.VoidDelegate(this.CancelEventHandler);
				transform6.gameObject.SetActive(true);
			}
			this.ShowEffect(false);
			break;
		}
		case 3:
		{
			Transform transform7 = this.uiMessageBox.transform.FindChild("sure");
			Transform transform8 = this.uiMessageBox.transform.FindChild("cancel");
			if (transform7)
			{
				Vector3 zero4 = Vector3.zero;
				zero4.x = -131f;
				zero4.y = -135f;
				transform7.localPosition = zero4;
				Transform exists = transform7.transform.FindChild("Label");
				if (exists)
				{
					UILabel component6 = transform7.transform.FindChild("Label").GetComponent<UILabel>();
					if (component6 != null)
					{
						component6.text = Config.GetUdpateLangage("ReTryBtn");
					}
				}
				UIEventListener.Get(transform7.gameObject).onClick = new UIEventListener.VoidDelegate(this.OkEventHandler);
				transform7.gameObject.SetActive(true);
			}
			if (transform8)
			{
				Vector3 zero5 = Vector3.zero;
				zero5.x = 135f;
				zero5.y = -135f;
				transform8.localPosition = zero5;
				Transform transform9 = transform8.transform.FindChild("Label");
				if (transform9)
				{
					UILabel component7 = transform9.GetComponent<UILabel>();
					if (component7 != null)
					{
						component7.text = Config.GetUdpateLangage("ExitBtn");
					}
				}
				UIEventListener.Get(transform8.gameObject).onClick = new UIEventListener.VoidDelegate(this.CancelEventHandler);
				transform8.gameObject.SetActive(true);
			}
			this.ShowEffect(false);
			break;
		}
		case 4:
		{
			Transform transform10 = this.uiMessageBox.transform.FindChild("sure");
			Transform transform11 = this.uiMessageBox.transform.FindChild("cancel");
			if (transform10)
			{
				Vector3 zero6 = Vector3.zero;
				zero6.x = -131f;
				zero6.y = -135f;
				transform10.localPosition = zero6;
				Transform exists2 = transform10.transform.FindChild("Label");
				if (exists2)
				{
					UILabel component8 = transform10.transform.FindChild("Label").GetComponent<UILabel>();
					if (component8 != null)
					{
						component8.text = Config.GetUdpateLangage("DownLoadBtn");
					}
				}
				UIEventListener.Get(transform10.gameObject).onClick = new UIEventListener.VoidDelegate(this.OkEventHandler);
				transform10.gameObject.SetActive(true);
			}
			if (transform11)
			{
				Vector3 zero7 = Vector3.zero;
				zero7.x = 135f;
				zero7.y = -135f;
				transform11.localPosition = zero7;
				Transform transform12 = transform11.transform.FindChild("Label");
				if (transform12)
				{
					UILabel component9 = transform12.GetComponent<UILabel>();
					if (component9 != null)
					{
						component9.text = Config.GetUdpateLangage("ExitBtn");
					}
				}
				UIEventListener.Get(transform11.gameObject).onClick = new UIEventListener.VoidDelegate(this.CancelEventHandler);
				transform11.gameObject.SetActive(true);
			}
			this.ShowEffect(false);
			break;
		}
		}
		return this.uiMessageBox;
	}

	private void OkEventHandler(GameObject go)
	{
		if (this.mOkCallback != null)
		{
			this.mOkCallback();
			this.mOkCallback = null;
		}
		this.ShowEffect(true);
		UnityEngine.Object.DestroyImmediate(this.uiMessageBox);
	}

	private void CancelEventHandler(GameObject go)
	{
		if (this.mCancelCallback != null)
		{
			this.mCancelCallback();
			this.mCancelCallback = null;
		}
		this.ShowEffect(true);
		UnityEngine.Object.DestroyImmediate(this.uiMessageBox);
	}

	public void SetVersion(string strSourceVersion, string strDestVersion)
	{
		if (this.uiLoading == null)
		{
			return;
		}
		this.ShowProgressInfo(true);
		if (this.mlblSourceVersion != null)
		{
			if (string.IsNullOrEmpty(strDestVersion))
			{
				this.mlblSourceVersion.text = "0.0.0";
			}
			else
			{
				this.mlblSourceVersion.text = strDestVersion;
			}
		}
		if (this.mlblDestVersion != null)
		{
			this.mlblDestVersion.text = strSourceVersion;
		}
	}

	public void CreateBackPanel()
	{
		if (this.muiBackPanel == null)
		{
			GameObject original = Resources.Load("Local/Prefabs/UI/BackPanel") as GameObject;
			this.muiBackPanel = (UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject);
			if (this.muiBackPanel != null)
			{
				this.muiBackPanel.transform.parent = this.uiCamera.transform;
				this.muiBackPanel.transform.localPosition = Vector3.zero;
				this.muiBackPanel.transform.localScale = Vector3.one;
				string config = this.GetConfig("GuideUI", "EffectName");
				if (!string.IsNullOrEmpty(config))
				{
					GameObject gameObject = Resources.Load(config) as GameObject;
					if (gameObject != null)
					{
						this.muiEffect = (UnityEngine.Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject);
						if (this.muiEffect != null)
						{
							Transform transform = this.muiBackPanel.transform.FindChild("back");
							if (transform != null)
							{
								this.muiEffect.transform.parent = transform;
								Vector3 zero = Vector3.zero;
								float configToFloat = this.GetConfigToFloat("GuideUI", "PosY");
								float configToFloat2 = this.GetConfigToFloat("GuideUI", "PosZ");
								zero.y = configToFloat;
								zero.z = configToFloat2;
								this.muiEffect.transform.localPosition = zero;
								float configToFloat3 = this.GetConfigToFloat("GuideUI", "RotX");
								float configToFloat4 = this.GetConfigToFloat("GuideUI", "RotY");
								float configToFloat5 = this.GetConfigToFloat("GuideUI", "RotZ");
								zero.x = configToFloat3;
								zero.y = configToFloat4;
								zero.z = configToFloat5;
								this.muiEffect.transform.localRotation = Quaternion.Euler(zero);
								float configToFloat6 = this.GetConfigToFloat("GuideUI", "Scale");
								this.muiEffect.transform.localScale = Vector3.one * configToFloat6;
								this.muiEffect.AddComponent<AdaptiveRenderQueue>();
							}
						}
					}
				}
				Transform transform2 = this.muiBackPanel.transform.FindChild("Panel/lblRate");
				if (transform2 != null)
				{
					this.mlblRate = transform2.GetComponent<UILabel>();
					if (this.mlblRate != null)
					{
						this.mlblRate.gameObject.SetActive(false);
					}
				}
			}
		}
		if (this.uiLoading == null)
		{
			GameObject original2 = Resources.Load("Local/Prefabs/UI/LoadingPanel") as GameObject;
			this.uiLoading = (UnityEngine.Object.Instantiate(original2, Vector3.zero, Quaternion.identity) as GameObject);
			this.uiLoading.transform.parent = this.uiCamera.transform;
			this.uiLoading.transform.localPosition = Vector3.zero;
			this.uiLoading.transform.localScale = Vector3.one;
			this.m_UIProgressLabel = this.uiLoading.transform.FindChild("content").GetComponent<UILabel>();
			this.m_UIProgressRate = this.uiLoading.transform.FindChild("rate").GetComponent<UILabel>();
			this.m_UIProgressThumb = this.uiLoading.transform.FindChild("back_thumb").GetComponent<UISprite>();
			this.m_UIProgressEffect = this.uiLoading.transform.FindChild("effect").GetComponent<UISprite>();
			this.mlblSourceVersion = this.uiLoading.transform.FindChild("SourceVersion").GetComponent<UILabel>();
			this.mlblDestVersion = this.uiLoading.transform.FindChild("DestVersion").GetComponent<UILabel>();
			this.mSpLeft = this.uiLoading.transform.FindChild("back_bar_Left").GetComponent<UISprite>();
			this.mSpRight = this.uiLoading.transform.FindChild("back_bar_Right").GetComponent<UISprite>();
			this.mSpIcon = this.uiLoading.transform.FindChild("Icon").GetComponent<UISprite>();
			this.ShowProgressInfo(false);
		}
	}

	private void SetObjRenderQ(GameObject oModel, int iRenderQueue)
	{
		if (oModel == null)
		{
			return;
		}
		Renderer[] componentsInChildren = oModel.GetComponentsInChildren<Renderer>(true);
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i];
				if (renderer != null)
				{
					renderer.material.renderQueue = iRenderQueue;
				}
			}
		}
	}

	public void ShowLoadingPanel()
	{
		if (this.uiLoading == null)
		{
			GameObject original = Resources.Load("Local/Prefabs/UI/LoadingPanel") as GameObject;
			this.uiLoading = (UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject);
			this.uiLoading.transform.parent = this.uiCamera.transform;
			this.uiLoading.transform.localPosition = Vector3.zero;
			this.uiLoading.transform.localScale = Vector3.one;
			this.m_UIProgressLabel = this.uiLoading.transform.FindChild("content").GetComponent<UILabel>();
			this.m_UIProgressRate = this.uiLoading.transform.FindChild("rate").GetComponent<UILabel>();
			this.m_UIProgressThumb = this.uiLoading.transform.FindChild("back_thumb").GetComponent<UISprite>();
			this.m_UIProgressEffect = this.uiLoading.transform.FindChild("effect").GetComponent<UISprite>();
			this.mlblSourceVersion = this.uiLoading.transform.FindChild("SourceVersion").GetComponent<UILabel>();
			this.mlblDestVersion = this.uiLoading.transform.FindChild("DestVersion").GetComponent<UILabel>();
			this.mSpLeft = this.uiLoading.transform.FindChild("back_bar_Left").GetComponent<UISprite>();
			this.mSpRight = this.uiLoading.transform.FindChild("back_bar_Right").GetComponent<UISprite>();
			this.mSpIcon = this.uiLoading.transform.FindChild("Icon").GetComponent<UISprite>();
			this.ShowProgressInfo(false);
		}
	}

	private void ShowProgressInfo(bool bShow)
	{
		this.m_UIProgressLabel.gameObject.SetActive(bShow);
		this.m_UIProgressThumb.gameObject.SetActive(bShow);
		this.m_UIProgressEffect.gameObject.SetActive(bShow);
		this.mlblSourceVersion.gameObject.SetActive(bShow);
		this.mlblDestVersion.gameObject.SetActive(bShow);
		this.mSpLeft.gameObject.SetActive(bShow);
		this.mSpRight.gameObject.SetActive(bShow);
		this.mSpIcon.gameObject.SetActive(!bShow);
	}

	public void ShowInfo(string strInfo)
	{
		if (this.m_UIProgressRate != null)
		{
			this.m_UIProgressRate.text = strInfo;
		}
	}

	public void SetRate(int iRateValue)
	{
		if (iRateValue < 0)
		{
			iRateValue = 0;
		}
		if (iRateValue > 100)
		{
			iRateValue = 100;
		}
		if (this.mlblRate != null)
		{
			this.mlblRate.text = string.Format("{0}%", iRateValue);
		}
	}

	public void ShowRate(bool bShow)
	{
		if (this.mlblRate != null)
		{
			this.mlblRate.gameObject.SetActive(bShow);
		}
	}

	public void ShowEffect(bool bShow)
	{
		if (this.muiEffect != null && this.muiEffect.activeSelf != bShow)
		{
			this.muiEffect.gameObject.SetActive(bShow);
		}
	}

	public void ShowProgress(int iType, int iLoadSize, int iTotalSize)
	{
		if (this.uiLoading == null)
		{
			return;
		}
		iLoadSize /= 1024;
		iTotalSize /= 1024;
		if (iTotalSize == 0)
		{
			iTotalSize = 1;
		}
		this.lastLoadSize = 0;
		this.lastTime = 0f;
		if (this.m_UIProgressThumb != null)
		{
			this.m_UIProgressThumb.width = Convert.ToInt32(886f * (float)iLoadSize / (float)iTotalSize);
		}
		if (this.m_UIProgressLabel != null)
		{
			this.m_UIProgressLabel.text = string.Concat(new object[]
			{
				iLoadSize,
				"/",
				iTotalSize,
				"   ",
				(iLoadSize * 100 / iTotalSize).ToString(),
				"%"
			});
		}
		if (this.m_UIProgressEffect != null && this.m_UIProgressThumb != null)
		{
			Vector3 zero = Vector3.zero;
			zero.x = -488f - (float)(-873 * iLoadSize / iTotalSize);
			zero.y = this.m_UIProgressEffect.transform.localPosition.y;
			zero.z = 0f;
			this.m_UIProgressEffect.transform.localPosition = zero;
		}
		if (iLoadSize - this.lastLoadSize > 0)
		{
			int num = (int)((float)(iLoadSize - this.lastLoadSize) / (Time.time - this.lastTime));
			if (iType == 0)
			{
				this.m_UIProgressRate.text = "Unpacking " + num.ToString() + " KB/s";
			}
			else
			{
				if (string.IsNullOrEmpty(this.strUpdateText))
				{
					this.strUpdateText = Config.GetUdpateLangage("Updating");
				}
				this.m_UIProgressRate.text = this.strUpdateText + num.ToString() + " KB/s";
			}
			this.lastLoadSize = iLoadSize;
			this.lastTime = Time.time;
		}
	}

	public void OnCompleted(int iType)
	{
		if (iType == 0)
		{
			this.m_UIProgressRate.text = "Unpack finished,Now Wait check remote update!";
			this.ShowProgressInfo(false);
		}
		else
		{
			this.m_UIProgressRate.text = "Update is finished,Now Wait Start Game!";
		}
	}

	private void OnDestroy()
	{
		if (this.muiEffect != null)
		{
			UnityEngine.Object.Destroy(this.muiEffect);
			this.muiEffect = null;
		}
		if (this.muiBackPanel != null)
		{
			UnityEngine.Object.Destroy(this.muiBackPanel);
			this.muiBackPanel = null;
		}
		if (this.uiLoading != null)
		{
			UnityEngine.Object.Destroy(this.uiLoading);
			this.uiLoading = null;
		}
		this.uiCamera = null;
		this.mlblRate = null;
		this.moMessageBox = null;
		this.uiMessageBox = null;
		this.mOkCallback = null;
		this.mCancelCallback = null;
		this.uiLoading = null;
		this.muiBackPanel = null;
		this.m_UIProgressLabel = null;
		this.m_UIProgressRate = null;
		this.m_UIProgressThumb = null;
		this.m_UIProgressEffect = null;
		this.mlblDestVersion = null;
		this.mlblSourceVersion = null;
		this.mSpLeft = null;
		this.mSpRight = null;
		this.mSpIcon = null;
		GuideUI.Instance = null;
	}

	public void OnEntryGame()
	{
		if (this.muiEffect != null)
		{
			UnityEngine.Object.Destroy(this.muiEffect);
			this.muiEffect = null;
		}
		if (this.muiBackPanel != null)
		{
			UnityEngine.Object.Destroy(this.muiBackPanel);
			this.muiBackPanel = null;
		}
		if (this.uiLoading != null)
		{
			UnityEngine.Object.Destroy(this.uiLoading);
			this.uiLoading = null;
		}
	}
}
