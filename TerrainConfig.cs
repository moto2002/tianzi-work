using System;
using UnityEngine;

public class TerrainConfig
{
    /// <summary>
    /// 场景宽度
    /// </summary>
	public int sceneWidth = 480;
    /// <summary>
    /// 场景高度
    /// </summary>
	public int sceneHeight = 480;
    /// <summary>
    /// 场景高度图分辨率
    /// </summary>
	public float sceneHeightmapResolution = 480f;
    /// <summary>
    /// 基础splat
    /// </summary>
	public Splat baseSplat = new Splat();
    /// <summary>
    /// 摄像机观察点
    /// </summary>
	public Vector3 cameraLookAt = Vector3.zero;
    /// <summary>
    /// 相机距离
    /// </summary>
	public float cameraDistance;
    /// <summary>
    /// 相机的x旋转
    /// </summary>
	public float cameraRotationX;
    /// <summary>
    /// 摄像机的Y旋转
    /// </summary>
	public float cameraRotationY;
    /// <summary>
    /// 摄像机的z旋转
    /// </summary>
	public float cameraRotationZ;
    /// <summary>
    /// 雾效颜色
    /// </summary>
	public Color fogColor = new Color(0.266666681f, 0.8156863f, 1f);
    /// <summary>
    /// 雾效的开始距离
    /// </summary>
	public float startDistance = 6f;
    /// <summary>
    /// 雾效的全局密度
    /// </summary>
	public float globalDensity = 1f;
    /// <summary>
    /// 雾效的高度缩放率
    /// </summary>
	public float heightScale = 1f;
    /// <summary>
    /// 雾效高度
    /// </summary>
	public float height = 110f;
    /// <summary>
    /// 雾效强度
    /// </summary>
	public Vector4 fogIntensity = new Vector4(1f, 1f, 1f, 1f);
    /// <summary>
    /// tile瓦片的尺寸
    /// </summary>
	public int tileSize = 32;
    /// <summary>
    /// 一个Region区域每边的tile数
    /// </summary>
	public int tileCountPerSide;
    /// <summary>
    /// 一个Region包含的tile数
    /// </summary>
	public int tileCountPerRegion;
    /// <summary>
    /// region区域的尺寸
    /// </summary>
	public int regionSize = 160;
    /// <summary>
    /// 高度图分辨率
    /// </summary>
	public int heightmapResolution = 32;
    /// <summary>
    /// 水效果深度图分辨率
    /// </summary>
	public int waterDepthmapResolution = 64;
    /// <summary>
    /// 网格分辨力？？？
    /// </summary>
	public int gridResolution = 32;
    /// <summary>
    /// splatmap分辨率???
    /// </summary>
	public int splatmapResolution = 32;
    /// <summary>
    /// splatmap(layer)层数 最多为4
    /// </summary>
	public int spaltsmapLayers = 4;

	public float blockHeight = 1f;
    /// <summary>
    /// 地形最大高度
    /// </summary>
	public float maxReachTerrainHeight = 200f;
    /// <summary>
    /// 网格尺寸
    /// </summary>
	public float gridSize = 1f;
    /// <summary>
    /// 地形默认高度值
    /// </summary>
	public float defaultTerrainHeight = 50f;
    /// <summary>
    /// 地形最大高度值
    /// </summary>
	public float maxTerrainHeight = 200f;
    /// <summary>
    /// 瓦片剔除距离
    /// </summary>
	public float tileCullingDistance = 100f;
    /// <summary>
    /// unit剔除距离
    /// </summary>
	public float unitCullingDistance = 30f;
    /// <summary>
    /// 基本剔除距离
    /// </summary>
	public float cullingBaseDistance = 10f;
    /// <summary>
    /// 剔除角度因子
    /// </summary>
	public float cullingAngleFactor = 3f;
    /// <summary>
    /// 视角LOD因子????/
    /// </summary>
	public float viewAngleLodFactor = 2f;
    /// <summary>
    /// 动态剔除距离
    /// </summary>
	public float dynamiCullingDistance = 15f;
    /// <summary>
    /// 默认剔除因子
    /// </summary>
	public float defautCullingFactor = 2f;
    /// <summary>
    /// 太阳光/平行光光照方向
    /// </summary>
	public Vector4 sunLightDir = new Vector4(-0.41f, 0.74f, 0.18f, 0f);
    /// <summary>
    /// 水体镜面(specular）反射范围
    /// </summary>
	public float waterSpecRange = 46.3f;
    /// <summary>
    /// 水体镜面(specular）反射强度
    /// </summary>
	public float waterSpecStrength = 0.84f;
    /// <summary>
    /// 水体(diffuse)漫反射范围
    /// </summary>
	public float waterDiffRange;
    /// <summary>
    /// 水体(diffuse)漫反射强度
    /// </summary>
	public float waterDiffStrength;
    /// <summary>
    /// 水波速度
    /// </summary>
	public Vector4 waveSpeed = new Vector4(2f, 2f, -2f, -2f);
    /// <summary>
    /// 水波缩放率
    /// </summary>
	public float waveScale = 0.02f;
    /// <summary>
    /// 垂直颜色
    /// </summary>
	public Color horizonColor;
    /// <summary>
    /// 控制纹理贴图
    /// </summary>
	public Texture2D colorControl;
    /// <summary>
    /// 水体凹凸贴图
    /// </summary>
	public Texture2D waterBumpMap;
    /// <summary>
    /// 默认水体高度
    /// </summary>
	public float defaultWaterHeight = 48f;
    /// <summary>
    /// 水体可视深度
    /// </summary>
	public float waterVisibleDepth = 0.5f;
    /// <summary>
    /// 水体透明度(alpha)
    /// </summary>
	public float waterAlpha = 1f;
    /// <summary>
    /// 反射扭曲
    /// </summary>
	public float reflDistort = 0.44f;
    /// <summary>
    /// 折射扭曲
    /// </summary>
	public float refrDistort = 0.2f;
    /// <summary>
    /// 水体漫反射值?????
    /// </summary>
	public float waterDiffValue;
    /// <summary>
    /// 碰撞计算范围
    /// </summary>
	public float collisionComputeRange = 3f;
    /// <summary>
    /// 是否启用点光源
    /// </summary>
	public bool enablePointLight = true;
    /// <summary>
    /// 点光源最小范围
    /// </summary>
	public float pointLightRangeMin = 2f;
    /// <summary>
    /// 点光源最大范围
    /// </summary>
	public float pointLightRangeMax = 5.6f;
    /// <summary>
    /// 点光源强度
    /// </summary>
	public float pointLightIntensity = 1f;
    /// <summary>
    /// 点光源颜色
    /// </summary>
	public Color pointLightColor = new Color(1f, 1f, 1f);
    /// <summary>
    /// 角色点光源位置
    /// </summary>
	public Vector3 rolePointLightPostion = new Vector3(-100.12f, -12.86f, 270.2f);
    /// <summary>
    /// 角色点光源颜色
    /// </summary>
	public Color rolePointLightColor = new Color(1f, 1f, 1f);
    /// <summary>
    /// 角色点光源范围
    /// </summary>
	public float rolePointLightRange = 19.7f;
    /// <summary>
    /// 角色点光源强度
    /// </summary>
	public float rolePointLightIntensity = 2.68f;
    /// <summary>
    /// 冷色颜色值
    /// </summary>
	public Color coolColor = new Color(1f, 1f, 1f, 1f);
    /// <summary>
    /// 暖色颜色值
    /// </summary>
	public Color warmColor = new Color(1f, 1f, 1f, 1f);
    /// <summary>
    /// 天气(值/类型？？？？)
    /// </summary>
	public int weather;

    /// <summary>
    /// 启用地形
    /// </summary>
	public bool enableTerrain = true;
}
