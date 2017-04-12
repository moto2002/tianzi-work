using System;
using UnityEngine;

public class NcDelayActive : MonoBehaviour
{
	public float m_fDelayTime;

	private NcTimerTool timer = new NcTimerTool();

	private bool usedflag;

	public float GetParentDelayTime(bool bCheckStarted)
	{
		return 0f;
	}

	private void Update()
	{
		if (this.timer.GetTime() > this.m_fDelayTime && !this.usedflag)
		{
			Renderer[] components = base.GetComponents<Renderer>();
			int i = 0;
			int num = components.Length;
			while (i < num)
			{
				if (components[i] != this)
				{
					components[i].enabled = true;
				}
				i++;
			}
			int j = 0;
			int childCount = base.transform.childCount;
			while (j < childCount)
			{
				base.transform.GetChild(j).gameObject.SetActive(true);
				j++;
			}
			this.usedflag = true;
		}
	}

	private void OnEnable()
	{
		this.timer.Start();
		Renderer[] components = base.GetComponents<Renderer>();
		int i = 0;
		int num = components.Length;
		while (i < num)
		{
			components[i].enabled = false;
			i++;
		}
		int j = 0;
		int childCount = base.transform.childCount;
		while (j < childCount)
		{
			base.transform.GetChild(j).gameObject.SetActive(false);
			j++;
		}
		this.usedflag = false;
	}

	public void ResetAnimation()
	{
		this.OnEnable();
	}
}
