using System;
using System.Reflection;
using UnityEngine;
using WellFired.Shared;

namespace WellFired
{
	[ExecuteInEditMode, USequencerEvent("Camera/Transition/Dissolve"), USequencerFriendlyName("Dissolve Transition")]
	public class USCameraDissolveTransition : USEventBase
	{
		[SerializeField]
		private Camera introCamera;

		[SerializeField]
		private Camera outroCamera;

		[SerializeField]
		private Material renderMaterial;

		private RenderTexture introRenderTexture;

		private RenderTexture outroRenderTexture;

		private bool shouldRender;

		private bool prevIntroCameraState;

		private bool prevOutroCameraState;

		private float alpha;

		private Vector2 MainGameViewSize
		{
			get
			{
				if (Application.isEditor)
				{
					Type type = Type.GetType("UnityEditor.GameView,UnityEditor");
					MethodInfo nonPublicStaticMethod = PlatformSpecificFactory.ReflectionHelper.GetNonPublicStaticMethod(type, "GetMainGameView");
					FieldInfo nonPublicInstanceField = PlatformSpecificFactory.ReflectionHelper.GetNonPublicInstanceField(type, "m_ShownResolution");
					object obj = nonPublicStaticMethod.Invoke(null, null);
					object value = nonPublicInstanceField.GetValue(obj);
					return (Vector2)value;
				}
				return new Vector2((float)Screen.width, (float)Screen.height);
			}
			set
			{
			}
		}

		private RenderTexture IntroRenderTexture
		{
			get
			{
				if (this.introRenderTexture == null)
				{
					this.introRenderTexture = new RenderTexture((int)this.MainGameViewSize.x, (int)this.MainGameViewSize.y, 24);
				}
				return this.introRenderTexture;
			}
			set
			{
			}
		}

		private RenderTexture OutroRenderTexture
		{
			get
			{
				if (this.outroRenderTexture == null)
				{
					this.outroRenderTexture = new RenderTexture((int)this.MainGameViewSize.x, (int)this.MainGameViewSize.y, 24);
				}
				return this.outroRenderTexture;
			}
			set
			{
			}
		}

		private void OnGUI()
		{
			if (!this.shouldRender)
			{
				return;
			}
			this.renderMaterial.SetTexture("_SecondTex", this.OutroRenderTexture);
			this.renderMaterial.SetFloat("_Alpha", this.alpha);
			Graphics.Blit(this.IntroRenderTexture, null, this.renderMaterial);
		}

		public override void FireEvent()
		{
			if (this.introCamera)
			{
				this.prevIntroCameraState = this.introCamera.enabled;
			}
			if (this.outroCamera)
			{
				this.prevOutroCameraState = this.outroCamera.enabled;
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
			if (this.introCamera)
			{
				this.introCamera.enabled = false;
			}
			if (this.outroCamera)
			{
				this.outroCamera.enabled = false;
			}
			if (this.introCamera)
			{
				this.introCamera.targetTexture = this.IntroRenderTexture;
				this.introCamera.Render();
			}
			if (this.outroCamera)
			{
				this.outroCamera.targetTexture = this.OutroRenderTexture;
				this.outroCamera.Render();
			}
			this.alpha = 1f - deltaTime / base.Duration;
			this.shouldRender = true;
		}

		public override void EndEvent()
		{
			this.shouldRender = false;
			if (this.outroCamera)
			{
				this.outroCamera.enabled = true;
				this.outroCamera.targetTexture = null;
			}
			if (this.introCamera)
			{
				this.introCamera.enabled = false;
				this.introCamera.targetTexture = null;
			}
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			if (this.introCamera)
			{
				this.introCamera.enabled = this.prevIntroCameraState;
				this.introCamera.targetTexture = null;
			}
			if (this.outroCamera)
			{
				this.outroCamera.enabled = this.prevOutroCameraState;
				this.outroCamera.targetTexture = null;
			}
			UnityEngine.Object.DestroyImmediate(this.IntroRenderTexture);
			UnityEngine.Object.DestroyImmediate(this.OutroRenderTexture);
			this.shouldRender = false;
		}
	}
}
