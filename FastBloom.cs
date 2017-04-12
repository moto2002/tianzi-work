using System;
using UnityEngine;

public class FastBloom : PostEffectsBase
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

	public float threshhold = 0.25f;

	public float intensity = 0.32f;

	public float blurSize = 1f;

	private FastBloom.Resolution resolution;

	public int blurIterations = 1;

	public FastBloom.BlurType blurType;

	public Shader fastBloomShader;

	private Material fastBloomMaterial;

	public override bool CheckResources()
	{
		base.CheckSupport(false);
		this.fastBloomShader = Shader.Find("Hidden/FastBloom");
		this.fastBloomMaterial = base.CheckShaderAndCreateMaterial(this.fastBloomShader, this.fastBloomMaterial);
		if (!this.isSupported)
		{
			base.ReportAutoDisable();
		}
		return this.isSupported;
	}

	private void OnDisable()
	{
		if (this.fastBloomMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.fastBloomMaterial);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		float num = (this.resolution != FastBloom.Resolution.Low) ? 1f : 0.5f;
		this.fastBloomMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num, 0f, this.threshhold, this.intensity));
		source.filterMode = FilterMode.Bilinear;
		int width = source.width / 2;
		int height = source.height / 2;
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
		renderTexture.filterMode = FilterMode.Bilinear;
		Graphics.Blit(source, renderTexture, this.fastBloomMaterial, 1);
		for (int i = 0; i < this.blurIterations; i++)
		{
			this.fastBloomMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num + (float)i * 1f, 0f, this.threshhold, this.intensity));
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, this.fastBloomMaterial, 2);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
			temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, this.fastBloomMaterial, 3);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		this.fastBloomMaterial.SetTexture("_Bloom", renderTexture);
		Graphics.Blit(source, destination, this.fastBloomMaterial, 0);
		RenderTexture.ReleaseTemporary(renderTexture);
	}
}
