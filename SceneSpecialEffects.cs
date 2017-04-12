using System;
using UnityEngine;

public class SceneSpecialEffects : MonoBehaviour
{
	public GameObject mGoEffect;

	public float mfMinX;

	public float mfMaxX;

	public float mfMinY;

	public float mfMaxY;

	public float mfMinZ;

	public float mfMaxZ;

	public float mfMaxTime;

	public float mfMinTime;

	public float mfLiveTime = 3f;

	private Vector3 mvecOffset = Vector3.zero;

	private float mfBornTime;

	private void Start()
	{
		if (this.mGoEffect == null)
		{
			UnityEngine.Object.Destroy(this);
			LogSystem.LogWarning(new object[]
			{
				"SceneSpecialEffects::not find Gameobject"
			});
		}
		this.mGoEffect.SetActive(false);
		this.mfBornTime = this.Randow(this.mfMinTime, this.mfMaxTime);
	}

	private void Update()
	{
		this.mfBornTime -= Time.deltaTime;
		if (this.mfBornTime < 0f)
		{
			this.mfBornTime = this.Randow(this.mfMinTime, this.mfMaxTime);
			this.mvecOffset.x = this.Randow(this.mfMinX, this.mfMaxX);
			this.mvecOffset.y = this.Randow(this.mfMinY, this.mfMaxY);
			this.mvecOffset.z = this.Randow(this.mfMinZ, this.mfMaxZ);
			this.PlayObject();
		}
	}

	private void PlayObject()
	{
		if (this.mGoEffect == null)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(this.mGoEffect, Vector3.zero, Quaternion.identity) as GameObject;
		if (gameObject == null)
		{
			return;
		}
		gameObject.transform.parent = base.transform;
		gameObject.transform.localRotation = this.mGoEffect.transform.localRotation;
		gameObject.transform.localScale = this.mGoEffect.transform.localScale;
		gameObject.transform.localPosition = this.mGoEffect.transform.localPosition + this.mvecOffset;
		gameObject.SetActive(true);
		DestroyForTime destroyForTime = gameObject.AddComponent<DestroyForTime>();
		destroyForTime.time = this.mfLiveTime;
	}

	private float Randow(float min, float max)
	{
		return UnityEngine.Random.Range(min, max);
	}
}
