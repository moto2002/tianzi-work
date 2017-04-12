using System;
using System.IO;
using UnityEngine;
/// <summary>
/// 水体数据
/// </summary>
public class WaterData
{
    /// <summary>
    /// 环境采样ID
    /// </summary>
	public int ambienceSamplerID;
    /// <summary>
    /// 水体高度
    /// </summary>
	public float height;
    /// <summary>
    /// 水波纹速度
    /// </summary>
	public Vector4 waveSpeed = new Vector4(2f, 2f, -2f, -2f);
    /// <summary>
    /// 水波缩放因子
    /// </summary>
	public float waveScale = 0.02f;
    /// <summary>
    /// 水体垂直颜色
    /// </summary>
	public Color horizonColor;
    /// <summary>
    /// 水体深度图资源路径
    /// </summary>
	public string depthMapPath = string.Empty;
    /// <summary>
    /// 控制色资源路径
    /// </summary>
	public string colorControlPath = string.Empty;
    /// <summary>
    /// 水体凹凸贴图路径
    /// </summary>
	public string waterBumpMapPath = string.Empty;
    /// <summary>
    /// 水体可视深度
    /// </summary>
	public float waterVisibleDepth;
    /// <summary>
    /// 水体漫反射值
    /// </summary>
	public float waterDiffValue;
    /// <summary>
    /// 反射扭曲因子
    /// </summary>
	public float reflDistort = 0.44f;
    /// <summary>
    /// 折射
    /// </summary>
	public float refrDistort = 0.2f;
    /// <summary>
    /// 控制颜色贴图纹理
    /// </summary>
	public Texture2D colorControlMap;
    /// <summary>
    /// 水体凹凸贴图纹理
    /// </summary>
	public Texture2D bumpMap;
    /// <summary>
    /// 水体深度贴图纹理
    /// </summary>
	public Texture2D depthMap;
    /// <summary>
    /// 水体透明度
    /// </summary>
	public float alpha = 1f;
    /// <summary>
    /// 解析读取属性信息
    /// </summary>
    /// <param name="br"></param>
	public void Read(BinaryReader br)
	{
		this.ambienceSamplerID = br.ReadInt32();
		this.height = br.ReadSingle();
		this.waveSpeed.x = br.ReadSingle();
		this.waveSpeed.y = br.ReadSingle();
		this.waveSpeed.z = br.ReadSingle();
		this.waveSpeed.w = br.ReadSingle();
		this.waveScale = br.ReadSingle();
		this.horizonColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
		this.waterVisibleDepth = br.ReadSingle();
		this.waterDiffValue = br.ReadSingle();
		long position = br.BaseStream.Position;
		if (br.ReadInt32() == 10015)
		{
			this.alpha = br.ReadSingle();
		}
		else
		{
			br.BaseStream.Position = position;
		}
		this.depthMapPath = br.ReadString();
		this.colorControlPath = br.ReadString();
		this.waterBumpMapPath = br.ReadString();
		this.colorControlMap = AssetLibrary.Load(this.colorControlPath, AssetType.Texture2D, LoadType.Type_Resources).texture2D;
		this.bumpMap = AssetLibrary.Load(this.waterBumpMapPath, AssetType.Texture2D, LoadType.Type_Resources).texture2D;
		this.depthMap = AssetLibrary.Load(this.depthMapPath, AssetType.Texture2D, LoadType.Type_Auto).texture2D;
	}
    /// <summary>
    /// 释放回收
    /// </summary>
	public void Release()
	{
		this.colorControlMap = null;
		this.bumpMap = null;
		this.depthMap = null;
	}
}
