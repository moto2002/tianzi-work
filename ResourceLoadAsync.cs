using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ResourceLoadAsync : MonoBehaviour
{
	public delegate void OnResourceLoadCallBack(string filename, UnityEngine.Object o);

	private static ResourceLoadAsync instance;

	public static ResourceLoadAsync Instance
	{
		get
		{
			if (ResourceLoadAsync.instance == null)
			{
				GameObject gameObject = new GameObject(Config.MessageName);
				ResourceLoadAsync.instance = gameObject.AddComponent<ResourceLoadAsync>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return ResourceLoadAsync.instance;
		}
	}

	public void Load(string filename, ResourceLoadAsync.OnResourceLoadCallBack callback)
	{
		base.StartCoroutine(this.LoadAsste(filename, callback));
	}

	[DebuggerHidden]
	private IEnumerator LoadAsste(string filename, ResourceLoadAsync.OnResourceLoadCallBack callback)
	{
		ResourceLoadAsync.<LoadAsste>c__Iterator7 <LoadAsste>c__Iterator = new ResourceLoadAsync.<LoadAsste>c__Iterator7();
		<LoadAsste>c__Iterator.filename = filename;
		<LoadAsste>c__Iterator.callback = callback;
		<LoadAsste>c__Iterator.<$>filename = filename;
		<LoadAsste>c__Iterator.<$>callback = callback;
		return <LoadAsste>c__Iterator;
	}
}
