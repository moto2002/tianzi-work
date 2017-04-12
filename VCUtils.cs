using System;
using UnityEngine;

public class VCUtils
{
	public static bool Approximately(float a, float b)
	{
		return Mathf.Approximately(a, b);
	}

	public static float GetSign(float val)
	{
		if (val < 0f)
		{
			return -1f;
		}
		return 1f;
	}

	public static Camera GetCamera(GameObject go)
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("UICamera");
		if (gameObject != null)
		{
			return gameObject.GetComponent<Camera>();
		}
		return null;
	}

	public static void ScaleRect(ref Rect r, Vector2 scale)
	{
		VCUtils.ScaleRect(ref r, scale.x, scale.y);
	}

	public static void ScaleRect(ref Rect r, float scaleX, float scaleY)
	{
		Vector2 center = r.center;
		r.width *= scaleX;
		r.height *= scaleY;
		r.center = center;
	}

	public static void DestroyWithError(GameObject go, string message)
	{
		LogSystem.LogWarning(new object[]
		{
			message
		});
		if (VCTouchController.Instance != null && VCTouchController.Instance.gameObject == go)
		{
			VCTouchController.ResetInstance();
		}
		UnityEngine.Object.Destroy(go);
	}

	public static void AddTouchController(GameObject go)
	{
		go.AddComponent<VCTouchController>();
	}
}
