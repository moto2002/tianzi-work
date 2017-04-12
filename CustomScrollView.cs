using System;
using UnityEngine;

[AddComponentMenu("NGUI/Custom/ScrollView"), ExecuteInEditMode, RequireComponent(typeof(UIPanel))]
public class CustomScrollView : UIScrollView
{
	public override bool shouldMoveHorizontally
	{
		get
		{
			float num = this.bounds.size.x;
			if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				num += this.mPanel.clipSoftness.x * 2f;
			}
			bool flag = num > this.mPanel.width;
			if (!flag)
			{
				this.mCalculatedBounds = false;
			}
			return flag;
		}
	}

	public override bool shouldMoveVertically
	{
		get
		{
			float num = this.bounds.size.y;
			if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				num += this.mPanel.clipSoftness.y * 2f;
			}
			bool flag = num > this.mPanel.height;
			if (!flag)
			{
				this.mCalculatedBounds = false;
			}
			return flag;
		}
	}

	public void Drag(Vector3 offset)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.mShouldMove)
		{
			if (offset.x != 0f || offset.y != 0f)
			{
				offset = this.mTrans.InverseTransformDirection(offset);
				if (this.movement == UIScrollView.Movement.Horizontal)
				{
					offset.y = 0f;
					offset.z = 0f;
				}
				else if (this.movement == UIScrollView.Movement.Vertical)
				{
					offset.x = 0f;
					offset.z = 0f;
				}
				else if (this.movement == UIScrollView.Movement.Unrestricted)
				{
					offset.z = 0f;
				}
				else
				{
					offset.Scale(this.customMovement);
				}
				offset = this.mTrans.TransformDirection(offset);
			}
			this.mMomentum = Vector3.Lerp(this.mMomentum, this.mMomentum + offset * (0.01f * this.momentumAmount), 0.67f);
			if (!this.iOSDragEmulation || this.dragEffect != UIScrollView.DragEffect.MomentumAndSpring)
			{
				base.MoveAbsolute(offset);
			}
			else if (this.mPanel.CalculateConstrainOffset(this.bounds.min, this.bounds.max).magnitude > 1f)
			{
				base.MoveAbsolute(offset * 0.5f);
				this.mMomentum *= 0.5f;
			}
			else
			{
				base.MoveAbsolute(offset);
			}
			if (this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None && this.dragEffect != UIScrollView.DragEffect.MomentumAndSpring)
			{
				base.RestrictWithinBounds(true, base.canMoveHorizontally, base.canMoveVertically);
			}
		}
	}
}
