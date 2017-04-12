using System;
using System.Collections.Generic;
using UnityEngine;

public class Instance
{
	private static Dictionary<Type, object> mTables = new Dictionary<Type, object>();

	public static bool IsHave<T>()
	{
		Type typeFromHandle = typeof(T);
		return Instance.mTables.ContainsKey(typeFromHandle);
	}

	public static T Get<T>()
	{
		Type typeFromHandle = typeof(T);
		T t = default(T);
		if (typeFromHandle.BaseType != null && typeFromHandle.BaseType == typeof(MonoBehaviour))
		{
			if (Instance.mTables.ContainsKey(typeFromHandle))
			{
				object obj = Instance.mTables[typeFromHandle];
				if (obj is T)
				{
					t = (T)((object)obj);
				}
			}
		}
		else if (Instance.mTables.ContainsKey(typeFromHandle))
		{
			object obj2 = Instance.mTables[typeFromHandle];
			if (obj2 is T)
			{
				t = (T)((object)obj2);
			}
			else
			{
				t = (T)((object)Activator.CreateInstance(typeFromHandle));
				Instance.mTables[typeFromHandle] = t;
			}
		}
		else
		{
			t = (T)((object)Activator.CreateInstance(typeFromHandle));
			Instance.mTables[typeFromHandle] = t;
		}
		return t;
	}

	public static void Set<T>(object o, bool bReWriter = true)
	{
		Type typeFromHandle = typeof(T);
		if (!Instance.mTables.ContainsKey(typeFromHandle) || bReWriter)
		{
			Instance.mTables[typeFromHandle] = o;
		}
	}

	public static void Create(object o)
	{
		if (o == null)
		{
			return;
		}
		if (!Instance.mTables.ContainsKey(o.GetType()))
		{
			Instance.mTables[o.GetType()] = o;
		}
	}

	public static void Clean<T>()
	{
		Type typeFromHandle = typeof(T);
		if (Instance.mTables.ContainsKey(typeFromHandle))
		{
			Instance.mTables.Remove(typeFromHandle);
		}
	}

	public static void CleanLogic()
	{
		List<Type> list = new List<Type>();
		foreach (Type current in Instance.mTables.Keys)
		{
			if (current.BaseType == null || current.BaseType != typeof(MonoBehaviour))
			{
				list.Add(current);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			Instance.mTables.Remove(list[i]);
		}
	}

	public static void ClearAll()
	{
		Instance.mTables.Clear();
	}

	public static void Print()
	{
		foreach (Type current in Instance.mTables.Keys)
		{
			LogSystem.Log(new object[]
			{
				"instance->",
				current.ToString()
			});
		}
	}
}
