using System;
using UnityEngine;

public class MeshAsset : ScriptableObject
{
	public Mesh mesh;

	private void OnDestroy()
	{
		if (this.mesh != null)
		{
			this.mesh = null;
		}
	}
}
