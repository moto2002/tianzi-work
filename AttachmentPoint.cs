using System;
using UnityEngine;

public class AttachmentPoint : MonoBehaviour
{
	public enum Slot
	{
		None,
		SelfBody,
		Head,
		LeftHand,
		RightHand,
		Shoulders,
		LeftFoot,
		RightFoot,
		FBX,
		Wing
	}

	public AttachmentPoint.Slot slot;

	[HideInInspector]
	public GameObject mChild;

	public bool Attach(GameObject go)
	{
		if (go == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"[Attach]::go is null"
			});
			return false;
		}
		this.mChild = go;
		go.transform.parent = base.transform;
		NGUITools.SetLayer(go, base.gameObject.layer);
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		return true;
	}

	public bool AttachEffect(GameObject go)
	{
		if (go == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"[AttachEffect]::go is null"
			});
			return false;
		}
		go.transform.parent = base.transform;
		return true;
	}
}
