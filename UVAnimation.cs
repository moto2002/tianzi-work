using System;
using UnityEngine;

public class UVAnimation : MonoBehaviour
{
	private UITexture muiTexture;

	public float mfStartU;

	public float mfStartV;

	public float mfWidth = 1f;

	public float mfHeight = 1f;

	public float mfDeltaU;

	public float mfDeltaV;

	private Rect mRect = default(Rect);

	private void Awake()
	{
		this.muiTexture = base.GetComponent<UITexture>();
		if (this.muiTexture != null)
		{
			this.mRect.Set(this.mfStartU, this.mfStartV, this.mfWidth, this.mfHeight);
			this.muiTexture.uvRect = this.mRect;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.muiTexture == null)
		{
			return;
		}
		float num = this.mfDeltaU * Time.deltaTime;
		float num2 = this.mfDeltaV * Time.deltaTime;
		this.mRect.Set(this.mRect.xMin + num, this.mRect.yMin + num2, this.mfWidth, this.mfHeight);
		this.muiTexture.uvRect = this.mRect;
	}
}
