using System;
using System.Collections.Generic;
using UnityEngine;

public class Blur : PEBase
{
	public enum BlurType
	{
		StandardGauss,
		SgxGauss
	}

	public int downsample = 1;

	public float blurSize = 1.1f;

	public int blurIterations = 1;

	public Blur.BlurType blurType;

	public Shader shader;

	public override Material material
	{
		get
		{
			if (this.m_Material == null)
			{
				if (this.shader == null)
				{
					this.shader = Shader.Find("Snail/Blur");
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
		this.blurSize = this.material.GetFloat("blurSize");
		this.blurIterations = this.material.GetInt("blurIterations");
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (GameScene.mainScene == null)
		{
			return;
		}
		if (this.shader == null)
		{
			this.shader = Shader.Find("Snail/MobileBlur");
		}
		float num = 1f / (1f * (float)(1 << this.downsample));
		this.material.SetVector("_Parameter", new Vector4(this.blurSize * num, -this.blurSize * num, 0f, 0f));
		source.filterMode = FilterMode.Bilinear;
		int width = source.width >> this.downsample;
		int height = source.height >> this.downsample;
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
		renderTexture.filterMode = FilterMode.Bilinear;
		Graphics.Blit(source, renderTexture, this.material, 0);
		int num2 = (this.blurType != Blur.BlurType.StandardGauss) ? 2 : 0;
		for (int i = 0; i < this.blurIterations; i++)
		{
			float num3 = (float)i * 1f;
			this.material.SetVector("_Parameter", new Vector4(this.blurSize * num + num3, -this.blurSize * num - num3, 0f, 0f));
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, this.material, 1 + num2);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
			temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, this.material, 2 + num2);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		Graphics.Blit(renderTexture, destination);
		RenderTexture.ReleaseTemporary(renderTexture);
	}
}
