using System;
using UnityEngine;

public class ActiveHelper : MonoBehaviour
{
	private void OnEnable()
	{
		LogSystem.LogWarning(new object[]
		{
			"OnEnable ->",
			base.gameObject.name
		});
	}

	private void OnDisable()
	{
		LogSystem.LogWarning(new object[]
		{
			"OnDisable ->",
			base.gameObject.name
		});
	}
}
