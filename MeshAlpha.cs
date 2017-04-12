using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshAlpha : MonoBehaviour
{
	public float alpha_1 = 1f;

	private float prevAlpha_1 = 1f;

	public float alpha_2 = 1f;

	private float prevAlpha_2 = 1f;

	public List<Material> mMaterials = new List<Material>();

	private void Start()
	{
		this.prevAlpha_1 = this.alpha_1;
		this.prevAlpha_2 = this.alpha_2;
		Renderer[] components = base.transform.GetComponents<Renderer>();
		for (int i = 0; i < components.Length; i++)
		{
			Renderer renderer = components[i];
			if (renderer.material != null)
			{
				this.mMaterials.Add(renderer.material);
			}
		}
	}

	private void Update()
	{
		this.alpha_1 = Mathf.Min(1f, this.alpha_1);
		this.alpha_1 = Mathf.Max(this.alpha_1, 0f);
		float num = Mathf.Abs(this.prevAlpha_1 - this.alpha_1);
		if (num > 0.001f)
		{
			this.SetAlpha(this.alpha_1, this.prevAlpha_1, "_Color1");
			this.prevAlpha_1 = this.alpha_1;
		}
		this.alpha_2 = Mathf.Min(1f, this.alpha_2);
		this.alpha_2 = Mathf.Max(this.alpha_2, 0f);
		num = Mathf.Abs(this.prevAlpha_2 - this.alpha_2);
		if (num > 0.001f)
		{
			this.SetAlpha(this.alpha_2, this.prevAlpha_2, "_Color2");
			this.prevAlpha_2 = this.alpha_2;
		}
	}

	private void SetAlpha(float alpha, float prealpha, string colorName)
	{
		alpha = Mathf.Min(1f, alpha);
		alpha = Mathf.Max(alpha, 0f);
		for (int i = 0; i < this.mMaterials.Count; i++)
		{
			Material material = this.mMaterials[i];
			if (material != null)
			{
				Color color = material.GetColor(colorName);
				color.a = alpha;
				material.SetColor(colorName, color);
			}
		}
	}
}
