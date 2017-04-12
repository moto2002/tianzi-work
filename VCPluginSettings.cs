using System;
using UnityEngine;

public class VCPluginSettings
{
	public const string kFakeMemberName = "fakeMember";

	public static bool EzguiEnabled(GameObject go)
	{
		if (typeof(SpriteRoot).GetMember("fakeMember").Length == 0)
		{
			return true;
		}
		VCUtils.DestroyWithError(go, "An EZGUI Virtual Control is being used, but EZGUI is not properly enabled!\nIn order to use EZGUI, open VCPluginSettings.cs and edit line 63 to #if false.\nSee that file for further instruction.  Destroying this control.");
		return false;
	}

	public static bool NguiEnabled(GameObject go)
	{
		if (typeof(UISprite).GetMember("fakeMember").Length == 0)
		{
			return true;
		}
		VCUtils.DestroyWithError(go, "An NGUI Virtual Control is being used, but NGUI is not properly enabled!\nIn order to use NGUI, open VCPluginSettings.cs and edit line 82 to #if false.\nSee that file for further instruction.  Destroying this control.");
		return false;
	}
}
