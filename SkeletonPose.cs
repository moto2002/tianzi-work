using System;
using System.Collections.Generic;

public class SkeletonPose
{
	public List<JointPose> jointPoses;

	public int numJointPoses
	{
		get
		{
			return this.jointPoses.Count;
		}
	}

	public SkeletonPose()
	{
		this.jointPoses = new List<JointPose>();
	}
}
