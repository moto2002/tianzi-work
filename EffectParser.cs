using System;
using UnityEngine;

public class EffectParser : UnitParser
{
	public override UnityEngine.Object Instantiate(UnityEngine.Object prefab)
	{
		return UnityEngine.Object.Instantiate(prefab) as GameObject;
	}

	public override UnitParser Clone()
	{
		return new EffectParser();
	}
}
