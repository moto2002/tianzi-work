using System;
using UnityEngine;

public class AppDataLoader : LoaderBase
{
	public override bool loaded
	{
		get
		{
			return true;
		}
	}

	public override GameScene scene
	{
		get
		{
			if (this._scene == null)
			{
				this.bytes = QFileUtils.ReadBinary(this.asset.assetPath);
				this._scene = new GameScene(false);
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
				this.bytes = QFileUtils.ReadBinary(this.asset.assetPath);
				this._region = new Region();
				this._region.Read(this.bytes);
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
				this.bytes = QFileUtils.ReadBinary(this.asset.assetPath);
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
				this.bytes = QFileUtils.ReadBinary(this.asset.assetPath);
				this._texture = new Texture2D(1, 1);
				this._texture.LoadImage(this.bytes);
			}
			return this._texture;
		}
	}

	public override void Load()
	{
		if (this.asset.loadedListener != null)
		{
			this.asset.loadedListener(this.asset);
		}
	}
}
