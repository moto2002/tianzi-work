using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventDelegate
{
	public delegate void Callback();

	[SerializeField]
	private MonoBehaviour mTarget;

	[SerializeField]
	private string mMethodName;

	public bool oneShot;

	private EventDelegate.Callback mCachedCallback;

	private bool mRawDelegate;

	private static int s_Hash = "EventDelegate".GetHashCode();

	public MonoBehaviour target
	{
		get
		{
			return this.mTarget;
		}
		set
		{
			this.mTarget = value;
			this.mCachedCallback = null;
			this.mRawDelegate = false;
		}
	}

	public string methodName
	{
		get
		{
			return this.mMethodName;
		}
		set
		{
			this.mMethodName = value;
			this.mCachedCallback = null;
			this.mRawDelegate = false;
		}
	}

	public bool isValid
	{
		get
		{
			return (this.mRawDelegate && this.mCachedCallback != null) || (this.mTarget != null && !string.IsNullOrEmpty(this.mMethodName));
		}
	}

	public bool isEnabled
	{
		get
		{
			if (this.mRawDelegate && this.mCachedCallback != null)
			{
				return true;
			}
			if (this.mTarget == null)
			{
				return false;
			}
			MonoBehaviour monoBehaviour = this.mTarget;
			return monoBehaviour == null || monoBehaviour.enabled;
		}
	}

	public EventDelegate()
	{
	}

	public EventDelegate(EventDelegate.Callback call)
	{
		this.Set(call);
	}

	public EventDelegate(MonoBehaviour target, string methodName)
	{
		this.Set(target, methodName);
	}

	private static string GetMethodName(EventDelegate.Callback callback)
	{
		return callback.Method.Name;
	}

	private static bool IsValid(EventDelegate.Callback callback)
	{
		return callback != null && callback.Method != null;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return !this.isValid;
		}
		if (obj is EventDelegate.Callback)
		{
			EventDelegate.Callback callback = obj as EventDelegate.Callback;
			if (callback.Equals(this.mCachedCallback))
			{
				return true;
			}
			MonoBehaviour y = callback.Target as MonoBehaviour;
			return this.mTarget == y && string.Equals(this.mMethodName, EventDelegate.GetMethodName(callback));
		}
		else
		{
			if (obj is EventDelegate)
			{
				EventDelegate eventDelegate = obj as EventDelegate;
				return this.mTarget == eventDelegate.mTarget && string.Equals(this.mMethodName, eventDelegate.mMethodName);
			}
			return false;
		}
	}

	public override int GetHashCode()
	{
		return EventDelegate.s_Hash;
	}

	private EventDelegate.Callback Get()
	{
		if (!this.mRawDelegate && (this.mCachedCallback == null || this.mCachedCallback.Target as MonoBehaviour != this.mTarget || EventDelegate.GetMethodName(this.mCachedCallback) != this.mMethodName))
		{
			if (!(this.mTarget != null) || string.IsNullOrEmpty(this.mMethodName))
			{
				return null;
			}
			this.mCachedCallback = (EventDelegate.Callback)Delegate.CreateDelegate(typeof(EventDelegate.Callback), this.mTarget, this.mMethodName);
		}
		return this.mCachedCallback;
	}

	private void Set(EventDelegate.Callback call)
	{
		if (call == null || !EventDelegate.IsValid(call))
		{
			this.mTarget = null;
			this.mMethodName = null;
			this.mCachedCallback = null;
			this.mRawDelegate = false;
		}
		else
		{
			this.mTarget = (call.Target as MonoBehaviour);
			if (this.mTarget == null)
			{
				this.mRawDelegate = true;
				this.mCachedCallback = call;
				this.mMethodName = null;
			}
			else
			{
				this.mMethodName = EventDelegate.GetMethodName(call);
				this.mRawDelegate = false;
			}
		}
	}

	public void Set(MonoBehaviour target, string methodName)
	{
		this.mTarget = target;
		this.mMethodName = methodName;
		this.mCachedCallback = null;
		this.mRawDelegate = false;
	}

	public bool Execute()
	{
		EventDelegate.Callback callback = this.Get();
		if (callback != null)
		{
			callback();
			return true;
		}
		return false;
	}

	public void Clear()
	{
		this.mTarget = null;
		this.mMethodName = null;
		this.mRawDelegate = false;
		this.mCachedCallback = null;
	}

	public override string ToString()
	{
		if (!(this.mTarget != null))
		{
			return (!this.mRawDelegate) ? null : "[delegate]";
		}
		string text = this.mTarget.GetType().ToString();
		int num = text.LastIndexOf('.');
		if (num > 0)
		{
			text = text.Substring(num + 1);
		}
		if (!string.IsNullOrEmpty(this.methodName))
		{
			return text + "." + this.methodName;
		}
		return text + ".[delegate]";
	}

	public static void Execute(List<EventDelegate> list)
	{
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null)
				{
					eventDelegate.Execute();
					if (i >= list.Count)
					{
						break;
					}
					if (list[i] != eventDelegate)
					{
						continue;
					}
					if (eventDelegate.oneShot)
					{
						list.RemoveAt(i);
						continue;
					}
				}
			}
		}
	}

	public static bool IsValid(List<EventDelegate> list)
	{
		if (list != null)
		{
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.isValid)
				{
					return true;
				}
				i++;
			}
		}
		return false;
	}

	public static void Set(List<EventDelegate> list, EventDelegate.Callback callback)
	{
		if (list != null)
		{
			list.Clear();
			list.Add(new EventDelegate(callback));
		}
	}

	public static void Set(List<EventDelegate> list, EventDelegate del)
	{
		if (list != null)
		{
			list.Clear();
			list.Add(del);
		}
	}

	public static void Add(List<EventDelegate> list, EventDelegate.Callback callback)
	{
		EventDelegate.Add(list, callback, false);
	}

	public static void Add(List<EventDelegate> list, EventDelegate.Callback callback, bool oneShot)
	{
		if (list != null)
		{
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(callback))
				{
					return;
				}
				i++;
			}
			list.Add(new EventDelegate(callback)
			{
				oneShot = oneShot
			});
		}
		else
		{
			LogSystem.LogWarning(new object[]
			{
				"Attempting to add a callback to a list that's null"
			});
		}
	}

	public static void Add(List<EventDelegate> list, EventDelegate ev)
	{
		EventDelegate.Add(list, ev, ev.oneShot);
	}

	public static void Add(List<EventDelegate> list, EventDelegate ev, bool oneShot)
	{
		if (ev.mRawDelegate || ev.target == null || string.IsNullOrEmpty(ev.methodName))
		{
			EventDelegate.Add(list, ev.mCachedCallback, oneShot);
		}
		else if (list != null)
		{
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(ev))
				{
					return;
				}
				i++;
			}
			list.Add(new EventDelegate(ev.target, ev.methodName)
			{
				oneShot = oneShot
			});
		}
		else
		{
			LogSystem.LogWarning(new object[]
			{
				"Attempting to add a callback to a list that's null"
			});
		}
	}

	public static bool Remove(List<EventDelegate> list, EventDelegate.Callback callback)
	{
		if (list != null)
		{
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(callback))
				{
					list.RemoveAt(i);
					return true;
				}
				i++;
			}
		}
		return false;
	}
}
