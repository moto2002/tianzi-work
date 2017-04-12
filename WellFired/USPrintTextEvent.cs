using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Fullscreen/Print Text"), USequencerFriendlyName("Print Text")]
	public class USPrintTextEvent : USEventBase
	{
		public UILayer uiLayer;

		public string textToPrint = string.Empty;

		public Rect position = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);

		public float printRatePerCharacter;

		private string priorText = string.Empty;

		private string currentText = string.Empty;

		private bool display;

		public override void FireEvent()
		{
			this.priorText = this.currentText;
			this.currentText = this.textToPrint;
			if (base.Duration > 0f)
			{
				this.currentText = string.Empty;
			}
			this.display = true;
		}

		public override void ProcessEvent(float deltaTime)
		{
			if (this.printRatePerCharacter <= 0f)
			{
				this.currentText = this.textToPrint;
			}
			else
			{
				int num = (int)(deltaTime / this.printRatePerCharacter);
				if (num < this.textToPrint.Length)
				{
					this.currentText = this.textToPrint.Substring(0, num);
				}
				else
				{
					this.currentText = this.textToPrint;
				}
			}
			this.display = true;
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			this.currentText = this.priorText;
			this.display = false;
		}

		private void OnGUI()
		{
			if (!base.Sequence.IsPlaying)
			{
				return;
			}
			if (!this.display)
			{
				return;
			}
			int depth = GUI.depth;
			GUI.depth = (int)this.uiLayer;
			GUI.Label(this.position, this.currentText);
			GUI.depth = depth;
		}
	}
}
