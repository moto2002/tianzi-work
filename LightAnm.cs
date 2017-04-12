using System;
using UnityEngine;

public class LightAnm : MonoBehaviour
{
	public Material ma;

	private float from;

	private float to;

	private float sp;

	private bool upOrDown;

	private bool going;

	private void Start()
	{
		this.ma = new Material(Shader.Find("Custom/CameraFade"));
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, this.ma);
	}

	public void startLight(float from, float to, float time)
	{
		this.from = from;
		this.to = to;
		this.sp = (to - from) / time;
		this.upOrDown = (to > from);
		this.going = true;
	}

	private void Update()
	{
		if (!this.going)
		{
			return;
		}
		this.from += this.sp * Time.deltaTime;
		if (this.upOrDown)
		{
			if ((double)(this.from - this.to) >= 0.01)
			{
				this.going = false;
				this.ma.SetFloat("_Float1", 0f);
				return;
			}
		}
		else if ((double)(this.to - this.from) >= 0.01)
		{
			this.going = false;
			this.ma.SetFloat("_Float1", 0f);
			return;
		}
		this.ma.SetFloat("_Float1", this.from);
	}
}
