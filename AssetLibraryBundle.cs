using System;
using System.Collections.Generic;

public class AssetLibraryBundle
{
	private Dictionary<string, Asset> _assetDictionary = new Dictionary<string, Asset>();

	private List<Asset> assets = new List<Asset>();

	private static AssetLibraryBundle _ins;

	public int count;

	public AssetLibraryBundle()
	{
		this.Reset();
	}

	public void Reset()
	{
		while (this.assets.Count > 0)
		{
			this.assets[0].Release();
			this.assets.RemoveAt(0);
		}
		this.assets.Clear();
		this._assetDictionary.Clear();
		this.count = this.assets.Count;
	}

	public static AssetLibraryBundle getInstance(string key = "default")
	{
		if (AssetLibraryBundle._ins == null)
		{
			AssetLibraryBundle._ins = new AssetLibraryBundle();
		}
		return AssetLibraryBundle._ins;
	}

	public Asset GetAsset(string path)
	{
		if (this._assetDictionary.ContainsKey(path))
		{
			return this._assetDictionary[path];
		}
		return null;
	}

	public void AddAsset(Asset asset)
	{
		this._assetDictionary[asset.assetPath] = asset;
		this.assets.Add(asset);
		this.count++;
	}

	public void RemoveAsset(Asset asset)
	{
		this._assetDictionary.Remove(asset.assetPath);
		this.assets.Remove(asset);
		asset = null;
		this.count--;
	}

	public void RemoveAllAssets()
	{
		this.Reset();
	}
}
