using System;
using System.Collections.Generic;
using UnityEngine;

public class Vignetting : PEBase
{
	public Shader shader;

	private Material mMaterial;

	public float _Intensity = 3f;

	public float _Blur = 0.2f;

	public override Material material
	{
		get
		{
			if (this.mMaterial == null)
			{
				if (this.shader == null)
				{
					this.shader = Shader.Find("Snail/Vignetting");
				}
				this.mMaterial = new Material(this.shader);
				this.mMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return this.mMaterial;
		}
	}

	public override Dictionary<string, int> matParams
	{
		get
		{
			if (this._matParams == null)
			{
				this._matParams = new Dictionary<string, int>();
			}
			this._matParams.Clear();
			this._matParams.Add("_Intensity", 1);
			this._matParams.Add("_Blur", 1);
			return this._matParams;
		}
	}

	protected virtual void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
			return;
		}
		if (!this.shader || !this.shader.isSupported)
		{
			base.enabled = false;
		}
	}

	public override void LoadParams()
	{
		this._Intensity = this.material.GetFloat("_Intensity");
		this._Blur = this.material.GetFloat("_Blur");
	}

	protected virtual void OnDisable()
	{
		if (this.mMaterial)
		{
			DelegateProxy.DestroyObjectImmediate(this.mMaterial);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (GameScene.mainScene == null)
		{
			return;
		}
		GameScene.mainScene.mainRTT = source;
		if (this.shader == null)
		{
			this.shader = Shader.Find("Snail/Vignetting");
		}
		this.material.SetFloat("_Intensity", this._Intensity);
		this.material.SetFloat("_Blur", this._Blur);
		Graphics.Blit(source, destination, this.material);
	}
}
