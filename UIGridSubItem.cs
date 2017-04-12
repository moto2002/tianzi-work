using System;
using UnityEngine;

public class UIGridSubItem : MonoBehaviour
{
	public UIGridItem oEventReciever;

	public UIGrid mGrid;

	private void OnClick()
	{
		if (this.mGrid != null && this.oEventReciever != null)
		{
			this.mGrid.OnClickItem(this.oEventReciever, base.gameObject);
		}
	}

	private void OnDestroy()
	{
		this.oEventReciever = null;
		this.mGrid = null;
	}
}
