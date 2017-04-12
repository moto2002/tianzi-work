using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Image Effects/WaterDistortion"), ExecuteInEditMode]
public class WaterDistortion : PEBase
{
	public float _BackgroundScrollX;

	public float _BackgroundScrollY;

	public float _BackgroundScaleX = 1f;

	public float _BackgroundScaleY = 1f;

	public float _Refraction = 0.045f;

	public Texture2D _Background2;

	public float _BackgroundScrollX2;

	public float _BackgroundScrollY2 = -0.06f;

	public float _BackgroundScaleX2 = 1f;

	public float _BackgroundScaleY2 = 4f;

	public float _BackgroundFade2 = 1f;

	public float _Refraction2;

	public Texture2D _Background3;

	public float _BackgroundScrollX3 = -0.15f;

	public float _BackgroundScrollY3 = 0.08f;

	public float _BackgroundScaleX3 = 0.25f;

	public float _BackgroundScaleY3 = 0.25f;

	public float _BackgroundFade3;

	public float _Refraction3 = 0.27f;

	public Texture2D _DistortionMap;

	public float _DistortionScrollX = 0.09f;

	public float _DistortionScrollY = 0.09f;

	public float _DistortionScaleX = 1f;

	public float _DistortionScaleY = 1f;

	public float _DistortionPower = 0.08f;

	public Texture2D _DistortionMap2;

	public float _DistortionScrollX2 = 0.5f;

	public float _DistortionScrollY2 = 0.09f;

	public float _DistortionScaleX2 = 0.13f;

	public float _DistortionScaleY2 = 0.8f;

	public float _DistortionPower2 = 0.08f;

	public Shader shader;

	public override Material material
	{
		get
		{
			if (this.m_Material == null)
			{
				if (this.shader == null)
				{
					this.shader = Shader.Find("Snail/WaterDistortion");
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
			this._matParams.Add("_BackgroundScrollX", 1);
			this._matParams.Add("_BackgroundScrollY", 1);
			this._matParams.Add("_BackgroundScaleX", 1);
			this._matParams.Add("_BackgroundScaleY", 1);
			this._matParams.Add("_Refraction", 1);
			this._matParams.Add("_Background2", 4);
			this._matParams.Add("_BackgroundScrollX2", 1);
			this._matParams.Add("_BackgroundScrollY2", 1);
			this._matParams.Add("_BackgroundScaleX2", 1);
			this._matParams.Add("_BackgroundScaleY2", 1);
			this._matParams.Add("_BackgroundFade2", 1);
			this._matParams.Add("_Refraction2", 1);
			this._matParams.Add("_Background3", 4);
			this._matParams.Add("_BackgroundScrollX3", 1);
			this._matParams.Add("_BackgroundScrollY3", 1);
			this._matParams.Add("_BackgroundScaleX3", 1);
			this._matParams.Add("_BackgroundScaleY3", 1);
			this._matParams.Add("_BackgroundFade3", 1);
			this._matParams.Add("_Refraction3", 1);
			this._matParams.Add("_DistortionMap", 4);
			this._matParams.Add("_DistortionScrollX", 1);
			this._matParams.Add("_DistortionScrollY", 1);
			this._matParams.Add("_DistortionScaleX", 1);
			this._matParams.Add("_DistortionScaleY", 1);
			this._matParams.Add("_DistortionPower", 1);
			this._matParams.Add("_DistortionMap2", 4);
			this._matParams.Add("_DistortionScrollX2", 1);
			this._matParams.Add("_DistortionScrollY2", 1);
			this._matParams.Add("_DistortionScaleX2", 1);
			this._matParams.Add("_DistortionScaleY2", 1);
			this._matParams.Add("_DistortionPower2", 1);
			return this._matParams;
		}
	}

	public override void LoadParams()
	{
		this._BackgroundScrollX = this.material.GetFloat("_BackgroundScrollX");
		this._BackgroundScrollY = this.material.GetFloat("_BackgroundScrollY");
		this._BackgroundScaleX = this.material.GetFloat("_BackgroundScaleX");
		this._BackgroundScaleY = this.material.GetFloat("_BackgroundScaleY");
		this._Refraction = this.material.GetFloat("_Refraction");
		this._Background2 = (this.material.GetTexture("_Background2") as Texture2D);
		this._BackgroundScrollX2 = this.material.GetFloat("_BackgroundScrollX2");
		this._BackgroundScrollY2 = this.material.GetFloat("_BackgroundScrollY2");
		this._BackgroundScaleX2 = this.material.GetFloat("_BackgroundScaleX2");
		this._BackgroundScaleY2 = this.material.GetFloat("_BackgroundScaleY2");
		this._BackgroundFade2 = this.material.GetFloat("_BackgroundFade2");
		this._Refraction2 = this.material.GetFloat("_Refraction2");
		this._Background3 = (this.material.GetTexture("_Background3") as Texture2D);
		this._BackgroundScrollX3 = this.material.GetFloat("_BackgroundScrollX3");
		this._BackgroundScrollY3 = this.material.GetFloat("_BackgroundScrollY3");
		this._BackgroundScaleX3 = this.material.GetFloat("_BackgroundScaleX3");
		this._BackgroundScaleY3 = this.material.GetFloat("_BackgroundScaleY3");
		this._BackgroundFade3 = this.material.GetFloat("_BackgroundFade3");
		this._Refraction3 = this.material.GetFloat("_Refraction3");
		this._DistortionMap = (this.material.GetTexture("_DistortionMap") as Texture2D);
		this._DistortionScrollX = this.material.GetFloat("_DistortionScrollX");
		this._DistortionScrollY = this.material.GetFloat("_DistortionScrollY");
		this._DistortionScaleX = this.material.GetFloat("_DistortionScaleX");
		this._DistortionScaleY = this.material.GetFloat("_DistortionScaleY");
		this._DistortionPower = this.material.GetFloat("_DistortionPower");
		this._DistortionMap2 = (this.material.GetTexture("_DistortionMap2") as Texture2D);
		this._DistortionScrollX2 = this.material.GetFloat("_DistortionScrollX2");
		this._DistortionScrollY2 = this.material.GetFloat("_DistortionScrollY2");
		this._DistortionScaleX2 = this.material.GetFloat("_DistortionScaleX2");
		this._DistortionScaleY2 = this.material.GetFloat("_DistortionScaleY2");
		this._DistortionPower2 = this.material.GetFloat("_DistortionPower2");
	}

	private void OnDisable()
	{
		if (this.m_Material)
		{
			UnityEngine.Object.DestroyImmediate(this.m_Material);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.material == null)
		{
			return;
		}
		this._DistortionScrollX += 0.001f;
		this._DistortionScrollX = Mathf.Repeat(this._DistortionScrollX, 1f);
		this._DistortionScrollY2 += 0.001f;
		this._DistortionScrollY2 = Mathf.Repeat(this._DistortionScrollY2, 1f);
		if (source != null)
		{
			this.material.SetTexture("_Background", source);
		}
		this.material.SetFloat("_BackgroundScrollX", this._BackgroundScrollX);
		this.material.SetFloat("_BackgroundScrollY", this._BackgroundScrollY);
		this.material.SetFloat("_BackgroundScaleX", this._BackgroundScaleX);
		this.material.SetFloat("_BackgroundScaleY", this._BackgroundScaleY);
		this.material.SetFloat("_Refraction", this._Refraction);
		this.material.SetTexture("_Background2", this._Background2);
		this.material.SetFloat("_BackgroundScrollX2", this._BackgroundScrollX2);
		this.material.SetFloat("_BackgroundScrollY2", this._BackgroundScrollY2);
		this.material.SetFloat("_BackgroundScaleX2", this._BackgroundScaleX2);
		this.material.SetFloat("_BackgroundScaleY2", this._BackgroundScaleY2);
		this.material.SetFloat("_BackgroundFade2", this._BackgroundFade2);
		this.material.SetFloat("_Refraction2", this._Refraction2);
		this.material.SetTexture("_Background3", this._Background3);
		this.material.SetFloat("_BackgroundScrollX3", this._BackgroundScrollX3);
		this.material.SetFloat("_BackgroundScrollY3", this._BackgroundScrollY3);
		this.material.SetFloat("_BackgroundScaleX3", this._BackgroundScaleX3);
		this.material.SetFloat("_BackgroundScaleY3", this._BackgroundScaleY3);
		this.material.SetFloat("_BackgroundFade3", this._BackgroundFade3);
		this.material.SetFloat("_Refraction3", this._Refraction3);
		this.material.SetTexture("_DistortionMap", this._DistortionMap);
		this.material.SetFloat("_DistortionScrollX", this._DistortionScrollX);
		this.material.SetFloat("_DistortionScrollY", this._DistortionScrollY);
		this.material.SetFloat("_DistortionScaleX", this._DistortionScaleX);
		this.material.SetFloat("_DistortionScaleY", this._DistortionScaleY);
		this.material.SetFloat("_DistortionPower", this._DistortionPower);
		this.material.SetTexture("_DistortionMap2", this._DistortionMap2);
		this.material.SetFloat("_DistortionScrollX2", this._DistortionScrollX2);
		this.material.SetFloat("_DistortionScrollY2", this._DistortionScrollY2);
		this.material.SetFloat("_DistortionScaleX2", this._DistortionScaleX2);
		this.material.SetFloat("_DistortionScaleY2", this._DistortionScaleY2);
		this.material.SetFloat("_DistortionPower2", this._DistortionPower2);
		Graphics.Blit(source, destination, this.material);
	}
}
