using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Reflecter : MonoBehaviour
{
	public delegate void OnFileLoaded(WWW www);

	public delegate void OnFileError(WWW www);

	private Assembly mRemoteAssembly;

	private Component mComGameEntery;

	private Reflecter.OnFileLoaded onFileLoaded;

	private Reflecter.OnFileError onFileError;

	private GuideUI muiGuide;

	private void Awake()
	{
		this.muiGuide = UnityEngine.Object.FindObjectOfType<GuideUI>();
	}

	public void LoadScriptLib()
	{
		LogSystem.LogWarning(new object[]
		{
			"LoadScriptLib"
		});
		this.muiGuide.ShowInfo(Config.GetUdpateLangage("LoaderScript"));
		string str = string.Empty;
		if (Config.mbMd5)
		{
			str = this.GetMd5(Encoding.Default.GetBytes("GameLib"));
		}
		else
		{
			str = "GameLib";
		}
		string text = Config.GetAssetBundleRootPath() + "/" + str + ".unity3d";
		if (File.Exists(text))
		{
			string strFilePath = Config.GetPreSuffix() + text;
			base.StartCoroutine(this.LoadScriptLib(strFilePath, new Reflecter.OnFileLoaded(this.OnLoadLibrarySucc), new Reflecter.OnFileError(this.OnLoadLibraryError)));
		}
	}

	private void OnLoadLibrarySucc(WWW www)
	{
		if (www == null || www.assetBundle == null)
		{
			this.muiGuide.ShowMBox(3, Config.GetUdpateLangage("LoadLibraryError"), new EventDelegate.Callback(this.LoadScriptLib), new EventDelegate.Callback(LaunchUtils.OnErrorQuit), string.Empty);
			return;
		}
		TextAsset textAsset = www.assetBundle.Load("GameLib") as TextAsset;
		if (textAsset == null)
		{
			this.muiGuide.ShowMBox(3, Config.GetUdpateLangage("LoadLibraryError"), new EventDelegate.Callback(this.LoadScriptLib), new EventDelegate.Callback(LaunchUtils.OnErrorQuit), string.Empty);
			return;
		}
		this.mRemoteAssembly = CoreLoader.LoadDllFile(textAsset.bytes);
		www.assetBundle.Unload(true);
		if (this.mRemoteAssembly == null)
		{
			this.muiGuide.ShowMBox(3, Config.GetUdpateLangage("LoadLibraryError"), new EventDelegate.Callback(this.LoadScriptLib), new EventDelegate.Callback(LaunchUtils.OnErrorQuit), string.Empty);
			return;
		}
		this.StartUpdater();
	}

	private void OnLoadLibraryError(WWW www)
	{
		LogSystem.LogWarning(new object[]
		{
			"OnLoadLibraryError:"
		});
		string str = string.Empty;
		if (Config.mbMd5)
		{
			str = this.GetMd5(Encoding.Default.GetBytes("GameLib"));
		}
		else
		{
			str = "GameLib";
		}
		string str2 = Config.GetStreamRootPath() + "/" + str + ".unity3d";
		string text = Config.GetStreamSuffix() + str2;
		base.StartCoroutine(this.LoadScriptLib(text, new Reflecter.OnFileLoaded(this.OnLoadLibrarySucc2), new Reflecter.OnFileError(this.OnLoadLibraryError2)));
		LogSystem.LogWarning(new object[]
		{
			"OnLoadLibraryError: strFile2:",
			text
		});
	}

	private void OnLoadLibrarySucc2(WWW www)
	{
		LogSystem.LogWarning(new object[]
		{
			"OnLoadLibrarySucc:"
		});
		if (www == null || www.assetBundle == null)
		{
			this.OnLoadLibraryError2(www);
			return;
		}
		LogSystem.LogWarning(new object[]
		{
			"OnLoadLibrarySucc:1"
		});
		TextAsset textAsset = www.assetBundle.Load("GameLib") as TextAsset;
		if (textAsset == null)
		{
			this.muiGuide.ShowMBox(3, Config.GetUdpateLangage("LoadLibraryError"), new EventDelegate.Callback(this.LoadScriptLib), new EventDelegate.Callback(LaunchUtils.OnErrorQuit), string.Empty);
			return;
		}
		this.mRemoteAssembly = CoreLoader.LoadDllFile(textAsset.bytes);
		www.assetBundle.Unload(true);
		if (this.mRemoteAssembly == null)
		{
			this.muiGuide.ShowMBox(3, Config.GetUdpateLangage("LoadLibraryError"), new EventDelegate.Callback(this.LoadScriptLib), new EventDelegate.Callback(LaunchUtils.OnErrorQuit), string.Empty);
			return;
		}
		this.StartUpdater();
	}

	private void OnLoadLibraryError2(WWW www)
	{
		this.mRemoteAssembly = Assembly.GetExecutingAssembly();
		this.StartUpdater();
	}

	private string GetMd5(byte[] bytes)
	{
		MD5 mD = MD5.Create();
		byte[] array = mD.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += array[i].ToString("X");
		}
		return text;
	}

	[DebuggerHidden]
	private IEnumerator LoadScriptLib(string strFilePath, Reflecter.OnFileLoaded onLoaded, Reflecter.OnFileError onError)
	{
		Reflecter.<LoadScriptLib>c__Iterator6 <LoadScriptLib>c__Iterator = new Reflecter.<LoadScriptLib>c__Iterator6();
		<LoadScriptLib>c__Iterator.onError = onError;
		<LoadScriptLib>c__Iterator.onLoaded = onLoaded;
		<LoadScriptLib>c__Iterator.strFilePath = strFilePath;
		<LoadScriptLib>c__Iterator.<$>onError = onError;
		<LoadScriptLib>c__Iterator.<$>onLoaded = onLoaded;
		<LoadScriptLib>c__Iterator.<$>strFilePath = strFilePath;
		<LoadScriptLib>c__Iterator.<>f__this = this;
		return <LoadScriptLib>c__Iterator;
	}

	private void StartUpdater()
	{
		if (this.mRemoteAssembly != null)
		{
			GameObject gameObject = new GameObject("Updater");
			Type type = this.mRemoteAssembly.GetType("Updater");
			if (type != null)
			{
				this.mComGameEntery = gameObject.AddComponent(type);
				if (this.mComGameEntery != null)
				{
					LogSystem.LogWarning(new object[]
					{
						"StartUpdater Successed!!!"
					});
					return;
				}
			}
		}
		this.muiGuide.ShowMBox(3, Config.GetUdpateLangage("LoadLibraryError"), new EventDelegate.Callback(this.LoadScriptLib), new EventDelegate.Callback(LaunchUtils.OnErrorQuit), string.Empty);
	}
}
