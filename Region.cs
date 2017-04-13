using System;
using System.IO;
using UnityEngine;

public class Region
{
    /// <summary>
    /// 当前游戏场景
    /// </summary>
	public GameScene scene;
    /// <summary>
    /// 当前Region的实际位置坐标
    /// </summary>
	public Vector3 postion;

	public int regionX = -1;

	public int regionY = -1;
    /// <summary>
    /// 当前Region的实际x坐标
    /// </summary>
	public float actualX;
    /// <summary>
    /// 当前的Region的实际z坐标
    /// </summary>
	public float actualY;
    /// <summary>
    /// 光照贴图数量
    /// </summary>
	public int lightmapCount;
    /// <summary>
    /// 当前Region包含的tile数组
    /// </summary>
	public Tile[] tiles;
    /// <summary>
    /// 当前Region的数据读取路径
    /// </summary>
	public string regionDataPath = string.Empty;
    /// <summary>
    /// Region局部x坐标
    /// </summary>
	private float localX;
    /// <summary>
    /// Region局部z坐标
    /// </summary>
	private float localY;
    /// <summary>
    /// tile网格的row坐标
    /// </summary>
	private int r;
    /// <summary>
    /// tile网格的colume坐标
    /// </summary>
	private int c;
    /// <summary>
    /// 获取tile时临时记录获取tile在tile数组中的索引
    /// </summary>
	private int tileInd;
    /// <summary>
    /// 视线距离判断时临时存储x方向的距离
    /// </summary>
	private float dx;
    /// <summary>
    /// 视线距离判断时临时存储z方向的距离
    /// </summary>
	private float dz;
    /// <summary>
    /// 获取指定位置所在的tile
    /// </summary>
    /// <param name="worldPosition">世界坐标？？？？</param>
    /// <returns></returns>
	public Tile GetTile(Vector3 worldPosition)
	{
		try
		{
			this.localX = worldPosition.x - this.actualX + (float)this.scene.terrainConfig.regionSize * 0.5f;
			this.localY = worldPosition.z - this.actualY + (float)this.scene.terrainConfig.regionSize * 0.5f;
			this.r = Mathf.FloorToInt(this.localX / (float)this.scene.terrainConfig.tileSize);
			this.c = Mathf.FloorToInt(this.localY / (float)this.scene.terrainConfig.tileSize);
			if (this.r > this.scene.terrainConfig.tileCountPerSide)
			{
				this.r = this.scene.terrainConfig.tileCountPerSide;
			}
			if (this.c > this.scene.terrainConfig.tileCountPerSide)
			{
				this.c = this.scene.terrainConfig.tileCountPerSide;
			}
			this.tileInd = this.c * this.scene.terrainConfig.tileCountPerSide + this.r;
			if (this.tileInd > this.tiles.Length - 1)  //越界，null
			{
				Tile result = null;
				return result;
			}
			if (this.tiles[this.tileInd] != null)
			{
				Tile result = this.tiles[this.tileInd];
				return result;
			}
		}
		catch (Exception ex)
		{
			LogSystem.Log(new object[]
			{
				"获取地形切片错误 :  " + ex.Message
			});
			Tile result = null;
			return result;
		}
		return null;
	}
    /// <summary>
    /// 获取指定tile坐标处的tile
    /// </summary>
    /// <param name="tileX">tile colume 坐标</param>
    /// <param name="tileY">tile row 坐标</param>
    /// <returns></returns>
	public Tile GetTile(int tileX, int tileY)
	{
		return this.tiles[tileX + tileY * this.scene.terrainConfig.tileCountPerSide];
	}
    /// <summary>
    /// 根据unitid获取gameobjecunit对象
    /// </summary>
    /// <param name="createID"></param>
    /// <returns></returns>
	public GameObjectUnit FindUint(int createID)
	{
		int num = this.tiles.Length;
		for (int i = 0; i < num; i++)
		{
			if (this.tiles[i] != null)
			{
				GameObjectUnit gameObjectUnit = this.tiles[i].FindUnit(createID);
				if (gameObjectUnit != null)
				{
					return gameObjectUnit;
				}
			}
		}
		return null;
	}
    /// <summary>
    /// 创建一个基础的Region对象
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="regionX"></param>
    /// <param name="regionY"></param>
    /// <returns></returns>
	public static Region Create(GameScene scene, int regionX, int regionY)
	{
		Region region = new Region();
		region.scene = scene;
		region.tiles = new Tile[scene.terrainConfig.tileCountPerRegion];
		region.regionX = regionX;
		region.regionY = regionY;
		region.regionDataPath = string.Concat(new object[]
		{
			"Scenes/",
			scene.sceneID,
			"/",
			regionX,
			"_",
			regionY,
			"/Region"
		});
		region.actualX = (float)(regionX * scene.terrainConfig.regionSize);
		region.actualY = (float)(regionY * scene.terrainConfig.regionSize);
		region.postion = Vector3.zero;
		region.postion.x = region.actualX;
		region.postion.y = 0f;
		region.postion.z = region.actualY;
		int num = Mathf.FloorToInt((float)scene.terrainConfig.tileCountPerSide * 0.5f);
		for (int i = 0; i < scene.terrainConfig.tileCountPerSide; i++)
		{
			for (int j = 0; j < scene.terrainConfig.tileCountPerSide; j++)
			{
				Tile tile = Tile.Create(region, i - num, j - num);
				region.tiles[j * scene.terrainConfig.tileCountPerSide + i] = tile;
			}
		}
		return region;
	}
    /// <summary>
    /// 更新Region内的tile的视野
    /// </summary>
	public void UpdateViewRange()
	{
		for (int i = 0; i < this.tiles.Length; i++)
		{
			if (this.tiles[i] != null)
			{
				this.tiles[i].UpdateViewRange();
			}
		}
	}
    /// <summary>
    /// 读取数据赋值相关Region的属性值
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
	public Region Read(byte[] bytes)
	{
		MemoryStream memoryStream = new MemoryStream(bytes);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		long position = binaryReader.BaseStream.Position;
		int num;
		if (binaryReader.ReadInt32() == 10013)
		{
			num = binaryReader.ReadInt32();
		}
		else
		{
			num = GameScene.mainScene.sceneID;
			binaryReader.BaseStream.Position = position;
		}
		this.scene = GameScene.mainScene;
		this.tiles = new Tile[this.scene.terrainConfig.tileCountPerRegion];
		this.regionX = binaryReader.ReadInt32();
		this.regionY = binaryReader.ReadInt32();
		this.regionDataPath = string.Concat(new object[]
		{
			"Scenes/",
			num ,
			"/",
			this.regionX,
			"_",
			this.regionY,
			"/Region"
		});
		this.actualX = (float)(this.regionX * this.scene.terrainConfig.regionSize);
		this.actualY = (float)(this.regionY * this.scene.terrainConfig.regionSize);
		this.lightmapCount = binaryReader.ReadInt32();
		int num2 = 0;
		int num3 = Mathf.FloorToInt((float)this.scene.terrainConfig.tileCountPerSide * 0.5f);
		while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length && binaryReader.ReadString() == "Tile")
		{
			Tile tile = new Tile(this);
			tile.Read(binaryReader);
			int num4 = tile.tileX + num3;
			int num5 = tile.tileY + num3;
			this.tiles[num5 * this.scene.terrainConfig.tileCountPerSide + num4] = tile;
			num2++;
			if (num2 >= this.scene.terrainConfig.tileCountPerRegion)
			{
				break;
			}
		}
		this.postion = Vector3.zero;
		this.postion.x = this.actualX;
		this.postion.y = 0f;
		this.postion.z = this.actualY;
		binaryReader.Close();
		memoryStream.Flush();
		return this;
	}
    /// <summary>
    /// 销毁回收
    /// </summary>
	public void Destroy()
	{
		for (int i = 0; i < this.tiles.Length; i++)
		{
			Tile tile = this.tiles[i];
			if (tile != null)
			{
				if (this.scene.ContainTile(tile))//包含在场景中
				{
					this.scene.RemoveTile(tile, true);//移除并销毁
				}
				else
				{
					tile.Destroy();
				}
			}
		}
		this.tiles = null;
		this.scene = null;
	}
    /// <summary>
    /// 根据视点位置，更新场景中的tile列表
    /// </summary>
    /// <param name="eyePos"></param>
	public void Update(Vector3 eyePos)
	{
		if (this.tiles == null)
		{
			return;
		}
		int num = this.tiles.Length;
		for (int i = 0; i < num; i++)//遍历Region包含的所有tile
		{
			if (this.tiles[i] != null && !this.tiles[i].visible && !this.scene.ContainTile(this.tiles[i])) //不可视的tile,并且不在场景tile列表中
			{
				this.dx = eyePos.x - this.tiles[i].position.x;
				this.dz = eyePos.z - this.tiles[i].position.z;
				this.tiles[i].viewDistance = Mathf.Sqrt(this.dx * this.dx + this.dz * this.dz);
				if (this.tiles[i].viewDistance < this.tiles[i].far) //在剔除范围内，才加入场景tile列表
				{
					this.scene.AddTile(this.tiles[i]);
				}
			}
		}
	}
}
