using System;
using UnityEngine;

public class ShaderAsset : ScriptableObject
{
	public Shader[] shaders;

	private void OnDestroy()
	{
		if (this.shaders != null)
		{
			this.shaders = null;
		}
	}
}
