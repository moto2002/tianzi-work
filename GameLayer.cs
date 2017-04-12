using System;
using UnityEngine;

public class GameLayer
{
	public static string Layer_Str_Ground = "Ground";

	public static string Layer_Str_Builder = "Builder";

	public static string Layer_Str_Builder_NO_Occlusion = "Builder_NO_Occlusion";

	public static string Layer_Str_Plant = "Plant";

	public static string Layer_Str_Water = "Water";

	public static string Layer_Str_Occlusion_Water = "OcclusionWater";

	public static string Layer_Str_Player = "Player";

	public static string Layer_Str_NPC = "NPC";

	public static string Layer_Str_Occlusion_Object = "Occlusion_Object";

	public static string Layer_Str_NO_Occlusion_Object = "NO_Occlusion_Object";

	public static string Layer_Str_Light = "Light";

	public static string Layer_Str_Camera = "Camera";

	public static string Layer_Str_Effect = "Effect";

	public static string Layer_Str_AmbienceSphere = "AmbienceSphere";

	public static string Layer_Str_UIEffect = "UIEffect";

	public static int Layer_Ground = LayerMask.NameToLayer(GameLayer.Layer_Str_Ground);

	public static int Layer_Builder = LayerMask.NameToLayer(GameLayer.Layer_Str_Builder);

	public static int Layer_Builder_NO_Occlusion = LayerMask.NameToLayer(GameLayer.Layer_Str_Builder_NO_Occlusion);

	public static int Layer_Plant = LayerMask.NameToLayer(GameLayer.Layer_Str_Plant);

	public static int Layer_Water = LayerMask.NameToLayer(GameLayer.Layer_Str_Water);

	public static int Layer_Occlusion_Water = LayerMask.NameToLayer(GameLayer.Layer_Str_Occlusion_Water);

	public static int Layer_Player = LayerMask.NameToLayer(GameLayer.Layer_Str_Player);

	public static int Layer_NPC = LayerMask.NameToLayer(GameLayer.Layer_Str_NPC);

	public static int Layer_Occlusion_Object = LayerMask.NameToLayer(GameLayer.Layer_Str_Occlusion_Object);

	public static int Layer_NO_Occlusion_Object = LayerMask.NameToLayer(GameLayer.Layer_Str_NO_Occlusion_Object);

	public static int Layer_Light = LayerMask.NameToLayer(GameLayer.Layer_Str_Light);

	public static int Layer_Camera = LayerMask.NameToLayer(GameLayer.Layer_Str_Camera);

	public static int Layer_Effect = LayerMask.NameToLayer(GameLayer.Layer_Str_Effect);

	public static int Layer_AmbienceSphere = LayerMask.NameToLayer(GameLayer.Layer_Str_AmbienceSphere);

	public static int Layer_UIEffect = LayerMask.NameToLayer(GameLayer.Layer_Str_UIEffect);

	public static int Mask_Ground = GameLayer.GetMask(GameLayer.Layer_Ground);

	public static int Mask_Builder = GameLayer.GetMask(GameLayer.Layer_Builder);

	public static int Mask_Builder_NO_Occlusion = GameLayer.GetMask(GameLayer.Layer_Builder_NO_Occlusion);

	public static int Mask_Plant = GameLayer.GetMask(GameLayer.Layer_Plant);

	public static int Mask_Water = GameLayer.GetMask(GameLayer.Layer_Water);

	public static int Mask_Occlusion_Water = GameLayer.GetMask(GameLayer.Layer_Occlusion_Water);

	public static int Mask_Player = GameLayer.GetMask(GameLayer.Layer_Player);

	public static int Mask_NPC = GameLayer.GetMask(GameLayer.Layer_NPC);

	public static int Mask_Occlusion_Object = GameLayer.GetMask(GameLayer.Layer_Occlusion_Object);

	public static int Mask_NO_Occlusion_Object = GameLayer.GetMask(GameLayer.Layer_NO_Occlusion_Object);

	public static int Mask_Light = GameLayer.GetMask(GameLayer.Layer_Light);

	public static int Mask_Camera = GameLayer.GetMask(GameLayer.Layer_Camera);

	public static int Mask_Effect = GameLayer.GetMask(GameLayer.Layer_Effect);

	public static int Mask_AmbienceSphere = GameLayer.GetMask(GameLayer.Layer_AmbienceSphere);

	public static int GetMask(int lay)
	{
		return 1 << lay;
	}

	public static int GetCombineMask(string[] layers)
	{
		string layerName = string.Empty;
		int num = 0;
		for (int i = 0; i < layers.Length; i++)
		{
			layerName = layers[i];
			int num2 = LayerMask.NameToLayer(layerName);
			num |= 1 << num2;
		}
		return num;
	}
}
