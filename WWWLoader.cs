using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class WWWLoader : LoaderBase
{
	private WWW source;

	public override bool loaded
	{
		get
		{
			return this.source.isDone;
		}
	}

	public override AssetBundle assetBundle
	{
		get
		{
			if (!this.loaded)
			{
				return null;
			}
			if (this._assetBundle == null)
			{
				this._assetBundle = this.source.assetBundle;
			}
			return this._assetBundle;
		}
	}

	public override Texture2D texture
	{
		get
		{
			if (!this.loaded)
			{
				return null;
			}
			if (this._texture == null)
			{
				this._texture = this.source.texture;
			}
			return this._texture;
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
		this._mesh = null;
		if (this._assetBundle != null)
		{
			this._assetBundle.Unload(true);
			this._assetBundle = null;
		}
	}

	public override void Load()
	{
		GameScene.root.StartCoroutine(this.wwwLoad());
	}

	[DebuggerHidden]
	private IEnumerator wwwLoad()
	{
		WWWLoader.<wwwLoad>c__Iterator8 <wwwLoad>c__Iterator = new WWWLoader.<wwwLoad>c__Iterator8();
		<wwwLoad>c__Iterator.<>f__this = this;
		return <wwwLoad>c__Iterator;
	}
}
