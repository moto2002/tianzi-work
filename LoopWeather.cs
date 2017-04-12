using System;
using System.Collections.Generic;
using UnityEngine;

public class LoopWeather : MonoBehaviour
{
	private List<Transform> snowplan = new List<Transform>();

	private static Transform m_target;

	private Vector3 centerPos;

	private Vector3 lastVec3 = Vector3.zero;

	public Vector2 weight = Vector2.zero;

	public float verticalSpeed = 0.02f;

	public float horizontalSpeed = 0.03f;

	private int m_curX;

	private int m_curY;

	private int m_lastX;

	private int m_lastY;

	private int m_indexX;

	private int m_indexY;

	private void Start()
	{
		Vector3 zero = Vector3.zero;
		Transform transform = base.transform.FindChild("snow_plan_0");
		if (transform != null)
		{
			transform.localPosition = zero;
			this.snowplan.Add(transform);
		}
		if (transform != null)
		{
			transform = base.transform.FindChild("snow_plan_1");
			zero = Vector3.zero;
			zero.x = this.weight.x;
			transform.localPosition = zero;
			this.snowplan.Add(transform);
		}
		if (transform != null)
		{
			transform = base.transform.FindChild("snow_plan_2");
			zero = Vector3.zero;
			zero.y = -this.weight.y;
			transform.localPosition = zero;
			this.snowplan.Add(transform);
		}
		if (transform != null)
		{
			transform = base.transform.FindChild("snow_plan_3");
			zero = Vector3.zero;
			zero.x = this.weight.x;
			zero.y = -this.weight.y;
			transform.localPosition = zero;
			this.snowplan.Add(transform);
		}
	}

	public static void SetTarget(Transform target)
	{
		LoopWeather.m_target = target;
	}

	private void Update()
	{
		if (Camera.main != null && LoopWeather.m_target != null)
		{
			this.lastVec3 = Camera.main.transform.InverseTransformDirection(LoopWeather.m_target.position);
		}
	}

	private void FixedUpdate()
	{
		if (Camera.main != null && LoopWeather.m_target != null)
		{
			Vector3 vector = Camera.main.transform.InverseTransformDirection(LoopWeather.m_target.position);
			if (vector != this.lastVec3 && this.lastVec3 != Vector3.zero)
			{
				Vector2 zero = Vector2.zero;
				this.CalculatePosition_X(ref zero, vector, this.lastVec3);
				this.CalculatePosition_Y(ref zero, vector, this.lastVec3);
				Transform transform = this.snowplan[0];
				transform.localPosition = this.GetPosition(zero, transform.localPosition);
				transform = this.snowplan[1];
				transform.localPosition = this.GetPosition(zero, transform.localPosition);
				transform = this.snowplan[2];
				transform.localPosition = this.GetPosition(zero, transform.localPosition);
				transform = this.snowplan[3];
				transform.localPosition = this.GetPosition(zero, transform.localPosition);
			}
		}
	}

	private void CalculatePosition_X(ref Vector2 distance, Vector3 tpos, Vector3 lastpos)
	{
		Vector3 vector = tpos - lastpos;
		distance.x = vector.x * this.verticalSpeed;
		this.centerPos.x = this.centerPos.x + distance.x;
		this.m_curX = Mathf.FloorToInt(this.centerPos.x / this.weight.x);
		if (this.m_lastX != this.m_curX)
		{
			if (vector.x < 0f)
			{
				this.m_indexX--;
				int num = Mathf.Abs(this.m_indexX % 2);
				Transform transform = this.snowplan[num];
				Vector3 localPosition = transform.localPosition;
				localPosition.x -= this.weight.x * 2f;
				transform.localPosition = localPosition;
				transform = this.snowplan[num + 2];
				localPosition.y = transform.localPosition.y;
				transform.localPosition = localPosition;
				this.m_lastX = this.m_curX;
			}
			else
			{
				int num2 = Mathf.Abs(this.m_indexX % 2);
				Transform transform2 = this.snowplan[num2];
				Vector3 localPosition2 = transform2.localPosition;
				localPosition2.x += this.weight.x * 2f;
				transform2.localPosition = localPosition2;
				transform2 = this.snowplan[num2 + 2];
				localPosition2.y = transform2.localPosition.y;
				transform2.localPosition = localPosition2;
				this.m_lastX = this.m_curX;
				this.m_indexX++;
			}
		}
	}

	private void CalculatePosition_Y(ref Vector2 distance, Vector3 tpos, Vector3 lastpos)
	{
		Vector3 vector = tpos - lastpos;
		if (vector.y < 0f)
		{
			distance.y = vector.y * 0f;
		}
		else
		{
			distance.y = vector.y * this.horizontalSpeed;
		}
		this.centerPos.y = this.centerPos.y + distance.y;
		this.m_curY = Mathf.CeilToInt(this.centerPos.y / this.weight.y);
		if (this.m_lastY != this.m_curY)
		{
			if (vector.y < 0f)
			{
				int num = Mathf.Abs(this.m_indexY % 2) * 2;
				Transform transform = this.snowplan[num];
				Vector3 localPosition = transform.localPosition;
				localPosition.y -= this.weight.y * 2f;
				transform.localPosition = localPosition;
				transform = this.snowplan[num + 1];
				localPosition.x = transform.localPosition.x;
				transform.localPosition = localPosition;
				this.m_lastY = this.m_curY;
				this.m_indexY++;
			}
			else
			{
				this.m_indexY--;
				int num2 = Mathf.Abs(this.m_indexY % 2) * 2;
				Transform transform2 = this.snowplan[num2];
				Vector3 localPosition2 = transform2.localPosition;
				localPosition2.y += this.weight.y * 2f;
				transform2.localPosition = localPosition2;
				transform2 = this.snowplan[num2 + 1];
				localPosition2.x = transform2.localPosition.x;
				transform2.localPosition = localPosition2;
				this.m_lastY = this.m_curY;
			}
		}
	}

	private Vector3 GetPosition(Vector2 distance, Vector3 selfPos)
	{
		Vector3 zero = Vector3.zero;
		zero.x = selfPos.x - distance.x;
		zero.y = selfPos.y - distance.y;
		zero.z = selfPos.z;
		return zero;
	}
}
