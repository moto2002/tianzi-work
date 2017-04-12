using System;
using System.Collections.Generic;

public class Aollector
{
	private static Dictionary<Type, Queue<object>> mQueues = new Dictionary<Type, Queue<object>>();

	public static object GetObject<T>()
	{
		Type typeFromHandle = typeof(T);
		if (!Aollector.mQueues.ContainsKey(typeFromHandle))
		{
			return Activator.CreateInstance(typeof(T));
		}
		Queue<object> queue = Aollector.mQueues[typeFromHandle];
		if (queue.Count == 0)
		{
			return Activator.CreateInstance(typeof(T));
		}
		return queue.Dequeue();
	}

	public static void CollectObject<T>(object obj)
	{
		Type typeFromHandle = typeof(T);
		if (Aollector.mQueues.ContainsKey(typeFromHandle))
		{
			Queue<object> queue = Aollector.mQueues[typeFromHandle];
			queue.Enqueue(obj);
		}
		else
		{
			Queue<object> queue2 = new Queue<object>();
			queue2.Enqueue(obj);
			Aollector.mQueues.Add(typeFromHandle, queue2);
		}
	}
}
