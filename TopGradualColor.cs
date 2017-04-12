using System;
using System.Collections.Generic;
using UnityEngine;

public class TopGradualColor : PEBase
{
	public Shader shader;

	public Color _GradualColor = new Color(0f, 0.215686277f, 1f);

	public float _fGradualStart = 2f;

	public float _fGradualEnd = 1f;

	public float _fGradualExp = 3f;

	public Vector4 _vColorAdjustParam = new Vector4(1f, 0f, 3f, 1f);

	public Color _GradualBaseColor = new Color(1f, 1f, 1f);

	public override Material material
	{
		get
		{
			if (this.m_Material == null)
			{
				if (this.shader == null)
				{
					this.shader = Shader.Find("Snail/TopGradualColor");
				}
				this.m_Material = new Material(this.shader);
				this.m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return this.m_Material;
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
			this._matParams.Add("_GradualColor", 2);
			this._matParams.Add("_fGradualStart", 1);
			this._matParams.Add("_fGradualEnd", 1);
			this._matParams.Add("_fGradualExp", 1);
			this._matParams.Add("_vColorAdjustParam", 3);
			this._matParams.Add("_GradualBaseColor", 2);
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
		this._GradualColor = this.material.GetColor("_GradualColor");
		this._fGradualStart = this.material.GetFloat("_fGradualStart");
		this._fGradualEnd = this.material.GetFloat("_fGradualEnd");
		this._fGradualExp = this.material.GetFloat("_fGradualExp");
		this._vColorAdjustParam = this.material.GetVector("_vColorAdjustParam");
		this._GradualBaseColor = this.material.GetColor("_GradualBaseColor");
	}

	protected virtual void OnDisable()
	{
		if (this.m_Material)
		{
			DelegateProxy.DestroyObjectImmediate(this.m_Material);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (GameScene.mainScene == null)
		{
			return;
		}
		if (this.shader == null)
		{
			this.shader = Shader.Find("Snail/TopGradualColor");
		}
		this.material.SetColor("_GradualColor", this._GradualColor);
		this.material.SetFloat("_fGradualStart", this._fGradualStart);
		this.material.SetFloat("_fGradualEnd", this._fGradualEnd);
		this.material.SetFloat("_fGradualExp", this._fGradualExp);
		this.material.SetVector("_vColorAdjustParam", this._vColorAdjustParam);
		this.material.SetColor("_GradualBaseColor", this._GradualBaseColor);
		Graphics.Blit(source, destination, this.material);
	}
}
