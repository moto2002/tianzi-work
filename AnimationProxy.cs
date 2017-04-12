using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationProxy : MonoBehaviour
{
	[Serializable]
	public class AnimationInfo
	{
		public string strName = string.Empty;

		public string strPath = string.Empty;
	}

	public delegate void OnAnimationConfigLoaded(AnimationProxy animationProxy);

	public delegate void CallLoadAsset(string strFileName, AssetCallBack callback);

	public const int miActionListMaxCount = 5;

	private const string ACTION_PAO = "run";

	private const string ACTION_STAND = "idle";

	public string modelAcionID;

	[SerializeField]
	public AnimationProxy.AnimationInfo[] mAnimations;

	[SerializeField]
	public AnimationProxy.AnimationInfo mMainClip;

	private string currentClipName;

	private Dictionary<string, string> mAnimationMaps = new Dictionary<string, string>();

	public Animation mAnimation;

	public List<string> mActionControlList = new List<string>();

	public Dictionary<string, AnimationClip> mActionLoadedDict = new Dictionary<string, AnimationClip>();

	private string initedAction = string.Empty;

	private object[] initedArgs;

	private bool configLoaded;

	private bool inited;

	private GameObject mGo;

	private AnimationProxy.OnAnimationConfigLoaded onAnimationConfigLoaded;

	public static AnimationProxy.CallLoadAsset monLoadAsset;

	private List<object[]> mActionWaitQueue = new List<object[]>();

	public string CurrentClipName
	{
		get
		{
			return this.currentClipName;
		}
	}

	public AnimationState this[string tkey]
	{
		get
		{
			if (this.mAnimation == null)
			{
				return null;
			}
			return this.mAnimation[tkey];
		}
	}

	private void Awake()
	{
		this.mGo = base.gameObject;
		this.mAnimation = this.mGo.GetComponent<Animation>();
		if (this.mAnimation == null)
		{
			this.mAnimation = this.mGo.AddComponent<Animation>();
		}
		if (this.mAnimation == null)
		{
			return;
		}
		this.mAnimation.playAutomatically = true;
		this.mAnimation.enabled = false;
		this.mAnimation.enabled = true;
		if (this.mAnimations == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"mAnimations is NULL " + this.mGo.name
			});
			return;
		}
		for (int i = 0; i < this.mAnimations.Length; i++)
		{
			AnimationProxy.AnimationInfo animationInfo = this.mAnimations[i];
			if (animationInfo != null)
			{
				if (!this.mAnimationMaps.ContainsKey(animationInfo.strName))
				{
					this.mAnimationMaps.Add(animationInfo.strName, animationInfo.strPath);
				}
			}
		}
		if (this.mAnimations.Length > 0)
		{
			this.configLoaded = true;
		}
		if (!this.mAnimationMaps.ContainsKey(this.mMainClip.strName))
		{
			this.mAnimationMaps.Add(this.mMainClip.strName, this.mMainClip.strPath);
		}
		if (!string.IsNullOrEmpty(this.modelAcionID))
		{
			string strFileName = DelegateProxy.StringBuilder(new object[]
			{
				"Config/AnimationInfos/",
				this.modelAcionID
			});
			DelegateProxy.LoadAsset(strFileName, new AssetCallBack(this.AniInfoAssetLoaded));
		}
		else
		{
			this.OnStart();
			if (this.onAnimationConfigLoaded != null)
			{
				this.onAnimationConfigLoaded(this);
			}
			LogSystem.LogWarning(new object[]
			{
				"Model does not has model action info,",
				this.mGo.name
			});
		}
	}

	private void AniInfoAssetLoaded(params object[] args)
	{
		AnimationProxyInfo animationProxyInfo = args[0] as AnimationProxyInfo;
		if (animationProxyInfo != null)
		{
			this.mAnimations = animationProxyInfo.mAnimations;
			this.mMainClip = animationProxyInfo.mMainClip;
			for (int i = 0; i < this.mAnimations.Length; i++)
			{
				AnimationProxy.AnimationInfo animationInfo = this.mAnimations[i];
				if (animationInfo != null)
				{
					if (!this.mAnimationMaps.ContainsKey(animationInfo.strName))
					{
						this.mAnimationMaps.Add(animationInfo.strName, animationInfo.strPath);
					}
				}
			}
			if (!this.mAnimationMaps.ContainsKey(this.mMainClip.strName))
			{
				this.mAnimationMaps.Add(this.mMainClip.strName, this.mMainClip.strPath);
			}
			this.configLoaded = true;
			this.OnStart();
			if (this.onAnimationConfigLoaded != null)
			{
				this.onAnimationConfigLoaded(this);
			}
		}
	}

	public void SetConfigLoadedDelegate(AnimationProxy.OnAnimationConfigLoaded call)
	{
		this.onAnimationConfigLoaded = call;
		if (this.configLoaded && this.onAnimationConfigLoaded != null)
		{
			this.onAnimationConfigLoaded(this);
		}
	}

	public static void SetLoadAssetCall(AnimationProxy.CallLoadAsset call)
	{
		AnimationProxy.monLoadAsset = call;
	}

	public void LoadAnimationClip(string strAnimationName)
	{
		string animationPath = this.GetAnimationPath(strAnimationName);
		if (string.IsNullOrEmpty(animationPath))
		{
			return;
		}
		if (AnimationProxy.monLoadAsset != null)
		{
			AnimationProxy.monLoadAsset(animationPath, delegate(object[] args)
			{
				AnimationClip animationClip = args[0] as AnimationClip;
				if (animationClip != null)
				{
					this.AddAnimationClip(strAnimationName, animationClip);
				}
				this.CheckAnimationPlay();
			});
		}
	}

	public void LoadAnimationClipNoPlay(string strAnimationName)
	{
		string animationPath = this.GetAnimationPath(strAnimationName);
		if (string.IsNullOrEmpty(animationPath))
		{
			return;
		}
		if (AnimationProxy.monLoadAsset != null)
		{
			AnimationProxy.monLoadAsset(animationPath, delegate(object[] args)
			{
				AnimationClip animationClip = args[0] as AnimationClip;
				if (animationClip != null)
				{
					this.AddAnimationClip(strAnimationName, animationClip);
				}
			});
		}
	}

	private void PreLoadAction()
	{
		if (this.mMainClip != null && this.mMainClip.strName != "idle")
		{
			this.PreLoadAction("idle");
		}
		if (this.mMainClip != null && this.mMainClip.strName != "run")
		{
			this.PreLoadAction("run");
		}
	}

	private void PreLoadAction(string strAction)
	{
		if (!this.mAnimationMaps.ContainsKey(strAction))
		{
			return;
		}
		if (this.mActionControlList.Count >= 5)
		{
			return;
		}
		if (this.mActionControlList.Contains(strAction))
		{
			return;
		}
		this.LoadAnimationClipNoPlay(strAction);
	}

	private void Start()
	{
		this.inited = true;
		if (this.configLoaded)
		{
			this.OnStart();
		}
	}

	private void OnStart()
	{
		this.PreLoadAction();
		if (!string.IsNullOrEmpty(this.initedAction) && this.mAnimationMaps.ContainsKey(this.initedAction) && this.initedArgs != null)
		{
			this.PlayAnimation(this.initedArgs);
			this.LoadAnimationClip(this.initedAction);
		}
		else if (this.mMainClip != null && !string.IsNullOrEmpty(this.mMainClip.strName) && string.IsNullOrEmpty(this.GetPlayingAnimationName()) && this.mActionWaitQueue.Count == 0)
		{
			object[] expr_9C = new object[7];
			expr_9C[0] = this.mMainClip.strName;
			expr_9C[1] = ActionPlayType.APT_PLAY;
			expr_9C[2] = -1f;
			expr_9C[3] = -1f;
			expr_9C[4] = string.Empty;
			this.PlayAnimation(expr_9C);
			this.LoadAnimationClip(this.mMainClip.strName);
		}
	}

	public bool PlayAnimation(params object[] args)
	{
		if (args == null || args.Length == 0)
		{
			return false;
		}
		string text = args[0] as string;
		if (!this.configLoaded || !this.inited)
		{
			this.initedAction = text;
			this.initedArgs = args;
		}
		if (!this.mAnimationMaps.ContainsKey(text))
		{
			this.PlayClipFail(args);
			return false;
		}
		for (int i = 0; i < this.mActionWaitQueue.Count; i++)
		{
			object[] array = this.mActionWaitQueue[i];
			if (text == array[0])
			{
				this.mActionWaitQueue.RemoveAt(i);
				break;
			}
		}
		this.mActionWaitQueue.Add(args);
		return true;
	}

	public void CheckAnimationPlay()
	{
		if (this.mActionWaitQueue.Count == 0)
		{
			return;
		}
		for (int i = this.mActionWaitQueue.Count - 1; i >= 0; i--)
		{
			object[] array = this.mActionWaitQueue[i];
			string key = array[0] as string;
			if (this.mActionLoadedDict.ContainsKey(key))
			{
				this.mActionWaitQueue.RemoveRange(0, i + 1);
				this.PlayClip(array);
				break;
			}
		}
	}

	private void PlayClipFail(object[] args)
	{
		string strAnimationName = args[0] as string;
		if (args.Length > 6)
		{
			string strNextAnimation = args[4] as string;
			OnWatchAnimationPlayed onWatchAnimationPlayed = args[5] as OnWatchAnimationPlayed;
			OnAnimationPlayCallBack onAnimationPlayCallBack = args[6] as OnAnimationPlayCallBack;
			if (onWatchAnimationPlayed != null)
			{
				onWatchAnimationPlayed(this, strAnimationName, strNextAnimation);
			}
			if (onAnimationPlayCallBack != null)
			{
				onAnimationPlayCallBack(strAnimationName, this);
			}
		}
	}

	private void PlayClip(object[] args)
	{
		string text = args[0] as string;
		if ((int)args[1] == 0)
		{
			float fSpeed = (float)args[2];
			float fNormalized = (float)args[3];
			string strNextAnimation = args[4] as string;
			OnWatchAnimationPlayed onWatchAnimationPlayed = args[5] as OnWatchAnimationPlayed;
			if (onWatchAnimationPlayed != null)
			{
				onWatchAnimationPlayed(this, text, strNextAnimation);
			}
			this.PlayAnimationClip(text, fSpeed, fNormalized);
			this.currentClipName = text;
			if (args.Length > 6)
			{
				OnAnimationPlayCallBack onAnimationPlayCallBack = args[6] as OnAnimationPlayCallBack;
				if (onAnimationPlayCallBack != null)
				{
					onAnimationPlayCallBack(text, this);
				}
			}
		}
		else
		{
			float fFade = (float)args[2];
			float fSpeed2 = (float)args[3];
			string strNextAnimation2 = args[4] as string;
			OnWatchAnimationPlayed onWatchAnimationPlayed2 = args[5] as OnWatchAnimationPlayed;
			if (onWatchAnimationPlayed2 != null)
			{
				onWatchAnimationPlayed2(this, text, strNextAnimation2);
			}
			this.CrossFadeClip(text, fFade, fSpeed2);
			this.currentClipName = text;
			if (args.Length > 6)
			{
				OnAnimationPlayCallBack onAnimationPlayCallBack2 = args[6] as OnAnimationPlayCallBack;
				if (onAnimationPlayCallBack2 != null)
				{
					onAnimationPlayCallBack2(text, this);
				}
			}
		}
	}

	public string GetAnimationPath(string strName)
	{
		if (this.mAnimations == null || this.mAnimationMaps == null)
		{
			return string.Empty;
		}
		string result;
		this.mAnimationMaps.TryGetValue(strName, out result);
		return result;
	}

	public string GetEditorAnimationPath(string strName)
	{
		if (this.mAnimations == null)
		{
			return string.Empty;
		}
		for (int i = 0; i < this.mAnimations.Length; i++)
		{
			if (this.mAnimations[i] != null && this.mAnimations[i].strName == strName)
			{
				return this.mAnimations[i].strPath;
			}
		}
		return string.Empty;
	}

	public bool IsAnimationClipLoaded(string strAnimation)
	{
		return this.mActionLoadedDict.ContainsKey(strAnimation);
	}

	private void AddAnimationClip(string strClipName, AnimationClip aClip)
	{
		if (this.mAnimation == null || aClip == null)
		{
			return;
		}
		if (this.IsAnimationClipLoaded(strClipName))
		{
			return;
		}
		if (this.mMainClip != null && this.mMainClip.strName == strClipName)
		{
			if (!this.mActionLoadedDict.ContainsKey(strClipName))
			{
				this.mActionLoadedDict.Add(strClipName, aClip);
			}
			this.mAnimation.clip = aClip;
			this.mAnimation.AddClip(aClip, strClipName);
		}
		else
		{
			this.mActionControlList.Add(strClipName);
			if (!this.mActionLoadedDict.ContainsKey(strClipName))
			{
				this.mActionLoadedDict.Add(strClipName, aClip);
			}
			this.mAnimation.AddClip(aClip, strClipName);
			this.PopAnimationControl();
		}
	}

	private void PopAnimationControl()
	{
		if (this.mActionControlList == null || this.mAnimation == null)
		{
			return;
		}
		if (this.mActionControlList.Count > 5)
		{
			string text = this.mActionControlList[0];
			this.mActionControlList.RemoveAt(0);
			if (this.mActionLoadedDict.ContainsKey(text))
			{
				this.mActionLoadedDict.Remove(text);
			}
			AnimationClip clip = this.mAnimation.GetClip(text);
			if (clip != null)
			{
				this.mAnimation.RemoveClip(text);
				DelegateProxy.PopCache(clip);
			}
		}
	}

	public bool IsPlaying(string strAnimation)
	{
		return !(this.mAnimation == null) && this.mAnimation.IsPlaying(strAnimation);
	}

	public void SequenceList(string strAnimation)
	{
		if (this.mActionControlList == null)
		{
			return;
		}
		int num = this.mActionControlList.IndexOf(strAnimation);
		if (num != -1 && num != this.mActionControlList.Count - 1)
		{
			this.mActionControlList.RemoveAt(num);
			this.mActionControlList.Add(strAnimation);
		}
	}

	private void PlayAnimationClip(string strAnimation, float fSpeed = -1f, float fNormalized = -1f)
	{
		if (this.mAnimation == null || !this.mActionLoadedDict.ContainsKey(strAnimation))
		{
			return;
		}
		this.SequenceList(strAnimation);
		if (fSpeed >= 0f)
		{
			this.mAnimation[strAnimation].speed = fSpeed;
		}
		if (fNormalized >= 0f)
		{
			this.mAnimation[strAnimation].normalizedTime = fNormalized;
		}
		this.mAnimation.enabled = false;
		this.mAnimation.enabled = true;
		this.mAnimation.Play(strAnimation);
		this.mAnimation.clip = this.mAnimation.GetClip(strAnimation);
	}

	private void CrossFadeClip(string strAnimation, float fFade, float fSpeed = -1f)
	{
		if (this.mAnimation == null || !this.mActionLoadedDict.ContainsKey(strAnimation))
		{
			return;
		}
		this.SequenceList(strAnimation);
		this.mAnimation.enabled = false;
		this.mAnimation.enabled = true;
		this.mAnimation.CrossFade(strAnimation, fFade);
		this.mAnimation.clip = this.mAnimation.GetClip(strAnimation);
		if (fSpeed >= 0f)
		{
			this.mAnimation[strAnimation].speed = fSpeed;
		}
		this.mAnimation.enabled = false;
		this.mAnimation.enabled = true;
	}

	public void StopClip(string actionName)
	{
		if (this.mAnimation == null)
		{
			return;
		}
		if (this.mAnimationMaps.ContainsKey(actionName))
		{
			this.mAnimation.Stop();
		}
	}

	public void SetAnimationSpeed(string strAnimation, float fSpeed)
	{
		if (this.mAnimation == null)
		{
			return;
		}
		if (!this.mActionLoadedDict.ContainsKey(strAnimation))
		{
			return;
		}
		this.SequenceList(strAnimation);
		this.mAnimation[strAnimation].speed = fSpeed;
	}

	public static string GetPlayingAnimationName(AnimationProxy aGroup)
	{
		if (aGroup == null)
		{
			return string.Empty;
		}
		return aGroup.GetPlayingAnimationName();
	}

	public string GetPlayingAnimationName()
	{
		if (this.mAnimation == null)
		{
			return string.Empty;
		}
		if (!this.mAnimation.isPlaying)
		{
			return string.Empty;
		}
		if (this.mMainClip != null && !string.IsNullOrEmpty(this.mMainClip.strName) && this.mAnimation.IsPlaying(this.mMainClip.strName))
		{
			return this.mMainClip.strName;
		}
		for (int i = 0; i < this.mActionControlList.Count; i++)
		{
			string text = this.mActionControlList[i];
			if (this.mAnimation.IsPlaying(text))
			{
				return text;
			}
		}
		return string.Empty;
	}

	public static string GetPlayingClipName(AnimationProxy aGroup)
	{
		if (aGroup == null)
		{
			return string.Empty;
		}
		return aGroup.CurrentClipName;
	}

	public string GetPlayingAniamtionName()
	{
		if (!string.IsNullOrEmpty(this.CurrentClipName) && base.animation.IsPlaying(this.currentClipName))
		{
			return this.currentClipName;
		}
		return this.GetPlayingAnimationName();
	}

	private void OnDestroy()
	{
		this.ClearAnimationClips();
		TimerManager.DestroyTimer(base.GetInstanceID().ToString());
		this.mAnimationMaps.Clear();
		this.mActionControlList.Clear();
		this.mActionLoadedDict.Clear();
		this.mActionWaitQueue.Clear();
		if (this.mAnimation != null)
		{
			UnityEngine.Object.Destroy(this.mAnimation);
		}
	}

	public void ClearAnimationClips()
	{
		if (this.mAnimation == null || this.mActionControlList.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.mActionControlList.Count; i++)
		{
			string key = this.mActionControlList[i];
			if (this.mActionLoadedDict.ContainsKey(key))
			{
				AnimationClip animationClip = this.mActionLoadedDict[key];
				if (animationClip != null)
				{
					this.mAnimation.RemoveClip(animationClip);
				}
				this.mActionLoadedDict.Remove(key);
				DelegateProxy.PopCache(animationClip);
			}
		}
		if (this.mAnimation.clip != null && !this.mActionControlList.Contains(this.mAnimation.clip.name))
		{
			this.mAnimation.RemoveClip(this.mAnimation.clip);
			DelegateProxy.PopCache(this.mAnimation.clip);
			this.mAnimation.clip = null;
		}
	}
}
