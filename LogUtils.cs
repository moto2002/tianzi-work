using System;
using UnityEngine;

public class LogUtils : MonoBehaviour
{
	private void OnGUI()
	{
		if (GUILayout.Button("clear", new GUILayoutOption[]
		{
			GUILayout.Height(30f)
		}))
		{
			NGUIDebug.mLines.Clear();
		}
		if (NGUIDebug.mLines.Count == 0)
		{
			if (NGUIDebug.mRayDebug && UICamera.hoveredObject != null)
			{
				GUILayout.Label("Last Hit: " + NGUITools.GetHierarchy(UICamera.hoveredObject).Replace("\"", string.Empty), new GUILayoutOption[0]);
			}
		}
		else
		{
			int i = 0;
			int count = NGUIDebug.mLines.Count;
			while (i < count)
			{
				GUIStyle gUIStyle = new GUIStyle();
				gUIStyle.fontSize = 26;
				gUIStyle.normal.textColor = Color.red;
				GUILayout.Label(NGUIDebug.mLines[i], gUIStyle, new GUILayoutOption[0]);
				i++;
			}
		}
	}
}
