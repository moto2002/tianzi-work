using System;
using System.IO;
using UnityEngine;

public class UnitParser
{
	public GameObjectUnit unit;

	public virtual void Read(BinaryReader br)
	{
	}

	public virtual void Write(BinaryWriter bw)
	{
	}

	public virtual void Update(GameObject ins)
	{
	}

	public virtual void Destroy()
	{
		this.unit = null;
	}

	public virtual bool SpecialStorage()
	{
		return false;
	}

	public virtual UnitParser Clone()
	{
		return new UnitParser();
	}

	public virtual UnityEngine.Object Instantiate(UnityEngine.Object prefab)
	{
		return DelegateProxy.Instantiate(prefab);
	}
}
