using System;
using UnityEngine;

namespace WellFired.Shared
{
	public class GUIChangeContentColor : IDisposable
	{
		[SerializeField]
		private Color PreviousColor
		{
			get;
			set;
		}

		public GUIChangeContentColor(Color newColor)
		{
			this.PreviousColor = GUI.contentColor;
			GUI.contentColor = newColor;
		}

		public void Dispose()
		{
			GUI.contentColor = this.PreviousColor;
		}
	}
}
