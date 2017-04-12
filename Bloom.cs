using System;
using System.Collections.Generic;
using UnityEngine;

public class Bloom : PEBase
{
	public enum Resolution
	{
		Low,
		High
	}

	public enum BlurType
	{
		Standard,
		Sgx
	}

	public Shader shader;

	public float threshhold = 0.25f;

	public float intensity = 0.32f;

	public float blurSize = 1f;

	public int blurIterations = 1;

	public override Material material
	{
		get
		{
			if (this.m_Material == null)
			{
				if (this.shader == null)
				{
					this.shader = Shader.Find("Snail/Bloom");
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
			this._matParams.Add("threshhold", 1);
			this._matParams.Add("intensity", 1);
			this._matParams.Add("blurSize", 1);
			return this._matParams;
		}
	}

	private void OnDisable()
	{
		if (this.m_Material)
		{
			DelegateProxy.DestroyObjectImmediate(this.m_Material);
		}
	}

	public override void LoadParams()
	{
		this.threshhold = this.material.GetFloat("threshhold");
		this.intensity = this.material.GetFloat("intensity");
		this.blurSize = this.material.GetFloat("blurSize");
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (GameScene.mainScene == null)
		{
			return;
		}
		if (this.shader == null)
		{
			this.shader = Shader.Find("Snail/Bloom");
		}
		if (this.shader == null)
		{
			return;
		}
		float num = 0.5f;
		this.material.SetFloat("threshhold", this.threshhold);
		this.material.SetFloat("intensity", this.intensity);
		this.material.SetFloat("blurSize", this.blurSize);
		this.material.SetVector("_Parameter", new Vector4(this.blurSize * num, 0f, this.threshhold, this.intensity));
		source.filterMode = FilterMode.Bilinear;
		int width = source.width / 2;
		int height = source.height / 2;
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
		renderTexture.filterMode = FilterMode.Bilinear;
		Graphics.Blit(source, renderTexture, this.material, 1);
		for (int i = 0; i < 1; i++)
		{
			this.material.SetVector("_Parameter", new Vector4(this.blurSize * num + (float)i * 1f, 0f, this.threshhold, this.intensity));
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, this.material, 2);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
			temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, this.material, 3);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		this.material.SetTexture("_Bloom", renderTexture);
		Graphics.Blit(source, destination, this.material, 0);
		RenderTexture.ReleaseTemporary(renderTexture);
	}
}
