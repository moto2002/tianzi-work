using System;
using UnityEngine;

[AddComponentMenu("Image Effects/PostEffects"), ExecuteInEditMode]
public class PostEffects : MonoBehaviour
{
	public Shader shader;

	private Material m_Material;

	public static Texture2D textureRamp;

	public static float desaturateAmount = 0.2f;

	public static float rampOffsetR = -0.01f;

	public static float rampOffsetG = -0.01f;

	public static float rampOffsetB = -0.01f;

	public static float threshhold = 0.25f;

	public static float intensity = 0.32f;

	public static float blurSize = 1f;

	public static int blurIterations = 1;

	protected Material material
	{
		get
		{
			if (this.m_Material == null)
			{
				this.m_Material = new Material(this.shader);
				this.m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return this.m_Material;
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

	protected virtual void OnDisable()
	{
		if (this.m_Material)
		{
			UnityEngine.Object.DestroyImmediate(this.m_Material);
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
			this.shader = Shader.Find("Snail/PostEffect");
		}
		if (PostEffects.textureRamp == null)
		{
			PostEffects.textureRamp = (Resources.Load("Textures/Water/grayscaleRamp", typeof(Texture2D)) as Texture2D);
		}
		this.material.SetTexture("_RampTex", PostEffects.textureRamp);
		this.material.SetFloat("_Desat", PostEffects.desaturateAmount);
		this.material.SetVector("_RampOffset", new Vector4(PostEffects.rampOffsetR, PostEffects.rampOffsetG, PostEffects.rampOffsetB, 0f));
		ImageEffects.BlitWithMaterial(this.material, source, destination);
	}
}
