using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleLoader : LoaderBase
{
	private WWW www;

	private AssetBundle source;

	private Asset mainDependAsset;

	private int dependsLoadedCount;

	private List<Asset> dependAssets = new List<Asset>();

	public override bool loaded
	{
		get
		{
			for (int i = 0; i < this.dependAssets.Count; i++)
			{
				if (!this.dependAssets[i].loaded)
				{
					return false;
				}
			}
			return true;
		}
	}

	public override GameScene scene
	{
		get
		{
			if (this._scene == null)
			{
				this._scene = new GameScene(false);
				this._scene.loadFromAssetBund = true;
			}
			if (this.mainDependAsset != null && this.mainDependAsset.assetBundle != null && !this._scene.readCompalte)
			{
				TextAsset textAsset = this.mainDependAsset.assetBundle.Load(this.asset.assetPath) as TextAsset;
				this.bytes = textAsset.bytes;
				this._scene.Read(this.bytes);
			}
			return this._scene;
		}
	}

	public override Region region
	{
		get
		{
			if (this._region == null)
			{
				TextAsset textAsset = this.mainDependAsset.assetBundle.Load(this.asset.assetPath) as TextAsset;
				if (textAsset != null)
				{
					this.bytes = textAsset.bytes;
					this._region = new Region();
					this._region.Read(this.bytes);
				}
				else
				{
					Debug.Log("terrain asset is missing.");
				}
			}
			return this._region;
		}
	}

	public override LODTerrain terrain
	{
		get
		{
			if (this._terrain == null)
			{
				TextAsset textAsset = this.mainDependAsset.assetBundle.Load(this.asset.assetPath) as TextAsset;
				this.bytes = textAsset.bytes;
				LODTerrainData lODTerrainData = new LODTerrainData();
				lODTerrainData.Read(this.bytes);
				this._terrain = LODTerrain.CreateTerrainGameObject(lODTerrainData, true);
			}
			return this._terrain;
		}
	}

	public override Texture2D texture
	{
		get
		{
			if (this._texture == null)
			{
				this._texture = (this.mainDependAsset.assetBundle.Load(this.asset.assetPath) as Texture2D);
			}
			return this._texture;
		}
	}

	public override void Load()
	{
		List<string> list = AssetLibrary.FindDepends(this.asset.assetPath);
		for (int i = 0; i < list.Count; i++)
		{
			Asset asset = AssetLibrary.Load(list[i], AssetType.AssetBundle, LoadType.Type_WWW);
			this.dependAssets.Add(asset);
			if (!asset.loaded)
			{
				asset.loadedListener = new Asset.LoadedListener(this.dependLoaded);
			}
			else
			{
				this.dependsLoadedCount++;
			}
		}
		this.CheckLoadedComplate();
		this.mainDependAsset = this.dependAssets[0];
	}

	private void dependLoaded(Asset asset)
	{
		this.dependsLoadedCount++;
		this.CheckLoadedComplate();
	}

	private void CheckLoadedComplate()
	{
		if (this.dependsLoadedCount == this.dependAssets.Count)
		{
			if (this.asset.type == AssetType.GameScene)
			{
				TextAsset textAsset = this.mainDependAsset.assetBundle.Load(this.asset.assetPath) as TextAsset;
				this.bytes = textAsset.bytes;
				this._scene.Read(this.bytes);
			}
			if (this.asset.loadedListener != null)
			{
				this.asset.loadedListener(this.asset);
			}
		}
	}
}
