using System;
using UnityEngine;

public class DestroyForTime : MonoBehaviour
{
	public delegate void OnDestroyEffect(GameObject o, bool forceDistroy = true);

	public static DestroyForTime.OnDestroyEffect monDestroyEffect;

	public DestroyForTime.OnDestroyEffect finishedCallback;

	public float time = 0.1f;

	public bool isDestory;

	private bool mbRemove;

	private float fStartTime;

	public static void SetDestroyEffect(DestroyForTime.OnDestroyEffect onDestroyEffect, bool forceDistroy = true)
	{
		DestroyForTime.monDestroyEffect = onDestroyEffect;
	}

	private void Start()
	{
		this.fStartTime = Time.time;
	}

	private void OnEnable()
	{
		this.fStartTime = Time.time;
	}

	public void Reset()
	{
		this.mbRemove = false;
		this.fStartTime = Time.time;
	}

	private void Update()
	{
		if (Time.time - this.fStartTime > this.time)
		{
			if (DestroyForTime.monDestroyEffect != null)
			{
				this.mbRemove = true;
				DestroyForTime.monDestroyEffect(base.gameObject, true);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		if (!this.mbRemove && DestroyForTime.monDestroyEffect != null)
		{
			this.mbRemove = true;
			DestroyForTime.monDestroyEffect(base.gameObject, true);
		}
	}
}
