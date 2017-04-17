using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 后期处理效果基类，其他效果皆实现基于此
/// </summary>
[ExecuteInEditMode]
public class PEBase : MonoBehaviour
{
	protected bool supportHDRTextures = true;

	protected bool supportDX11;

	protected bool isSupported = true;

	protected Material m_Material;

	protected Dictionary<string, int> _matParams;
    /// <summary>
    /// 效果相关的材质
    /// </summary>
	public virtual Material material
	{
		get
		{
			return null;
		}
	}
    /// <summary>
    /// 材质相关的属相列表
    /// </summary>
	public virtual Dictionary<string, int> matParams
	{
		get
		{
			return null;
		}
	}
    /// <summary>
    /// 加载参数属性
    /// </summary>
	public virtual void LoadParams()
	{
	}
    /// <summary>
    /// 检查shader,并创建效果相关的材质
    /// </summary>
    /// <param name="s"></param>
    /// <param name="m2Create"></param>
    /// <returns></returns>
	public Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		if (s == null)
		{
			Debug.Log("Missing shader in " + base.name);
			base.enabled = false;
			return null;
		}
		if (s.isSupported && m2Create && m2Create.shader == s)
		{
			return m2Create;
		}
		if (!s.isSupported)    //不支持，材质为空
		{
			this.NotSupported();
			Debug.Log(string.Concat(new string[]
			{
				"The shader ",
				s.ToString(),
				" on effect ",
				this.ToString(),
				" is not supported on this platform!"
			}));
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		if (m2Create)
		{
			return m2Create;
		}
		return null;
	}
    /// <summary>
    /// 同上，创建后期处理相关的材质
    /// </summary>
    /// <param name="s"></param>
    /// <param name="m2Create"></param>
    /// <returns></returns>
	public Material CreateMaterial(Shader s, Material m2Create)
	{
		if (s == null)
		{
			Debug.Log("Missing shader in " + this.ToString());
			return null;
		}
		if (m2Create != null && m2Create.shader == s && s.isSupported)
		{
			return m2Create;
		}
		if (!s.isSupported)
		{
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		if (m2Create)
		{
			return m2Create;
		}
		return null;
	}
    /// <summary>
    /// 检查是否支持
    /// </summary>
    /// <returns></returns>
	public bool CheckSupport()
	{
		return this.CheckSupport(false);
	}
    /// <summary>
    /// 检查是否支持深度支持
    /// </summary>
    /// <param name="needDepth"></param>
    /// <returns></returns>
	public bool CheckSupport(bool needDepth)
	{
		this.isSupported = true;
		this.supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
		this.supportDX11 = (SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders);
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)   //判断是否支持后期处理效果
		{
			this.NotSupported();
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth)) //是否支持深度检测
		{
			this.NotSupported();
			return false;
		}
		if (needDepth)
		{
			base.camera.depthTextureMode |= DepthTextureMode.Depth;   //开始摄像机深度贴图模式
		}
		return true;
	}
    /// <summary>
    /// 检测深度和hdr支持
    /// </summary>
    /// <param name="needDepth"></param>
    /// <param name="needHdr"></param>
    /// <returns></returns>
	public bool CheckSupport(bool needDepth, bool needHdr)
	{
		if (!this.CheckSupport(needDepth))           //深度支持检测
		{
			return false;
		}
		if (needHdr && !this.supportHDRTextures)     //hdr支持检测
		{
			this.NotSupported();
			return false;
		}
		return true;
	}
    /// <summary>
    /// dx11支持检测
    /// </summary>
    /// <returns></returns>
	public bool Dx11Support()
	{
		return this.supportDX11;
	}
    /// <summary>
    /// 自动禁用报告
    /// </summary>
	public void ReportAutoDisable()
	{
		Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
	}
    /// <summary>
    /// 检测shader支持
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
	public bool CheckShader(Shader s)
	{
		Debug.Log(string.Concat(new string[]
		{
			"The shader ",
			s.ToString(),
			" on effect ",
			this.ToString(),
			" is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package."
		}));
		if (!s.isSupported)
		{
			this.NotSupported();
			return false;
		}
		return false;
	}
    /// <summary>
    /// 不支持，禁用效果
    /// </summary>
	public void NotSupported()
	{
		base.enabled = false;
		this.isSupported = false;
	}
    /// <summary>
    /// 绘制边框
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="material"></param>
	public void DrawBorder(RenderTexture dest, Material material)
	{
		RenderTexture.active = dest;
		bool flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			float y;
			float y2;
			if (flag)
			{
				y = 1f;
				y2 = 0f;
			}
			else
			{
				y = 0f;
				y2 = 1f;
			}
			float x = 0f;
			float x2 = 0f + 1f / ((float)dest.width * 1f);
			float y3 = 0f;
			float y4 = 1f;
			GL.Begin(7);
			GL.TexCoord2(0f, y);
			GL.Vertex3(x, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x, y4, 0.1f);
			x = 1f - 1f / ((float)dest.width * 1f);
			x2 = 1f;
			y3 = 0f;
			y4 = 1f;
			GL.TexCoord2(0f, y);
			GL.Vertex3(x, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x, y4, 0.1f);
			x = 0f;
			x2 = 1f;
			y3 = 0f;
			y4 = 0f + 1f / ((float)dest.height * 1f);
			GL.TexCoord2(0f, y);
			GL.Vertex3(x, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x, y4, 0.1f);
			x = 0f;
			x2 = 1f;
			y3 = 1f - 1f / ((float)dest.height * 1f);
			y4 = 1f;
			GL.TexCoord2(0f, y);
			GL.Vertex3(x, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x, y4, 0.1f);
			GL.End();
		}
		GL.PopMatrix();
	}
}
