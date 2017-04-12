using System;
using UnityEngine;
/// <summary>
/// 光照贴图原型，包含相关属性信息
/// </summary>
public class LightmapPrototype
{
	public int rendererChildIndex = -1;

	public float scale = 1f;
    /// <summary>
    /// 光照贴图索引
    /// </summary>
	public int lightmapIndex = -1;
    /// <summary>
    /// 光照贴图偏移量
    /// </summary>
	public Vector4 lightmapTilingOffset;

	public int size = 16;
}
