using System;
using UnityEngine;

namespace WellFired.Shared
{
	public class GizmosChangeColor : IDisposable
	{
		[SerializeField]
		private Color PreviousColor
		{
			get;
			set;
		}

		public GizmosChangeColor(Color newColor)
		{
			this.PreviousColor = GUI.color;
			Gizmos.color = newColor;
		}

		public void Dispose()
		{
			Gizmos.color = this.PreviousColor;
		}
	}
}
