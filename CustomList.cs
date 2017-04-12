using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomList<T> where T : UnityEngine.Object
{
	private List<T> list;

	public T this[int index]
	{
		get
		{
			if (index < 0)
			{
				return (T)((object)null);
			}
			if (index >= this.list.Count)
			{
				return (T)((object)null);
			}
			return this.list[index];
		}
	}

	[Obsolete("Count已经过时，请使用Length")]
	public int Count
	{
		get
		{
			return this.Length;
		}
	}

	public int Length
	{
		get
		{
			if (this.list == null)
			{
				return 0;
			}
			return this.list.Count;
		}
	}

	public CustomList()
	{
		this.list = new List<T>();
	}

	public CustomList(T[] items)
	{
		this.list = new List<T>(items);
	}

	public void Add(T item)
	{
		this.list.Add(item);
	}

	public bool Remove(T item)
	{
		int count = this.list.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.list[i] == item)
			{
				this.list.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public bool RemoveAt(int index)
	{
		int count = this.list.Count;
		if (index >= 0 && index < count)
		{
			this.list.RemoveAt(index);
			return true;
		}
		return false;
	}

	public void Clear()
	{
		this.list.Clear();
	}

	public void Sort()
	{
		DinoComparer<T> comparer = new DinoComparer<T>();
		this.list.Sort(comparer);
	}

	public void Sort(IComparer<T> dc)
	{
		this.list.Sort(dc);
	}

	public void Sort(Comparison<T> dc)
	{
		this.list.Sort(dc);
	}
}
