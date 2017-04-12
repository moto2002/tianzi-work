using System;
using System.IO;
using UnityEngine;

public class ResourceLoader : LoaderBase
{
	private UnityEngine.Object source;

	private bool _loaded;

	private SkinnedMeshParser smParser;

	private MeshParser mParser;

	public override bool loaded
	{
		get
		{
			return this._loaded;
		}
	}

	public override GameObject gameObject
	{
		get
		{
			if ((this.asset.type == AssetType.Prefab || this.asset.type == AssetType.GameObject) && this._gameObject == null)
			{
				this._gameObject = (this.source as GameObject);
				this._loaded = true;
			}
			return this._gameObject;
		}
	}

	public override GameObject model
	{
		get
		{
			if (this.asset.type == AssetType.Model && this._model == null && this.smParser == null)
			{
				TextAsset textAsset = this.source as TextAsset;
				if (textAsset != null && textAsset.bytes != null)
				{
					this.bytes = textAsset.bytes;
					MemoryStream input = new MemoryStream(this.bytes);
					BinaryReader binaryReader = new BinaryReader(input);
					string a = binaryReader.ReadString();
					if (a == "skeletonModel")
					{
						this.smParser = new SkinnedMeshParser();
						this.smParser.ParseAsync(this.bytes, 30);
						this.smParser.parseCompleteListener = new ParserBase.ParseCompleteListener(this.OnSkeletonModelParseComplate);
					}
					else if (a == "model")
					{
						this.mParser = new MeshParser();
						this.mParser.ParseAsync(this.bytes, 30);
						this.mParser.parseCompleteListener = new ParserBase.ParseCompleteListener(this.OnModelParseComplate);
					}
				}
			}
			return this._model;
		}
	}

	public override GameScene scene
	{
		get
		{
			if (this.asset.type == AssetType.GameScene && this._scene == null)
			{
				TextAsset textAsset = this.source as TextAsset;
				if (textAsset != null && textAsset.bytes != null)
				{
					this.bytes = textAsset.bytes;
					this._scene = new GameScene(false);
					try
					{
						this._scene.Read(this.bytes);
					}
					catch (Exception ex)
					{
						Debug.Log("场景文件读取失败,请检查场景文件是否损坏。文件:" + this.asset.assetPath + " " + ex.ToString());
					}
					this.source = null;
					this.bytes = null;
					this._loaded = true;
					Resources.UnloadUnusedAssets();
					GC.Collect();
				}
				else
				{
					Debug.Log("场景文件丢失 : " + this.asset.assetPath);
				}
			}
			return this._scene;
		}
	}

	public override Region region
	{
		get
		{
			if (this.asset.type == AssetType.Region && this._region == null)
			{
				TextAsset textAsset = this.source as TextAsset;
				if (textAsset != null && textAsset.bytes != null)
				{
					this.bytes = textAsset.bytes;
					this._region = new Region();
					try
					{
						this._region.Read(this.bytes);
					}
					catch (Exception ex)
					{
						Debug.Log("地域文件读取失败,请检查地域文件是否损坏。文件:" + this.asset.assetPath + " " + ex.ToString());
					}
					this.source = null;
					this.bytes = null;
					this._loaded = true;
				}
			}
			return this._region;
		}
	}

	public override LODTerrain terrain
	{
		get
		{
			if (this.asset.type == AssetType.Terrain && this._terrain == null)
			{
				TextAsset textAsset = this.source as TextAsset;
				if (textAsset != null && textAsset.bytes != null)
				{
					this.bytes = textAsset.bytes;
					LODTerrainData lODTerrainData = new LODTerrainData();
					try
					{
						lODTerrainData.Read(this.bytes);
					}
					catch (Exception)
					{
					}
					this._terrain = LODTerrain.CreateTerrainGameObject(lODTerrainData, true);
					this.source = null;
					lODTerrainData = null;
					this.bytes = null;
					this._loaded = true;
				}
			}
			return this._terrain;
		}
	}

	public override Texture2D texture
	{
		get
		{
			if (this.asset.type == AssetType.Texture2D && this._texture == null)
			{
				this._texture = (this.source as Texture2D);
				this._loaded = true;
			}
			return this._texture;
		}
	}

	public override void Load()
	{
		this.source = Resources.Load(this.asset.assetPath, typeof(UnityEngine.Object));
		if ((this.gameObject != null || this.model != null || this.scene != null || this.region != null || this.terrain != null || this.texture != null) && this.asset.loadedListener != null)
		{
			this.asset.loadedListener(this.asset);
		}
	}

	public override void Release()
	{
		this.source = null;
		this._region = null;
		this._scene = null;
		this._terrain = null;
		this._texture = null;
		this._gameObject = null;
		this._assetBundle = null;
		this._model = null;
	}

	private void OnModelParseComplate(ParserBase parser)
	{
		this._model = (parser as MeshParser).go;
		this.mParser.parseCompleteListener = null;
		this._loaded = true;
		this.source = null;
		if (this.asset.loadedListener != null)
		{
			this.asset.loadedListener(this.asset);
		}
	}

	private void OnSkeletonModelParseComplate(ParserBase parser)
	{
		this._model = (parser as SkinnedMeshParser).go;
		this.smParser.parseCompleteListener = null;
		this._loaded = true;
		this.source = null;
		if (this.asset.loadedListener != null)
		{
			this.asset.loadedListener(this.asset);
		}
	}
}
