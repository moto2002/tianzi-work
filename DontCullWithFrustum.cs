using System;
using UnityEngine;

public class DontCullWithFrustum : MonoBehaviour
{
	private MeshFilter mf;

	private void Start()
	{
		this.mf = base.GetComponent<MeshFilter>();
		Vector3 zero = Vector3.zero;
		zero.x = 100000f;
		zero.y = 100000f;
		zero.z = 100000f;
		this.mf.mesh.bounds = new Bounds(Vector3.zero, zero);
	}
}
