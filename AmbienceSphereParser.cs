using System;
using UnityEngine;

public class AmbienceSphereParser : UnitParser
{
	public override UnityEngine.Object Instantiate(UnityEngine.Object prefab)
	{
		GameObject gameObject = DelegateProxy.Instantiate(prefab) as GameObject;
		GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		MeshFilter component = gameObject2.GetComponent<MeshFilter>();
		gameObject.AddComponent<MeshFilter>().sharedMesh = component.sharedMesh;
		gameObject.AddComponent<MeshCollider>();
		Material sharedMaterial = new Material(Shader.Find("Snail/Diffuse"));
		gameObject.AddComponent<MeshRenderer>().sharedMaterial = sharedMaterial;
		gameObject.GetComponent<MeshRenderer>().castShadows = false;
		gameObject.GetComponent<MeshRenderer>().receiveShadows = false;
		this.unit.localScale = new Vector3(5f, 5f, 5f);
		gameObject.AddComponent<AmbienceSampler>();
		DelegateProxy.DestroyObjectImmediate(gameObject2);
		return gameObject;
	}

	public override UnitParser Clone()
	{
		return new AmbienceSphereParser();
	}
}
