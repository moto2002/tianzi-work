using System;
using UnityEngine;

public class UnitType
{
	public static int UnitType_General;

	public static int UnitType_Light = 1;

	public static int UnitType_Camera = 2;

	public static int UnitType_Effect = 3;

	public static int UnitType_Sound = 4;

	public static int UnitType_AmbienceSphere = 5;

	public static int UnitType_Npc = 6;

	public static UnitParser GenUnitParser(int type)
	{
		if (type == UnitType.UnitType_General)
		{
			return new UnitParser();
		}
		if (type == UnitType.UnitType_Light)
		{
			return new LightParser();
		}
		if (type == UnitType.UnitType_Effect)
		{
			return new EffectParser();
		}
		if (type == UnitType.UnitType_AmbienceSphere)
		{
			return new AmbienceSphereParser();
		}
		if (type == UnitType.UnitType_Npc)
		{
			return new NPCParser();
		}
		return null;
	}

	public static string GetTypeName(int type)
	{
		if (type == 1)
		{
			return "灯光";
		}
		if (type == 2)
		{
			return "摄像机";
		}
		if (type == 3)
		{
			return "特效";
		}
		if (type == 4)
		{
			return "声源";
		}
		if (type == 5)
		{
			return "环境球";
		}
		if (type == 6)
		{
			return "Npc";
		}
		return "普通类型";
	}

	public static int GetTypeByComponent(GameObject gameObject)
	{
		if (gameObject.GetComponent<Light>() != null)
		{
			return UnitType.UnitType_Light;
		}
		if (gameObject.GetComponent<Camera>() != null)
		{
			return UnitType.UnitType_Camera;
		}
		if (gameObject.GetComponent<AmbienceSampler>() != null)
		{
			return UnitType.UnitType_AmbienceSphere;
		}
		if (gameObject.GetComponent<Npc>() != null)
		{
			return UnitType.UnitType_Npc;
		}
		return -1;
	}

	public static int GetType(int lay)
	{
		if (lay == GameLayer.Layer_Light)
		{
			return UnitType.UnitType_Light;
		}
		if (lay == GameLayer.Layer_Camera)
		{
			return UnitType.UnitType_Camera;
		}
		if (lay == GameLayer.Layer_Effect)
		{
			return UnitType.UnitType_Effect;
		}
		if (lay == GameLayer.Layer_AmbienceSphere)
		{
			return UnitType.UnitType_AmbienceSphere;
		}
		if (lay == GameLayer.Layer_NPC)
		{
			return UnitType.UnitType_Npc;
		}
		return UnitType.UnitType_General;
	}
}
