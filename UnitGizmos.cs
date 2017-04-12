using System;
using UnityEngine;

public class UnitGizmos : MonoBehaviour
{
	public string iconFileName = string.Empty;

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, this.iconFileName);
	}
}
