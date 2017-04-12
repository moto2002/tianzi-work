using System;
using UnityEngine;

public class StutasControl : MonoBehaviour
{
	private GameObject go;

	private void Awake()
	{
		this.go = base.gameObject;
	}

	private void Start()
	{
		if (this.go == null)
		{
			this.go = base.gameObject;
		}
		this.go.SetActive(false);
	}

	private void OnEnable()
	{
		if (this.go == null)
		{
			this.go = base.gameObject;
		}
		this.go.SetActive(false);
	}
}
