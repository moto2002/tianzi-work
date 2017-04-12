using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SetUICamVP2Shader : MonoBehaviour
{
	private float size;

	private float Size
	{
		set
		{
			if (this.size - value > 0.01f || this.size - value < -0.01f)
			{
				this.size = value;
				this.SetCamera();
			}
		}
	}

	private void Start()
	{
		this.Size = base.GetComponent<Camera>().orthographicSize;
	}

	private void SetCamera()
	{
		Matrix4x4 matrix4x = default(Matrix4x4);
		Matrix4x4 matrix4x2 = default(Matrix4x4);
		float num = this.size * base.GetComponent<Camera>().aspect;
		float nearClipPlane = base.GetComponent<Camera>().nearClipPlane;
		float farClipPlane = base.GetComponent<Camera>().farClipPlane;
		float value = 1f / num;
		float value2 = 1f / this.size;
		float value3 = -2f / (farClipPlane - nearClipPlane);
		float value4 = -2f / (farClipPlane - nearClipPlane);
		float value5 = -(farClipPlane + nearClipPlane) / (farClipPlane - nearClipPlane);
		float value6 = -nearClipPlane / (farClipPlane - nearClipPlane);
		matrix4x[0, 0] = value;
		matrix4x[0, 1] = 0f;
		matrix4x[0, 2] = 0f;
		matrix4x[0, 3] = 0f;
		matrix4x[1, 0] = 0f;
		matrix4x[1, 1] = value2;
		matrix4x[1, 2] = 0f;
		matrix4x[1, 3] = 0f;
		matrix4x[2, 0] = 0f;
		matrix4x[2, 1] = 0f;
		matrix4x[2, 2] = value3;
		matrix4x[2, 3] = value5;
		matrix4x[3, 0] = 0f;
		matrix4x[3, 1] = 0f;
		matrix4x[3, 2] = 0f;
		matrix4x[3, 3] = 1f;
		matrix4x2[0, 0] = value;
		matrix4x2[0, 1] = 0f;
		matrix4x2[0, 2] = 0f;
		matrix4x2[0, 3] = 0f;
		matrix4x2[1, 0] = 0f;
		matrix4x2[1, 1] = value2;
		matrix4x2[1, 2] = 0f;
		matrix4x2[1, 3] = 0f;
		matrix4x2[2, 0] = 0f;
		matrix4x2[2, 1] = 0f;
		matrix4x2[2, 2] = value4;
		matrix4x2[2, 3] = value6;
		matrix4x2[3, 0] = 0f;
		matrix4x2[3, 1] = 0f;
		matrix4x2[3, 2] = 0f;
		matrix4x2[3, 3] = 1f;
		Matrix4x4 mat = base.GetComponent<Camera>().projectionMatrix * base.GetComponent<Camera>().worldToCameraMatrix;
		Shader.SetGlobalMatrix("UI_CAM_MATRIX_VP", mat);
	}

	private void LateUpdate()
	{
		this.Size = base.GetComponent<Camera>().orthographicSize;
	}
}
