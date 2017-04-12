using System;
using UnityEngine;

public class UIGridItem : MonoBehaviour
{
	public delegate void VoidDelegate(UIGridItem item);

	public delegate void BoolDelegate(UIGridItem item, bool state);

	public delegate void FloatDelegate(UIGridItem item, float delta);

	public delegate void VectorDelegate(UIGridItem item, Vector2 delta);

	public delegate void ObjectDelegate(UIGridItem item, GameObject draggedObject);

	public delegate void KeyCodeDelegate(UIGridItem item, KeyCode key);

	public MonoBehaviour[] mScripts;

	public object oData;

	public int miCurIndex = -1;

	public object parameter;

	public UIGridItem.VoidDelegate onSubmit;

	public UIGridItem.VoidDelegate onClick;

	public UIGridItem.VoidDelegate onDoubleClick;

	public UIGridItem.BoolDelegate onHover;

	public UIGridItem.BoolDelegate onPress;

	public UIGridItem.BoolDelegate onSelect;

	public UIGridItem.FloatDelegate onScroll;

	public UIGridItem.VectorDelegate onDrag;

	public UIGridItem.ObjectDelegate onDrop;

	public UIGridItem.KeyCodeDelegate onKey;

	public void setIndex(int iIndex)
	{
		this.miCurIndex = iIndex;
	}

	public int GetIndex()
	{
		return this.miCurIndex;
	}

	private void OnSubmit()
	{
		if (this.onSubmit != null)
		{
			this.onSubmit(this);
		}
	}

	private void OnClick()
	{
		if (this.onClick != null)
		{
			this.onClick(this);
		}
	}

	private void OnDoubleClick()
	{
		if (this.onDoubleClick != null)
		{
			this.onDoubleClick(this);
		}
	}

	private void OnHover(bool isOver)
	{
		if (this.onHover != null)
		{
			this.onHover(this, isOver);
		}
	}

	private void OnPress(bool isPressed)
	{
		if (this.onPress != null)
		{
			this.onPress(this, isPressed);
		}
	}

	private void OnSelect(bool selected)
	{
		if (this.onSelect != null)
		{
			this.onSelect(this, selected);
		}
	}

	private void OnScroll(float delta)
	{
		if (this.onScroll != null)
		{
			this.onScroll(this, delta);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (this.onDrag != null)
		{
			this.onDrag(this, delta);
		}
	}

	private void OnDrop(GameObject go)
	{
		if (this.onDrop != null)
		{
			this.onDrop(this, go);
		}
	}

	private void OnKey(KeyCode key)
	{
		if (this.onKey != null)
		{
			this.onKey(this, key);
		}
	}

	private void OnDestroy()
	{
		if (this.mScripts != null)
		{
			for (int i = 0; i < this.mScripts.Length; i++)
			{
				this.mScripts[i] = null;
			}
			this.mScripts = null;
		}
	}
}
