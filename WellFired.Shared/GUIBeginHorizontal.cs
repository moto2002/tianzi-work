using System;
using UnityEngine;

namespace WellFired.Shared
{
	public class GUIBeginHorizontal : IDisposable
	{
		public GUIBeginHorizontal()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		}

		public GUIBeginHorizontal(params GUILayoutOption[] layoutOptions)
		{
			GUILayout.BeginHorizontal(layoutOptions);
		}

		public GUIBeginHorizontal(GUIStyle guiStyle, params GUILayoutOption[] layoutOptions)
		{
			GUILayout.BeginHorizontal(guiStyle, layoutOptions);
		}

		public void Dispose()
		{
			GUILayout.EndHorizontal();
		}
	}
}
