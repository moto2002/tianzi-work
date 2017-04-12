using System;
using System.Collections.Generic;
using UnityEngine;

public class Heap
{
	public List<HValue> _Values = new List<HValue>();

	public int PushCount;

	public float randomCode;

	public int Count
	{
		get
		{
			if (this._Values == null)
			{
				return 0;
			}
			return this._Values.Count;
		}
	}

	public bool IsEmpty
	{
		get
		{
			return this._Values == null || this._Values.Count == 0;
		}
	}

	public Heap()
	{
		this.Reset();
		this.randomCode = UnityEngine.Random.Range(0f, 10000f);
	}

	public void Clear()
	{
		this.Reset();
	}

	public int Find(HValue instance)
	{
		if (this._Values != null)
		{
			for (int i = 0; i < this._Values.Count; i++)
			{
				if (instance.X == this._Values[i].X && instance.Y == this._Values[i].Y)
				{
					return i;
				}
			}
		}
		return -1;
	}

	private void Reset()
	{
		this._Values = new List<HValue>();
		this._Values.Clear();
		this.PushCount = 0;
	}

	public void Push(HValue value)
	{
		this._Values.Add(value);
		int pos = this._Values.Count - 1;
		this.Upper(pos);
		this.PushCount++;
	}

	public HValue Pop()
	{
		if (this._Values.Count >= 1)
		{
			HValue result = this._Values[0];
			this._Values[0] = this._Values[this._Values.Count - 1];
			this._Values.RemoveAt(this._Values.Count - 1);
			this.Lower(0);
			return result;
		}
		return new HValue();
	}

	public HValue Peek()
	{
		if (this._Values.Count >= 1)
		{
			return this._Values[0];
		}
		return new HValue();
	}

	public void Upper(int pos)
	{
		int num = pos;
		int num2 = (pos - 1) / 2;
		while (true)
		{
			num2 = (num - 1) / 2;
			if (num == 0 || !HValue.Compare(this._Values[num], this._Values[num2]))
			{
				break;
			}
			HValue value = this._Values[num];
			this._Values[num] = this._Values[num2];
			this._Values[num2] = value;
			num = num2;
		}
	}

	public void Lower(int pos)
	{
		if (this._Values.Count == 0)
		{
			return;
		}
		int num = pos;
		int num2 = pos * 2 + 1;
		int num3 = pos * 2 + 2;
		while (true)
		{
			int num4 = num;
			num2 = num * 2 + 1;
			num3 = num * 2 + 2;
			if (num2 < this._Values.Count && HValue.Compare(this._Values[num2], this._Values[num4]))
			{
				num4 = num2;
			}
			if (num3 < this._Values.Count && HValue.Compare(this._Values[num3], this._Values[num4]))
			{
				num4 = num3;
			}
			if (num4 == num)
			{
				break;
			}
			HValue value = this._Values[num];
			this._Values[num] = this._Values[num4];
			this._Values[num4] = value;
			num = num4;
		}
	}

	public bool Validate()
	{
		bool flag = true;
		for (int i = 0; i < this._Values.Count; i++)
		{
			if (i * 2 + 1 < this._Values.Count && HValue.Compare(this._Values[2 * i + 1], this._Values[i]))
			{
				flag = false;
			}
			if (i * 2 + 2 < this._Values.Count && HValue.Compare(this._Values[2 * i + 2], this._Values[i]))
			{
				flag = false;
			}
			if (!flag || i * 2 >= this._Values.Count)
			{
				break;
			}
		}
		return flag;
	}

	public bool AllDiffer()
	{
		for (int i = 0; i < this._Values.Count; i++)
		{
			for (int j = i + 1; j < this._Values.Count; j++)
			{
				if (this._Values[i].X == this._Values[j].X && this._Values[i].Y == this._Values[j].Y)
				{
					return false;
				}
			}
		}
		return true;
	}
}
