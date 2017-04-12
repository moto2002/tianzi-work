using System;
using System.Collections.Generic;

public class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>
{
	public List<TKey> mList = new List<TKey>();

	public new TValue this[TKey tkey]
	{
		get
		{
			return base[tkey];
		}
		set
		{
			if (this.ContainsKey(tkey))
			{
				base[tkey] = value;
			}
			else
			{
				this.Add(tkey, value);
			}
		}
	}

	public new void Add(TKey tkey, TValue tvalue)
	{
		this.mList.Add(tkey);
		base.Add(tkey, tvalue);
	}

	public new bool Remove(TKey tkey)
	{
		this.mList.Remove(tkey);
		return base.Remove(tkey);
	}

	public new bool ContainsKey(TKey tkey)
	{
		return this.mList.Contains(tkey);
	}

	public new void Clear()
	{
		this.mList.Clear();
		base.Clear();
	}
}
