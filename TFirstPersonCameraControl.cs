using System;
using UnityEngine;

public class TFirstPersonCameraControl : MonoBehaviour
{
	public GameObjectUnit target;

	public float xSpeed;

	public float ySpeed;

	public float yMinLimit;

	public float yMaxLimit;

	public float scrollSpeed;

	public float zoomMin;

	public float zoomMax;

	private float distance;

	private float distanceLerp;

	private Vector3 position;

	private bool isActivated;

	private float x;

	private float y;

	private bool setupCamera;

	private void Start()
	{
	}

	public void SetPose(Vector3 eulerAngles)
	{
		Vector3 vector = eulerAngles;
		this.x = vector.y;
		this.y = vector.x;
		this.CalDistance();
		this.RotateCamera(false);
	}

	private void LateUpdate()
	{
		if (this.target == null)
		{
			if (GameScene.mainScene != null)
			{
				this.target = GameScene.mainScene.mainUnit;
			}
			if (this.target != null)
			{
				Vector3 eulerAngles = base.transform.eulerAngles;
				this.x = eulerAngles.y;
				this.y = eulerAngles.x;
				this.CalDistance();
				this.RotateCamera(true);
			}
		}
		if (this.target != null)
		{
			this.ScrollMouse();
			this.RotateCamera(false);
		}
	}

	private void RotateCamera(bool forcibly = false)
	{
		if (Input.GetMouseButtonDown(1))
		{
			this.isActivated = true;
		}
		if (Input.GetMouseButtonUp(1))
		{
			this.isActivated = false;
		}
		if ((this.target != null && this.isActivated) || forcibly)
		{
			this.y -= Input.GetAxis("Mouse Y") * this.ySpeed;
			this.x += Input.GetAxis("Mouse X") * this.xSpeed;
			this.y = this.ClampAngle(this.y, this.yMinLimit, this.yMaxLimit);
			Quaternion rotation = Quaternion.Euler(this.y, this.x, 0f);
			Vector3 point = new Vector3(0f, 0f, -this.distanceLerp);
			this.position = rotation * point + this.target.position;
			base.transform.rotation = rotation;
			base.transform.position = this.position;
		}
		else
		{
			Quaternion rotation2 = Quaternion.Euler(this.y, this.x, 0f);
			Vector3 point2 = new Vector3(0f, 0f, -this.distanceLerp);
			this.position = rotation2 * point2 + this.target.position;
			base.transform.rotation = rotation2;
			base.transform.position = this.position;
		}
	}

	private void CalDistance()
	{
		this.distance = this.zoomMax;
		this.distanceLerp = this.distance;
		Quaternion rotation = Quaternion.Euler(this.y, this.x, 0f);
		Vector3 point = new Vector3(0f, 0f, -this.distanceLerp);
		this.position = rotation * point + this.target.position;
		base.transform.rotation = rotation;
		base.transform.position = this.position;
	}

	private void ScrollMouse()
	{
		this.distanceLerp = Mathf.Lerp(this.distanceLerp, this.distance, Time.deltaTime * 5f);
		if (Input.GetAxis("Mouse ScrollWheel") != 0f)
		{
			this.distance = Vector3.Distance(base.transform.position, this.target.position);
			this.distance = this.ScrollLimit(this.distance - Input.GetAxis("Mouse ScrollWheel") * this.scrollSpeed, this.zoomMin, this.zoomMax);
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
