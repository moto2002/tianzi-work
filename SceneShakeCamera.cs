using System;
using UnityEngine;

public class SceneShakeCamera : MonoBehaviour
{
	public delegate void SceneShakeCameraDelegate(float shakeStrength = 0.2f, float rate = 14f, float shakeTime = 0.4f);

	public static SceneShakeCamera.SceneShakeCameraDelegate ShakeCamera;

	public float shakeStrength = 0.2f;

	public float rate = 14f;

	private float shakeTime = 0.4f;

	public int miMin = 5;

	public int miMax = 8;

	private float miCurNum;

	private void Start()
	{
		if (SceneShakeCamera.ShakeCamera == null)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (this.miCurNum <= 0f)
		{
			this.miCurNum = (float)UnityEngine.Random.Range(this.miMin, this.miMax);
			if (SceneShakeCamera.ShakeCamera != null)
			{
				SceneShakeCamera.ShakeCamera(this.shakeStrength, this.rate, this.shakeTime);
			}
		}
		else
		{
			this.miCurNum -= Time.deltaTime;
		}
	}
}
