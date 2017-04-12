using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Panel"), ExecuteInEditMode]
public class UIPanel : UIRect
{
	public enum DebugInfo
	{
		None,
		Gizmos,
		Geometry
	}

	public enum RenderQueue
	{
		Automatic,
		StartAt,
		Explicit
	}

	public delegate void OnGeometryUpdated();

	public delegate void OnClippingMoved(UIPanel panel);

	public static BetterList<UIPanel> list = new BetterList<UIPanel>();

	public UIPanel.OnGeometryUpdated onGeometryUpdated;

	public bool showInPanelTool = true;

	public bool generateNormals;

	public bool widgetsAreStatic;

	public bool cullWhileDragging;

	public bool alwaysOnScreen;

	public bool anchorOffset;

	public UIPanel.RenderQueue renderQueue;

	public int startingRenderQueue = 3000;

	[NonSerialized]
	public BetterList<UIWidget> widgets = new BetterList<UIWidget>();

	[NonSerialized]
	public BetterList<UIDrawCall> drawCalls = new BetterList<UIDrawCall>();

	public BetterList<AdaptiveRenderQueue> renderQuenes = new BetterList<AdaptiveRenderQueue>();

	[NonSerialized]
	public Matrix4x4 worldToLocal = Matrix4x4.identity;

	public UIPanel.OnClippingMoved onClipMove;

	[HideInInspector, SerializeField]
	private float mAlpha = 1f;

	[HideInInspector, SerializeField]
	private UIDrawCall.Clipping mClipping;

	[HideInInspector, SerializeField]
	private Vector4 mClipRange = new Vector4(0f, 0f, 300f, 200f);

	[HideInInspector, SerializeField]
	private Vector2 mClipSoftness = new Vector2(4f, 4f);

	[HideInInspector, SerializeField]
	private int mDepth;

	[HideInInspector, SerializeField]
	private int mSortingOrder;

	private bool mRebuild;

	private bool mResized;

	private Camera mCam;

	[SerializeField]
	private Vector2 mClipOffset = Vector2.zero;

	private float mCullTime;

	private float mUpdateTime;

	private int mMatrixFrame = -1;

	private int mAlphaFrameID;

	private int mLayer = -1;

	private static float[] mTemp = new float[4];

	private Vector2 mMin = Vector2.zero;

	private Vector2 mMax = Vector2.zero;

	private bool mHalfPixelOffset;

	private bool mSortWidgets;

	private static Vector3[] mCorners = new Vector3[4];

	private static int mUpdateFrame = -1;

	private bool mForced;

	public static int nextUnusedDepth
	{
		get
		{
			int num = -2147483648;
			for (int i = 0; i < UIPanel.list.size; i++)
			{
				num = Mathf.Max(num, UIPanel.list[i].depth);
			}
			return (num != -2147483648) ? (num + 1) : 0;
		}
	}

	public override bool canBeAnchored
	{
		get
		{
			return this.mClipping != UIDrawCall.Clipping.None;
		}
	}

	public override float alpha
	{
		get
		{
			return this.mAlpha;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (this.mAlpha != num)
			{
				this.mAlphaFrameID = -1;
				this.mResized = true;
				this.mAlpha = num;
				this.SetDirty();
			}
		}
	}

	public int depth
	{
		get
		{
			return this.mDepth;
		}
		set
		{
			if (this.mDepth != value)
			{
				this.mDepth = value;
				UIPanel.list.Sort(new BetterList<UIPanel>.CompareFunc(UIPanel.CompareFunc));
			}
		}
	}

	public int sortingOrder
	{
		get
		{
			return this.mSortingOrder;
		}
		set
		{
			if (this.mSortingOrder != value)
			{
				this.mSortingOrder = value;
				this.UpdateDrawCalls();
			}
		}
	}

	public float width
	{
		get
		{
			return this.GetViewSize().x;
		}
	}

	public float height
	{
		get
		{
			return this.GetViewSize().y;
		}
	}

	public bool halfPixelOffset
	{
		get
		{
			return this.mHalfPixelOffset;
		}
	}

	public bool usedForUI
	{
		get
		{
			return this.mCam != null && this.mCam.isOrthoGraphic;
		}
	}

	public Vector3 drawCallOffset
	{
		get
		{
			if (this.mHalfPixelOffset && this.mCam != null && this.mCam.isOrthoGraphic)
			{
				float num = 1f / (this.GetWindowSize().y * this.mCam.orthographicSize);
				Vector3 zero = Vector3.zero;
				zero.x = -num;
				zero.y = num;
				return zero;
			}
			return Vector3.zero;
		}
	}

	public UIDrawCall.Clipping clipping
	{
		get
		{
			return this.mClipping;
		}
		set
		{
			if (this.mClipping != value)
			{
				this.mResized = true;
				this.mClipping = value;
				this.mMatrixFrame = -1;
			}
		}
	}

	public bool clipsChildren
	{
		get
		{
			return this.mClipping == UIDrawCall.Clipping.AlphaClip || this.mClipping == UIDrawCall.Clipping.SoftClip;
		}
	}

	public Vector2 clipOffset
	{
		get
		{
			return this.mClipOffset;
		}
		set
		{
			if (Mathf.Abs(this.mClipOffset.x - value.x) > 0.001f || Mathf.Abs(this.mClipOffset.y - value.y) > 0.001f)
			{
				this.mResized = true;
				this.mCullTime = ((this.mCullTime != 0f) ? (RealTime.time + 0.15f) : 0.001f);
				this.mClipOffset = value;
				this.mMatrixFrame = -1;
				if (this.onClipMove != null)
				{
					this.onClipMove(this);
				}
			}
		}
	}

	[Obsolete("Use 'finalClipRegion' or 'baseClipRegion' instead")]
	public Vector4 clipRange
	{
		get
		{
			return this.baseClipRegion;
		}
		set
		{
			this.baseClipRegion = value;
		}
	}

	public Vector4 baseClipRegion
	{
		get
		{
			return this.mClipRange;
		}
		set
		{
			if (Mathf.Abs(this.mClipRange.x - value.x) > 0.001f || Mathf.Abs(this.mClipRange.y - value.y) > 0.001f || Mathf.Abs(this.mClipRange.z - value.z) > 0.001f || Mathf.Abs(this.mClipRange.w - value.w) > 0.001f)
			{
				this.mResized = true;
				this.mCullTime = ((this.mCullTime != 0f) ? (RealTime.time + 0.15f) : 0.001f);
				this.mClipRange = value;
				this.mMatrixFrame = -1;
				UIScrollView component = base.GetComponent<UIScrollView>();
				if (component != null)
				{
					component.UpdatePosition();
				}
				if (this.onClipMove != null)
				{
					this.onClipMove(this);
				}
			}
		}
	}

	public Vector4 finalClipRegion
	{
		get
		{
			Vector2 viewSize = this.GetViewSize();
			Vector4 zero = Vector4.zero;
			if (this.mClipping != UIDrawCall.Clipping.None)
			{
				zero.x = this.mClipRange.x + this.mClipOffset.x;
				zero.y = this.mClipRange.y + this.mClipOffset.y;
				zero.z = viewSize.x;
				zero.w = viewSize.y;
				return zero;
			}
			zero.x = 0f;
			zero.y = 0f;
			zero.z = viewSize.x;
			zero.w = viewSize.y;
			return zero;
		}
	}

	public Vector2 clipSoftness
	{
		get
		{
			return this.mClipSoftness;
		}
		set
		{
			if (this.mClipSoftness != value)
			{
				this.mClipSoftness = value;
			}
		}
	}

	public override Vector3[] localCorners
	{
		get
		{
			if (this.mClipping == UIDrawCall.Clipping.None)
			{
				Vector2 viewSize = this.GetViewSize();
				float num = -0.5f * viewSize.x;
				float num2 = -0.5f * viewSize.y;
				float x = num + viewSize.x;
				float y = num2 + viewSize.y;
				Transform transform = (!(this.mCam != null)) ? null : this.mCam.transform;
				if (transform != null)
				{
					UIPanel.mCorners[0] = transform.TransformPoint(num, num2, 0f);
					UIPanel.mCorners[1] = transform.TransformPoint(num, y, 0f);
					UIPanel.mCorners[2] = transform.TransformPoint(x, y, 0f);
					UIPanel.mCorners[3] = transform.TransformPoint(x, num2, 0f);
					transform = base.cachedTransform;
					for (int i = 0; i < 4; i++)
					{
						UIPanel.mCorners[i] = transform.InverseTransformPoint(UIPanel.mCorners[i]);
					}
				}
				else
				{
					Vector3 zero = Vector3.zero;
					zero.x = num;
					zero.y = num2;
					UIPanel.mCorners[0] = zero;
					zero.x = num;
					zero.y = y;
					UIPanel.mCorners[1] = zero;
					zero.x = x;
					zero.y = y;
					UIPanel.mCorners[2] = zero;
					zero.x = x;
					zero.y = num2;
					UIPanel.mCorners[3] = zero;
				}
			}
			else
			{
				float num3 = this.mClipOffset.x + this.mClipRange.x - 0.5f * this.mClipRange.z;
				float num4 = this.mClipOffset.y + this.mClipRange.y - 0.5f * this.mClipRange.w;
				float x2 = num3 + this.mClipRange.z;
				float y2 = num4 + this.mClipRange.w;
				Vector3 zero2 = Vector3.zero;
				zero2.x = num3;
				zero2.y = num4;
				UIPanel.mCorners[0] = zero2;
				zero2.x = num3;
				zero2.y = y2;
				UIPanel.mCorners[1] = zero2;
				zero2.x = x2;
				zero2.y = y2;
				UIPanel.mCorners[2] = zero2;
				zero2.x = x2;
				zero2.y = num4;
				UIPanel.mCorners[3] = zero2;
			}
			return UIPanel.mCorners;
		}
	}

	public override Vector3[] worldCorners
	{
		get
		{
			if (this.mClipping == UIDrawCall.Clipping.None)
			{
				Vector2 viewSize = this.GetViewSize();
				float num = -0.5f * viewSize.x;
				float num2 = -0.5f * viewSize.y;
				float x = num + viewSize.x;
				float y = num2 + viewSize.y;
				Transform transform = (!(this.mCam != null)) ? null : this.mCam.transform;
				if (transform != null)
				{
					UIPanel.mCorners[0] = transform.TransformPoint(num, num2, 0f);
					UIPanel.mCorners[1] = transform.TransformPoint(num, y, 0f);
					UIPanel.mCorners[2] = transform.TransformPoint(x, y, 0f);
					UIPanel.mCorners[3] = transform.TransformPoint(x, num2, 0f);
				}
			}
			else
			{
				float num3 = this.mClipOffset.x + this.mClipRange.x - 0.5f * this.mClipRange.z;
				float num4 = this.mClipOffset.y + this.mClipRange.y - 0.5f * this.mClipRange.w;
				float x2 = num3 + this.mClipRange.z;
				float y2 = num4 + this.mClipRange.w;
				Transform cachedTransform = base.cachedTransform;
				UIPanel.mCorners[0] = cachedTransform.TransformPoint(num3, num4, 0f);
				UIPanel.mCorners[1] = cachedTransform.TransformPoint(num3, y2, 0f);
				UIPanel.mCorners[2] = cachedTransform.TransformPoint(x2, y2, 0f);
				UIPanel.mCorners[3] = cachedTransform.TransformPoint(x2, num4, 0f);
			}
			return UIPanel.mCorners;
		}
	}

	public static int CompareFunc(UIPanel a, UIPanel b)
	{
		if (!(a != b) || !(a != null) || !(b != null))
		{
			return 0;
		}
		if (a.mDepth < b.mDepth)
		{
			return -1;
		}
		if (a.mDepth > b.mDepth)
		{
			return 1;
		}
		return (a.GetInstanceID() >= b.GetInstanceID()) ? 1 : -1;
	}

	public override Vector3[] GetSides(Transform relativeTo)
	{
		if (this.mClipping != UIDrawCall.Clipping.None || this.anchorOffset)
		{
			Vector2 viewSize = this.GetViewSize();
			Vector2 vector = (this.mClipping == UIDrawCall.Clipping.None) ? Vector2.zero : (this.mClipRange + this.mClipOffset);
			float num = vector.x - 0.5f * viewSize.x;
			float num2 = vector.y - 0.5f * viewSize.y;
			float num3 = num + viewSize.x;
			float num4 = num2 + viewSize.y;
			float x = (num + num3) * 0.5f;
			float y = (num2 + num4) * 0.5f;
			Matrix4x4 localToWorldMatrix = base.cachedTransform.localToWorldMatrix;
			Vector3 zero = Vector3.zero;
			zero.x = num;
			zero.y = y;
			UIPanel.mCorners[0] = localToWorldMatrix.MultiplyPoint3x4(zero);
			zero.x = x;
			zero.y = num4;
			UIPanel.mCorners[1] = localToWorldMatrix.MultiplyPoint3x4(zero);
			zero.x = num3;
			zero.y = y;
			UIPanel.mCorners[2] = localToWorldMatrix.MultiplyPoint3x4(zero);
			zero.x = x;
			zero.y = num2;
			UIPanel.mCorners[3] = localToWorldMatrix.MultiplyPoint3x4(zero);
			if (relativeTo != null)
			{
				for (int i = 0; i < 4; i++)
				{
					UIPanel.mCorners[i] = relativeTo.InverseTransformPoint(UIPanel.mCorners[i]);
				}
			}
			return UIPanel.mCorners;
		}
		return base.GetSides(relativeTo);
	}

	public override void Invalidate(bool includeChildren)
	{
		this.mAlphaFrameID = -1;
		base.Invalidate(includeChildren);
	}

	public override float CalculateFinalAlpha(int frameID)
	{
		if (this.mAlphaFrameID != frameID)
		{
			this.mAlphaFrameID = frameID;
			UIRect parent = base.parent;
			this.finalAlpha = ((!(base.parent != null)) ? this.mAlpha : (parent.CalculateFinalAlpha(frameID) * this.mAlpha));
		}
		return this.finalAlpha;
	}

	public bool IsVisible(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		this.UpdateTransformMatrix();
		a = this.worldToLocal.MultiplyPoint3x4(a);
		b = this.worldToLocal.MultiplyPoint3x4(b);
		c = this.worldToLocal.MultiplyPoint3x4(c);
		d = this.worldToLocal.MultiplyPoint3x4(d);
		UIPanel.mTemp[0] = a.x;
		UIPanel.mTemp[1] = b.x;
		UIPanel.mTemp[2] = c.x;
		UIPanel.mTemp[3] = d.x;
		float num = Mathf.Min(UIPanel.mTemp);
		float num2 = Mathf.Max(UIPanel.mTemp);
		UIPanel.mTemp[0] = a.y;
		UIPanel.mTemp[1] = b.y;
		UIPanel.mTemp[2] = c.y;
		UIPanel.mTemp[3] = d.y;
		float num3 = Mathf.Min(UIPanel.mTemp);
		float num4 = Mathf.Max(UIPanel.mTemp);
		return num2 >= this.mMin.x && num4 >= this.mMin.y && num <= this.mMax.x && num3 <= this.mMax.y;
	}

	public bool IsVisible(Vector3 worldPos)
	{
		if (this.mAlpha < 0.001f)
		{
			return false;
		}
		if (this.mClipping == UIDrawCall.Clipping.None || this.mClipping == UIDrawCall.Clipping.ConstrainButDontClip)
		{
			return true;
		}
		this.UpdateTransformMatrix();
		Vector3 vector = this.worldToLocal.MultiplyPoint3x4(worldPos);
		return vector.x >= this.mMin.x && vector.y >= this.mMin.y && vector.x <= this.mMax.x && vector.y <= this.mMax.y;
	}

	public bool IsVisible(UIWidget w)
	{
		if ((this.mClipping == UIDrawCall.Clipping.None || this.mClipping == UIDrawCall.Clipping.ConstrainButDontClip) && !w.hideIfOffScreen)
		{
			return true;
		}
		Vector3[] worldCorners = w.worldCorners;
		return this.IsVisible(worldCorners[0], worldCorners[1], worldCorners[2], worldCorners[3]);
	}

	[ContextMenu("Force Refresh")]
	public void RebuildAllDrawCalls()
	{
		this.mRebuild = true;
	}

	public void SetDirty()
	{
		for (int i = 0; i < this.drawCalls.size; i++)
		{
			this.drawCalls.buffer[i].isDirty = true;
		}
		this.Invalidate(true);
	}

	private void Awake()
	{
		this.mGo = base.gameObject;
		this.mTrans = base.transform;
		this.mHalfPixelOffset = (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.XBOX360 || Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.WindowsEditor);
		if (this.mHalfPixelOffset)
		{
			this.mHalfPixelOffset = (SystemInfo.graphicsShaderLevel < 40);
		}
	}

	protected override void OnStart()
	{
		this.mLayer = this.mGo.layer;
		UICamera uICamera = UICamera.FindCameraForLayer(this.mLayer);
		this.mCam = ((!(uICamera != null)) ? NGUITools.FindCameraForLayer(this.mLayer) : uICamera.cachedCamera);
	}

	protected override void OnInit()
	{
		base.OnInit();
		if (base.rigidbody == null)
		{
			Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
		}
		this.mRebuild = true;
		this.mAlphaFrameID = -1;
		this.mMatrixFrame = -1;
		UIPanel.list.Add(this);
		UIPanel.list.Sort(new BetterList<UIPanel>.CompareFunc(UIPanel.CompareFunc));
	}

	public void AddRenderQuene(AdaptiveRenderQueue _renderQuene)
	{
		if (_renderQuene == null)
		{
			return;
		}
		if (!this.renderQuenes.Contains(_renderQuene))
		{
			this.renderQuenes.Add(_renderQuene);
		}
		this.renderQuenes.Sort(new BetterList<AdaptiveRenderQueue>.CompareFunc(AdaptiveRenderQueue.RenderQueueCompareFunc));
	}

	public void RemoveRenderQuene(AdaptiveRenderQueue _renderQuene)
	{
		if (_renderQuene == null)
		{
			return;
		}
		this.renderQuenes.Remove(_renderQuene);
		this.renderQuenes.Sort(new BetterList<AdaptiveRenderQueue>.CompareFunc(AdaptiveRenderQueue.RenderQueueCompareFunc));
	}

	protected override void OnDisable()
	{
		for (int i = 0; i < this.drawCalls.size; i++)
		{
			UIDrawCall uIDrawCall = this.drawCalls.buffer[i];
			if (uIDrawCall != null)
			{
				UIDrawCall.Destroy(uIDrawCall);
			}
		}
		this.drawCalls.Clear();
		UIPanel.list.Remove(this);
		this.mAlphaFrameID = -1;
		this.mMatrixFrame = -1;
		if (UIPanel.list.size == 0)
		{
			UIDrawCall.ReleaseAll();
			UIPanel.mUpdateFrame = -1;
		}
		base.OnDisable();
	}

	private void UpdateTransformMatrix()
	{
		int frameCount = Time.frameCount;
		if (this.mMatrixFrame != frameCount)
		{
			this.mMatrixFrame = frameCount;
			this.worldToLocal = base.cachedTransform.worldToLocalMatrix;
			Vector2 vector = this.GetViewSize() * 0.5f;
			float num = this.mClipOffset.x + this.mClipRange.x;
			float num2 = this.mClipOffset.y + this.mClipRange.y;
			this.mMin.x = num - vector.x;
			this.mMin.y = num2 - vector.y;
			this.mMax.x = num + vector.x;
			this.mMax.y = num2 + vector.y;
		}
	}

	protected override void OnAnchor()
	{
		if (this.mClipping == UIDrawCall.Clipping.None)
		{
			return;
		}
		Transform cachedTransform = base.cachedTransform;
		Transform parent = cachedTransform.parent;
		Vector2 viewSize = this.GetViewSize();
		Vector2 vector = cachedTransform.localPosition;
		float num;
		float num2;
		float num3;
		float num4;
		if (this.leftAnchor.target == this.bottomAnchor.target && this.leftAnchor.target == this.rightAnchor.target && this.leftAnchor.target == this.topAnchor.target)
		{
			Vector3[] sides = this.leftAnchor.GetSides(parent);
			if (sides != null)
			{
				num = NGUIMath.Lerp(sides[0].x, sides[2].x, this.leftAnchor.relative) + (float)this.leftAnchor.absolute;
				num2 = NGUIMath.Lerp(sides[0].x, sides[2].x, this.rightAnchor.relative) + (float)this.rightAnchor.absolute;
				num3 = NGUIMath.Lerp(sides[3].y, sides[1].y, this.bottomAnchor.relative) + (float)this.bottomAnchor.absolute;
				num4 = NGUIMath.Lerp(sides[3].y, sides[1].y, this.topAnchor.relative) + (float)this.topAnchor.absolute;
			}
			else
			{
				Vector2 vector2 = base.GetLocalPos(this.leftAnchor, parent);
				num = vector2.x + (float)this.leftAnchor.absolute;
				num3 = vector2.y + (float)this.bottomAnchor.absolute;
				num2 = vector2.x + (float)this.rightAnchor.absolute;
				num4 = vector2.y + (float)this.topAnchor.absolute;
			}
		}
		else
		{
			if (this.leftAnchor.target)
			{
				Vector3[] sides2 = this.leftAnchor.GetSides(parent);
				if (sides2 != null)
				{
					num = NGUIMath.Lerp(sides2[0].x, sides2[2].x, this.leftAnchor.relative) + (float)this.leftAnchor.absolute;
				}
				else
				{
					num = base.GetLocalPos(this.leftAnchor, parent).x + (float)this.leftAnchor.absolute;
				}
			}
			else
			{
				num = this.mClipRange.x - 0.5f * viewSize.x;
			}
			if (this.rightAnchor.target)
			{
				Vector3[] sides3 = this.rightAnchor.GetSides(parent);
				if (sides3 != null)
				{
					num2 = NGUIMath.Lerp(sides3[0].x, sides3[2].x, this.rightAnchor.relative) + (float)this.rightAnchor.absolute;
				}
				else
				{
					num2 = base.GetLocalPos(this.rightAnchor, parent).x + (float)this.rightAnchor.absolute;
				}
			}
			else
			{
				num2 = this.mClipRange.x + 0.5f * viewSize.x;
			}
			if (this.bottomAnchor.target)
			{
				Vector3[] sides4 = this.bottomAnchor.GetSides(parent);
				if (sides4 != null)
				{
					num3 = NGUIMath.Lerp(sides4[3].y, sides4[1].y, this.bottomAnchor.relative) + (float)this.bottomAnchor.absolute;
				}
				else
				{
					num3 = base.GetLocalPos(this.bottomAnchor, parent).y + (float)this.bottomAnchor.absolute;
				}
			}
			else
			{
				num3 = this.mClipRange.y - 0.5f * viewSize.y;
			}
			if (this.topAnchor.target)
			{
				Vector3[] sides5 = this.topAnchor.GetSides(parent);
				if (sides5 != null)
				{
					num4 = NGUIMath.Lerp(sides5[3].y, sides5[1].y, this.topAnchor.relative) + (float)this.topAnchor.absolute;
				}
				else
				{
					num4 = base.GetLocalPos(this.topAnchor, parent).y + (float)this.topAnchor.absolute;
				}
			}
			else
			{
				num4 = this.mClipRange.y + 0.5f * viewSize.y;
			}
		}
		num -= vector.x + this.mClipOffset.x;
		num2 -= vector.x + this.mClipOffset.x;
		num3 -= vector.y + this.mClipOffset.y;
		num4 -= vector.y + this.mClipOffset.y;
		float x = Mathf.Lerp(num, num2, 0.5f);
		float y = Mathf.Lerp(num3, num4, 0.5f);
		float num5 = num2 - num;
		float num6 = num4 - num3;
		float num7 = Mathf.Max(20f, this.mClipSoftness.x);
		float num8 = Mathf.Max(20f, this.mClipSoftness.y);
		if (num5 < num7)
		{
			num5 = num7;
		}
		if (num6 < num8)
		{
			num6 = num8;
		}
		Vector4 zero = Vector4.zero;
		zero.x = x;
		zero.y = y;
		zero.z = num5;
		zero.w = num6;
		this.baseClipRegion = zero;
	}

	private void LateUpdate()
	{
		if (UIPanel.mUpdateFrame != Time.frameCount)
		{
			UIPanel.mUpdateFrame = Time.frameCount;
			for (int i = 0; i < UIPanel.list.size; i++)
			{
				UIPanel.list[i].UpdateSelf();
			}
			int num = 3000;
			for (int j = 0; j < UIPanel.list.size; j++)
			{
				UIPanel uIPanel = UIPanel.list.buffer[j];
				if (uIPanel.renderQueue == UIPanel.RenderQueue.Automatic)
				{
					uIPanel.startingRenderQueue = num;
					uIPanel.UpdateDrawCalls();
					num += uIPanel.drawCalls.size;
					if (uIPanel.renderQuenes.size > 0)
					{
						for (int k = uIPanel.renderQuenes.size - 1; k >= 0; k--)
						{
							if (uIPanel.renderQuenes[k] != null)
							{
								uIPanel.renderQuenes[k].OnUpdate(uIPanel.startingRenderQueue, uIPanel.drawCalls.size, false);
								num += uIPanel.renderQuenes[k].GetDrawCalls + uIPanel.renderQuenes[k].mEffectDepth + 1;
							}
							else
							{
								uIPanel.renderQuenes.RemoveAt(k);
							}
						}
					}
				}
				else if (uIPanel.renderQueue == UIPanel.RenderQueue.StartAt)
				{
					uIPanel.UpdateDrawCalls();
					if (uIPanel.drawCalls.size != 0)
					{
						num = Mathf.Max(num, uIPanel.startingRenderQueue + uIPanel.drawCalls.size);
					}
				}
				else
				{
					uIPanel.UpdateDrawCalls();
					if (uIPanel.drawCalls.size != 0)
					{
						num = Mathf.Max(num, uIPanel.startingRenderQueue + 1);
					}
				}
			}
		}
	}

	private void UpdateSelf()
	{
		this.mUpdateTime = RealTime.time;
		this.UpdateTransformMatrix();
		this.UpdateLayers();
		this.UpdateWidgets();
		if (this.mRebuild)
		{
			this.mRebuild = false;
			this.FillAllDrawCalls();
		}
		else
		{
			int i = 0;
			while (i < this.drawCalls.size)
			{
				UIDrawCall uIDrawCall = this.drawCalls.buffer[i];
				if (uIDrawCall.isDirty && !this.FillDrawCall(uIDrawCall))
				{
					UIDrawCall.Destroy(uIDrawCall);
					this.drawCalls.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}
	}

	public void SortWidgets()
	{
		this.mSortWidgets = false;
		this.widgets.Sort(new BetterList<UIWidget>.CompareFunc(UIWidget.PanelCompareFunc));
	}

	private void FillAllDrawCalls()
	{
		for (int i = 0; i < this.drawCalls.size; i++)
		{
			UIDrawCall.Destroy(this.drawCalls.buffer[i]);
		}
		this.drawCalls.Clear();
		Material material = null;
		Texture texture = null;
		Shader shader = null;
		UIDrawCall uIDrawCall = null;
		if (this.mSortWidgets)
		{
			this.SortWidgets();
		}
		for (int j = 0; j < this.widgets.size; j++)
		{
			UIWidget uIWidget = this.widgets.buffer[j];
			if (uIWidget.isVisible && uIWidget.hasVertices)
			{
				Material material2 = uIWidget.material;
				Texture mainTexture = uIWidget.mainTexture;
				Shader shader2 = uIWidget.shader;
				if (material != material2 || texture != mainTexture || shader != shader2)
				{
					if (uIDrawCall != null && uIDrawCall.verts.size != 0)
					{
						this.drawCalls.Add(uIDrawCall);
						uIDrawCall.UpdateGeometry();
						uIDrawCall = null;
					}
					material = material2;
					texture = mainTexture;
					shader = shader2;
				}
				if (material != null || shader != null || texture != null)
				{
					if (uIDrawCall == null)
					{
						uIDrawCall = UIDrawCall.Create(this, material, texture, shader);
						uIDrawCall.depthStart = uIWidget.depth;
						uIDrawCall.depthEnd = uIDrawCall.depthStart;
						uIDrawCall.panel = this;
					}
					else
					{
						int depth = uIWidget.depth;
						if (depth < uIDrawCall.depthStart)
						{
							uIDrawCall.depthStart = depth;
						}
						if (depth > uIDrawCall.depthEnd)
						{
							uIDrawCall.depthEnd = depth;
						}
					}
					uIWidget.drawCall = uIDrawCall;
					if (this.generateNormals)
					{
						uIWidget.WriteToBuffers(uIDrawCall.verts, uIDrawCall.uvs, uIDrawCall.cols, uIDrawCall.norms, uIDrawCall.tans);
					}
					else
					{
						uIWidget.WriteToBuffers(uIDrawCall.verts, uIDrawCall.uvs, uIDrawCall.cols, null, null);
					}
				}
			}
			else
			{
				uIWidget.drawCall = null;
			}
		}
		if (uIDrawCall != null && uIDrawCall.verts.size != 0)
		{
			this.drawCalls.Add(uIDrawCall);
			uIDrawCall.UpdateGeometry();
		}
	}

	private bool FillDrawCall(UIDrawCall dc)
	{
		if (dc != null)
		{
			dc.isDirty = false;
			int i = 0;
			while (i < this.widgets.size)
			{
				UIWidget uIWidget = this.widgets[i];
				if (uIWidget == null)
				{
					this.widgets.RemoveAt(i);
				}
				else
				{
					if (uIWidget.drawCall == dc)
					{
						if (uIWidget.isVisible && uIWidget.hasVertices)
						{
							if (this.generateNormals)
							{
								uIWidget.WriteToBuffers(dc.verts, dc.uvs, dc.cols, dc.norms, dc.tans);
							}
							else
							{
								uIWidget.WriteToBuffers(dc.verts, dc.uvs, dc.cols, null, null);
							}
						}
						else
						{
							uIWidget.drawCall = null;
						}
					}
					i++;
				}
			}
			if (dc.verts.size != 0)
			{
				dc.UpdateGeometry();
				return true;
			}
		}
		return false;
	}

	private void UpdateDrawCalls()
	{
		Transform cachedTransform = base.cachedTransform;
		Vector4 zero = Vector4.zero;
		bool usedForUI = this.usedForUI;
		if (this.clipping != UIDrawCall.Clipping.None)
		{
			Vector4 finalClipRegion = this.finalClipRegion;
			zero.x = finalClipRegion.x;
			zero.y = finalClipRegion.y;
			zero.z = finalClipRegion.z * 0.5f;
			zero.w = finalClipRegion.w * 0.5f;
		}
		if (zero.z == 0f)
		{
			zero.z = (float)ResolutionConstrain.Instance.width * 0.5f;
		}
		if (zero.w == 0f)
		{
			zero.w = (float)ResolutionConstrain.Instance.height * 0.5f;
		}
		if (this.halfPixelOffset)
		{
			zero.x -= 0.5f;
			zero.y += 0.5f;
		}
		Vector3 vector;
		if (usedForUI)
		{
			Transform parent = base.cachedTransform.parent;
			vector = base.cachedTransform.localPosition;
			if (parent != null)
			{
				float num = Mathf.Round(vector.x);
				float num2 = Mathf.Round(vector.y);
				zero.x += vector.x - num;
				zero.y += vector.y - num2;
				vector.x = num;
				vector.y = num2;
				vector = parent.TransformPoint(vector);
			}
			vector += this.drawCallOffset;
		}
		else
		{
			vector = cachedTransform.position;
		}
		Quaternion rotation = cachedTransform.rotation;
		Vector3 lossyScale = cachedTransform.lossyScale;
		for (int i = 0; i < this.drawCalls.size; i++)
		{
			UIDrawCall uIDrawCall = this.drawCalls.buffer[i];
			Transform cachedTransform2 = uIDrawCall.cachedTransform;
			cachedTransform2.position = vector;
			cachedTransform2.rotation = rotation;
			cachedTransform2.localScale = lossyScale;
			uIDrawCall.renderQueue = ((this.renderQueue != UIPanel.RenderQueue.Explicit) ? (this.startingRenderQueue + i) : this.startingRenderQueue);
			uIDrawCall.clipping = this.clipping;
			uIDrawCall.clipRange = zero;
			uIDrawCall.clipSoftness = this.mClipSoftness;
			uIDrawCall.alwaysOnScreen = (this.alwaysOnScreen && (this.mClipping == UIDrawCall.Clipping.None || this.mClipping == UIDrawCall.Clipping.ConstrainButDontClip));
			uIDrawCall.sortingOrder = this.mSortingOrder;
		}
	}

	private void UpdateLayers()
	{
		if (this.mLayer != base.cachedGameObject.layer)
		{
			this.mLayer = this.mGo.layer;
			UICamera uICamera = UICamera.FindCameraForLayer(this.mLayer);
			this.mCam = ((!(uICamera != null)) ? NGUITools.FindCameraForLayer(this.mLayer) : uICamera.cachedCamera);
			NGUITools.SetChildLayer(base.cachedTransform, this.mLayer);
			for (int i = 0; i < this.drawCalls.size; i++)
			{
				this.drawCalls.buffer[i].gameObject.layer = this.mLayer;
			}
		}
	}

	private void UpdateWidgets()
	{
		bool flag = !this.cullWhileDragging && this.mCullTime > this.mUpdateTime;
		bool flag2 = false;
		if (this.mForced != flag)
		{
			this.mForced = flag;
			this.mResized = true;
		}
		bool clipsChildren = this.clipsChildren;
		int i = 0;
		int size = this.widgets.size;
		while (i < size)
		{
			UIWidget uIWidget = this.widgets.buffer[i];
			if (uIWidget.panel == this && uIWidget.enabled)
			{
				int frameCount = Time.frameCount;
				if (uIWidget.UpdateTransform(frameCount) || this.mResized)
				{
					bool visibleByAlpha = flag || uIWidget.CalculateCumulativeAlpha(frameCount) > 0.001f;
					uIWidget.UpdateVisibility(visibleByAlpha, flag || (!clipsChildren && !uIWidget.hideIfOffScreen) || this.IsVisible(uIWidget));
				}
				if (uIWidget.UpdateGeometry(frameCount))
				{
					flag2 = true;
					if (!this.mRebuild)
					{
						if (uIWidget.drawCall != null)
						{
							uIWidget.drawCall.isDirty = true;
						}
						else
						{
							this.FindDrawCall(uIWidget);
						}
					}
				}
			}
			i++;
		}
		if (flag2 && this.onGeometryUpdated != null)
		{
			this.onGeometryUpdated();
		}
		this.mResized = false;
	}

	public UIDrawCall FindDrawCall(UIWidget w)
	{
		Material material = w.material;
		Texture mainTexture = w.mainTexture;
		int depth = w.depth;
		for (int i = 0; i < this.drawCalls.size; i++)
		{
			UIDrawCall uIDrawCall = this.drawCalls.buffer[i];
			int num = (i != 0) ? (this.drawCalls.buffer[i - 1].depthEnd + 1) : -2147483648;
			int num2 = (i + 1 != this.drawCalls.size) ? (this.drawCalls.buffer[i + 1].depthStart - 1) : 2147483647;
			if (num <= depth && num2 >= depth)
			{
				if (uIDrawCall.baseMaterial == material && uIDrawCall.mainTexture == mainTexture)
				{
					if (w.isVisible)
					{
						w.drawCall = uIDrawCall;
						if (w.hasVertices)
						{
							uIDrawCall.isDirty = true;
						}
						return uIDrawCall;
					}
				}
				else
				{
					this.mRebuild = true;
				}
				return null;
			}
		}
		this.mRebuild = true;
		return null;
	}

	public void AddWidget(UIWidget w)
	{
		if (this.widgets.size == 0)
		{
			this.widgets.Add(w);
		}
		else if (this.mSortWidgets)
		{
			this.widgets.Add(w);
			this.SortWidgets();
		}
		else if (UIWidget.PanelCompareFunc(w, this.widgets[0]) == -1)
		{
			this.widgets.Insert(0, w);
		}
		else
		{
			int i = this.widgets.size;
			while (i > 0)
			{
				if (UIWidget.PanelCompareFunc(w, this.widgets[--i]) != -1)
				{
					this.widgets.Insert(i + 1, w);
					break;
				}
			}
		}
		this.FindDrawCall(w);
	}

	public void RemoveWidget(UIWidget w)
	{
		if (this.widgets.Remove(w) && w.drawCall != null)
		{
			int depth = w.depth;
			if (depth == w.drawCall.depthStart || depth == w.drawCall.depthEnd)
			{
				this.mRebuild = true;
			}
			w.drawCall.isDirty = true;
			w.drawCall = null;
		}
	}

	public void Refresh()
	{
		this.mRebuild = true;
		if (UIPanel.list.size > 0)
		{
			UIPanel.list[0].LateUpdate();
		}
	}

	public virtual Vector3 CalculateConstrainOffset(Vector2 min, Vector2 max)
	{
		Vector4 finalClipRegion = this.finalClipRegion;
		float num = finalClipRegion.z * 0.5f;
		float num2 = finalClipRegion.w * 0.5f;
		Vector2 zero = Vector2.zero;
		zero.x = min.x;
		zero.y = min.y;
		Vector2 minRect = zero;
		zero.x = max.x;
		zero.y = max.y;
		Vector2 maxRect = zero;
		zero.x = finalClipRegion.x - num;
		zero.y = finalClipRegion.y - num2;
		Vector2 minArea = zero;
		zero.x = finalClipRegion.x + num;
		zero.y = finalClipRegion.y + num2;
		Vector2 maxArea = zero;
		if (this.clipping == UIDrawCall.Clipping.SoftClip)
		{
			minArea.x += this.clipSoftness.x;
			minArea.y += this.clipSoftness.y;
			maxArea.x -= this.clipSoftness.x;
			maxArea.y -= this.clipSoftness.y;
		}
		return NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
	}

	public bool ConstrainTargetToBounds(Transform target, ref Bounds targetBounds, bool immediate)
	{
		Vector3 b = this.CalculateConstrainOffset(targetBounds.min, targetBounds.max);
		if (b.sqrMagnitude > 0f)
		{
			if (immediate)
			{
				target.localPosition += b;
				targetBounds.center += b;
				SpringPosition component = target.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			else
			{
				SpringPosition springPosition = SpringPosition.Begin(target.gameObject, target.localPosition + b, 13f);
				springPosition.ignoreTimeScale = true;
				springPosition.worldSpace = false;
			}
			return true;
		}
		return false;
	}

	public bool ConstrainTargetToBounds(Transform target, bool immediate)
	{
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.cachedTransform, target);
		return this.ConstrainTargetToBounds(target, ref bounds, immediate);
	}

	public static UIPanel Find(Transform trans)
	{
		return UIPanel.Find(trans, false, -1);
	}

	public static UIPanel Find(Transform trans, bool createIfMissing)
	{
		return UIPanel.Find(trans, createIfMissing, -1);
	}

	public static UIPanel Find(Transform trans, bool createIfMissing, int layer)
	{
		UIPanel uIPanel = null;
		while (uIPanel == null && trans != null)
		{
			uIPanel = trans.GetComponent<UIPanel>();
			if (uIPanel != null)
			{
				return uIPanel;
			}
			if (trans.parent == null)
			{
				break;
			}
			trans = trans.parent;
		}
		return (!createIfMissing) ? null : NGUITools.CreateUI(trans, false, layer);
	}

	private Vector2 GetWindowSize()
	{
		UIRoot root = base.root;
		Vector2 vector = Vector2.zero;
		vector.x = (float)Screen.width;
		vector.y = (float)Screen.height;
		if (root != null)
		{
			vector *= root.GetPixelSizeAdjustment(Screen.height);
		}
		return vector;
	}

	public Vector2 GetViewSize()
	{
		bool flag = this.mClipping != UIDrawCall.Clipping.None;
		Vector2 vector = Vector2.zero;
		if (flag)
		{
			vector.x = this.mClipRange.z;
			vector.y = this.mClipRange.w;
		}
		else
		{
			vector.x = (float)Screen.width;
			vector.y = (float)Screen.height;
		}
		if (!flag)
		{
			UIRoot root = base.root;
			if (root != null)
			{
				vector *= root.GetPixelSizeAdjustment(Screen.height);
			}
		}
		return vector;
	}
}
