using System;
using UnityEngine;

public class NcDetachParent : NcEffectBehaviour
{
	public bool m_bFollowParentTransform = true;

	public bool m_bParentHideToStartDestroy = true;

	public float m_fSmoothDestroyTime = 2f;

	public bool m_bDisableEmit = true;

	public bool m_bSmoothHide = true;

	protected bool m_bStartDetach;

	protected float m_fStartDestroyTime;

	protected GameObject m_ParentGameObject;

	protected NcTransformTool m_OriginalPos = new NcTransformTool();

	public void SetDestroyValue(bool bParentHideToStart, bool bStartDisableEmit, float fSmoothDestroyTime, bool bSmoothHide)
	{
		this.m_bParentHideToStartDestroy = bParentHideToStart;
		this.m_bDisableEmit = bStartDisableEmit;
		this.m_bSmoothHide = bSmoothHide;
		this.m_fSmoothDestroyTime = fSmoothDestroyTime;
	}

	private void Update()
	{
		if (!this.m_bStartDetach)
		{
			this.m_bStartDetach = true;
			if (base.transform.parent != null)
			{
				this.m_ParentGameObject = base.transform.parent.gameObject;
				NcDetachObject.Create(this.m_ParentGameObject, base.transform.gameObject);
			}
			GameObject rootInstanceEffect = NcEffectBehaviour.GetRootInstanceEffect();
			if (this.m_bFollowParentTransform)
			{
				this.m_OriginalPos.SetLocalTransform(base.transform);
				base.ChangeParent(rootInstanceEffect.transform, base.transform, false, null);
				this.m_OriginalPos.CopyToLocalTransform(base.transform);
			}
			else
			{
				base.ChangeParent(rootInstanceEffect.transform, base.transform, false, null);
			}
			if (!this.m_bParentHideToStartDestroy)
			{
				this.StartDestroy();
			}
		}
		if (0f < this.m_fStartDestroyTime)
		{
			if (0f < this.m_fSmoothDestroyTime)
			{
				if (this.m_bSmoothHide)
				{
					float num = 1f - (NcEffectBehaviour.GetEngineTime() - this.m_fStartDestroyTime) / this.m_fSmoothDestroyTime;
					if (num < 0f)
					{
						num = 0f;
					}
					Renderer[] componentsInChildren = base.transform.GetComponentsInChildren<Renderer>(true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						Renderer renderer = componentsInChildren[i];
						string materialColorName = NcEffectBehaviour.GetMaterialColorName(renderer.material);
						if (materialColorName != null)
						{
							Color color = renderer.material.GetColor(materialColorName);
							color.a = Mathf.Min(num, color.a);
							renderer.material.SetColor(materialColorName, color);
						}
					}
				}
				if (this.m_fStartDestroyTime + this.m_fSmoothDestroyTime < NcEffectBehaviour.GetEngineTime())
				{
					DelegateProxy.GameDestory(base.gameObject);
				}
			}
		}
		else if (this.m_bParentHideToStartDestroy && (this.m_ParentGameObject == null || !NcEffectBehaviour.IsActive(this.m_ParentGameObject)))
		{
			this.StartDestroy();
		}
		if (this.m_bFollowParentTransform && this.m_ParentGameObject != null && this.m_ParentGameObject.transform != null)
		{
			NcTransformTool ncTransformTool = new NcTransformTool();
			ncTransformTool.SetTransform(this.m_OriginalPos);
			ncTransformTool.AddTransform(this.m_ParentGameObject.transform);
			ncTransformTool.CopyToLocalTransform(base.transform);
		}
	}

	private void StartDestroy()
	{
		this.m_fStartDestroyTime = NcEffectBehaviour.GetEngineTime();
		if (this.m_bDisableEmit)
		{
			base.DisableEmit();
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fSmoothDestroyTime /= fSpeedRate;
	}
}
