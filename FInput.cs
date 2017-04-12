using System;
using System.Collections;
using UnityEngine;

public class FInput : MonoBehaviour
{
	public static FInput ins;

	private Hashtable _uiMap = new Hashtable();

	private Hashtable _onDownKeys = new Hashtable();

	private int _onDownKeysCount;

	private void Awake()
	{
		FInput.ins = this;
	}

	private void Update()
	{
	}

	public void LateUpdate()
	{
		this.Reset();
	}

	private void OnKeyDown(UIKeyCode keyCode)
	{
		this._onDownKeys.Add(keyCode, true);
		this._onDownKeysCount++;
	}

	private void OnKeyBinding(KeyBinding binder)
	{
		if (this._uiMap.ContainsKey(binder.keyCode))
		{
			this._uiMap[binder] = binder;
		}
		else
		{
			this._uiMap.Add(binder.keyCode, binder);
		}
	}

	private void OnKeyRemove(KeyBinding binder)
	{
		if (this._uiMap.ContainsKey(binder.keyCode))
		{
			this._uiMap.Remove(binder.keyCode);
			return;
		}
	}

	private void Reset()
	{
		this._onDownKeys.Clear();
		this._onDownKeysCount = 0;
	}

	public bool GetKeyDown(UIKeyCode keyCode)
	{
		return this._onDownKeys.ContainsKey(keyCode);
	}
}
