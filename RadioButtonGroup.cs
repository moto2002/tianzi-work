using System;
using System.Collections.Generic;
using UnityEngine;

public class RadioButtonGroup : MonoBehaviour
{
	public delegate void RadioButtonDelagate(GameObject button);

	public List<UIButton> list = new List<UIButton>();

	private RadioButtonGroup.RadioButtonDelagate m_GroupChangeEvent;

	private int m_selectedIndex = -1;

	private UIButton m_lastButton;

	public RadioButtonGroup.RadioButtonDelagate GroupChangeEvent
	{
		set
		{
			this.m_GroupChangeEvent = value;
		}
	}

	public int SelectedIndex
	{
		get
		{
			return this.m_selectedIndex;
		}
		set
		{
			this.m_selectedIndex = Mathf.Min(value, this.list.Count - 1);
			this.m_selectedIndex = Mathf.Max(this.m_selectedIndex, 0);
			this.UpdateGroup();
		}
	}

	public GameObject SelectedItem
	{
		get
		{
			return this.list[this.m_selectedIndex].gameObject;
		}
		set
		{
			int num = this.list.IndexOf(value.GetComponent<UIButton>());
			if (num > -1)
			{
				this.m_selectedIndex = num;
			}
			this.UpdateGroup();
		}
	}

	public void Reset()
	{
		this.m_selectedIndex = -1;
		for (int i = 0; i < this.list.Count; i++)
		{
			UIButton uIButton = this.list[i];
			uIButton.isEnabled = (this.m_selectedIndex != i);
		}
	}

	public void Init()
	{
		for (int i = 0; i < this.list.Count; i++)
		{
			UIButton uIButton = this.list[i];
			uIButton.isEnabled = (this.m_selectedIndex != i);
			UIEventListener.Get(uIButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnChangeEvent);
		}
	}

	private void OnChangeEvent(GameObject go)
	{
		UIButton component = go.GetComponent<UIButton>();
		this.m_selectedIndex = this.list.IndexOf(component);
		this.UpdateGroup();
	}

	private void UpdateGroup()
	{
		if (this.m_lastButton != null)
		{
			this.m_lastButton.isEnabled = true;
		}
		this.m_lastButton = this.list[this.m_selectedIndex];
		this.m_lastButton.isEnabled = false;
		if (this.m_GroupChangeEvent != null)
		{
			this.m_GroupChangeEvent(this.m_lastButton.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (this.list != null)
		{
			this.list.Clear();
		}
		this.m_lastButton = null;
	}
}
