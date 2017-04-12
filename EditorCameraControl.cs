using System;
using UnityEngine;

public class EditorCameraControl : MonoBehaviour
{
	public GameObjectUnit target;

	public float xSpeed = 3.5f;

	public float ySpeed = 3.5f;

	public float yMinLimit = 25f;

	public float yMaxLimit = 100f;

	public float scrollSpeed = 0.3f;

	public float zoomMin;

	public float zoomMax = 200f;

	public float distance;

	private float distanceLerp;

	private Vector3 position;

	private bool isActivated;

	public float x;

	public float y;

	public GameScene mainScene = new GameScene(false);

	public TerrainConfig terrainConfig;

	private Vector2 oldMousePos;

	private float mousetAxisX;

	private float mousetAxisY;

	private void Start()
	{
	}

	public void Set()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		this.x = eulerAngles.y;
		this.y = eulerAngles.x;
		this.CalDistance();
		this.RotateCamera(true);
	}

	public void SetR(Vector3 eulerAngles)
	{
		this.x = eulerAngles.y;
		this.y = eulerAngles.x;
		this.CalDistance();
		this.RotateCamera(true);
	}

	public void Update()
	{
	}

	public void Update(GameObjectUnit target)
	{
		if (this.target == null)
		{
			this.target = target;
			Vector3 eulerAngles = base.transform.eulerAngles;
			this.x = eulerAngles.y;
			this.y = eulerAngles.x;
			this.CalDistance();
			this.RotateCamera(true);
		}
		if (this.target != null)
		{
			this.ScrollMouse();
			this.RotateCamera(false);
		}
	}

	public void RotateCamera(bool forcibly = false)
	{
		Event current = Event.current;
		if (current.control || current.shift)
		{
			return;
		}
		if (current.isMouse && current.type == EventType.MouseDown && current.button == 1)
		{
			this.oldMousePos = current.mousePosition;
			this.isActivated = true;
		}
		if ((current.isMouse && current.type == EventType.MouseUp) || current.button != 1)
		{
			this.isActivated = false;
		}
		if ((this.target != null && this.isActivated) || forcibly)
		{
			this.mousetAxisX = (current.mousePosition.x - this.oldMousePos.x) / 100f;
			this.mousetAxisY = -(current.mousePosition.y - this.oldMousePos.y) / 100f;
			this.oldMousePos = current.mousePosition;
			this.y -= this.mousetAxisY * this.ySpeed;
			this.x += this.mousetAxisX * this.xSpeed;
			this.y = this.ClampAngle(this.y, this.yMinLimit, this.yMaxLimit);
			Quaternion rotation = Quaternion.Euler(this.y, this.x, 0f);
			Vector3 zero = Vector3.zero;
			zero.x = 0f;
			zero.y = 0f;
			zero.z = -this.distanceLerp;
			Vector3 point = zero;
			this.position = rotation * point + this.target.position;
			base.transform.rotation = rotation;
			base.transform.position = this.position;
		}
		else
		{
			Quaternion rotation2 = Quaternion.Euler(this.y, this.x, 0f);
			Vector3 zero2 = Vector3.zero;
			zero2.x = 0f;
			zero2.y = 0f;
			zero2.z = -this.distanceLerp;
			Vector3 point2 = zero2;
			this.position = rotation2 * point2 + this.target.position;
			base.transform.rotation = rotation2;
			base.transform.position = this.position;
		}
	}

	private void CalDistance()
	{
		this.distance = 100f;
		this.distanceLerp = this.distance;
		Quaternion rotation = Quaternion.Euler(this.y, this.x, 0f);
		Vector3 zero = Vector3.zero;
		zero.x = 0f;
		zero.y = 0f;
		zero.z = -this.distanceLerp;
		Vector3 point = zero;
		this.position = rotation * point + this.target.position;
		base.transform.rotation = rotation;
		base.transform.position = this.position;
	}

	public void SetDistance(float dis)
	{
		this.distance = dis;
		this.distanceLerp = this.distance;
		Quaternion rotation = Quaternion.Euler(this.y, this.x, 0f);
		Vector3 zero = Vector3.zero;
		zero.x = 0f;
		zero.y = 0f;
		zero.z = -this.distanceLerp;
		Vector3 point = zero;
		this.position = rotation * point + this.target.position;
		base.transform.rotation = rotation;
		base.transform.position = this.position;
		this.RotateCamera(true);
	}

	public void SetCustom()
	{
		this.distanceLerp = this.distance;
		Quaternion rotation = Quaternion.Euler(this.y, this.x, 0f);
		Vector3 zero = Vector3.zero;
		zero.x = 0f;
		zero.y = 0f;
		zero.z = -this.distanceLerp;
		Vector3 point = zero;
		this.position = rotation * point + this.target.position;
		base.transform.rotation = rotation;
		base.transform.position = this.position;
		this.RotateCamera(true);
	}

	private void ScrollMouse()
	{
		this.distanceLerp = Mathf.Lerp(this.distanceLerp, this.distance, 0.5f);
		Event current = Event.current;
		float num = this.scrollSpeed;
		if (current.shift)
		{
			num = this.scrollSpeed * 5f;
		}
		if (current.type == EventType.ScrollWheel)
		{
			float num2 = -current.delta.y;
			if (num2 != 0f)
			{
				this.distance = Vector3.Distance(base.transform.position, this.target.position);
				this.distance = this.ScrollLimit(this.distance - num2 * num * 1f, this.zoomMin, this.zoomMax);
			}
		}
	}

	private float ScrollLimit(float dist, float min, float max)
	{
		if (dist < min)
		{
			dist = min;
		}
		if (dist > max)
		{
			dist = max;
		}
		return dist;
	}

	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
