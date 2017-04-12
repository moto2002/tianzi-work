using System;
using System.IO;
using UnityEngine;

public class LightParser : UnitParser
{
	private Light light;

	public bool firstRun = true;

	public LightType type;

	public Color color;

	public float intensity;

	public LightShadows shadows;

	public float shadowStrength;

	public float shadowBias;

	public float shadowSoftness;

	public float shadowSoftnessFade;

	public float range;

	public float spotAngle;

	public int cullingMask;

	public override void Destroy()
	{
		this.light = null;
		base.Destroy();
	}

	public override void Update(GameObject ins)
	{
		if (ins.light == null)
		{
			return;
		}
		this.light = ins.light;
		this.firstRun = false;
		this.type = this.light.type;
		this.color = this.light.color;
		this.intensity = this.light.intensity;
		this.shadows = this.light.shadows;
		this.shadowStrength = this.light.shadowStrength;
		this.shadowBias = this.light.shadowBias;
		this.shadowSoftness = this.light.shadowSoftness;
		this.shadowSoftnessFade = this.light.shadowSoftnessFade;
		this.range = this.light.range;
		this.spotAngle = this.light.spotAngle;
		this.cullingMask = this.light.cullingMask;
	}

	public override UnityEngine.Object Instantiate(UnityEngine.Object prefab)
	{
		GameObject gameObject = DelegateProxy.Instantiate(prefab) as GameObject;
		this.light = gameObject.light;
		if (gameObject.light == null)
		{
			return gameObject;
		}
		if (!this.firstRun)
		{
			this.light.type = this.type;
			this.light.color = this.color;
			this.light.intensity = this.intensity;
			this.light.shadows = this.shadows;
			this.light.shadowStrength = this.shadowStrength;
			this.light.shadowBias = this.shadowBias;
			this.light.shadowSoftness = this.shadowSoftness;
			this.light.shadowSoftnessFade = this.shadowSoftnessFade;
			this.light.range = this.range;
			this.light.spotAngle = this.spotAngle;
			this.light.cullingMask = this.cullingMask;
		}
		if (!Application.isPlaying)
		{
			GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			MeshFilter component = gameObject2.GetComponent<MeshFilter>();
			gameObject.AddComponent<MeshFilter>().sharedMesh = component.sharedMesh;
			gameObject.AddComponent<MeshCollider>();
			DelegateProxy.DestroyObjectImmediate(gameObject2);
		}
		if (LightmapSettings.lightmaps.Length > 0)
		{
			this.light.enabled = false;
			gameObject.SetActive(false);
		}
		return gameObject;
	}

	public override UnitParser Clone()
	{
		return new LightParser
		{
			firstRun = false,
			type = this.type,
			color = this.color,
			intensity = this.intensity,
			shadows = this.shadows,
			shadowStrength = this.shadowStrength,
			shadowBias = this.shadowBias,
			shadowSoftness = this.shadowSoftness,
			shadowSoftnessFade = this.shadowSoftnessFade,
			range = this.range,
			spotAngle = this.spotAngle,
			cullingMask = this.cullingMask
		};
	}

	public override void Read(BinaryReader br)
	{
		this.firstRun = false;
		this.type = (LightType)br.ReadInt32();
		this.color = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
		this.intensity = br.ReadSingle();
		this.shadows = (LightShadows)br.ReadInt32();
		this.shadowStrength = br.ReadSingle();
		this.shadowBias = br.ReadSingle();
		this.shadowSoftness = br.ReadSingle();
		this.shadowSoftnessFade = br.ReadSingle();
		this.range = br.ReadSingle();
		this.spotAngle = br.ReadSingle();
		this.cullingMask = br.ReadInt32();
	}

	public override void Write(BinaryWriter bw)
	{
		bw.Write((int)this.type);
		bw.Write(this.color.r);
		bw.Write(this.color.g);
		bw.Write(this.color.b);
		bw.Write(this.color.a);
		bw.Write(this.intensity);
		bw.Write((int)this.shadows);
		bw.Write(this.shadowStrength);
		bw.Write(this.shadowBias);
		bw.Write(this.shadowSoftness);
		bw.Write(this.shadowSoftnessFade);
		bw.Write(this.range);
		bw.Write(this.spotAngle);
		bw.Write(this.cullingMask);
	}
}
