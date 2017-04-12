using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapPath
{
	public delegate void PathFindEndBack(List<Vector3> paths);

	private const int baseValue = 2048;

	public int width;

	public int height;

	private float gridSize;

	public int halfWidth;

	public int halfHeight;

	public int[,] grids;

	public int maxDynamicCollisizeSize = 2;

	public int gridTypeMask = 5;

	private int fullMask;

	private int[,] path;

	private List<HValue> HValueCache = new List<HValue>();

	private int cacheIndex;

	private int cacheCount = 10000;

	private HValue nearestTarget;

	private Heap[] heap = new Heap[2];

	private int computeTick;

	public bool pathFindResult = true;

	private HValue curPathValue;

	private int[] heapX = new int[2];

	private int[] heapY = new int[2];

	private int markBase = 10;

	private int[] dx = new int[8];

	private int[] dy = new int[8];

	private List<HValue> pathHValues = new List<HValue>();

	public bool pathFindEnd = true;

	public Vector3 pfStartPoint;

	public Vector3 pfEndPoint;

	public int pfCollisionSize;

	public List<Vector3> paths = new List<Vector3>();

	public MapPath.PathFindEndBack pathFindEndBack;

	private Dictionary<int, float> distanceRecord = new Dictionary<int, float>();

	private List<Vector2> tryPath = new List<Vector2>();

	private List<int> pointsList = new List<int>();

	public MapPath(float gridSize, int width = 0, int height = 0)
	{
		for (int i = 0; i < this.maxDynamicCollisizeSize; i++)
		{
			this.fullMask |= 1 << i;
		}
		this.width = width;
		this.height = height;
		this.halfWidth = width / 2;
		this.halfHeight = height / 2;
		this.gridSize = gridSize;
		this.grids = new int[width, height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				this.grids[i, j] = 0;
			}
		}
		if (this.path == null)
		{
			this.path = new int[this.width, this.height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					this.path[i, j] = -1;
				}
			}
		}
		for (int i = 0; i < this.cacheCount; i++)
		{
			this.HValueCache.Add(new HValue());
		}
	}

	private HValue getHValue(int x, int y)
	{
		if (this.path[x, y] == -1)
		{
			if (this.cacheIndex == this.cacheCount)
			{
				for (int i = 0; i < 1000; i++)
				{
					HValue hValue = new HValue();
					hValue.Reset();
					this.HValueCache.Add(hValue);
				}
				this.cacheCount += 1000;
			}
			this.path[x, y] = this.cacheIndex;
			this.cacheIndex++;
		}
		return this.HValueCache[this.path[x, y]];
	}

	public void Init()
	{
	}

	public void Reset()
	{
	}

	public void Read(BinaryReader br)
	{
		this.width = br.ReadInt32();
		this.height = br.ReadInt32();
		this.halfWidth = this.width / 2;
		this.halfHeight = this.height / 2;
		this.grids = new int[this.width, this.height];
		for (int i = 0; i < this.width; i++)
		{
			for (int j = 0; j < this.height; j++)
			{
				this.grids[i, j] = br.ReadInt32();
			}
		}
	}

	public int[,] CheckCustomGrids(int[,] customGrids)
	{
		int length = customGrids.GetLength(0);
		List<int> list = new List<int>();
		for (int i = 0; i < length; i++)
		{
			int num = customGrids[i, 0];
			int num2 = customGrids[i, 1];
			if (this.grids[num, num2] != 3)
			{
				list.Add(num);
				list.Add(num2);
			}
		}
		customGrids = new int[list.Count / 2, 2];
		int num3 = 0;
		for (int i = 0; i < list.Count; i++)
		{
			int num = list[i];
			int num2 = list[i + 1];
			i++;
			customGrids[num3, 0] = num;
			customGrids[num3, 1] = num2;
			num3++;
		}
		return customGrids;
	}

	public void SetDynamicCollision(Vector3 worldPostion, int[,] customGrids, bool isRemove = false, int type = 0)
	{
		int length = customGrids.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = customGrids[i, 0];
			int num2 = customGrids[i, 1];
			if (!isRemove)
			{
				if (this.grids[num, num2] != 1 && this.grids[num, num2] != 2)
				{
					this.grids[num, num2] = 1;
					for (int j = 1; j < this.maxDynamicCollisizeSize; j++)
					{
						this.grids[num, num2] |= 1 << j;
					}
				}
			}
			else if ((this.grids[num, num2] & 1) > 0)
			{
				this.grids[num, num2] = 0;
			}
		}
	}

	public void SetDynamicCollision(Vector3 worldPostion, int size = 1, bool isRemove = false, int type = 1)
	{
		int num = Mathf.FloorToInt(worldPostion.x / this.gridSize) + this.halfWidth;
		int num2 = Mathf.FloorToInt(worldPostion.z / this.gridSize) + this.halfHeight;
		for (int i = -size; i <= size; i++)
		{
			for (int j = -size; j <= size; j++)
			{
				if (this.grids[num + i, num2 + j] >> this.gridTypeMask >= 1 || this.grids[num + i, num2 + j] <= 0)
				{
					if (!isRemove)
					{
						this.grids[num + i, num2 + j] = 1;
						for (int k = 1; k < this.maxDynamicCollisizeSize; k++)
						{
							this.grids[num + i, num2 + j] |= 1 << k;
						}
						this.grids[num + i, num2 + j] |= type << this.gridTypeMask;
					}
					else if (this.grids[num + i, num2 + j] > 0)
					{
						this.grids[num + i, num2 + j] = 0;
					}
				}
			}
		}
	}

	public void PretestWalkBlocker(int x, int y, int type = 0)
	{
		bool flag = (this.grids[x, y] & 1) > 0;
		int i;
		for (i = 1; i <= this.maxDynamicCollisizeSize; i++)
		{
			if (flag)
			{
				break;
			}
			for (int j = -i; j <= i; j++)
			{
				int num = x - i;
				int num2 = y + j;
				if (num >= 0 && num < this.width && num2 >= 0 && num2 < this.height)
				{
					int num3 = this.grids[num, num2];
					if ((num3 & 1) > 0)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
			for (int k = -i; k <= i; k++)
			{
				int num = x + i;
				int num2 = y + k;
				if (num >= 0 && num < this.width && num2 >= 0 && num2 < this.height)
				{
					int num3 = this.grids[num, num2];
					if ((num3 & 1) > 0)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
			for (int l = -i; l <= i; l++)
			{
				int num = x + l;
				int num2 = y + i;
				if (num >= 0 && num < this.width && num2 >= 0 && num2 < this.height)
				{
					int num3 = this.grids[num, num2];
					if ((num3 & 1) > 0)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
			for (int m = -i; m <= i; m++)
			{
				int num = x + m;
				int num2 = y - i;
				if (num >= 0 && num < this.width && num2 >= 0 && num2 < this.height)
				{
					int num3 = this.grids[num, num2];
					if ((num3 & 1) > 0)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
			if ((this.grids[x, y] & 1 << i) != 0)
			{
				this.grids[x, y] -= 1 << i;
			}
		}
		if (flag)
		{
			for (int n = i; n < this.maxDynamicCollisizeSize; n++)
			{
				this.grids[x, y] |= 1 << n;
			}
			this.grids[x, y] |= type << this.gridTypeMask;
		}
		else if ((this.grids[x, y] & this.fullMask) == 0)
		{
			this.grids[x, y] = 0;
		}
	}

	public bool IsValidForWalk(int x, int y, int collisionSize)
	{
		bool result;
		try
		{
			if (x > this.width || y > this.height)
			{
				LogSystem.LogWarning(new object[]
				{
					"Terrain array Error!!!"
				});
				result = false;
			}
			else
			{
				int num = this.grids[x, y] & 1 << collisionSize;
				if (num < 1)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
		}
		catch (Exception)
		{
			LogSystem.LogWarning(new object[]
			{
				"Find path isValidForWalk wrong!"
			});
			result = false;
		}
		return result;
	}

	public bool IsValidForWalk(Vector3 postion, int collisionSize)
	{
		int num = Mathf.FloorToInt(postion.x / this.gridSize);
		int num2 = Mathf.FloorToInt(postion.z / this.gridSize);
		int num3 = num + this.halfWidth;
		int num4 = num2 + this.halfHeight;
		int num5 = this.grids[num3, num4] & 1 << collisionSize;
		return num5 < 1;
	}

	public bool IsValidForWalk(float worldGridX, float worldGridZ, int collisionSize)
	{
		int num = Mathf.FloorToInt(worldGridX);
		int num2 = Mathf.FloorToInt(worldGridZ);
		if (Math.Abs(num) > this.halfWidth)
		{
			return false;
		}
		if (Math.Abs(num2) > this.halfHeight)
		{
			return false;
		}
		int num3 = num + this.halfWidth;
		int num4 = num2 + this.halfHeight;
		int num5 = this.grids[num3, num4] & 1 << collisionSize;
		return num5 < 1;
	}

	public void PrepareForPathSearch()
	{
		for (int i = 0; i < this.width; i++)
		{
			for (int j = 0; j < this.height; j++)
			{
				this.PretestWalkBlocker(i, j, 0);
			}
		}
	}

	public void Clear()
	{
		this.pathFindEnd = true;
		this.pathFindEndBack = null;
	}

	private int Sign(double value)
	{
		if (Math.Abs(value) < 0.0010000000474974513)
		{
			return 0;
		}
		if (value > 0.0)
		{
			return 1;
		}
		if (value < 0.0)
		{
			return -1;
		}
		return 0;
	}

	private float Distance(int x, int y, int tx, int ty)
	{
		return (float)Math.Sqrt((double)((x - tx) * (x - tx) + (y - ty) * (y - ty)));
	}

	private float HeapHFunction(int nx, int ny, int gx, int gy)
	{
		float num = (float)Math.Min(Math.Abs(nx - gx), Math.Abs(ny - gy));
		float num2 = (float)(Math.Abs(nx - gx) + Math.Abs(ny - gy));
		return 1.414f * num + 1f * (num2 - 2f * num);
	}

	public void RequestPaths(Vector3 startPoint, Vector3 endPoint, int collisionSize, out List<Vector3> paths)
	{
		this.RequestPaths(startPoint, endPoint, collisionSize, null, 2000000);
		paths = this.paths;
	}

	public void RequestPaths(Vector3 startPoint, Vector3 endPoint, int collisionSize, MapPath.PathFindEndBack pathFindEndBack = null, int maxComputeCount = 8000)
	{
		try
		{
			this.pfStartPoint = startPoint;
			this.pfEndPoint = endPoint;
			this.pfCollisionSize = collisionSize;
			this.pathFindEnd = false;
			this.pathFindResult = true;
			this.pathFindEndBack = pathFindEndBack;
			this.paths.Clear();
			this.computeTick = 0;
			this.heapX = new int[2];
			this.heapY = new int[2];
			this.heapX[0] = Mathf.FloorToInt(startPoint.x / this.gridSize) + this.halfWidth;
			this.heapY[0] = Mathf.FloorToInt(startPoint.z / this.gridSize) + this.halfHeight;
			this.heapX[1] = Mathf.FloorToInt(endPoint.x / this.gridSize) + this.halfWidth;
			this.heapY[1] = Mathf.FloorToInt(endPoint.z / this.gridSize) + this.halfHeight;
			if (this.heapX[0] == this.heapX[1] && this.heapY[0] == this.heapY[1])
			{
				this.paths.Add(endPoint);
				this.pathFindEnd = true;
				if (pathFindEndBack != null)
				{
					pathFindEndBack(this.paths);
				}
			}
			else
			{
				for (int i = 0; i < 2; i++)
				{
					if (this.heap[i] == null)
					{
						this.heap[i] = new Heap();
					}
					this.heap[i].Clear();
				}
				for (int i = 0; i < this.HValueCache.Count; i++)
				{
					this.HValueCache[i].Reset();
				}
				for (int i = 0; i < this.width; i++)
				{
					for (int j = 0; j < this.height; j++)
					{
						this.path[i, j] = -1;
					}
				}
				this.cacheIndex = 0;
				for (int k = 0; k < 2; k++)
				{
					int num = this.heapX[k];
					int num2 = this.heapY[k];
					for (int i = -1; i <= 1; i++)
					{
						for (int j = -1; j <= 1; j++)
						{
							int num3 = num + i;
							int num4 = num2 + j;
							Vector3 vector = new Vector3((float)(this.heapX[0] - this.halfWidth) * this.gridSize, 0f, (float)(this.heapY[0] - this.halfHeight) * this.gridSize);
							Vector3 vector2 = new Vector3((float)(this.heapX[1] - this.halfWidth) * this.gridSize, 0f, (float)(this.heapY[1] - this.halfHeight) * this.gridSize);
							Vector3 vector3 = (k != 0) ? vector2 : vector;
							Vector3 vector4 = new Vector3((float)(num3 - this.halfWidth) * this.gridSize, 0f, (float)(num4 - this.halfHeight) * this.gridSize);
							vector3.y = (vector4.y = 0f);
							float num5 = Vector3.Distance(vector3, vector4);
							float num6 = this.TryWalkDistance(vector3, vector4, collisionSize);
							if (Math.Abs(num6 - num5) < 0.01f)
							{
								HValue hValue = this.getHValue(num3, num4);
								hValue.Set((short)num3, (short)num4, num5, 0f);
								hValue.Mark = (short)(1 + k * this.markBase);
								hValue.PreX = -1;
								hValue.PreY = -1;
								this.heap[k].Push(hValue);
							}
						}
					}
				}
				this.nearestTarget = this.heap[0].Peek();
				int num7 = 8;
				this.dx = new int[num7];
				this.dy = new int[num7];
				for (int i = 0; i < num7; i++)
				{
					float num8 = (float)i * 2f * 3.14159274f / (float)num7;
					this.dx[i] = this.Sign(Math.Cos((double)num8));
					this.dy[i] = this.Sign(Math.Sin((double)num8));
				}
				this.pathHValues.Clear();
				this.RequestPathsImmed(this.pfStartPoint, this.pfEndPoint, this.pfCollisionSize, maxComputeCount);
			}
		}
		catch (Exception ex)
		{
			this.paths = new List<Vector3>();
			if (GameScene.isEditor)
			{
				LogSystem.Log(new object[]
				{
					string.Concat(new object[]
					{
						"寻路失败: 起始点 ",
						startPoint,
						" 目标点 : ",
						endPoint,
						" 错误消息 : ",
						ex.Message
					})
				});
			}
		}
	}

	public void Update()
	{
		if (!this.pathFindEnd)
		{
			this.RequestPathsImmed(this.pfStartPoint, this.pfEndPoint, this.pfCollisionSize, 8000);
		}
	}

	private void RequestPathsImmed(Vector3 startPoint, Vector3 endPoint, int collisionSize, int maxComputeCount = 8000)
	{
		while (!this.heap[0].IsEmpty || !this.heap[1].IsEmpty)
		{
			int num;
			if (this.heap[0].Count < this.heap[1].Count)
			{
				num = 0;
			}
			else
			{
				num = 1;
			}
			if (this.heap[0].Count == 0)
			{
				num = 1;
			}
			if (this.heap[1].Count == 0)
			{
				num = 0;
			}
			HValue hValue = this.heap[num].Pop();
			hValue.Mark = (short)(num * this.markBase + 2);
			for (int i = 0; i < 8; i++)
			{
				float num2 = (float)(this.dx[i] * this.dx[i] + this.dy[i] * this.dy[i]);
				num2 = (float)Math.Sqrt((double)num2);
				int num3 = (int)hValue.X + this.dx[i];
				int num4 = (int)hValue.Y + this.dy[i];
				bool flag = this.IsValidForWalk((int)hValue.X, (int)hValue.Y, collisionSize);
				if (Math.Abs(num2 - 1f) < 0.01f)
				{
					flag = this.IsValidForWalk(num3, num4, collisionSize);
				}
				else
				{
					for (int j = 0; j <= 1; j++)
					{
						for (int k = 0; k <= 1; k++)
						{
							flag = (flag && this.IsValidForWalk((int)hValue.X + j * this.dx[i], (int)hValue.Y + k * this.dy[i], collisionSize));
						}
					}
				}
				if (flag)
				{
					this.curPathValue = this.getHValue(num3, num4);
					if (this.curPathValue.Mark == -1)
					{
						float num5 = hValue.Cost + num2;
						float num6 = this.HeapHFunction(num3, num4, this.heapX[1 - num], this.heapY[1 - num]);
						this.curPathValue.Set((short)num3, (short)num4, num5, num5 + num6);
						this.curPathValue.PreX = hValue.X;
						this.curPathValue.PreY = hValue.Y;
						this.curPathValue.Mark = (short)(num * this.markBase + 1);
						this.heap[num].Push(this.curPathValue);
					}
					if ((int)this.curPathValue.Mark == num * this.markBase + 1)
					{
						float num7 = hValue.Cost + num2;
						if (num7 + 0.1f < this.curPathValue.Cost)
						{
							float num8 = this.Distance((int)hValue.X, (int)hValue.Y, num3, num4);
							int pos = this.heap[num].Find(this.curPathValue);
							this.curPathValue.Set((short)num3, (short)num4, num7, num7 + num8);
							this.curPathValue.PreX = hValue.X;
							this.curPathValue.PreY = hValue.Y;
							this.heap[num].Upper(pos);
						}
					}
					this.computeTick++;
					if (this.computeTick > maxComputeCount && i == 7)
					{
						this.computeTick = 0;
						return;
					}
					if (num == 0)
					{
						float num9 = this.Distance((int)this.nearestTarget.X, (int)this.nearestTarget.Y, this.heapX[1], this.heapY[1]);
						float num10 = this.Distance(num3, num4, this.heapX[1], this.heapY[1]);
						if (num10 < num9)
						{
							this.nearestTarget = this.curPathValue;
						}
						float num11 = this.nearestTarget.Cost + 8f * num9;
						float num12 = hValue.Cost + num10;
						if (num12 > num11 && this.heap[0].PushCount > 4096 && this.heap[1].IsEmpty)
						{
							hValue = this.nearestTarget;
							this.pathHValues.Add(hValue);
							while (hValue.PreX != -1 && hValue.PreY != -1)
							{
								this.pathHValues.Add(this.getHValue((int)hValue.PreX, (int)hValue.PreY));
								hValue = this.getHValue((int)hValue.PreX, (int)hValue.PreY);
							}
							for (int l = 0; l < this.pathHValues.Count / 2; l++)
							{
								HValue value = this.pathHValues[l];
								this.pathHValues[l] = this.pathHValues[this.pathHValues.Count - l - 1];
								this.pathHValues[this.pathHValues.Count - l - 1] = value;
							}
							Vector3 end = new Vector3((float)((int)this.nearestTarget.X - this.halfWidth) * this.gridSize, 0f, (float)((int)this.nearestTarget.Y - this.halfHeight) * this.gridSize);
							this.paths = this.BuildPath(this.pathHValues, startPoint, end, collisionSize);
							this.pathFindEnd = true;
							if (this.pathFindEndBack != null)
							{
								this.pathFindEndBack(this.paths);
							}
							return;
						}
					}
					if (this.curPathValue.Mark >= 0 && (int)(this.curPathValue.Mark / 10) != num)
					{
						HValue[] array = new HValue[2];
						array[num] = hValue;
						array[1 - num] = this.curPathValue;
						this.paths.Clear();
						hValue = array[0];
						this.pathHValues.Add(hValue);
						while (hValue.PreX != -1 && hValue.PreY != -1)
						{
							this.pathHValues.Add(this.getHValue((int)hValue.PreX, (int)hValue.PreY));
							hValue = this.getHValue((int)hValue.PreX, (int)hValue.PreY);
						}
						for (int l = 0; l < this.pathHValues.Count / 2; l++)
						{
							HValue value2 = this.pathHValues[l];
							this.pathHValues[l] = this.pathHValues[this.pathHValues.Count - l - 1];
							this.pathHValues[this.pathHValues.Count - l - 1] = value2;
						}
						hValue = array[1];
						this.pathHValues.Add(hValue);
						while (hValue.PreX != -1 && hValue.PreY != -1)
						{
							this.pathHValues.Add(this.getHValue((int)hValue.PreX, (int)hValue.PreY));
							hValue = this.getHValue((int)hValue.PreX, (int)hValue.PreY);
						}
						this.paths = this.BuildPath(this.pathHValues, startPoint, endPoint, collisionSize);
						this.pathFindEnd = true;
						if (this.pathFindEndBack != null)
						{
							this.pathFindEndBack(this.paths);
						}
						return;
					}
				}
			}
		}
	}

	private List<Vector3> BuildPath(List<HValue> paths, Vector3 start, Vector3 end, int collisionSize)
	{
		List<Vector3> list = new List<Vector3>();
		list.Clear();
		List<HValue> list2 = new List<HValue>();
		for (int i = 0; i < paths.Count; i++)
		{
			bool flag = true;
			if (0 < i && i + 1 < paths.Count)
			{
				int direction = this.GetDirection(paths[i - 1], paths[i]);
				int direction2 = this.GetDirection(paths[i], paths[i + 1]);
				flag = (direction != direction2);
			}
			if (flag)
			{
				list2.Add(paths[i]);
			}
		}
		paths = list2;
		list2 = new List<HValue>();
		list2.Clear();
		List<Vector3> list3 = new List<Vector3>();
		list3.Add(start);
		foreach (HValue current in paths)
		{
			list3.Add(new Vector3((float)((int)current.X - this.halfWidth) * this.gridSize, 0f, (float)((int)current.Y - this.halfHeight) * this.gridSize));
		}
		int count = list3.Count;
		if (count > 2 && Vector3.Distance(list3[0], list3[1]) < 0.1f)
		{
			list3.RemoveAt(1);
		}
		if (Vector3.Distance(end, list3[list3.Count - 1]) < 0.1f)
		{
			list3.Add(end);
		}
		list.Clear();
		int j = 0;
		while (j < list3.Count)
		{
			list.Add(list3[j]);
			int num = -1;
			for (int k = list3.Count - 1; k > j; k--)
			{
				Vector3 vector = list3[j];
				Vector3 vector2 = list3[k];
				vector.y = (vector2.y = 0f);
				float num2 = this.TryWalkDistance(vector, vector2, collisionSize);
				if (Math.Abs(Vector3.Distance(vector, vector2) - num2) < 0.01f)
				{
					num = k;
					break;
				}
			}
			if (num == -1)
			{
				num = j + 1;
			}
			j = num;
			if (j == list3.Count - 1)
			{
				list.Add(list3[j]);
				break;
			}
		}
		paths = list2;
		return list;
	}

	private int GetDirection(HValue p, HValue c)
	{
		int num = (int)(c.X - p.X);
		int num2 = (int)(c.Y - p.Y);
		return (num + 1) * 3 + num2;
	}

	private float Left(float x)
	{
		float num = Mathf.Floor(x);
		if (Math.Abs(x - num) < 0.01f)
		{
			return x - 1f;
		}
		return num;
	}

	private float Right(float x)
	{
		return (float)Mathf.FloorToInt(x + 1f);
	}

	private int GetKey(Vector3 pos)
	{
		int num = Mathf.FloorToInt(pos.x);
		int num2 = Mathf.FloorToInt(pos.z);
		return num * 2048 + num2;
	}

	private Vector2 ToValue(int key)
	{
		return new Vector2((float)(key / 2048), (float)(key % 2048));
	}

	public float TryWalkDistance(Vector3 start, Vector3 target, int collisionSize)
	{
		start.y = (target.y = 0f);
		start /= this.gridSize;
		start.x += (float)this.halfWidth;
		start.z += (float)this.halfHeight;
		target /= this.gridSize;
		target.x += (float)this.halfWidth;
		target.z += (float)this.halfHeight;
		if (!this.IsValidForWalk((int)start.x, (int)start.z, 0))
		{
			return 0f;
		}
		float num = Vector3.Distance(start, target);
		Vector3 vector = target - start;
		if (vector == Vector3.zero)
		{
			return 0f;
		}
		vector.Normalize();
		float num2 = 0f;
		Vector3 pos = start;
		this.distanceRecord.Clear();
		this.pointsList.Clear();
		this.tryPath.Clear();
		while (num2 <= num)
		{
			float num3 = 1E+07f;
			float num4 = 1E+07f;
			float num5 = 0.5f;
			if (vector.x != 0f)
			{
				if (vector.x > 0f)
				{
					float num6 = this.Right(pos.x);
					num3 = (num6 - pos.x) / vector.x;
					if (num3 <= 0f)
					{
						num3 = (num6 + num5 - pos.x) / vector.x;
					}
				}
				else
				{
					float num7 = this.Left(pos.x);
					num3 = (pos.x - num7) / -vector.x;
					if (num3 <= 0f)
					{
						num3 = (pos.x + num5 - num7) / -vector.x;
					}
				}
			}
			if (vector.z != 0f)
			{
				if (vector.z > 0f)
				{
					float num8 = this.Right(pos.z);
					num4 = (num8 - pos.z) / vector.z;
					if (num4 <= 0f)
					{
						num4 = (num8 + num5 - pos.z) / vector.z;
					}
				}
				else
				{
					float num9 = this.Left(pos.z);
					num4 = (pos.z - num9) / -vector.z;
					if (num4 <= 0f)
					{
						num4 = (pos.z + num5 - num9) / -vector.z;
					}
				}
			}
			if (num3 <= 0f || num4 <= 0f)
			{
				throw new Exception("步伐应该大于0.");
			}
			float num10;
			if (num3 < num4)
			{
				num10 = num3;
			}
			else
			{
				num10 = num4;
			}
			float num11 = num2 + num10 * 0.5f;
			if (num11 > num)
			{
				num11 = num;
			}
			pos = start + num11 * vector;
			int key = this.GetKey(pos);
			if (!this.distanceRecord.ContainsKey(key))
			{
				this.distanceRecord.Add(key, num2);
				this.pointsList.Add(key);
				this.tryPath.Add(new Vector2(pos.x, pos.z));
			}
			num11 = num2 + num10;
			if (num11 > num)
			{
				num11 = num;
			}
			pos = start + num11 * vector;
			key = this.GetKey(pos);
			if (!this.distanceRecord.ContainsKey(key))
			{
				this.distanceRecord.Add(key, num2);
				this.pointsList.Add(key);
				this.tryPath.Add(new Vector2(pos.x, pos.z));
			}
			num2 = num11;
			if (num2 > num - 0.1f)
			{
				num2 = num;
				break;
			}
		}
		int num12 = -1;
		Vector2 vector2 = new Vector2(0f, 0f);
		for (int i = 0; i < this.pointsList.Count; i++)
		{
			int num13 = this.pointsList[i];
			Vector2 vector3 = this.ToValue(num13);
			if (num12 != -1 && Math.Abs(vector2.x - vector3.x) + Math.Abs(vector2.y - vector3.y) == 2f && (!this.IsValidForWalk((int)vector2.x, (int)vector3.y, collisionSize) || !this.IsValidForWalk((int)vector3.x, (int)vector2.y, collisionSize)))
			{
				num2 = this.distanceRecord[num13];
				break;
			}
			if (!this.IsValidForWalk((int)vector3.x, (int)vector3.y, collisionSize))
			{
				num2 = this.distanceRecord[num13];
				break;
			}
			vector2 = vector3;
			num12 = num13;
		}
		this.distanceRecord.Clear();
		this.tryPath.Clear();
		this.pointsList.Clear();
		return num2 * this.gridSize;
	}
}
