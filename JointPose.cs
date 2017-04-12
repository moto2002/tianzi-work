using System;
using UnityEngine;

public class JointPose
{
	public Quaternion rotation;

	public Vector3 translation;

	public Matrix4x4 ToMatrix3D(ref Matrix4x4 target)
	{
		return target;
	}

	public void CopyFrom(JointPose pose)
	{
		Quaternion quaternion = pose.rotation;
		Vector3 vector = pose.translation;
		this.rotation.x = quaternion.x;
		this.rotation.y = quaternion.y;
		this.rotation.z = quaternion.z;
		this.rotation.w = quaternion.w;
		this.translation.x = vector.x;
		this.translation.y = vector.y;
		this.translation.z = vector.z;
	}
}
