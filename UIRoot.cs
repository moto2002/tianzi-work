using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Root"), ExecuteInEditMode]
public class UIRoot : MonoBehaviour
{
	public enum Scaling
	{
		PixelPerfect,
		FixedSize,
		FixedSizeOnMobiles
	}

	public static List<UIRoot> list = new List<UIRoot>();

	public UIRoot.Scaling scalingStyle;

	public int manualHeight = 720;

	public int minimumHeight = 320;

	public int maximumHeight = 1536;

	public bool adjustByDPI;

	public bool shrinkPortraitUI;

	private static UIRoot sRoot = null;

	private Transform mTrans;

	public int activeHeight
	{
		get
		{
			int height = ResolutionConstrain.Instance.height;
			int num = Mathf.Max(2, height);
			if (this.scalingStyle == UIRoot.Scaling.FixedSize)
			{
				return this.manualHeight;
			}
			int width = ResolutionConstrain.Instance.width;
			if (this.scalingStyle == UIRoot.Scaling.FixedSizeOnMobiles)
			{
				return this.manualHeight;
			}
			if (num < this.minimumHeight)
			{
				num = this.minimumHeight;
			}
			if (num > this.maximumHeight)
			{
				num = this.maximumHeight;
			}
			if (this.shrinkPortraitUI && height > width)
			{
				num = Mathf.RoundToInt((float)num * ((float)height / (float)width));
			}
			return (!this.adjustByDPI) ? num : NGUIMath.AdjustByDPI((float)num);
		}
	}

	public float pixelSizeAdjustment
	{
		get
		{
			return this.GetPixelSizeAdjustment(ResolutionConstrain.Instance.height);
		}
	}

	public void OnApplicationQuit()
	{
	}

	public static void SetUIRoot(UIRoot root)
	{
		UIRoot.sRoot = root;
	}

	public static float GetPixelSizeAdjustment(GameObject go)
	{
		UIRoot uIRoot;
		if (UIRoot.sRoot == null)
		{
			uIRoot = NGUITools.FindInParents<UIRoot>(go);
		}
		else
		{
			uIRoot = UIRoot.sRoot;
		}
		return (!(uIRoot != null)) ? 1f : uIRoot.pixelSizeAdjustment;
	}

	public float GetPixelSizeAdjustment(int height)
	{
		height = Mathf.Max(2, height);
		if (this.scalingStyle == UIRoot.Scaling.FixedSize)
		{
			return (float)this.manualHeight / (float)height;
		}
		if (this.scalingStyle == UIRoot.Scaling.FixedSizeOnMobiles)
		{
			return (float)this.manualHeight / (float)height;
		}
		if (height < this.minimumHeight)
		{
			return (float)this.minimumHeight / (float)height;
		}
		if (height > this.maximumHeight)
		{
			return (float)this.maximumHeight / (float)height;
		}
		return 1f;
	}

	protected virtual void Awake()
	{
		if (UIRoot.sRoot == null)
		{
			UIRoot.sRoot = this;
		}
		this.mTrans = base.transform;
	}

	protected virtual void OnEnable()
	{
		UIRoot.list.Add(this);
	}

	protected virtual void OnDisable()
	{
		UIRoot.list.Remove(this);
	}

	protected virtual void Start()
	{
		UIOrthoCamera componentInChildren = base.GetComponentInChildren<UIOrthoCamera>();
		if (componentInChildren != null)
		{
			LogSystem.LogWarning(new object[]
			{
				"UIRoot should not be active at the same time as UIOrthoCamera. Disabling UIOrthoCamera.",
				componentInChildren
			});
			Camera component = componentInChildren.gameObject.GetComponent<Camera>();
			componentInChildren.enabled = false;
			if (component != null)
			{
				component.orthographicSize = 1f;
			}
		}
		else
		{
			this.Update();
		}
	}

	private void Update()
	{
		if (this.mTrans != null)
		{
			float num = (float)this.activeHeight;
			if (num > 0f)
			{
				float num2 = 2f / num;
				Vector3 localScale = this.mTrans.localScale;
				if (Mathf.Abs(localScale.x - num2) > 1.401298E-45f || Mathf.Abs(localScale.y - num2) > 1.401298E-45f || Mathf.Abs(localScale.z - num2) > 1.401298E-45f)
				{
					this.mTrans.localScale = Vector3.one * num2;
				}
			}
		}
	}

	public static void Broadcast(string funcName)
	{
		int i = 0;
		int count = UIRoot.list.Count;
		while (i < count)
		{
			UIRoot uIRoot = UIRoot.list[i];
			if (uIRoot != null)
			{
				uIRoot.BroadcastMessage(funcName, SendMessageOptions.DontRequireReceiver);
			}
			i++;
		}
	}

	public static void Broadcast(string funcName, object param)
	{
		if (param == null)
		{
			LogSystem.LogWarning(new object[]
			{
				"SendMessage is bugged when you try to pass 'null' in the parameter field. It behaves as if no parameter was specified."
			});
		}
		else
		{
			int i = 0;
			int count = UIRoot.list.Count;
			while (i < count)
			{
				UIRoot uIRoot = UIRoot.list[i];
				if (uIRoot != null)
				{
					uIRoot.BroadcastMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
				}
				i++;
			}
		}
	}
}
