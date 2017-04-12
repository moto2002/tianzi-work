using System;
using UnityEngine;

public class Asset
{
	public delegate void LoadedListener(Asset asset);

	public static string prefix = string.Empty;

	public static string strStreamSuffix = string.Empty;

	public static string rootPath = string.Empty;

	public static string suffix = string.Empty;

	public int ID;

	public Asset.LoadedListener loadedListener;

	public AssetType type;

	public string assetPath = string.Empty;

	public LoaderBase loader;

	public bool loaded
	{
		get
		{
			return this.loader.loaded;
		}
	}

	public GameObject gameObject
	{
		get
		{
			return this.loader.gameObject;
		}
	}

	public Texture2D texture2D
	{
		get
		{
			return this.loader.texture;
		}
	}

	public Region region
	{
		get
		{
			return this.loader.region;
		}
	}

	public GameScene scene
	{
		get
		{
			return this.loader.scene;
		}
	}

	public LODTerrain terrain
	{
		get
		{
			return this.loader.terrain;
		}
	}

	public AssetBundle assetBundle
	{
		get
		{
			return this.loader.assetBundle;
		}
	}

	public Mesh mesh
	{
		get
		{
			return this.loader.mesh;
		}
	}

	public GameObject model
	{
		get
		{
			return this.loader.model;
		}
	}

	public Asset(string path, LoadType loadType, AssetType type)
	{
		this.assetPath = path;
		this.type = type;
		if (loadType == LoadType.Type_AssetBundle)
		{
			this.loader = new AssetBundleLoader();
		}
		else if (loadType == LoadType.Type_AppData)
		{
			this.loader = new AppDataLoader();
		}
		else if (loadType == LoadType.Type_Resources)
		{
			this.loader = new ResourceLoader();
		}
		else if (loadType == LoadType.Type_Auto)
		{
			if (AssetLibrary.FindDepends(path) != null)
			{
				this.loader = new AssetBundleLoader();
			}
			else
			{
				this.loader = new ResourceLoader();
			}
		}
		else if (loadType == LoadType.Type_WWW)
		{
			this.loader = new WWWLoader();
		}
		if (this.loader != null)
		{
			this.loader.asset = this;
			this.loader.Load();
		}
	}

	public void Release()
	{
		this.loadedListener = null;
		this.loader.Release();
	}
}
