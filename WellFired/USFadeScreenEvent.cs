using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Fullscreen/Fade Screen"), USequencerEventHideDuration, USequencerFriendlyName("Fade Screen")]
	public class USFadeScreenEvent : USEventBase
	{
		public UILayer uiLayer;

		public AnimationCurve fadeCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f),
			new Keyframe(3f, 1f),
			new Keyframe(4f, 0f)
		});

		public Color fadeColour = Color.black;

		private float currentCurveSampleTime;

		public static Texture2D texture;

		public override void FireEvent()
		{
		}

		public override void ProcessEvent(float deltaTime)
		{
			this.currentCurveSampleTime = deltaTime;
			if (!USFadeScreenEvent.texture)
			{
				USFadeScreenEvent.texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			}
			float num = this.fadeCurve.Evaluate(this.currentCurveSampleTime);
			num = Mathf.Min(Mathf.Max(0f, num), 1f);
			USFadeScreenEvent.texture.SetPixel(0, 0, new Color(this.fadeColour.r, this.fadeColour.g, this.fadeColour.b, num));
			USFadeScreenEvent.texture.Apply();
		}

		public override void EndEvent()
		{
			if (!USFadeScreenEvent.texture)
			{
				USFadeScreenEvent.texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			}
			float num = this.fadeCurve.Evaluate(this.fadeCurve.keys[this.fadeCurve.length - 1].time);
			num = Mathf.Min(Mathf.Max(0f, num), 1f);
			USFadeScreenEvent.texture.SetPixel(0, 0, new Color(this.fadeColour.r, this.fadeColour.g, this.fadeColour.b, num));
			USFadeScreenEvent.texture.Apply();
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			this.currentCurveSampleTime = 0f;
			if (!USFadeScreenEvent.texture)
			{
				USFadeScreenEvent.texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			}
			USFadeScreenEvent.texture.SetPixel(0, 0, new Color(this.fadeColour.r, this.fadeColour.g, this.fadeColour.b, 0f));
			USFadeScreenEvent.texture.Apply();
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
			if (!USFadeScreenEvent.texture)
			{
				return;
			}
			int depth = GUI.depth;
			GUI.depth = (int)this.uiLayer;
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), USFadeScreenEvent.texture);
			GUI.depth = depth;
		}

		private void OnEnable()
		{
			if (USFadeScreenEvent.texture == null)
			{
				USFadeScreenEvent.texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			}
			USFadeScreenEvent.texture.SetPixel(0, 0, new Color(this.fadeColour.r, this.fadeColour.g, this.fadeColour.b, 0f));
			USFadeScreenEvent.texture.Apply();
		}
	}
}
