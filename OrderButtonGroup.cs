using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderButtonGroup : MonoBehaviour
{
	public Action<string> callback;

	public OrderMode mode = OrderMode.Radial360;

	public float offest;

	private bool bInit;

	private Vector3 point = Vector3.zero;

	private Vector3 size;

	private int len;

	private List<OrderButton> orders = new List<OrderButton>();

	public GameObject root;

	public GameObject rotationObj;

	private BoxCollider col;

	private bool bShow;

	private Vector3 rotation = Vector3.zero;

	private Camera uiCamera;

	private void Awake()
	{
		this.bInit = false;
	}

	private void OnDisable()
	{
		this.bInit = false;
	}

	private void OnEnable()
	{
		this.OnInit();
	}

	private void OnDestory()
	{
		this.callback = null;
		if (this.orders != null)
		{
			this.orders.Clear();
		}
		this.col = null;
		this.root = null;
		OrderButton.Current = null;
	}

	private void OnInit()
	{
		this.bInit = true;
		this.bShow = false;
		this.col = base.GetComponent<BoxCollider>();
		GameObject gameObject = GameObject.FindGameObjectWithTag("UICamera");
		if (gameObject != null)
		{
			this.uiCamera = gameObject.GetComponent<Camera>();
		}
		this.size = ((!(this.col != null)) ? new Vector3(20f, 20f) : this.col.size);
		this.orders.Clear();
		OrderButton[] componentsInChildren = base.GetComponentsInChildren<OrderButton>(true);
		if (componentsInChildren != null)
		{
			this.orders = new List<OrderButton>(componentsInChildren);
			this.len = this.orders.Count;
		}
		if (this.orders != null)
		{
			this.orders.Sort(new Comparison<OrderButton>(OrderButtonGroup.SortByName));
		}
		if (this.root != null)
		{
			this.root.SetActive(false);
		}
		if (this.rotationObj != null)
		{
			this.rotationObj.SetActive(false);
		}
	}

	public static int SortByName(OrderButton a, OrderButton b)
	{
		return string.Compare(a.name, b.name);
	}

	private void Update()
	{
		if (!this.bInit)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (this.ClickOrderGameObject(Input.mousePosition))
			{
				if (this.root != null)
				{
					this.root.SetActive(true);
					this.bShow = true;
				}
				if (this.rotationObj != null)
				{
					this.rotationObj.SetActive(true);
				}
			}
		}
		else if (Input.GetMouseButton(0))
		{
			if (!this.bShow)
			{
				return;
			}
			if (this.ClickOrderGameObject(Input.mousePosition))
			{
				if (OrderButton.Current != null)
				{
					OrderButton.Current.SetState(UIButtonColor.State.Normal, false);
				}
			}
			else
			{
				int orderIndex = this.GetOrderIndex(Input.mousePosition);
				if (orderIndex < 0)
				{
					if (OrderButton.Current != null)
					{
						OrderButton.Current.SetState(UIButtonColor.State.Normal, false);
					}
				}
				else if (orderIndex < this.len)
				{
					if (OrderButton.Current != null)
					{
						OrderButton.Current.SetState(UIButtonColor.State.Normal, false);
					}
					OrderButton.Current = this.orders[orderIndex];
					OrderButton.Current.SetState(UIButtonColor.State.Hover, false);
				}
				if (this.rotationObj != null)
				{
					this.rotation.z = this.GetAngle(Input.mousePosition);
					this.rotationObj.transform.localEulerAngles = this.rotation;
				}
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (!this.bShow)
			{
				return;
			}
			if (!this.ClickOrderGameObject(Input.mousePosition))
			{
				int orderIndex2 = this.GetOrderIndex(Input.mousePosition);
				if (orderIndex2 >= 0 && orderIndex2 < this.len)
				{
					if (OrderButton.Current != null)
					{
						OrderButton.Current.SetState(UIButtonColor.State.Normal, false);
					}
					OrderButton.Current = this.orders[orderIndex2];
					OrderButton.Current.SetState(UIButtonColor.State.Normal, false);
					if (this.callback != null)
					{
						this.callback(OrderButton.Current.Text);
					}
				}
			}
			if (this.root != null)
			{
				this.root.SetActive(false);
			}
			if (this.rotationObj != null)
			{
				this.rotationObj.SetActive(false);
			}
			this.bShow = false;
		}
	}

	private bool ClickOrderGameObject(Vector3 position)
	{
		if (this.uiCamera != null)
		{
			position = this.uiCamera.ScreenToWorldPoint(position);
			this.point = base.transform.InverseTransformPoint(position);
			return this.point.x >= -this.size.x * 0.5f && this.point.x <= this.size.x * 0.5f && this.point.y >= -this.size.y * 0.5f && this.point.y <= this.size.y * 0.5f;
		}
		return false;
	}

	private float GetAngle(Vector3 position)
	{
		if (this.uiCamera != null)
		{
			position = this.uiCamera.ScreenToWorldPoint(position);
			this.point = base.transform.InverseTransformPoint(position);
			this.point.z = 0f;
			float num = Vector3.Angle(Vector3.left, this.point);
			if (this.point.y < 0f)
			{
				num = 360f - num;
			}
			num += 360f;
			num %= 360f;
			num -= this.offest;
			return -num;
		}
		return 0f;
	}

	private int GetOrderIndex(Vector3 position)
	{
		if (this.len <= 0)
		{
			return -1;
		}
		if (!(this.uiCamera != null))
		{
			return -1;
		}
		position = this.uiCamera.ScreenToWorldPoint(position);
		this.point = base.transform.InverseTransformPoint(position);
		this.point.z = 0f;
		float num = Vector3.Angle(Vector3.left, this.point);
		if (this.point.y < 0f)
		{
			num = 360f - num;
		}
		num += 360f;
		num %= 360f;
		if (this.mode != OrderMode.Radial180)
		{
			float num2 = (float)(360 / this.len);
			num2 = num / num2;
			return Mathf.RoundToInt(num2);
		}
		float num3 = (float)(180 / (this.len - 1));
		if (num <= 180f)
		{
			num3 = num / num3;
			return Mathf.RoundToInt(num3);
		}
		if (num <= 180f + num3 / 2f)
		{
			return this.len - 1;
		}
		if (num > 360f - num3 / 2f)
		{
			return 0;
		}
		return -1;
	}
}
