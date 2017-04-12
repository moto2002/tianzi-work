using System;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAnimator
{
	private float[] _globalMatrices;

	private int _numJoints;

	private SkeletonPose _globalPose = new SkeletonPose();

	private Skeleton _skeleton;

	private int _jointsPerVertex;

	private bool _globalPropertiesDirty;

	public SkeletonPose globalPose
	{
		get
		{
			if (this._globalPropertiesDirty)
			{
				this.UpdateGlobalProperties();
			}
			return this._globalPose;
		}
	}

	public SkeletonAnimator()
	{
		this._globalMatrices = new float[this._numJoints * 12];
		int num = 0;
		for (int i = 0; i < this._numJoints; i++)
		{
			this._globalMatrices[num++] = 1f;
			this._globalMatrices[num++] = 0f;
			this._globalMatrices[num++] = 0f;
			this._globalMatrices[num++] = 0f;
			this._globalMatrices[num++] = 0f;
			this._globalMatrices[num++] = 1f;
			this._globalMatrices[num++] = 0f;
			this._globalMatrices[num++] = 0f;
			this._globalMatrices[num++] = 0f;
			this._globalMatrices[num++] = 0f;
			this._globalMatrices[num++] = 1f;
			this._globalMatrices[num++] = 0f;
		}
	}

	private void UpdateGlobalProperties()
	{
		this._globalPropertiesDirty = false;
		int num = 0;
		List<JointPose> jointPoses = this._globalPose.jointPoses;
		List<SkeletonJoint> joints = this._skeleton.joints;
		for (int i = 0; i < this._numJoints; i++)
		{
			JointPose jointPose = jointPoses[i];
			Quaternion rotation = jointPose.rotation;
			Vector3 translation = jointPose.translation;
			float num2 = rotation.x;
			float num3 = rotation.y;
			float num4 = rotation.z;
			float num5 = rotation.w;
			float num7;
			float num6 = (num7 = 2f * num2) * num3;
			float num8 = num7 * num4;
			float num9 = num7 * num5;
			float num10 = (num7 = 2f * num3) * num4;
			float num11 = num7 * num5;
			float num12 = 2f * num4 * num5;
			num10 = 2f * num3 * num4;
			num11 = 2f * num3 * num5;
			num12 = 2f * num4 * num5;
			num2 *= num2;
			num3 *= num3;
			num4 *= num4;
			num5 *= num5;
			float num13 = (num7 = num2 - num3) - num4 + num5;
			float num14 = num6 - num12;
			float num15 = num8 + num11;
			float num16 = num6 + num12;
			float num17 = -num7 - num4 + num5;
			float num18 = num10 - num9;
			float num19 = num8 - num11;
			float num20 = num10 + num9;
			float num21 = -num2 - num3 + num4 + num5;
			float[] inverseBindPose = joints[i].inverseBindPose;
			float num22 = inverseBindPose[0];
			float num23 = inverseBindPose[4];
			float num24 = inverseBindPose[8];
			float num25 = inverseBindPose[12];
			float num26 = inverseBindPose[1];
			float num27 = inverseBindPose[5];
			float num28 = inverseBindPose[9];
			float num29 = inverseBindPose[13];
			float num30 = inverseBindPose[2];
			float num31 = inverseBindPose[6];
			float num32 = inverseBindPose[10];
			float num33 = inverseBindPose[14];
			this._globalMatrices[num] = num13 * num22 + num14 * num26 + num15 * num30;
			this._globalMatrices[num + 1] = num13 * num23 + num14 * num27 + num15 * num31;
			this._globalMatrices[num + 2] = num13 * num24 + num14 * num28 + num15 * num32;
			this._globalMatrices[num + 3] = num13 * num25 + num14 * num29 + num15 * num33 + translation.x;
			this._globalMatrices[num + 4] = num16 * num22 + num17 * num26 + num18 * num30;
			this._globalMatrices[num + 5] = num16 * num23 + num17 * num27 + num18 * num31;
			this._globalMatrices[num + 6] = num16 * num24 + num17 * num28 + num18 * num32;
			this._globalMatrices[num + 7] = num16 * num25 + num17 * num29 + num18 * num33 + translation.y;
			this._globalMatrices[num + 8] = num19 * num22 + num20 * num26 + num21 * num30;
			this._globalMatrices[num + 9] = num19 * num23 + num20 * num27 + num21 * num31;
			this._globalMatrices[num + 10] = num19 * num24 + num20 * num28 + num21 * num32;
			this._globalMatrices[num + 11] = num19 * num25 + num20 * num29 + num21 * num33 + translation.z;
			num += 12;
		}
	}

	private void LocalToGlobalPose(SkeletonPose sourcePose, SkeletonPose targetPose, Skeleton skeleton)
	{
		List<JointPose> jointPoses = targetPose.jointPoses;
		List<SkeletonJoint> joints = skeleton.joints;
		int numJointPoses = sourcePose.numJointPoses;
		List<JointPose> jointPoses2 = sourcePose.jointPoses;
		for (int i = 0; i < numJointPoses; i++)
		{
			JointPose jointPose = jointPoses[i];
			if (jointPose == null)
			{
				jointPose = new JointPose();
			}
			SkeletonJoint skeletonJoint = joints[i];
			int parentIndex = skeletonJoint.parentIndex;
			JointPose jointPose2 = jointPoses2[i];
			Quaternion rotation = jointPose.rotation;
			Vector3 translation = jointPose.translation;
			if (parentIndex < 0)
			{
				Vector3 translation2 = jointPose2.translation;
				Quaternion rotation2 = jointPose2.rotation;
				rotation.x = rotation2.x;
				rotation.y = rotation2.y;
				rotation.z = rotation2.z;
				rotation.w = rotation2.w;
				translation.x = translation2.x;
				translation.y = translation2.y;
				translation.z = translation2.z;
			}
			else
			{
				JointPose jointPose3 = jointPoses[parentIndex];
				Quaternion rotation2 = jointPose3.rotation;
				Vector3 translation2 = jointPose2.translation;
				float x = rotation2.x;
				float y = rotation2.y;
				float z = rotation2.z;
				float w = rotation2.w;
				float x2 = translation2.x;
				float y2 = translation2.y;
				float z2 = translation2.z;
				float num = -x * x2 - y * y2 - z * z2;
				float num2 = w * x2 + y * z2 - z * y2;
				float num3 = w * y2 - x * z2 + z * x2;
				float num4 = w * z2 + x * y2 - y * x2;
				translation2 = jointPose3.translation;
				translation.x = -num * x + num2 * w - num3 * z + num4 * y + translation2.x;
				translation.y = -num * y + num2 * z + num3 * w - num4 * x + translation2.y;
				translation.z = -num * z - num2 * y + num3 * x + num4 * w + translation2.z;
				num2 = rotation2.x;
				num3 = rotation2.y;
				num4 = rotation2.z;
				num = rotation2.w;
				rotation2 = jointPose2.rotation;
				x = rotation2.x;
				y = rotation2.y;
				z = rotation2.z;
				w = rotation2.w;
				rotation.w = num * w - num2 * x - num3 * y - num4 * z;
				rotation.x = num * x + num2 * w + num3 * z - num4 * y;
				rotation.y = num * y - num2 * z + num3 * w + num4 * x;
				rotation.z = num * z + num2 * y - num3 * x + num4 * w;
			}
		}
	}

	private void MorphGeometry()
	{
		List<float> list = new List<float>();
		List<float> list2 = new List<float>();
		List<int> list3 = new List<int>();
		List<float> list4 = new List<float>();
		int i = 0;
		int num = 0;
		int count = list.Count;
		while (i < count)
		{
			float num2 = list[i];
			float num3 = list[i + 1];
			float num4 = list[i + 2];
			float num5 = list[i + 3];
			float num6 = list[i + 4];
			float num7 = list[i + 5];
			float num8 = list[i + 6];
			float num9 = list[i + 7];
			float num10 = list[i + 8];
			float num11 = 0f;
			float num12 = 0f;
			float num13 = 0f;
			float num14 = 0f;
			float num15 = 0f;
			float num16 = 0f;
			float num17 = 0f;
			float num18 = 0f;
			float num19 = 0f;
			int j = 0;
			while (j < this._jointsPerVertex)
			{
				float num20 = list4[num];
				if (num20 > 0f)
				{
					int num21 = list3[num++] << 2;
					float num22 = this._globalMatrices[num21];
					float num23 = this._globalMatrices[num21 + 1];
					float num24 = this._globalMatrices[num21 + 2];
					float num25 = this._globalMatrices[num21 + 3];
					float num26 = this._globalMatrices[num21 + 4];
					float num27 = this._globalMatrices[num21 + 5];
					float num28 = this._globalMatrices[num21 + 6];
					float num29 = this._globalMatrices[num21 + 7];
					float num30 = this._globalMatrices[num21 + 8];
					float num31 = this._globalMatrices[num21 + 9];
					float num32 = this._globalMatrices[num21 + 10];
					float num33 = this._globalMatrices[num21 + 11];
					num11 += num20 * (num22 * num2 + num23 * num3 + num24 * num4 + num25);
					num12 += num20 * (num26 * num2 + num27 * num3 + num28 * num4 + num29);
					num13 += num20 * (num30 * num2 + num31 * num3 + num32 * num4 + num33);
					num14 += num20 * (num22 * num5 + num23 * num6 + num24 * num7);
					num15 += num20 * (num26 * num5 + num27 * num6 + num28 * num7);
					num16 += num20 * (num30 * num5 + num31 * num6 + num32 * num7);
					num17 += num20 * (num22 * num8 + num23 * num9 + num24 * num10);
					num18 += num20 * (num26 * num8 + num27 * num9 + num28 * num10);
					num19 += num20 * (num30 * num8 + num31 * num9 + num32 * num10);
					j++;
				}
				else
				{
					num += this._jointsPerVertex - j;
					j = this._jointsPerVertex;
				}
			}
			list2[i] = num11;
			list2[i + 1] = num12;
			list2[i + 2] = num13;
			list2[i + 3] = num14;
			list2[i + 4] = num15;
			list2[i + 5] = num16;
			list2[i + 6] = num17;
			list2[i + 7] = num18;
			list2[i + 8] = num19;
			i += 13;
		}
	}
}
