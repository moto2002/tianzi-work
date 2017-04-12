using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Debug")]
public class NGUIDebug : MonoBehaviour
{
	public static int miOpenLog = 0;

	public static bool mRayDebug = false;

	public static List<string> mLines = new List<string>();

	private static NGUIDebug mInstance = null;

	public static bool debugRaycast
	{
		get
		{
			return NGUIDebug.mRayDebug;
		}
		set
		{
			if (Application.isPlaying)
			{
				NGUIDebug.mRayDebug = value;
			}
		}
	}

	public static void CreateInstance()
	{
		if (NGUIDebug.mInstance == null)
		{
			GameObject gameObject = new GameObject("_NGUI Debug");
			NGUIDebug.mInstance = gameObject.AddComponent<NGUIDebug>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	private static void LogString(string text)
	{
		if (NGUIDebug.miOpenLog <= 0 || NGUIDebug.miOpenLog > 3)
		{
			return;
		}
		if (NGUIDebug.mLines.Count > 20)
		{
			NGUIDebug.mLines.RemoveAt(0);
		}
		NGUIDebug.mLines.Add(text);
	}

	public static void Log(params object[] objs)
	{
		if (NGUIDebug.miOpenLog <= 0 || NGUIDebug.miOpenLog > 3)
		{
			return;
		}
		string text = string.Empty;
		for (int i = 0; i < objs.Length; i++)
		{
			if (objs[i] != null)
			{
				if (i == 0)
				{
					text += objs[i].ToString();
				}
				else
				{
					text = text + ", " + objs[i].ToString();
				}
			}
		}
		NGUIDebug.LogString(text);
	}

	public static void DrawBounds(Bounds b)
	{
		Vector3 center = b.center;
		Vector3 vector = b.center - b.extents;
		Vector3 vector2 = b.center + b.extents;
		Debug.DrawLine(new Vector3(vector.x, vector.y, center.z), new Vector3(vector2.x, vector.y, center.z), Color.red);
		Debug.DrawLine(new Vector3(vector.x, vector.y, center.z), new Vector3(vector.x, vector2.y, center.z), Color.red);
		Debug.DrawLine(new Vector3(vector2.x, vector.y, center.z), new Vector3(vector2.x, vector2.y, center.z), Color.red);
		Debug.DrawLine(new Vector3(vector.x, vector2.y, center.z), new Vector3(vector2.x, vector2.y, center.z), Color.red);
	}
}
