using System;
using System.Collections.Generic;

public class AsyncTrigger
{
	public class TriggerInfo
	{
		public string strTriggerName = string.Empty;

		public int iTriggerCount;

		public AsyncTrigger.OnTrigger onTrigger;

		public object[] args;
	}

	public class FrameTriggerInfo
	{
		public int iFrameDelay = 1;

		public AsyncTrigger.OnTrigger onTrigger;

		public object[] args;
	}

	public delegate void OnTrigger(params object[] args);

	private static Dictionary<string, AsyncTrigger.TriggerInfo> mTriggers = new Dictionary<string, AsyncTrigger.TriggerInfo>();

	private static string strNULL = "null";

	public static List<AsyncTrigger.FrameTriggerInfo> mFrameTrigger = new List<AsyncTrigger.FrameTriggerInfo>();

	public static bool IsTargetValid(object oFuncTarget)
	{
		return oFuncTarget == null || !oFuncTarget.Equals(null);
	}

	public static bool CreateTimeTrigger(string strTriggerName, float fDelayTime, AsyncTrigger.OnTrigger onTrigger, params object[] args)
	{
		if (AsyncTrigger.mTriggers.ContainsKey(strTriggerName))
		{
			return false;
		}
		AsyncTrigger.TriggerInfo triggerInfo = new AsyncTrigger.TriggerInfo();
		triggerInfo.strTriggerName = strTriggerName;
		triggerInfo.onTrigger = onTrigger;
		triggerInfo.args = args;
		AsyncTrigger.mTriggers.Add(strTriggerName, triggerInfo);
		TimerManager.AddTimer(strTriggerName, fDelayTime, new TimerManager.TimerManagerHandlerArgs(AsyncTrigger.OnTriggerTimer), new object[]
		{
			strTriggerName,
			onTrigger
		});
		return true;
	}

	public static void OnTriggerTimer(params object[] args)
	{
		if (args == null || args.Length < 2)
		{
			return;
		}
		string text = args[0] as string;
		if (!AsyncTrigger.mTriggers.ContainsKey(text))
		{
			return;
		}
		AsyncTrigger.TriggerInfo triggerInfo = AsyncTrigger.mTriggers[text];
		if (triggerInfo.onTrigger != null)
		{
			try
			{
				triggerInfo.onTrigger(new object[]
				{
					text
				});
			}
			catch (Exception ex)
			{
				LogSystem.LogError(new object[]
				{
					ex.ToString()
				});
			}
		}
		AsyncTrigger.mTriggers.Remove(text);
		TimerManager.DestroyTimer(text);
	}

	public static bool CreateCountTrigger(string strTriggerName, float fDelayTime, int iCount, AsyncTrigger.OnTrigger onTrigger, params object[] args)
	{
		if (AsyncTrigger.mTriggers.ContainsKey(strTriggerName))
		{
			return false;
		}
		AsyncTrigger.TriggerInfo triggerInfo = new AsyncTrigger.TriggerInfo();
		triggerInfo.strTriggerName = strTriggerName;
		triggerInfo.iTriggerCount = iCount;
		triggerInfo.onTrigger = onTrigger;
		triggerInfo.args = args;
		AsyncTrigger.mTriggers.Add(strTriggerName, triggerInfo);
		TimerManager.AddTimerRepeat(strTriggerName, fDelayTime, new TimerManager.TimerManagerHandlerArgs(AsyncTrigger.OnTriggerCountTimer), new object[]
		{
			strTriggerName,
			onTrigger
		});
		return true;
	}

	public static void OnTriggerCountTimer(params object[] args)
	{
		if (args == null || args.Length < 2)
		{
			return;
		}
		string text = args[0] as string;
		if (!AsyncTrigger.mTriggers.ContainsKey(text))
		{
			return;
		}
		AsyncTrigger.TriggerInfo triggerInfo = AsyncTrigger.mTriggers[text];
		triggerInfo.iTriggerCount--;
		if (triggerInfo.onTrigger != null)
		{
			try
			{
				triggerInfo.onTrigger(new object[]
				{
					text
				});
			}
			catch (Exception ex)
			{
				LogSystem.LogError(new object[]
				{
					ex.ToString()
				});
			}
		}
		if (triggerInfo.iTriggerCount == 0)
		{
			AsyncTrigger.mTriggers.Remove(text);
			TimerManager.DestroyTimer(text);
		}
	}

	public static void CreateFrameTrigger(int frameDelay, AsyncTrigger.OnTrigger onTrigger, params object[] args)
	{
		AsyncTrigger.FrameTriggerInfo frameTriggerInfo = new AsyncTrigger.FrameTriggerInfo();
		frameTriggerInfo.iFrameDelay = ((frameDelay >= 1) ? frameDelay : 1);
		frameTriggerInfo.onTrigger = onTrigger;
		frameTriggerInfo.args = args;
		AsyncTrigger.mFrameTrigger.Add(frameTriggerInfo);
	}

	public static void UpdateFrameTrigger()
	{
		try
		{
			for (int i = 0; i < AsyncTrigger.mFrameTrigger.Count; i++)
			{
				AsyncTrigger.FrameTriggerInfo frameTriggerInfo = AsyncTrigger.mFrameTrigger[i];
				if (frameTriggerInfo == null)
				{
					AsyncTrigger.mFrameTrigger.RemoveAt(i);
					i--;
				}
				else
				{
					frameTriggerInfo.iFrameDelay--;
					if (frameTriggerInfo.iFrameDelay <= 1)
					{
						AsyncTrigger.mFrameTrigger.RemoveAt(i);
						if (frameTriggerInfo.onTrigger != null)
						{
							try
							{
								frameTriggerInfo.onTrigger(frameTriggerInfo.args);
							}
							catch (Exception ex)
							{
								LogSystem.LogError(new object[]
								{
									"UpdateFrameTrigger : ",
									ex.ToString()
								});
							}
						}
						i--;
					}
				}
			}
		}
		catch (Exception ex2)
		{
			LogSystem.LogError(new object[]
			{
				ex2.ToString()
			});
		}
	}

	public static bool CreateTrigger(string strTriggerName, int iTriggerCount = 1)
	{
		if (AsyncTrigger.mTriggers.ContainsKey(strTriggerName))
		{
			return false;
		}
		AsyncTrigger.TriggerInfo triggerInfo = new AsyncTrigger.TriggerInfo();
		triggerInfo.strTriggerName = strTriggerName;
		triggerInfo.iTriggerCount = iTriggerCount;
		AsyncTrigger.mTriggers.Add(strTriggerName, triggerInfo);
		return true;
	}

	public static void Trigger(string strTriggerName, AsyncTrigger.OnTrigger onTrigger, params object[] args)
	{
		if (!AsyncTrigger.mTriggers.ContainsKey(strTriggerName))
		{
			return;
		}
		AsyncTrigger.TriggerInfo triggerInfo = AsyncTrigger.mTriggers[strTriggerName];
		triggerInfo.iTriggerCount--;
		if (triggerInfo.iTriggerCount > 0)
		{
			return;
		}
		if (onTrigger != null)
		{
			try
			{
				onTrigger(args);
			}
			catch (Exception ex)
			{
				LogSystem.LogError(new object[]
				{
					ex.ToString()
				});
			}
		}
		AsyncTrigger.mTriggers.Remove(strTriggerName);
	}
}
