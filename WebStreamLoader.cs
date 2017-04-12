using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class WebStreamLoader : MonoBehaviour
{
	public delegate void OnWebReqCallBack(string strUrl, WebLoadStatus eLoadStatus, string strContent);

	public delegate void OnResponseCallBack(string info, WebLoadStatus eLoadStatus);

	public void ReadStream(string strUrl, WebStreamLoader.OnWebReqCallBack oncallback)
	{
		base.StartCoroutine(WebStreamLoader.StartStreamReader(strUrl, oncallback));
	}

	public void ReadStream(string strUrl, byte[] postBytes, WebStreamLoader.OnResponseCallBack oncallback)
	{
		base.StartCoroutine(WebStreamLoader.StartStreamReader(strUrl, oncallback, postBytes));
	}

	[DebuggerHidden]
	public static IEnumerator StartStreamReader(string strUrl, WebStreamLoader.OnWebReqCallBack oncallback)
	{
		WebStreamLoader.<StartStreamReader>c__Iterator4 <StartStreamReader>c__Iterator = new WebStreamLoader.<StartStreamReader>c__Iterator4();
		<StartStreamReader>c__Iterator.strUrl = strUrl;
		<StartStreamReader>c__Iterator.oncallback = oncallback;
		<StartStreamReader>c__Iterator.<$>strUrl = strUrl;
		<StartStreamReader>c__Iterator.<$>oncallback = oncallback;
		return <StartStreamReader>c__Iterator;
	}

	[DebuggerHidden]
	public static IEnumerator StartStreamReader(string strUrl, WebStreamLoader.OnResponseCallBack oncallback, byte[] postBytes)
	{
		WebStreamLoader.<StartStreamReader>c__Iterator5 <StartStreamReader>c__Iterator = new WebStreamLoader.<StartStreamReader>c__Iterator5();
		<StartStreamReader>c__Iterator.strUrl = strUrl;
		<StartStreamReader>c__Iterator.postBytes = postBytes;
		<StartStreamReader>c__Iterator.oncallback = oncallback;
		<StartStreamReader>c__Iterator.<$>strUrl = strUrl;
		<StartStreamReader>c__Iterator.<$>postBytes = postBytes;
		<StartStreamReader>c__Iterator.<$>oncallback = oncallback;
		return <StartStreamReader>c__Iterator;
	}

	private void Awake()
	{
		Instance.Set<WebStreamLoader>(this, true);
	}

	private void Start()
	{
	}
}
