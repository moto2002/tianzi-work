using System;
using UnityEngine;

public class UIAtlasAsset : ScriptableObject
{
	public UIAtlas uiAtlas;

	private void OnDestroy()
	{
		if (this.uiAtlas != null)
		{
			this.uiAtlas = null;
		}
	}
}
