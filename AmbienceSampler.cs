using System;
using System.Collections.Generic;
using UnityEngine;
 /// <summary>
 /// 环境采样器
 /// </summary>
[ExecuteInEditMode]
public class AmbienceSampler : MonoBehaviour
{
    /// <summary>
    /// 水体模式
    /// </summary>
	public enum WaterMode
	{
		Simple,
		Reflective,
		Refractive
	}
    /// <summary>
    /// 水体采样模式
    /// </summary>
	public AmbienceSampler.WaterMode m_WaterMode = AmbienceSampler.WaterMode.Refractive;
    /// <summary>
    /// 是否禁用像素光源
    /// </summary>
	public bool m_DisablePixelLights = true;
    /// <summary>
    /// 贴图纹理尺寸
    /// </summary>
	public int m_TextureSize = 256;
    /// <summary>
    /// 剪裁平面偏移量
    /// </summary>
	public float m_ClipPlaneOffset = 0.07f;
    /// <summary>
    /// 反射layer
    /// </summary>
	public LayerMask m_ReflectLayers = -1;
    /// <summary>
    /// 折射layer
    /// </summary>
	public LayerMask m_RefractLayers = -1;
    /// <summary>
    /// 反射相机列表
    /// </summary>
	private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>();
    /// <summary>
    /// 折射相机列表
    /// </summary>
	private Dictionary<Camera, Camera> m_RefractionCameras = new Dictionary<Camera, Camera>();
    /// <summary>
    /// 反射纹理贴图
    /// </summary>
	private RenderTexture m_ReflectionTexture;
    /// <summary>
    /// 折射问题贴图
    /// </summary>
	private RenderTexture m_RefractionTexture;
    /// <summary>
    /// 硬件水体支持
    /// </summary>
	private AmbienceSampler.WaterMode m_HardwareWaterSupport = AmbienceSampler.WaterMode.Refractive;
    /// <summary>
    /// 已记录的反射纹理贴图
    /// </summary>
	private int m_OldReflectionTextureSize;
    /// <summary>
    /// 已记录的折射纹理贴图
    /// </summary>
	private int m_OldRefractionTextureSize;
    /// <summary>
    /// 反射镜像纹理贴图
    /// </summary>
	private Texture m_MirrorReflectionTexture;
    /// <summary>
    /// 折射镜像纹理贴图
    /// </summary>
	private Texture m_MirrorRefractionTexture;
    /// <summary>
    /// 获取反射纹理贴图，如果有镜像反射纹理贴图，优先返回
    /// </summary>
	public Texture reflectionTexture
	{
		get
		{
			if (this.m_MirrorReflectionTexture != null)
			{
				return this.m_MirrorReflectionTexture;
			}
			return this.m_ReflectionTexture;
		}
	}
    /// <summary>
    /// 获取折射纹理贴图，如果有镜像折射纹理贴图，优先返回
    /// </summary>
	public Texture refractionTexture
	{
		get
		{
			if (this.m_MirrorRefractionTexture != null)
			{
				return this.m_MirrorRefractionTexture;
			}
			return this.m_RefractionTexture;
		}
	}
    /// <summary>
    /// 反射RenderTexture
    /// </summary>
	public RenderTexture reflectionRenderTexture
	{
		get
		{
			return this.m_ReflectionTexture;
		}
	}
    /// <summary>
    /// 折射RenderTexture
    /// </summary>
	public RenderTexture refractionRenderTexture
	{
		get
		{
			return this.m_RefractionTexture;
		}
	}

	private void Start()
	{
	}
    /// <summary>
    /// 如果对象可见，则为每个相机调用一次此函数
    /// </summary>
	public void OnWillRenderObject()
	{
	}
    /// <summary>
    /// 禁用，销毁采样得到的纹理,临时用相机等
    /// </summary>
	private void OnDisable()
	{
		if (this.m_ReflectionTexture)
		{
			DelegateProxy.DestroyObjectImmediate(this.m_ReflectionTexture);
			this.m_ReflectionTexture = null;
		}
		if (this.m_RefractionTexture)
		{
			DelegateProxy.DestroyObjectImmediate(this.m_RefractionTexture);
			this.m_RefractionTexture = null;
		}
		foreach (KeyValuePair<Camera, Camera> current in this.m_ReflectionCameras)
		{
			DelegateProxy.DestroyObjectImmediate(current.Value.gameObject);
		}
		this.m_ReflectionCameras.Clear();
		foreach (KeyValuePair<Camera, Camera> current2 in this.m_RefractionCameras)
		{
			DelegateProxy.DestroyObjectImmediate(current2.Value.gameObject);
		}
		this.m_RefractionCameras.Clear();
	}
    /// <summary>
    /// 复制拷贝摄像机设置
    /// </summary>
    /// <param name="src"></param>
    /// <param name="dest"></param>
	private void UpdateCameraModes(Camera src, Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox skybox = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox skybox2 = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!skybox || !skybox.material)
			{
				skybox2.enabled = false;
			}
			else
			{
				skybox2.enabled = true;
				skybox2.material = skybox.material;
			}
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}
    /// <summary>
    /// 给指定相机创建采样水体需要的摄像机对象
    /// </summary>
    /// <param name="currentCamera"></param>
    /// <param name="reflectionCamera"></param>
    /// <param name="refractionCamera"></param>
	private void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera refractionCamera)
	{
		AmbienceSampler.WaterMode waterMode = this.GetWaterMode();
		reflectionCamera = null;
		refractionCamera = null;
		if (waterMode >= AmbienceSampler.WaterMode.Reflective)
		{
			if (!this.m_ReflectionTexture || this.m_OldReflectionTextureSize != this.m_TextureSize)
			{
				if (this.m_ReflectionTexture)
				{
					DelegateProxy.DestroyObjectImmediate(this.m_ReflectionTexture);
				}
				this.m_ReflectionTexture = new RenderTexture(this.m_TextureSize, this.m_TextureSize, 16);
				this.m_ReflectionTexture.name = "__WaterReflection" + base.GetInstanceID();
				this.m_ReflectionTexture.isPowerOfTwo = true;
				this.m_ReflectionTexture.hideFlags = HideFlags.DontSave;
				this.m_ReflectionTexture.format = RenderTextureFormat.ARGB32;
				this.m_OldReflectionTextureSize = this.m_TextureSize;
			}
			this.m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
			if (!reflectionCamera)
			{
				GameObject gameObject = new GameObject(string.Concat(new object[]
				{
					"Water Refl Camera id",
					base.GetInstanceID(),
					" for ",
					currentCamera.GetInstanceID()
				}), new Type[]
				{
					typeof(Camera),
					typeof(Skybox)
				});
				reflectionCamera = gameObject.camera;
				reflectionCamera.enabled = false;
				reflectionCamera.transform.position = base.transform.position;
				reflectionCamera.transform.rotation = base.transform.rotation;
				reflectionCamera.gameObject.AddComponent("FlareLayer");
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				this.m_ReflectionCameras[currentCamera] = reflectionCamera;
			}
		}
		if (waterMode >= AmbienceSampler.WaterMode.Refractive)
		{
			if (!this.m_RefractionTexture || this.m_OldRefractionTextureSize != this.m_TextureSize)
			{
				if (this.m_RefractionTexture)
				{
					DelegateProxy.DestroyObjectImmediate(this.m_RefractionTexture);
				}
				this.m_RefractionTexture = new RenderTexture(this.m_TextureSize, this.m_TextureSize, 16);
				this.m_RefractionTexture.name = "__WaterRefraction" + base.GetInstanceID();
				this.m_ReflectionTexture.isPowerOfTwo = true;
				this.m_RefractionTexture.hideFlags = HideFlags.DontSave;
				this.m_RefractionTexture.format = RenderTextureFormat.ARGB32;
				this.m_OldRefractionTextureSize = this.m_TextureSize;
			}
			this.m_RefractionCameras.TryGetValue(currentCamera, out refractionCamera);
			if (!refractionCamera)
			{
				GameObject gameObject2 = new GameObject(string.Concat(new object[]
				{
					"Water Refr Camera id",
					base.GetInstanceID(),
					" for ",
					currentCamera.GetInstanceID()
				}), new Type[]
				{
					typeof(Camera),
					typeof(Skybox)
				});
				refractionCamera = gameObject2.camera;
				refractionCamera.enabled = false;
				refractionCamera.transform.position = base.transform.position;
				refractionCamera.transform.rotation = base.transform.rotation;
				refractionCamera.gameObject.AddComponent("FlareLayer");
				gameObject2.hideFlags = HideFlags.HideAndDontSave;
				this.m_RefractionCameras[currentCamera] = refractionCamera;
			}
		}
	}
    /// <summary>
    /// 获取水体模式
    /// </summary>
    /// <returns></returns>
	private AmbienceSampler.WaterMode GetWaterMode()
	{
		if (this.m_HardwareWaterSupport < this.m_WaterMode)
		{
			return this.m_HardwareWaterSupport;
		}
		return this.m_WaterMode;
	}
    /// <summary>
    /// 符号判断
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
	private static float sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 v = pos + normal * this.m_ClipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(v);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, -Vector3.Dot(lhs, rhs));
	}

	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 b = projection.inverse * new Vector4(AmbienceSampler.sgn(clipPlane.x), AmbienceSampler.sgn(clipPlane.y), 1f, 1f);
		Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
		projection[2] = vector.x - projection[3];
		projection[6] = vector.y - projection[7];
		projection[10] = vector.z - projection[11];
		projection[14] = vector.w - projection[15];
	}

	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}
}
