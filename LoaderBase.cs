using System;
using UnityEngine;

public class LoaderBase
{
	public Asset asset;

	public byte[] bytes;

	protected Region _region;

	protected GameScene _scene;

	protected LODTerrain _terrain;

	protected Texture2D _texture;

	protected GameObject _gameObject;

	protected AssetBundle _assetBundle;

	protected Mesh _mesh;

	protected GameObject _model;

	public virtual bool loaded
	{
		get
		{
			return false;
		}
	}

	public virtual AssetBundle assetBundle
	{
		get
		{
			return null;
		}
	}

	public virtual GameObject gameObject
	{
		get
		{
			return null;
		}
	}

	public virtual GameScene scene
	{
		get
		{
			return null;
		}
	}

	public virtual Region region
	{
		get
		{
			return null;
		}
	}

	public virtual LODTerrain terrain
	{
		get
		{
			return null;
		}
	}

	public virtual Texture2D texture
	{
		get
		{
			return null;
		}
	}

	public virtual Mesh mesh
	{
		get
		{
			return null;
		}
	}

	public virtual GameObject model
	{
		get
		{
			return null;
		}
	}

	public virtual void Load()
	{
	}

	public virtual void Release()
	{
	}
}
