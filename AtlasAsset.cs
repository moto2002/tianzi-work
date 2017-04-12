using System;
using UnityEngine;

public class AtlasAsset : ScriptableObject
{
	public Material material;

	private void OnDestroy()
	{
		if (this.material != null)
		{
			this.material = null;
		}
	}
}
