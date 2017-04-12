using System;
using UnityEngine;

public class SZUIRenderQueue : MonoBehaviour
{
	public int renderQueue = 3000;

	public bool runOnlyOnce;

	private void Start()
	{
		this.SetRenderQueue();
	}

	private void SetRenderQueue()
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material.renderQueue = this.renderQueue;
			}
		}
		if (this.runOnlyOnce && Application.isPlaying)
		{
			base.enabled = false;
		}
	}
}
