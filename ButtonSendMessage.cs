using System;
using UnityEngine;

public class ButtonSendMessage : MonoBehaviour
{
	public int messageID;

	public int subMessagetID;

	private void Start()
	{
		UIEventListener expr_0B = UIEventListener.Get(base.gameObject);
		expr_0B.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_0B.onClick, new UIEventListener.VoidDelegate(this.OnClickEvent));
	}

	private void OnClickEvent(GameObject go)
	{
		DelegateProxy.OnSendMessageCallback(this.messageID, new object[]
		{
			this.subMessagetID
		});
	}
}
