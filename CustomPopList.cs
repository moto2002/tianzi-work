using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Custom/PopList")]
public class CustomPopList : MonoBehaviour
{
	public enum Movement
	{
		Horizontal,
		Vertical
	}

	public CustomPopList.Movement movement = CustomPopList.Movement.Vertical;

	public UIGridItem self;

	public UIGrid m_gird;

	public UIPlayTween playTween;

	private List<object> oDatas;

	public Action<UIGridItem> UpdateDataCallBack;

	public Action<int> onSelectedIndex;

	public Action<object> onSelectedObject;

	private int selectedIndex;

	public int Value
	{
		get
		{
			return this.selectedIndex;
		}
	}

	private void Awake()
	{
		this.playTween.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.onTweenFinished)));
		this.m_gird.SelectItem = new UIGrid.OnSelectEvent(this.OnClickSubItem);
		this.m_gird.BindCustomCallBack(new UIGrid.OnUpdateDataRow(this.OnUpdateDataRow));
		this.m_gird.StartCustom();
		this.selectedIndex = -1;
	}

	private void OnEnable()
	{
		if (this.movement == CustomPopList.Movement.Vertical)
		{
			Vector3 zero = Vector3.zero;
			zero.x = 1f;
			zero.y = 0f;
			zero.z = 1f;
			this.playTween.tweenTarget.transform.localScale = zero;
		}
		else
		{
			Vector3 zero2 = Vector3.zero;
			zero2.x = 0f;
			zero2.y = 1f;
			zero2.z = 1f;
			this.playTween.tweenTarget.transform.localScale = zero2;
		}
	}

	private void OnClickSubItem(int index, bool active, bool select)
	{
		if (!active || !select)
		{
			return;
		}
		this.selectedIndex = index;
		this.playTween.Play(true);
	}

	private void onTweenFinished()
	{
		if (this.selectedIndex >= 0)
		{
			if (this.onSelectedIndex != null)
			{
				this.onSelectedIndex(this.selectedIndex);
			}
			if (this.onSelectedObject != null)
			{
				this.onSelectedObject(this.oDatas[this.selectedIndex]);
			}
		}
	}

	public void AddItems(List<object> list)
	{
		this.oDatas = list;
		this.m_gird.AddCustomDataList(list);
	}

	public void SetdefaultObject()
	{
		if (this.UpdateDataCallBack != null)
		{
			this.UpdateDataCallBack(this.self);
		}
	}

	private void OnUpdateDataRow(UIGridItem item)
	{
		if (this.UpdateDataCallBack != null)
		{
			this.UpdateDataCallBack(item);
		}
	}

	public void Clear()
	{
		this.m_gird.ClearCustomGrid();
	}
}
