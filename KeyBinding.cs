using System;
using UnityEngine;

public class KeyBinding : MonoBehaviour
{
	public enum UIAction
	{
		PressAndClick,
		Select
	}

	public UIKeyCode keyCode;

	public KeyBinding.UIAction action;

	private void Start()
	{
		FInput.ins.gameObject.SendMessage("OnKeyBinding", this);
	}

	private void OnClick()
	{
		if (this.action == KeyBinding.UIAction.PressAndClick)
		{
			FInput.ins.gameObject.SendMessage("OnKeyDown", this.keyCode);
		}
	}
}
