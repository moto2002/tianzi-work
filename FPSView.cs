using System;
using UnityEngine;

[ExecuteInEditMode]
public class FPSView : MonoBehaviour
{
	private float updateInterval = 0.5f;

	private float lastInterval = Time.realtimeSinceStartup;

	private int frames;

	public float fps = 1f;

	public float ms = 1f;

	public float time;

	private void Start()
	{
		Application.targetFrameRate = 30;
	}

	private void OnGUI()
	{
		GUI.TextField(new Rect(10f, 10f, 300f, 35f), "fps->" + this.fps);
	}

	private void Update()
	{
		this.frames++;
		float num = Time.time;
		if (num > this.lastInterval + this.updateInterval)
		{
			this.fps = (float)this.frames / (num - this.lastInterval);
			this.ms = 1000f / Mathf.Max(this.fps, 1E-05f);
			this.frames = 0;
			this.lastInterval = num;
		}
	}
}
