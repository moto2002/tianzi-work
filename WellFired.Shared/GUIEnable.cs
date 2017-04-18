using System;
using UnityEngine;

namespace WellFired.Shared
{
	public class GUIEnable : IDisposable
	{
		[SerializeField]
		private bool PreviousState
		{
			get;
			set;
		}

		public GUIEnable(bool newState)
		{
			this.PreviousState = GUI.enabled;
			GUI.enabled = newState;
		}

		public void Dispose()
		{
			GUI.enabled = this.PreviousState;
		}
	}
}
