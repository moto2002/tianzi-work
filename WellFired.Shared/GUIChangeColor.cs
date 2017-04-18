using System;
using UnityEngine;

namespace WellFired.Shared
{
	public class GUIChangeColor : IDisposable
	{
		[SerializeField]
		private Color PreviousColor
		{
			get;
			set;
		}

		public GUIChangeColor(Color newColor)
		{
			this.PreviousColor = GUI.color;
			GUI.color = newColor;
		}

		public void Dispose()
		{
			GUI.color = this.PreviousColor;
		}
	}
}
