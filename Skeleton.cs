using System;
using System.Collections.Generic;

public class Skeleton
{
	public List<SkeletonJoint> joints;

	public Skeleton()
	{
		this.joints = new List<SkeletonJoint>();
	}

	public int numJoints()
	{
		return this.joints.Count;
	}

	public void Dispose()
	{
		this.joints.Clear();
	}
}
