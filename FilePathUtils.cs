using System;
using UnityEngine;

public class FilePathUtils
{
	public static string GetMobileFilePath(string path)
	{
		return Application.persistentDataPath + path;
	}
}
