using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Fullscreen/Display Image"), USequencerEventHideDuration, USequencerFriendlyName("Display Image")]
	public class USDisplayImageEvent : USEventBase
	{
		public UILayer uiLayer;

		public AnimationCurve fadeCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f),
			new Keyframe(3f, 1f),
			new Keyframe(4f, 0f)
		});

		public Texture2D displayImage;

		public UIPosition displayPosition;

		public UIPosition anchorPosition;

		private float currentCurveSampleTime;

		public override void FireEvent()
		{
			if (!this.displayImage)
			{
				Debug.LogWarning("Trying to use a DisplayImage Event, but you didn't give it an image to display", this);
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
			this.currentCurveSampleTime = deltaTime;
		}

		public override void EndEvent()
		{
			float b = this.fadeCurve.Evaluate(this.fadeCurve.keys[this.fadeCurve.length - 1].time);
			b = Mathf.Min(Mathf.Max(0f, b), 1f);
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			this.currentCurveSampleTime = 0f;
		}

		private void OnGUI()
		{
			if (!base.Sequence.IsPlaying)
			{
				return;
			}
			float num = 0f;
			Keyframe[] keys = this.fadeCurve.keys;
			for (int i = 0; i < keys.Length; i++)
			{
				Keyframe keyframe = keys[i];
				if (keyframe.time > num)
				{
					num = keyframe.time;
				}
			}
			base.Duration = num;
			float num2 = this.fadeCurve.Evaluate(this.currentCurveSampleTime);
			num2 = Mathf.Min(Mathf.Max(0f, num2), 1f);
			if (!this.displayImage)
			{
				return;
			}
			Rect position = new Rect((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, (float)this.displayImage.width, (float)this.displayImage.height);
			switch (this.displayPosition)
			{
			case UIPosition.TopLeft:
				position.x = 0f;
				position.y = 0f;
				break;
			case UIPosition.TopRight:
				position.x = (float)Screen.width;
				position.y = 0f;
				break;
			case UIPosition.BottomLeft:
				position.x = 0f;
				position.y = (float)Screen.height;
				break;
			case UIPosition.BottomRight:
				position.x = (float)Screen.width;
				position.y = (float)Screen.height;
				break;
			}
			switch (this.anchorPosition)
			{
			case UIPosition.Center:
				position.x -= (float)this.displayImage.width * 0.5f;
				position.y -= (float)this.displayImage.height * 0.5f;
				break;
			case UIPosition.TopRight:
				position.x -= (float)this.displayImage.width;
				break;
			case UIPosition.BottomLeft:
				position.y -= (float)this.displayImage.height;
				break;
			case UIPosition.BottomRight:
				position.x -= (float)this.displayImage.width;
				position.y -= (float)this.displayImage.height;
				break;
			}
			GUI.depth = (int)this.uiLayer;
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, num2);
			GUI.DrawTexture(position, this.displayImage);
			GUI.color = color;
		}
	}
}
