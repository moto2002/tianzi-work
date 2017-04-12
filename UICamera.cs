using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Event System (UICamera)"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour
{
	public enum ControlScheme
	{
		Mouse,
		Touch,
		Controller
	}

	public enum ClickNotification
	{
		None,
		Always,
		BasedOnDelta
	}

	public class MouseOrTouch
	{
		public Vector2 pos;

		public Vector2 lastPos;

		public Vector2 delta;

		public Vector2 totalDelta;

		public Camera pressedCam;

		public GameObject last;

		public GameObject current;

		public GameObject pressed;

		public GameObject dragged;

		public float clickTime;

		public UICamera.ClickNotification clickNotification = UICamera.ClickNotification.Always;

		public bool touchBegan = true;

		public bool pressStarted;

		public bool dragStarted;

		public int id;

		public float pressure;

		public float deltaPressure;

		public float maximumPossiblePressure;

		public TouchPhase phase;

		public float deltaTime;

		public float eventTime;

		public bool enterDispatched;

		public bool exitDispatched;
	}

	public class PressureTouch
	{
		public int id;

		public Vector2 pos;

		public float pressure;

		public float deltaPressure;

		public float maximumPossiblePressure;

		public TouchPhase phase;

		public float deltaTime;

		public float eventTime;
	}

	public enum EventType
	{
		World,
		UI,
		Unity2D
	}

	private struct DepthEntry
	{
		public int depth;

		public RaycastHit hit;
	}

	public delegate void OnScreenResize();

	public delegate void OnCustomInput();

	public delegate void OnNotifyUIEvent(GameObject go, string funcName, object obj);

	private delegate void OnForceTouchCallbackFromNativeDelegate(int touchId, float pressure, float maximumPossiblePressure, float pos_x, float pos_y, TouchPhase phase);

	public static BetterList<UICamera> list = new BetterList<UICamera>();

	public static List<UICamera.OnScreenResize> mOnScreenReSize = new List<UICamera.OnScreenResize>();

	public UICamera.EventType eventType = UICamera.EventType.UI;

	public LayerMask eventReceiverMask = -1;

	public bool debug;

	public bool useMouse = true;

	public bool useTouch = true;

	public bool allowMultiTouch = true;

	public bool useKeyboard = true;

	public bool useController = true;

	public bool stickyTooltip = true;

	public float tooltipDelay = 1f;

	public float mouseDragThreshold = 4f;

	public float mouseClickThreshold = 10f;

	public float touchDragThreshold = 40f;

	public float touchClickThreshold = 40f;

	public float rangeDistance = -1f;

	public string scrollAxisName = "Mouse ScrollWheel";

	public string verticalAxisName = "Vertical";

	public string horizontalAxisName = "Horizontal";

	public KeyCode submitKey0 = KeyCode.Return;

	public KeyCode submitKey1 = KeyCode.JoystickButton0;

	public KeyCode cancelKey0 = KeyCode.Escape;

	public KeyCode cancelKey1 = KeyCode.JoystickButton1;

	public static UICamera.OnCustomInput onCustomInput;

	public static bool showTooltips = true;

	public static Vector2 lastTouchPosition = Vector2.zero;

	public static RaycastHit lastHit;

	public static UICamera current = null;

	public static Camera currentCamera = null;

	public static UICamera.ControlScheme currentScheme = UICamera.ControlScheme.Mouse;

	public static int currentTouchID = -1;

	public static KeyCode currentKey = KeyCode.None;

	public static UICamera.MouseOrTouch currentTouch = null;

	public static bool inputHasFocus = false;

	public static GameObject genericEventHandler;

	public static GameObject fallThrough;

	private static GameObject mCurrentSelection = null;

	private static UIInput mCurrentInput = null;

	private static GameObject mNextSelection = null;

	private static UICamera.ControlScheme mNextScheme = UICamera.ControlScheme.Controller;

	private static UICamera.MouseOrTouch[] mMouse = new UICamera.MouseOrTouch[]
	{
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch()
	};

	private static GameObject mHover;

	public static UICamera.MouseOrTouch controller = new UICamera.MouseOrTouch();

	private static float mNextEvent = 0f;

	private static Dictionary<int, UICamera.MouseOrTouch> mTouches = new Dictionary<int, UICamera.MouseOrTouch>();

	private static int mWidth = 0;

	private static int mHeight = 0;

	private GameObject mTooltip;

	private Camera mCam;

	private float mTooltipTime;

	private float mNextRaycast;

	public static bool isDragging = false;

	public static GameObject hoveredObject;

	private static int MAX_TOUCHES_COUNT = 16;

	private static bool is3DTouchSupported = false;

	private static int s_curTouchesCount = 0;

	private static int s_preTouchesCount = 0;

	private static UICamera.PressureTouch[] s_curTouches = null;

	private static UICamera.PressureTouch[] s_preTouches = null;

	private static UICamera.DepthEntry mHit = default(UICamera.DepthEntry);

	private static BetterList<UICamera.DepthEntry> mHits = new BetterList<UICamera.DepthEntry>();

	private static RaycastHit mEmpty = default(RaycastHit);

	private static Plane m2DPlane = new Plane(Vector3.back, 0f);

	private static bool mNotifying = false;

	public static UICamera.OnNotifyUIEvent moNotifyLisnter = null;

	[Obsolete("Use new OnDragStart / OnDragOver / OnDragOut / OnDragEnd events instead")]
	public bool stickyPress
	{
		get
		{
			return true;
		}
	}

	public static Ray currentRay
	{
		get
		{
			return (!(UICamera.currentCamera != null) || UICamera.currentTouch == null) ? default(Ray) : UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
		}
	}

	private bool handlesEvents
	{
		get
		{
			return UICamera.eventHandler == this;
		}
	}

	public Camera cachedCamera
	{
		get
		{
			if (this.mCam == null)
			{
				this.mCam = base.camera;
			}
			return this.mCam;
		}
	}

	public static int touchesCount
	{
		get
		{
			if (UICamera.is3DTouchSupported)
			{
				return UICamera.s_curTouchesCount;
			}
			UnityEngine.Debug.LogWarning("该设备不支持3DTouch，请勿调用S3DTouchInput.touchesCount。");
			return 0;
		}
	}

	public static GameObject selectedObject
	{
		get
		{
			return UICamera.mCurrentSelection;
		}
		set
		{
			UICamera.SetSelection(value, UICamera.currentScheme);
		}
	}

	public static int touchCount
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<int, UICamera.MouseOrTouch> keyValuePair in UICamera.mTouches)
			{
				if (keyValuePair.Value.pressed != null)
				{
					num++;
				}
			}
			for (int i = 0; i < UICamera.mMouse.Length; i++)
			{
				if (UICamera.mMouse[i].pressed != null)
				{
					num++;
				}
			}
			if (UICamera.controller.pressed != null)
			{
				num++;
			}
			return num;
		}
	}

	public static int dragCount
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<int, UICamera.MouseOrTouch> keyValuePair in UICamera.mTouches)
			{
				if (keyValuePair.Value.dragged != null)
				{
					num++;
				}
			}
			for (int i = 0; i < UICamera.mMouse.Length; i++)
			{
				if (UICamera.mMouse[i].dragged != null)
				{
					num++;
				}
			}
			if (UICamera.controller.dragged != null)
			{
				num++;
			}
			return num;
		}
	}

	public static Camera mainCamera
	{
		get
		{
			UICamera eventHandler = UICamera.eventHandler;
			return (!(eventHandler != null)) ? null : eventHandler.cachedCamera;
		}
	}

	public static UICamera eventHandler
	{
		get
		{
			for (int i = 0; i < UICamera.list.size; i++)
			{
				UICamera uICamera = UICamera.list.buffer[i];
				if (!(uICamera == null) && uICamera.enabled && NGUITools.GetActive(uICamera.gameObject))
				{
					return uICamera;
				}
			}
			return null;
		}
	}

	public static bool IsPressed(GameObject go)
	{
		for (int i = 0; i < 3; i++)
		{
			if (UICamera.mMouse[i].pressed == go)
			{
				return true;
			}
		}
		foreach (KeyValuePair<int, UICamera.MouseOrTouch> keyValuePair in UICamera.mTouches)
		{
			if (keyValuePair.Value.pressed == go)
			{
				return true;
			}
		}
		return UICamera.controller.pressed == go;
	}

	protected static void SetSelection(GameObject go, UICamera.ControlScheme scheme)
	{
		if (UICamera.mNextSelection != null)
		{
			UICamera.mNextSelection = go;
		}
		else if (UICamera.mCurrentSelection != go)
		{
			UICamera.mNextSelection = go;
			UICamera.mNextScheme = scheme;
			if (UICamera.list.size > 0)
			{
				UICamera uICamera = (!(UICamera.mNextSelection != null)) ? UICamera.list[0] : UICamera.FindCameraForLayer(UICamera.mNextSelection.layer);
				if (uICamera != null)
				{
					uICamera.StartCoroutine(uICamera.ChangeSelection());
				}
			}
		}
	}

	private void GetCurrentSelectionInput()
	{
		if (UICamera.mCurrentSelection != null)
		{
			UICamera.mCurrentInput = UICamera.mCurrentSelection.GetComponent<UIInput>();
		}
	}

	[DebuggerHidden]
	private IEnumerator ChangeSelection()
	{
		UICamera.<ChangeSelection>c__Iterator17 <ChangeSelection>c__Iterator = new UICamera.<ChangeSelection>c__Iterator17();
		<ChangeSelection>c__Iterator.<>f__this = this;
		return <ChangeSelection>c__Iterator;
	}

	private static int CompareFunc(UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth)
		{
			return 1;
		}
		if (a.cachedCamera.depth > b.cachedCamera.depth)
		{
			return -1;
		}
		return 0;
	}

	public static bool Raycast(Vector3 inPos, out RaycastHit hit)
	{
		for (int i = 0; i < UICamera.list.size; i++)
		{
			UICamera uICamera = UICamera.list.buffer[i];
			if (uICamera.enabled && NGUITools.GetActive(uICamera.gameObject))
			{
				UICamera.currentCamera = uICamera.cachedCamera;
				Vector3 point = UICamera.currentCamera.ScreenToViewportPoint(inPos);
				if (!float.IsNaN(point.x) && !float.IsNaN(point.y))
				{
					if (point.x >= 0f && point.x <= 1f && point.y >= 0f && point.y <= 1f)
					{
						Ray ray = UICamera.currentCamera.ScreenPointToRay(inPos);
						int layerMask = UICamera.currentCamera.cullingMask & uICamera.eventReceiverMask;
						float distance = (uICamera.rangeDistance <= 0f) ? (UICamera.currentCamera.farClipPlane - UICamera.currentCamera.nearClipPlane) : uICamera.rangeDistance;
						if (uICamera.eventType == UICamera.EventType.World)
						{
							if (Physics.Raycast(ray, out hit, distance, layerMask))
							{
								UICamera.hoveredObject = hit.collider.gameObject;
								return true;
							}
						}
						else if (uICamera.eventType == UICamera.EventType.UI)
						{
							RaycastHit[] array = Physics.RaycastAll(ray, distance, layerMask);
							if (array.Length > 1)
							{
								int j = 0;
								while (j < array.Length)
								{
									GameObject gameObject = array[j].collider.gameObject;
									UIWidget component = gameObject.GetComponent<UIWidget>();
									if (component != null)
									{
										if (component.isVisible)
										{
											if (component.hitCheck == null || component.hitCheck(array[j].point))
											{
												goto IL_20A;
											}
										}
									}
									else
									{
										UIRect uIRect = NGUITools.FindInParents<UIRect>(gameObject);
										if (!(uIRect != null) || uIRect.finalAlpha >= 0.001f)
										{
											goto IL_20A;
										}
									}
									IL_256:
									j++;
									continue;
									IL_20A:
									UICamera.mHit.depth = NGUITools.CalculateRaycastDepth(gameObject);
									if (UICamera.mHit.depth != 2147483647)
									{
										UICamera.mHit.hit = array[j];
										UICamera.mHits.Add(UICamera.mHit);
										goto IL_256;
									}
									goto IL_256;
								}
								UICamera.mHits.Sort((UICamera.DepthEntry r1, UICamera.DepthEntry r2) => r2.depth.CompareTo(r1.depth));
								for (int k = 0; k < UICamera.mHits.size; k++)
								{
									if (UICamera.IsVisible(ref UICamera.mHits.buffer[k]))
									{
										hit = UICamera.mHits[k].hit;
										UICamera.hoveredObject = hit.collider.gameObject;
										UICamera.mHits.Clear();
										return true;
									}
								}
								UICamera.mHits.Clear();
							}
							else if (array.Length == 1)
							{
								Collider collider = array[0].collider;
								UIWidget component2 = collider.GetComponent<UIWidget>();
								if (component2 != null)
								{
									if (!component2.isVisible)
									{
										goto IL_455;
									}
									if (component2.hitCheck != null && !component2.hitCheck(array[0].point))
									{
										goto IL_455;
									}
								}
								else
								{
									UIRect uIRect2 = NGUITools.FindInParents<UIRect>(collider.gameObject);
									if (uIRect2 != null && uIRect2.finalAlpha < 0.001f)
									{
										goto IL_455;
									}
								}
								if (UICamera.IsVisible(ref array[0]))
								{
									hit = array[0];
									UICamera.hoveredObject = hit.collider.gameObject;
									return true;
								}
							}
						}
						else if (uICamera.eventType == UICamera.EventType.Unity2D)
						{
							if (UICamera.m2DPlane.Raycast(ray, out distance))
							{
								Collider2D collider2D = Physics2D.OverlapPoint(ray.GetPoint(distance), layerMask);
								if (collider2D)
								{
									hit = UICamera.lastHit;
									hit.point = point;
									UICamera.hoveredObject = collider2D.gameObject;
									return true;
								}
							}
						}
					}
				}
			}
			IL_455:;
		}
		hit = UICamera.mEmpty;
		return false;
	}

	private static bool IsVisible(ref RaycastHit hit)
	{
		UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(hit.collider.gameObject);
		return uIPanel == null || uIPanel.IsVisible(hit.point);
	}

	private static bool IsVisible(ref UICamera.DepthEntry de)
	{
		UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(de.hit.collider.gameObject);
		return uIPanel == null || uIPanel.IsVisible(de.hit.point);
	}

	public static bool IsHighlighted(GameObject go)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Mouse)
		{
			return UICamera.hoveredObject == go;
		}
		return UICamera.currentScheme == UICamera.ControlScheme.Controller && UICamera.selectedObject == go;
	}

	public static UICamera FindCameraForLayer(int layer)
	{
		int num = 1 << layer;
		for (int i = 0; i < UICamera.list.size; i++)
		{
			UICamera uICamera = UICamera.list.buffer[i];
			Camera cachedCamera = uICamera.cachedCamera;
			if (cachedCamera != null && (cachedCamera.cullingMask & num) != 0)
			{
				return uICamera;
			}
		}
		return null;
	}

	private static int GetDirection(KeyCode up, KeyCode down)
	{
		if (Input.GetKeyDown(up))
		{
			return 1;
		}
		if (Input.GetKeyDown(down))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		if (Input.GetKeyDown(up0) || Input.GetKeyDown(up1))
		{
			return 1;
		}
		if (Input.GetKeyDown(down0) || Input.GetKeyDown(down1))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(string axis)
	{
		float time = RealTime.time;
		if (UICamera.mNextEvent < time && !string.IsNullOrEmpty(axis))
		{
			float axis2 = Input.GetAxis(axis);
			if (axis2 > 0.75f)
			{
				UICamera.mNextEvent = time + 0.25f;
				return 1;
			}
			if (axis2 < -0.75f)
			{
				UICamera.mNextEvent = time + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	public static void Notify(GameObject go, string funcName, object obj)
	{
		if (UICamera.mNotifying)
		{
			return;
		}
		UICamera.mNotifying = true;
		try
		{
			if (NGUITools.GetActive(go))
			{
				go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
				if (UICamera.genericEventHandler != null && UICamera.genericEventHandler != go)
				{
					UICamera.genericEventHandler.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
				}
				if (UICamera.moNotifyLisnter != null)
				{
					UICamera.moNotifyLisnter(go, funcName, obj);
				}
			}
			UICamera.mNotifying = false;
		}
		catch (Exception ex)
		{
			UICamera.mNotifying = false;
			LogSystem.LogError(new object[]
			{
				"Notify",
				ex.ToString()
			});
		}
	}

	public static void SetNotifyUIEvent(UICamera.OnNotifyUIEvent onNotifyEvent)
	{
		UICamera.moNotifyLisnter = onNotifyEvent;
	}

	public static UICamera.MouseOrTouch GetMouse(int button)
	{
		return UICamera.mMouse[button];
	}

	public static UICamera.MouseOrTouch GetTouch(int id)
	{
		UICamera.MouseOrTouch mouseOrTouch = null;
		if (id < 0)
		{
			return UICamera.GetMouse(-id - 1);
		}
		if (!UICamera.mTouches.TryGetValue(id, out mouseOrTouch))
		{
			mouseOrTouch = new UICamera.MouseOrTouch();
			mouseOrTouch.touchBegan = true;
			UICamera.mTouches.Add(id, mouseOrTouch);
		}
		return mouseOrTouch;
	}

	public static void RemoveTouch(int id)
	{
		UICamera.mTouches.Remove(id);
	}

	private void Awake()
	{
		UICamera.is3DTouchSupported = UICamera.Check3DTouchFeatureSupported();
		LogSystem.Log(new object[]
		{
			"is3DTouchSupported--->",
			UICamera.is3DTouchSupported
		});
		if (UICamera.is3DTouchSupported)
		{
			UICamera.s_curTouches = new UICamera.PressureTouch[UICamera.MAX_TOUCHES_COUNT];
			for (int i = 0; i < UICamera.MAX_TOUCHES_COUNT; i++)
			{
				UICamera.s_curTouches[i] = new UICamera.PressureTouch();
			}
			UICamera.s_preTouches = new UICamera.PressureTouch[UICamera.MAX_TOUCHES_COUNT];
			for (int j = 0; j < UICamera.MAX_TOUCHES_COUNT; j++)
			{
				UICamera.s_preTouches[j] = new UICamera.PressureTouch();
			}
			UICamera.RegistForceTouchCallback();
		}
		UICamera.mWidth = ResolutionConstrain.Instance.width;
		UICamera.mHeight = ResolutionConstrain.Instance.height;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WP8Player || Application.platform == RuntimePlatform.BB10Player)
		{
			this.useMouse = false;
			this.useTouch = true;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				this.useKeyboard = false;
				this.useController = false;
			}
		}
		else if (Application.platform == RuntimePlatform.PS3 || Application.platform == RuntimePlatform.XBOX360)
		{
			this.useMouse = false;
			this.useTouch = false;
			this.useKeyboard = false;
			this.useController = true;
		}
		UICamera.mMouse[0].pos.x = Input.mousePosition.x;
		UICamera.mMouse[0].pos.y = Input.mousePosition.y;
		for (int k = 1; k < 3; k++)
		{
			UICamera.mMouse[k].pos = UICamera.mMouse[0].pos;
			UICamera.mMouse[k].lastPos = UICamera.mMouse[0].pos;
		}
		UICamera.lastTouchPosition = UICamera.mMouse[0].pos;
	}

	private void OnEnable()
	{
		UICamera.list.Add(this);
		UICamera.list.Sort(new BetterList<UICamera>.CompareFunc(UICamera.CompareFunc));
	}

	private void OnDisable()
	{
		UICamera.list.Remove(this);
	}

	private void Start()
	{
		if (this.eventType != UICamera.EventType.World && this.cachedCamera.transparencySortMode != TransparencySortMode.Orthographic)
		{
			this.cachedCamera.transparencySortMode = TransparencySortMode.Orthographic;
		}
		if (Application.isPlaying)
		{
			this.cachedCamera.eventMask = 0;
		}
		if (this.handlesEvents)
		{
			NGUIDebug.debugRaycast = this.debug;
		}
		JoyStickControlor.GetInstance().AddListener(new JoyStickControlor.OnInputCode(this.OnStickInput));
	}

	private void Update()
	{
		if (!Application.isPlaying || !this.handlesEvents)
		{
			return;
		}
		UICamera.current = this;
		if (ResolutionConstrain.Instance.width != UICamera.mWidth || ResolutionConstrain.Instance.height != UICamera.mHeight)
		{
			UICamera.mWidth = ResolutionConstrain.Instance.width;
			UICamera.mHeight = ResolutionConstrain.Instance.height;
			if (UICamera.mOnScreenReSize.Count > 0)
			{
				int count = UICamera.mOnScreenReSize.Count;
				for (int i = 0; i < count; i++)
				{
					if (UICamera.mOnScreenReSize[i] != null)
					{
						UICamera.mOnScreenReSize[i]();
					}
				}
			}
		}
		if (this.useTouch)
		{
			this.ProcessTouches();
		}
		else if (this.useMouse)
		{
			this.ProcessMouse();
		}
		if (UICamera.is3DTouchSupported)
		{
			this.SwapTouchBuffers();
		}
		if (UICamera.onCustomInput != null)
		{
			UICamera.onCustomInput();
		}
		if (this.useMouse && UICamera.mCurrentSelection != null)
		{
			if (this.cancelKey0 != KeyCode.None && Input.GetKeyDown(this.cancelKey0))
			{
				UICamera.currentScheme = UICamera.ControlScheme.Controller;
				UICamera.currentKey = this.cancelKey0;
				UICamera.selectedObject = null;
			}
			else if (this.cancelKey1 != KeyCode.None && Input.GetKeyDown(this.cancelKey1))
			{
				UICamera.currentScheme = UICamera.ControlScheme.Controller;
				UICamera.currentKey = this.cancelKey1;
				UICamera.selectedObject = null;
			}
		}
		if (UICamera.mCurrentSelection == null)
		{
			UICamera.inputHasFocus = false;
		}
		if (UICamera.mCurrentSelection != null)
		{
			this.ProcessOthers();
		}
		if (this.useMouse && UICamera.mHover != null)
		{
			float num = string.IsNullOrEmpty(this.scrollAxisName) ? 0f : Input.GetAxis(this.scrollAxisName);
			if (num != 0f)
			{
				UICamera.Notify(UICamera.mHover, "OnScroll", num);
			}
			if (UICamera.showTooltips && this.mTooltipTime != 0f && (this.mTooltipTime < RealTime.time || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				this.mTooltip = UICamera.mHover;
				this.ShowTooltip(true);
			}
		}
		UICamera.current = null;
	}

	public void ProcessMouse()
	{
		UICamera.lastTouchPosition = Input.mousePosition;
		UICamera.mMouse[0].delta = UICamera.lastTouchPosition - UICamera.mMouse[0].pos;
		UICamera.mMouse[0].pos = UICamera.lastTouchPosition;
		bool flag = UICamera.mMouse[0].delta.sqrMagnitude > 0.001f;
		for (int i = 1; i < 3; i++)
		{
			UICamera.mMouse[i].pos = UICamera.mMouse[0].pos;
			UICamera.mMouse[i].delta = UICamera.mMouse[0].delta;
		}
		bool flag2 = false;
		bool flag3 = false;
		for (int j = 0; j < 3; j++)
		{
			if (Input.GetMouseButtonDown(j))
			{
				UICamera.currentScheme = UICamera.ControlScheme.Mouse;
				flag3 = true;
				flag2 = true;
			}
			else if (Input.GetMouseButton(j))
			{
				UICamera.currentScheme = UICamera.ControlScheme.Mouse;
				flag2 = true;
			}
		}
		if (flag2 || flag || this.mNextRaycast < RealTime.time)
		{
			this.mNextRaycast = RealTime.time + 0.02f;
			if (!UICamera.Raycast(Input.mousePosition, out UICamera.lastHit))
			{
				UICamera.hoveredObject = UICamera.fallThrough;
			}
			if (UICamera.hoveredObject == null)
			{
				UICamera.hoveredObject = UICamera.genericEventHandler;
			}
			for (int k = 0; k < 3; k++)
			{
				UICamera.mMouse[k].current = UICamera.hoveredObject;
			}
		}
		bool flag4 = UICamera.mMouse[0].last != UICamera.mMouse[0].current;
		if (flag4)
		{
			UICamera.currentScheme = UICamera.ControlScheme.Mouse;
		}
		if (flag2)
		{
			this.mTooltipTime = 0f;
		}
		else if (flag && (!this.stickyTooltip || flag4))
		{
			if (this.mTooltipTime != 0f)
			{
				this.mTooltipTime = RealTime.time + this.tooltipDelay;
			}
			else if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
		}
		if ((flag3 || !flag2) && UICamera.mHover != null && flag4)
		{
			UICamera.currentScheme = UICamera.ControlScheme.Mouse;
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			UICamera.Notify(UICamera.mHover, "OnHover", false);
			UICamera.mHover = null;
		}
		for (int l = 0; l < 3; l++)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(l);
			bool mouseButtonUp = Input.GetMouseButtonUp(l);
			if (mouseButtonDown || mouseButtonUp)
			{
				UICamera.currentScheme = UICamera.ControlScheme.Mouse;
			}
			UICamera.currentTouch = UICamera.mMouse[l];
			UICamera.currentTouchID = -1 - l;
			UICamera.currentKey = KeyCode.Mouse0 + l;
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			this.ProcessTouch(mouseButtonDown, mouseButtonUp);
			UICamera.currentKey = KeyCode.None;
		}
		UICamera.currentTouch = null;
		if (!flag2 && flag4)
		{
			UICamera.currentScheme = UICamera.ControlScheme.Mouse;
			this.mTooltipTime = RealTime.time + this.tooltipDelay;
			UICamera.mHover = UICamera.mMouse[0].current;
			UICamera.Notify(UICamera.mHover, "OnHover", true);
		}
		UICamera.mMouse[0].last = UICamera.mMouse[0].current;
		for (int m = 1; m < 3; m++)
		{
			UICamera.mMouse[m].last = UICamera.mMouse[0].last;
		}
	}

	public void ProcessTouches()
	{
		UICamera.currentScheme = UICamera.ControlScheme.Touch;
		if (UICamera.is3DTouchSupported)
		{
			for (int i = 0; i < UICamera.touchesCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				UICamera.PressureTouch pressureTouch = UICamera.s_curTouches[i];
				UICamera.currentTouchID = ((!this.allowMultiTouch) ? 1 : pressureTouch.id);
				UICamera.currentTouch = UICamera.GetTouch(UICamera.currentTouchID);
				bool flag = pressureTouch.phase == TouchPhase.Began || UICamera.currentTouch.touchBegan;
				bool flag2 = pressureTouch.phase == TouchPhase.Canceled || pressureTouch.phase == TouchPhase.Ended;
				UICamera.currentTouch.touchBegan = false;
				UICamera.currentTouch.delta = ((!flag) ? (pressureTouch.pos - UICamera.currentTouch.pos) : Vector2.zero);
				UICamera.currentTouch.pos = pressureTouch.pos;
				UICamera.currentTouch.deltaPressure = ((!flag) ? 0f : (pressureTouch.pressure - UICamera.currentTouch.pressure));
				UICamera.currentTouch.pressure = pressureTouch.pressure;
				UICamera.currentTouch.maximumPossiblePressure = pressureTouch.maximumPossiblePressure;
				if (!UICamera.Raycast(UICamera.currentTouch.pos, out UICamera.lastHit))
				{
					UICamera.hoveredObject = UICamera.fallThrough;
				}
				if (UICamera.hoveredObject == null)
				{
					UICamera.hoveredObject = UICamera.genericEventHandler;
				}
				UICamera.currentTouch.last = UICamera.currentTouch.current;
				UICamera.currentTouch.current = UICamera.hoveredObject;
				UICamera.lastTouchPosition = UICamera.currentTouch.pos;
				if (flag)
				{
					UICamera.currentTouch.pressedCam = UICamera.currentCamera;
				}
				else if (UICamera.currentTouch.pressed != null)
				{
					UICamera.currentCamera = UICamera.currentTouch.pressedCam;
				}
				if (touch.tapCount > 1)
				{
					UICamera.currentTouch.clickTime = RealTime.time;
				}
				this.ProcessTouch(flag, flag2);
				if (flag2)
				{
					UICamera.RemoveTouch(UICamera.currentTouchID);
				}
				UICamera.currentTouch.last = null;
				UICamera.currentTouch = null;
				if (!this.allowMultiTouch)
				{
					break;
				}
			}
		}
		else
		{
			for (int j = 0; j < Input.touchCount; j++)
			{
				Touch touch2 = Input.GetTouch(j);
				UICamera.currentTouchID = ((!this.allowMultiTouch) ? 1 : touch2.fingerId);
				UICamera.currentTouch = UICamera.GetTouch(UICamera.currentTouchID);
				bool flag3 = touch2.phase == TouchPhase.Began || UICamera.currentTouch.touchBegan;
				bool flag4 = touch2.phase == TouchPhase.Canceled || touch2.phase == TouchPhase.Ended;
				UICamera.currentTouch.touchBegan = false;
				UICamera.currentTouch.delta = ((!flag3) ? (touch2.position - UICamera.currentTouch.pos) : Vector2.zero);
				UICamera.currentTouch.pos = touch2.position;
				if (!UICamera.Raycast(UICamera.currentTouch.pos, out UICamera.lastHit))
				{
					UICamera.hoveredObject = UICamera.fallThrough;
				}
				if (UICamera.hoveredObject == null)
				{
					UICamera.hoveredObject = UICamera.genericEventHandler;
				}
				UICamera.currentTouch.last = UICamera.currentTouch.current;
				UICamera.currentTouch.current = UICamera.hoveredObject;
				UICamera.lastTouchPosition = UICamera.currentTouch.pos;
				if (flag3)
				{
					UICamera.currentTouch.pressedCam = UICamera.currentCamera;
				}
				else if (UICamera.currentTouch.pressed != null)
				{
					UICamera.currentCamera = UICamera.currentTouch.pressedCam;
				}
				if (touch2.tapCount > 1)
				{
					UICamera.currentTouch.clickTime = RealTime.time;
				}
				this.ProcessTouch(flag3, flag4);
				if (flag4)
				{
					UICamera.RemoveTouch(UICamera.currentTouchID);
				}
				UICamera.currentTouch.last = null;
				UICamera.currentTouch = null;
				if (!this.allowMultiTouch)
				{
					break;
				}
			}
		}
		if (Input.touchCount == 0 && this.useMouse)
		{
			this.ProcessMouse();
		}
	}

	private void ProcessFakeTouches()
	{
		bool mouseButtonDown = Input.GetMouseButtonDown(0);
		bool mouseButtonUp = Input.GetMouseButtonUp(0);
		bool mouseButton = Input.GetMouseButton(0);
		if (mouseButtonDown || mouseButtonUp || mouseButton)
		{
			UICamera.currentTouchID = 1;
			UICamera.currentTouch = UICamera.mMouse[0];
			UICamera.currentTouch.touchBegan = mouseButtonDown;
			Vector2 vector = Input.mousePosition;
			UICamera.currentTouch.delta = ((!mouseButtonDown) ? (vector - UICamera.currentTouch.pos) : Vector2.zero);
			UICamera.currentTouch.pos = vector;
			if (!UICamera.Raycast(UICamera.currentTouch.pos, out UICamera.lastHit))
			{
				UICamera.hoveredObject = UICamera.fallThrough;
			}
			if (UICamera.hoveredObject == null)
			{
				UICamera.hoveredObject = UICamera.genericEventHandler;
			}
			UICamera.currentTouch.last = UICamera.currentTouch.current;
			UICamera.currentTouch.current = UICamera.hoveredObject;
			UICamera.lastTouchPosition = UICamera.currentTouch.pos;
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			this.ProcessTouch(mouseButtonDown, mouseButtonUp);
			if (mouseButtonUp)
			{
				UICamera.RemoveTouch(UICamera.currentTouchID);
			}
			UICamera.currentTouch.last = null;
			UICamera.currentTouch = null;
		}
	}

	public void ProcessOthers()
	{
		UICamera.currentTouchID = -100;
		UICamera.currentTouch = UICamera.controller;
		UICamera.inputHasFocus = (UICamera.mCurrentSelection != null && UICamera.mCurrentInput != null);
		bool flag = false;
		bool flag2 = false;
		if (this.submitKey0 != KeyCode.None && Input.GetKeyDown(this.submitKey0))
		{
			UICamera.currentKey = this.submitKey0;
			flag = true;
		}
		if (this.submitKey1 != KeyCode.None && Input.GetKeyDown(this.submitKey1))
		{
			UICamera.currentKey = this.submitKey1;
			flag = true;
		}
		if (this.submitKey0 != KeyCode.None && Input.GetKeyUp(this.submitKey0))
		{
			UICamera.currentKey = this.submitKey0;
			flag2 = true;
		}
		if (this.submitKey1 != KeyCode.None && Input.GetKeyUp(this.submitKey1))
		{
			UICamera.currentKey = this.submitKey1;
			flag2 = true;
		}
		if (flag || flag2)
		{
			UICamera.currentScheme = UICamera.ControlScheme.Controller;
			UICamera.currentTouch.last = UICamera.currentTouch.current;
			UICamera.currentTouch.current = UICamera.mCurrentSelection;
			this.ProcessTouch(flag, flag2);
			UICamera.currentTouch.last = null;
		}
		int num = 0;
		int num2 = 0;
		if (this.useKeyboard)
		{
			if (UICamera.inputHasFocus)
			{
				num += UICamera.GetDirection(KeyCode.UpArrow, KeyCode.DownArrow);
				num2 += UICamera.GetDirection(KeyCode.RightArrow, KeyCode.LeftArrow);
			}
			else
			{
				num += UICamera.GetDirection(KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow);
				num2 += UICamera.GetDirection(KeyCode.D, KeyCode.RightArrow, KeyCode.A, KeyCode.LeftArrow);
			}
		}
		if (this.useController)
		{
			if (!string.IsNullOrEmpty(this.verticalAxisName))
			{
				num += UICamera.GetDirection(this.verticalAxisName);
			}
			if (!string.IsNullOrEmpty(this.horizontalAxisName))
			{
				num2 += UICamera.GetDirection(this.horizontalAxisName);
			}
		}
		if (num != 0)
		{
			UICamera.currentScheme = UICamera.ControlScheme.Controller;
			UICamera.Notify(UICamera.mCurrentSelection, "OnKey", (num <= 0) ? KeyCode.DownArrow : KeyCode.UpArrow);
		}
		if (num2 != 0)
		{
			UICamera.currentScheme = UICamera.ControlScheme.Controller;
			UICamera.Notify(UICamera.mCurrentSelection, "OnKey", (num2 <= 0) ? KeyCode.LeftArrow : KeyCode.RightArrow);
		}
		if (this.useKeyboard && Input.GetKeyDown(KeyCode.Tab))
		{
			UICamera.currentKey = KeyCode.Tab;
			UICamera.currentScheme = UICamera.ControlScheme.Controller;
			UICamera.Notify(UICamera.mCurrentSelection, "OnKey", KeyCode.Tab);
		}
		if (this.cancelKey0 != KeyCode.None && Input.GetKeyDown(this.cancelKey0))
		{
			UICamera.currentKey = this.cancelKey0;
			UICamera.currentScheme = UICamera.ControlScheme.Controller;
			UICamera.Notify(UICamera.mCurrentSelection, "OnKey", KeyCode.Escape);
		}
		if (this.cancelKey1 != KeyCode.None && Input.GetKeyDown(this.cancelKey1))
		{
			UICamera.currentKey = this.cancelKey1;
			UICamera.currentScheme = UICamera.ControlScheme.Controller;
			UICamera.Notify(UICamera.mCurrentSelection, "OnKey", KeyCode.Escape);
		}
		UICamera.currentTouch = null;
		UICamera.currentKey = KeyCode.None;
	}

	public void ProcessTouch(bool pressed, bool unpressed)
	{
		bool flag = UICamera.currentScheme == UICamera.ControlScheme.Mouse;
		float num = (!flag) ? this.touchDragThreshold : this.mouseDragThreshold;
		float num2 = (!flag) ? this.touchClickThreshold : this.mouseClickThreshold;
		num *= num;
		num2 *= num2;
		if (pressed)
		{
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			UICamera.currentTouch.pressStarted = true;
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
			UICamera.currentTouch.pressed = UICamera.currentTouch.current;
			UICamera.currentTouch.dragged = UICamera.currentTouch.current;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			UICamera.currentTouch.totalDelta = Vector2.zero;
			UICamera.currentTouch.dragStarted = false;
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", true);
			if (UICamera.is3DTouchSupported)
			{
				if (UICamera.currentTouch.pressure > UICamera.currentTouch.maximumPossiblePressure * 0.5f)
				{
					if (!UICamera.currentTouch.enterDispatched)
					{
						UICamera.Notify(UICamera.currentTouch.pressed, "On3DPress", true);
						UICamera.currentTouch.enterDispatched = true;
					}
				}
				else if (UICamera.currentTouch.deltaPressure < 0f && UICamera.currentTouch.enterDispatched && !UICamera.currentTouch.exitDispatched)
				{
					UICamera.Notify(UICamera.currentTouch.pressed, "On3DPress", false);
					UICamera.currentTouch.exitDispatched = true;
				}
			}
			if (UICamera.currentTouch.pressed != UICamera.mCurrentSelection)
			{
				if (this.mTooltip != null)
				{
					this.ShowTooltip(false);
				}
				UICamera.currentScheme = UICamera.ControlScheme.Touch;
				UICamera.selectedObject = null;
			}
		}
		else if (UICamera.currentTouch.pressed != null && (UICamera.currentTouch.delta.sqrMagnitude != 0f || UICamera.currentTouch.current != UICamera.currentTouch.last))
		{
			UICamera.currentTouch.totalDelta += UICamera.currentTouch.delta;
			float sqrMagnitude = UICamera.currentTouch.totalDelta.sqrMagnitude;
			bool flag2 = false;
			if (!UICamera.currentTouch.dragStarted && UICamera.currentTouch.last != UICamera.currentTouch.current)
			{
				UICamera.currentTouch.dragStarted = true;
				UICamera.currentTouch.delta = UICamera.currentTouch.totalDelta;
				UICamera.isDragging = true;
				UICamera.Notify(UICamera.currentTouch.dragged, "OnDragStart", null);
				UICamera.Notify(UICamera.currentTouch.last, "OnDragOver", UICamera.currentTouch.dragged);
				UICamera.isDragging = false;
			}
			else if (!UICamera.currentTouch.dragStarted && num < sqrMagnitude)
			{
				flag2 = true;
				UICamera.currentTouch.dragStarted = true;
				UICamera.currentTouch.delta = UICamera.currentTouch.totalDelta;
			}
			if (UICamera.currentTouch.dragStarted)
			{
				if (this.mTooltip != null)
				{
					this.ShowTooltip(false);
				}
				UICamera.isDragging = true;
				bool flag3 = UICamera.currentTouch.clickNotification == UICamera.ClickNotification.None;
				if (flag2)
				{
					UICamera.Notify(UICamera.currentTouch.dragged, "OnDragStart", null);
					UICamera.Notify(UICamera.currentTouch.current, "OnDragOver", UICamera.currentTouch.dragged);
				}
				else if (UICamera.currentTouch.last != UICamera.currentTouch.current)
				{
					UICamera.Notify(UICamera.currentTouch.last, "OnDragOut", UICamera.currentTouch.dragged);
					UICamera.Notify(UICamera.currentTouch.current, "OnDragOver", UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.dragged, "OnDrag", UICamera.currentTouch.delta);
				UICamera.currentTouch.last = UICamera.currentTouch.current;
				UICamera.isDragging = false;
				if (flag3)
				{
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				}
				else if (UICamera.currentTouch.clickNotification == UICamera.ClickNotification.BasedOnDelta && num2 < sqrMagnitude)
				{
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				}
			}
		}
		else if (UICamera.is3DTouchSupported && UICamera.currentTouch.pressure > UICamera.currentTouch.maximumPossiblePressure * 0.5f && !UICamera.currentTouch.enterDispatched)
		{
			UICamera.Notify(UICamera.currentTouch.pressed, "On3DPress", true);
			UICamera.currentTouch.enterDispatched = true;
		}
		if (unpressed)
		{
			UICamera.currentTouch.pressStarted = false;
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			if (UICamera.currentTouch.pressed != null)
			{
				if (UICamera.currentTouch.dragStarted)
				{
					UICamera.Notify(UICamera.currentTouch.last, "OnDragOut", UICamera.currentTouch.dragged);
					UICamera.Notify(UICamera.currentTouch.dragged, "OnDragEnd", null);
				}
				UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
				if (flag)
				{
					UICamera.Notify(UICamera.currentTouch.current, "OnHover", true);
				}
				UICamera.mHover = UICamera.currentTouch.current;
				if (UICamera.currentTouch.dragged == UICamera.currentTouch.current || (UICamera.currentScheme != UICamera.ControlScheme.Controller && UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && UICamera.currentTouch.totalDelta.sqrMagnitude < num))
				{
					if (UICamera.currentTouch.pressed != UICamera.mCurrentSelection)
					{
						UICamera.mNextSelection = null;
						UICamera.mCurrentSelection = UICamera.currentTouch.pressed;
						this.GetCurrentSelectionInput();
						UICamera.Notify(UICamera.currentTouch.pressed, "OnSelect", true);
					}
					else
					{
						UICamera.mNextSelection = null;
						UICamera.mCurrentSelection = UICamera.currentTouch.pressed;
						this.GetCurrentSelectionInput();
					}
					if (UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && UICamera.currentTouch.pressed == UICamera.currentTouch.current)
					{
						float time = RealTime.time;
						UICamera.Notify(UICamera.currentTouch.pressed, "OnClick", null);
						if (UICamera.currentTouch.clickTime + 0.35f > time)
						{
							UICamera.Notify(UICamera.currentTouch.pressed, "OnDoubleClick", null);
						}
						UICamera.currentTouch.clickTime = time;
					}
				}
				else if (UICamera.currentTouch.dragStarted)
				{
					UICamera.Notify(UICamera.currentTouch.current, "OnDrop", UICamera.currentTouch.dragged);
				}
				if (UICamera.is3DTouchSupported && UICamera.currentTouch.enterDispatched && !UICamera.currentTouch.exitDispatched)
				{
					UICamera.Notify(UICamera.currentTouch.pressed, "On3DPress", false);
					UICamera.currentTouch.exitDispatched = true;
				}
			}
			UICamera.currentTouch.dragStarted = false;
			UICamera.currentTouch.pressed = null;
			UICamera.currentTouch.dragged = null;
		}
	}

	public void ShowTooltip(bool val)
	{
		this.mTooltipTime = 0f;
		UICamera.Notify(this.mTooltip, "OnTooltip", val);
		if (!val)
		{
			this.mTooltip = null;
		}
	}

	private void SwapTouchBuffers()
	{
		UICamera.s_preTouchesCount = UICamera.s_curTouchesCount;
		UICamera.s_curTouchesCount = 0;
		UICamera.PressureTouch[] array = UICamera.s_preTouches;
		UICamera.s_preTouches = UICamera.s_curTouches;
		UICamera.s_curTouches = array;
	}

	private static int FindIndexOfTouchIdExist(int touchId, UICamera.PressureTouch[] touches, int length)
	{
		for (int i = 0; i < length; i++)
		{
			if (touchId == touches[i].id)
			{
				return i;
			}
		}
		return -1;
	}

	public static bool Check3DTouchFeatureSupported()
	{
		return false;
	}

	public static UICamera.PressureTouch[] GetTouches()
	{
		if (UICamera.is3DTouchSupported)
		{
			return UICamera.s_curTouches;
		}
		LogSystem.LogWarning(new object[]
		{
			"该设备不支持3DTouch，请勿调用S3DTouchInput.GetTouches()。"
		});
		return null;
	}

	private static void RegistForceTouchCallback()
	{
	}

	[MonoPInvokeCallback(typeof(UICamera.OnForceTouchCallbackFromNativeDelegate))]
	private static void OnForceTouchCallbackFromNative(int touchId, float pressure, float maximumPossiblePressure, float pos_x, float pos_y, TouchPhase phase)
	{
		int num = UICamera.FindIndexOfTouchIdExist(touchId, UICamera.s_curTouches, UICamera.s_curTouchesCount);
		if (num == -1 && UICamera.s_curTouchesCount < UICamera.MAX_TOUCHES_COUNT)
		{
			UICamera.s_curTouches[UICamera.s_curTouchesCount].id = touchId;
			UICamera.s_curTouches[UICamera.s_curTouchesCount].pos = new Vector2(pos_x * (float)Screen.width, pos_y * (float)Screen.height);
			UICamera.s_curTouches[UICamera.s_curTouchesCount].pressure = pressure;
			UICamera.s_curTouches[UICamera.s_curTouchesCount].maximumPossiblePressure = maximumPossiblePressure;
			UICamera.s_curTouches[UICamera.s_curTouchesCount].phase = phase;
			UICamera.s_curTouches[UICamera.s_curTouchesCount].eventTime = Time.time;
			num = UICamera.s_curTouchesCount;
			UICamera.s_curTouchesCount++;
		}
		else if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled || (UICamera.s_curTouches[num].phase != TouchPhase.Ended && UICamera.s_curTouches[num].phase != TouchPhase.Canceled))
		{
			UICamera.s_curTouches[num].id = touchId;
			UICamera.s_curTouches[num].pos = new Vector2(pos_x * (float)Screen.width, pos_y * (float)Screen.height);
			UICamera.s_curTouches[num].pressure = pressure;
			UICamera.s_curTouches[num].maximumPossiblePressure = maximumPossiblePressure;
			UICamera.s_curTouches[num].phase = phase;
			UICamera.s_curTouches[UICamera.s_curTouchesCount].eventTime = Time.time;
		}
		int num2 = UICamera.FindIndexOfTouchIdExist(touchId, UICamera.s_preTouches, UICamera.s_preTouchesCount);
		if (num2 != -1)
		{
			UICamera.s_curTouches[num].deltaTime = UICamera.s_curTouches[num].eventTime - UICamera.s_preTouches[num2].eventTime;
			UICamera.s_curTouches[num].deltaPressure = UICamera.s_curTouches[num].pressure - UICamera.s_preTouches[num2].pressure;
		}
		else
		{
			UICamera.s_curTouches[num].deltaTime = 0f;
			UICamera.s_curTouches[num].deltaPressure = 0f;
		}
	}

	private void OnStickInput(JoyStickControlor.StickButtonCode code)
	{
		JoyStickControlor instance = JoyStickControlor.GetInstance();
		if (instance.MoustStatus == JoyStickControlor.UIMouseStatus.MouseActive && code == JoyStickControlor.StickButtonCode.A)
		{
			this.ProcessStickMouse(code);
		}
	}

	public void ProcessStickMouse(JoyStickControlor.StickButtonCode code)
	{
		JoyStickControlor instance = JoyStickControlor.GetInstance();
		UICamera.lastTouchPosition = instance.MousePositon;
		UICamera.mMouse[0].delta = UICamera.lastTouchPosition - UICamera.mMouse[0].pos;
		UICamera.mMouse[0].pos = UICamera.lastTouchPosition;
		bool flag = UICamera.mMouse[0].delta.sqrMagnitude > 0.001f;
		for (int i = 1; i < 3; i++)
		{
			UICamera.mMouse[i].pos = UICamera.mMouse[0].pos;
			UICamera.mMouse[i].delta = UICamera.mMouse[0].delta;
		}
		bool flag2 = false;
		bool flag3 = false;
		if (instance.GetKeyDown(instance.KeyCode_A))
		{
			flag2 = true;
		}
		else if (instance.GetKeyUp(instance.KeyCode_A))
		{
			flag2 = false;
		}
		if (flag2 || flag || this.mNextRaycast < RealTime.time)
		{
			this.mNextRaycast = RealTime.time + 0.02f;
			if (!UICamera.Raycast(UICamera.lastTouchPosition, out UICamera.lastHit))
			{
				UICamera.hoveredObject = UICamera.fallThrough;
			}
			if (UICamera.hoveredObject == null)
			{
				UICamera.hoveredObject = UICamera.genericEventHandler;
			}
			for (int j = 0; j < 3; j++)
			{
				UICamera.mMouse[j].current = UICamera.hoveredObject;
			}
		}
		bool flag4 = UICamera.mMouse[0].last != UICamera.mMouse[0].current;
		if (flag4)
		{
			UICamera.currentScheme = UICamera.ControlScheme.Mouse;
		}
		if (flag2)
		{
			this.mTooltipTime = 0f;
		}
		else if (flag && (!this.stickyTooltip || flag4))
		{
			if (this.mTooltipTime != 0f)
			{
				this.mTooltipTime = RealTime.time + this.tooltipDelay;
			}
			else if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
		}
		if ((flag3 || !flag2) && UICamera.mHover != null && flag4)
		{
			UICamera.currentScheme = UICamera.ControlScheme.Mouse;
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			UICamera.Notify(UICamera.mHover, "OnHover", false);
			UICamera.mHover = null;
		}
		bool keyDown = instance.GetKeyDown(instance.KeyCode_A);
		bool keyUp = instance.GetKeyUp(instance.KeyCode_A);
		if (keyDown || keyUp)
		{
			UICamera.currentScheme = UICamera.ControlScheme.Mouse;
		}
		UICamera.currentTouch = UICamera.mMouse[0];
		UICamera.currentTouchID = -1;
		UICamera.currentKey = KeyCode.Mouse0;
		if (keyDown)
		{
			UICamera.currentTouch.pressedCam = UICamera.currentCamera;
		}
		else if (UICamera.currentTouch.pressed != null)
		{
			UICamera.currentCamera = UICamera.currentTouch.pressedCam;
		}
		this.ProcessTouch(keyDown, keyUp);
		UICamera.currentKey = KeyCode.None;
		UICamera.currentTouch = null;
		if (!flag2 && flag4)
		{
			UICamera.currentScheme = UICamera.ControlScheme.Mouse;
			this.mTooltipTime = RealTime.time + this.tooltipDelay;
			UICamera.mHover = UICamera.mMouse[0].current;
			UICamera.Notify(UICamera.mHover, "OnHover", true);
		}
		UICamera.mMouse[0].last = UICamera.mMouse[0].current;
		for (int k = 1; k < 3; k++)
		{
			UICamera.mMouse[k].last = UICamera.mMouse[0].last;
		}
	}

	private void OnApplicationPause()
	{
		UICamera.MouseOrTouch mouseOrTouch = UICamera.currentTouch;
		if (this.useTouch)
		{
			BetterList<int> betterList = new BetterList<int>();
			foreach (KeyValuePair<int, UICamera.MouseOrTouch> keyValuePair in UICamera.mTouches)
			{
				if (keyValuePair.Value != null && keyValuePair.Value.pressed)
				{
					UICamera.currentTouch = keyValuePair.Value;
					UICamera.currentTouchID = keyValuePair.Key;
					UICamera.currentScheme = UICamera.ControlScheme.Touch;
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
					this.ProcessTouch(false, true);
					betterList.Add(UICamera.currentTouchID);
				}
			}
			for (int i = 0; i < betterList.size; i++)
			{
				UICamera.RemoveTouch(betterList[i]);
			}
		}
		if (this.useMouse)
		{
			for (int j = 0; j < 3; j++)
			{
				if (UICamera.mMouse[j].pressed)
				{
					UICamera.currentTouch = UICamera.mMouse[j];
					UICamera.currentTouchID = -1 - j;
					UICamera.currentKey = KeyCode.Mouse0 + j;
					UICamera.currentScheme = UICamera.ControlScheme.Mouse;
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
					this.ProcessTouch(false, true);
				}
			}
		}
		if (this.useController && UICamera.controller.pressed)
		{
			UICamera.currentTouch = UICamera.controller;
			UICamera.currentTouchID = -100;
			UICamera.currentScheme = UICamera.ControlScheme.Controller;
			UICamera.currentTouch.last = UICamera.currentTouch.current;
			UICamera.currentTouch.current = UICamera.mCurrentSelection;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
			this.ProcessTouch(false, true);
			UICamera.currentTouch.last = null;
		}
		UICamera.currentTouch = mouseOrTouch;
	}
}
