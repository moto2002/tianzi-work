using System;
using UnityEngine;

public class SceneCameraControl : MonoBehaviour
{
	public Transform end;

	private float distance;

	public float maxView = 26f;

	public float minView = 20f;

	public float range = 10f;

	public bool inverseFlag;

	public static Action<float> scrollMouseDelegation;

	public static Transform characterTransform;

	public float rate = 1f;

	public static void init(Action<float> _scrollMouseDelegation, Transform _mRoleTransform)
	{
		SceneCameraControl.scrollMouseDelegation = _scrollMouseDelegation;
		SceneCameraControl.characterTransform = _mRoleTransform;
	}

	private void Start()
	{
		if (SceneCameraControl.characterTransform == null)
		{
			return;
		}
		this.distance = Vector3.Distance(this.end.position, base.transform.position);
	}

	private void Update()
	{
		if (SceneCameraControl.characterTransform == null)
		{
			return;
		}
		float num = Vector3.Distance(SceneCameraControl.characterTransform.position, base.transform.position);
		if (num > this.distance)
		{
			return;
		}
		float num2 = 1f - num / this.distance;
		if (num2 < 0f || num2 > 1f)
		{
			return;
		}
		SceneCameraControl.scrollMouseDelegation(num2 * this.range);
	}
}
