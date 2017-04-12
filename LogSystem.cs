using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class LogSystem
{
	private static bool mbFileLog = false;

	private static bool mbDebugLog = false;

	private static StreamWriter mStreamFileWtiter = null;

	private static FileStream mfstream = null;

	private static List<string> mLines = new List<string>();

	private static int miLogCount = 0;

	private static int miLogCountMax = 256;

	private static string mstrLastFileLog = string.Empty;

	private static StringBuilder sb = new StringBuilder();

	private static string strInfo1 = "[Info ";

	private static string strInfo2 = "[Warn ";

	private static string strInfo3 = "[Error ";

	private static string strInfo4 = "]";

	public static bool Init(string strLogFile, bool bFileLog = true, bool bDebugLog = false, int iLogMaxLines = 256)
	{
		LogSystem.mbFileLog = bFileLog;
		LogSystem.mbDebugLog = bDebugLog;
		if (LogSystem.mbFileLog)
		{
			try
			{
				if (File.Exists(strLogFile))
				{
					File.Delete(strLogFile);
				}
				LogSystem.mfstream = new FileStream(strLogFile, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
				LogSystem.mStreamFileWtiter = new StreamWriter(LogSystem.mfstream);
			}
			catch (Exception)
			{
				LogSystem.mfstream = new FileStream(strLogFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				LogSystem.mStreamFileWtiter = new StreamWriter(LogSystem.mfstream);
			}
			LogSystem.miLogCountMax = iLogMaxLines;
		}
		return true;
	}

	public static void TraceLine(string strLine)
	{
		string item = string.Concat(new object[]
		{
			DateTime.Now.ToString("MM-dd HH:mm:ss.fff"),
			"\t",
			strLine,
			"\t",
			GC.GetTotalMemory(false)
		});
		LogSystem.mLines.Add(item);
	}

	public static void TraceAllLine()
	{
		if (LogSystem.mStreamFileWtiter == null)
		{
			return;
		}
		for (int i = 0; i < LogSystem.mLines.Count; i++)
		{
			LogSystem.mStreamFileWtiter.WriteLine(LogSystem.mLines[i]);
		}
		LogSystem.mStreamFileWtiter.Flush();
	}

	public static void TraceFile(int iType, string str)
	{
		if (LogSystem.mStreamFileWtiter == null)
		{
			return;
		}
		try
		{
			string value = LogSystem.StringBuilderCurrTime(iType);
			LogSystem.mStreamFileWtiter.Write(value);
			LogSystem.mStreamFileWtiter.WriteLine(str);
			LogSystem.mStreamFileWtiter.Flush();
		}
		catch (IOException)
		{
		}
		catch (Exception ex)
		{
			LogSystem.LogError(new object[]
			{
				ex.ToString()
			});
		}
		try
		{
			if (LogSystem.miLogCount++ > LogSystem.miLogCountMax)
			{
				if (LogSystem.mfstream != null)
				{
					LogSystem.mfstream.Seek(0L, SeekOrigin.Begin);
				}
				LogSystem.miLogCount = 0;
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

	private static string StringBuilder(int iType, object[] args)
	{
		if (LogSystem.sb != null)
		{
			if (LogSystem.sb.Length > 0)
			{
				LogSystem.sb.Remove(0, LogSystem.sb.Length);
			}
			if (iType == 0)
			{
				LogSystem.sb.Append(LogSystem.strInfo1);
			}
			else if (iType == 1)
			{
				LogSystem.sb.Append(LogSystem.strInfo2);
			}
			else
			{
				LogSystem.sb.Append(LogSystem.strInfo3);
			}
			LogSystem.sb.Append(DateTime.Now.ToString("MM-dd HH:mm:ss"));
			LogSystem.sb.Append(LogSystem.strInfo4);
			int num = args.Length;
			for (int i = 0; i < num; i++)
			{
				LogSystem.sb.Append(args[i]);
			}
			return LogSystem.sb.ToString();
		}
		return string.Empty;
	}

	private static string StringBuilderCurrTime(int iType)
	{
		if (LogSystem.sb != null)
		{
			if (LogSystem.sb.Length > 0)
			{
				LogSystem.sb.Remove(0, LogSystem.sb.Length);
			}
			if (iType == 0)
			{
				LogSystem.sb.Append(LogSystem.strInfo1);
			}
			else if (iType == 1)
			{
				LogSystem.sb.Append(LogSystem.strInfo2);
			}
			else
			{
				LogSystem.sb.Append(LogSystem.strInfo3);
			}
			LogSystem.sb.Append(DateTime.Now.ToString("MM-dd HH:mm:ss"));
			LogSystem.sb.Append(LogSystem.strInfo4);
			return LogSystem.sb.ToString();
		}
		return string.Empty;
	}

	private static string StringBuilderContent(object[] args)
	{
		if (LogSystem.sb != null)
		{
			if (LogSystem.sb.Length > 0)
			{
				LogSystem.sb.Remove(0, LogSystem.sb.Length);
			}
			int num = args.Length;
			for (int i = 0; i < num; i++)
			{
				LogSystem.sb.Append(args[i]);
			}
			return LogSystem.sb.ToString();
		}
		return string.Empty;
	}

	public static void Log(params object[] args)
	{
	}

	public static void LogWarning(params object[] args)
	{
		string text = LogSystem.StringBuilderContent(args);
		if (string.IsNullOrEmpty(LogSystem.mstrLastFileLog))
		{
			LogSystem.mstrLastFileLog = text;
		}
		else
		{
			if (LogSystem.mstrLastFileLog.Equals(text))
			{
				return;
			}
			LogSystem.mstrLastFileLog = text;
		}
		if (LogSystem.mbDebugLog)
		{
			Debug.LogWarning(text, null);
		}
		if (LogSystem.mbFileLog)
		{
			LogSystem.TraceFile(1, text);
		}
	}

	public static void LogError(params object[] args)
	{
		string text = LogSystem.StringBuilderContent(args);
		if (string.IsNullOrEmpty(LogSystem.mstrLastFileLog))
		{
			LogSystem.mstrLastFileLog = text;
		}
		else
		{
			if (LogSystem.mstrLastFileLog.Equals(text))
			{
				return;
			}
			LogSystem.mstrLastFileLog = text;
		}
		Debug.LogError(text, null);
		if (LogSystem.mbFileLog)
		{
			LogSystem.TraceFile(2, text);
		}
	}
}
