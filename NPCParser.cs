using System;
using UnityEngine;

public class NPCParser : UnitParser
{
	public override UnityEngine.Object Instantiate(UnityEngine.Object prefab)
	{
		return DelegateProxy.Instantiate(prefab) as GameObject;
	}

	public override bool SpecialStorage()
	{
		return true;
	}

	public override UnitParser Clone()
	{
		return new NPCParser();
	}
}
