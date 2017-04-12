using System;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickControlor : MonoBehaviour
{
	public enum StickButtonCode
	{
		None,
		A,
		A_DOWN,
		A_UP,
		B,
		X,
		Y,
		L1,
		R1,
		L1_X,
		L1_Y,
		L1_A,
		R1_X,
		R1_Y,
		R1_A,
		L1_R1,
		LeftArrow,
		RightArrow,
		UpArrow,
		DownArrow,
		Both
	}

	public enum UIMouseStatus
	{
		MouseHide,
		MouseActive
	}

	public delegate void OnInputCode(JoyStickControlor.StickButtonCode code);

	[HideInInspector]
	public KeyCode KeyCode_X = KeyCode.JoystickButton2;

	[HideInInspector]
	public KeyCode KeyCode_Y = KeyCode.JoystickButton3;

	[HideInInspector]
	public KeyCode KeyCode_A = KeyCode.JoystickButton0;

	[HideInInspector]
	public KeyCode KeyCode_B = KeyCode.JoystickButton1;

	[HideInInspector]
	public KeyCode KeyCode_R1 = KeyCode.JoystickButton5;

	[HideInInspector]
	public KeyCode KeyCode_L1 = KeyCode.JoystickButton4;

	[HideInInspector]
	public KeyCode KeyCode_Left = KeyCode.LeftArrow;

	[HideInInspector]
	public KeyCode KeyCode_Right = KeyCode.RightArrow;

	[HideInInspector]
	public KeyCode KeyCode_Up = KeyCode.UpArrow;

	[HideInInspector]
	public KeyCode KeyCode_Down = KeyCode.DownArrow;

	private List<JoyStickControlor.OnInputCode> inputListeners = new List<JoyStickControlor.OnInputCode>();

	private List<JoyStickControlor.StickButtonCode> codeList = new List<JoyStickControlor.StickButtonCode>();

	private JoyStickControlor.UIMouseStatus _mouseStatus;

	private int detecFrameInterval = 1;

	private int beforeDownFrame;

	private int mouseMoveSpeed = 2;

	private GameObject mouseObj;

	private Transform mouseTrans;

	private Camera uicamera;

	private static JoyStickControlor instance;

	public JoyStickControlor.UIMouseStatus MoustStatus
	{
		get
		{
			return this._mouseStatus;
		}
		set
		{
			this._mouseStatus = value;
		}
	}

	public Vector3 MousePositon
	{
		get
		{
			if (this.mouseTrans != null)
			{
				Camera currentCamera = UICamera.currentCamera;
				return currentCamera.WorldToScreenPoint(this.mouseTrans.position);
			}
			return Vector3.zero;
		}
	}

	public static JoyStickControlor GetInstance()
	{
		if (JoyStickControlor.instance == null)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("UICamera");
			if (gameObject != null)
			{
				JoyStickControlor.instance = gameObject.GetComponent<JoyStickControlor>();
				if (JoyStickControlor.instance == null)
				{
					JoyStickControlor.instance = gameObject.AddComponent<JoyStickControlor>();
					JoyStickControlor.instance.uicamera = gameObject.GetComponent<Camera>();
				}
			}
		}
		return JoyStickControlor.instance;
	}

	private void Awake()
	{
		Instance.Set<JoyStickControlor>(this, true);
	}

	public void Init()
	{
		this.CreateMouseObj();
	}

	private void CreateMouseObj()
	{
		string text = "Prefabs/UI/MousePanel";
		string text2 = text.ToLower();
		DelegateProxy.LoadAsset(text, new AssetCallBack(this.OnPrefabLoaded));
	}

	private void OnPrefabLoaded(params object[] param)
	{
		UnityEngine.Object @object = param[0] as UnityEngine.Object;
		if (@object != null && this.uicamera != null)
		{
			this.mouseObj = (UnityEngine.Object.Instantiate(@object) as GameObject);
			this.mouseObj.SetActive(false);
			this.mouseObj.name = "Mouse";
			this.mouseTrans = this.mouseObj.transform;
			this.mouseTrans.parent = this.uicamera.transform;
			this.mouseTrans.localScale = Vector3.one;
			this.mouseTrans.localPosition = Vector3.zero;
			UIPanel component = this.mouseTrans.GetComponent<UIPanel>();
			component.depth = 2000;
		}
		DelegateProxy.UnloadAsset(param);
	}

	private void Update()
	{
		if (this._mouseStatus == JoyStickControlor.UIMouseStatus.MouseActive)
		{
			this.MouseMove();
			if (Input.GetKey(this.KeyCode_A) || Input.GetKeyUp(this.KeyCode_A) || Input.GetKeyDown(this.KeyCode_A))
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.A);
			}
			else if (Input.GetKeyUp(this.KeyCode_Y))
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.Y);
			}
			return;
		}
		if (Input.GetKeyDown(this.KeyCode_A))
		{
			this.StickCodeHandle(JoyStickControlor.StickButtonCode.A_DOWN);
		}
		if (Input.GetKey(this.KeyCode_A))
		{
			if (Input.GetKey(this.KeyCode_L1))
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.L1_A);
			}
			else if (Input.GetKey(this.KeyCode_R1))
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.R1_A);
			}
			else
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.A);
			}
		}
		if (Input.GetKey(this.KeyCode_X))
		{
			if (Input.GetKey(this.KeyCode_L1))
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.L1_X);
			}
			else if (Input.GetKey(this.KeyCode_R1))
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.R1_X);
			}
			else
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.X);
			}
		}
		if (Input.GetKey(this.KeyCode_Y))
		{
			if (Input.GetKey(this.KeyCode_L1))
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.L1_Y);
			}
			else if (Input.GetKey(this.KeyCode_R1))
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.R1_Y);
			}
			else
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.Y);
			}
		}
		if (Input.GetKey(this.KeyCode_L1))
		{
			if (Input.GetKey(this.KeyCode_R1))
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.L1_R1);
			}
			else
			{
				this.StickCodeHandle(JoyStickControlor.StickButtonCode.L1);
			}
		}
		else if (Input.GetKey(this.KeyCode_R1))
		{
			this.StickCodeHandle(JoyStickControlor.StickButtonCode.R1);
		}
	}

	private void StickCodeHandle(JoyStickControlor.StickButtonCode code)
	{
		if (code == JoyStickControlor.StickButtonCode.L1_R1)
		{
			this.HideOrShowMouse(true);
		}
		else if (code == JoyStickControlor.StickButtonCode.Y && this._mouseStatus == JoyStickControlor.UIMouseStatus.MouseActive)
		{
			this.HideOrShowMouse(false);
			return;
		}
		if (this._mouseStatus == JoyStickControlor.UIMouseStatus.MouseHide)
		{
			this.DispatchEvents(code);
		}
		else if (code == JoyStickControlor.StickButtonCode.A)
		{
			this.DispatchEvents(code);
		}
	}

	private void HideOrShowMouse(bool state)
	{
		if (this.mouseObj != null)
		{
			this.mouseObj.SetActive(state);
			if (state)
			{
				this._mouseStatus = JoyStickControlor.UIMouseStatus.MouseActive;
			}
			else
			{
				this._mouseStatus = JoyStickControlor.UIMouseStatus.MouseHide;
			}
		}
	}

	private void MouseMove()
	{
		Vector3 zero = Vector3.zero;
		if (Input.GetAxis("joy_horizontal") != 0f)
		{
			zero.x = Input.GetAxis("joy_horizontal");
		}
		else if (Input.GetAxis("dpad_horizontal") != 0f)
		{
			zero.x = Input.GetAxis("dpad_horizontal");
		}
		if (Input.GetAxis("joy_vertical") != 0f)
		{
			zero.y = Input.GetAxis("joy_vertical");
		}
		else if (Input.GetAxis("dpad_vertical") != 0f)
		{
			zero.y = Input.GetAxis("dpad_vertical");
		}
		zero.Normalize();
		this.mouseTrans.Translate(zero * (float)this.mouseMoveSpeed * Time.deltaTime);
		Vector3 localPosition = this.mouseTrans.localPosition;
		float pixelWidth = this.uicamera.pixelWidth;
		float pixelHeight = this.uicamera.pixelHeight;
		if (localPosition.x > pixelWidth / 2f)
		{
			localPosition.x = pixelWidth / 2f;
		}
		else if (localPosition.x < pixelWidth / 2f * -1f)
		{
			localPosition.x = pixelWidth / 2f * -1f;
		}
		if (localPosition.y > pixelHeight / 2f)
		{
			localPosition.y = pixelHeight / 2f;
		}
		else if (localPosition.y < pixelHeight / 2f * -1f)
		{
			localPosition.y = pixelHeight / 2f * -1f;
		}
		this.mouseTrans.localPosition = localPosition;
	}

	public void AddListener(JoyStickControlor.OnInputCode onInput)
	{
		if (this.inputListeners != null && !this.inputListeners.Contains(onInput))
		{
			this.inputListeners.Add(onInput);
		}
	}

	public void RemoveListener(JoyStickControlor.OnInputCode onInput)
	{
		if (this.inputListeners != null && this.inputListeners.Contains(onInput))
		{
			this.inputListeners.Remove(onInput);
		}
	}

	private void DispatchEvents(JoyStickControlor.StickButtonCode code)
	{
		if (this.inputListeners == null || this.inputListeners.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.inputListeners.Count; i++)
		{
			this.inputListeners[i](code);
		}
	}

	public bool GetKey(KeyCode key)
	{
		return Input.GetKey(key);
	}

	public bool GetKeyDown(KeyCode key)
	{
		return Input.GetKeyDown(key);
	}

	public bool GetKeyUp(KeyCode key)
	{
		return Input.GetKeyUp(key);
	}
}
