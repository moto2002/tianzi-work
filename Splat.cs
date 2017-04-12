using System;
using UnityEngine;

public class Splat
{
	public Texture2D texture;

	public string key = string.Empty;

	public Vector4 tilingOffset = new Vector4(8f, 8f, 0f, 0f);

	private Texture2D _normalMap;

	public Texture2D normalMap
	{
		get
		{
			if (this._normalMap == null)
			{
				this._normalMap = AssetLibrary.Load(this.key + "_nmp", AssetType.Texture2D, LoadType.Type_Resources).texture2D;
			}
			return this._normalMap;
		}
	}
}
