using System;
using UnityEngine;

namespace WellFired.Shared
{
	public class GUIBeginVertical : IDisposable
	{
		public GUIBeginVertical()
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
		}

		public GUIBeginVertical(params GUILayoutOption[] layoutOptions)
		{
			GUILayout.BeginVertical(layoutOptions);
		}

		public GUIBeginVertical(GUIStyle guiStyle, params GUILayoutOption[] layoutOptions)
		{
			GUILayout.BeginVertical(guiStyle, layoutOptions);
		}

		public void Dispose()
		{
			GUILayout.EndVertical();
		}
	}
}
