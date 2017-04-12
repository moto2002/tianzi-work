using System;
using UnityEngine;

public class AlphaCutoffControler : MonoBehaviour
{
	public AnimationCurve mAnimationCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 1f),
		new Keyframe(1f, 1f, 1f, 0f)
	});

	public float mCurveTimes = 1f;

	private float mfAlphaCutoff;

	private float mFactor;

	private Renderer mRender;

	private void Awake()
	{
		this.mRender = base.transform.GetComponent<Renderer>();
	}

	private void Start()
	{
		this.mfAlphaCutoff = this.GetAlphaCutoff(this.mRender.sharedMaterial);
	}

	private void SetAlphaCutoff(Material mat, float fAlphaCutoff)
	{
		if (mat.HasProperty("_Cutoff"))
		{
			mat.SetFloat("_Cutoff", fAlphaCutoff);
		}
	}

	private float GetAlphaCutoff(Material mat)
	{
		if (mat.HasProperty("_Cutoff"))
		{
			return mat.GetFloat("_Cutoff");
		}
		return 0f;
	}

	private void Update()
	{
		if (this.mRender == null)
		{
			return;
		}
		this.mFactor += Time.deltaTime;
		float num = this.mFactor / this.mCurveTimes;
		num = Mathf.Clamp01(num);
		if (num < 1f)
		{
			float fAlphaCutoff = this.mAnimationCurve.Evaluate(num);
			this.SetAlphaCutoff(this.mRender.sharedMaterial, fAlphaCutoff);
		}
		else
		{
			float fAlphaCutoff2 = this.mAnimationCurve.Evaluate(num);
			this.SetAlphaCutoff(this.mRender.sharedMaterial, fAlphaCutoff2);
			base.gameObject.SetActive(false);
			UnityEngine.Object.DestroyObject(this);
		}
	}

	private void OnDestroy()
	{
		this.mFactor = 0f;
		this.mfAlphaCutoff = 0f;
		this.mRender = null;
	}
}
