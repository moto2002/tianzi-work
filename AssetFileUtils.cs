using System;
using System.IO;
using System.Text;

public class AssetFileUtils
{
	public static bool DeleteAsset(string strFilePath)
	{
		try
		{
			if (File.Exists(strFilePath))
			{
				File.Delete(strFilePath);
				return true;
			}
		}
		catch (Exception ex)
		{
			LogSystem.LogError(new object[]
			{
				ex.ToString()
			});
		}
		return false;
	}

	public static bool WriteLocalAsset(string strPath, byte[] bytes)
	{
		FileInfo fileInfo = new FileInfo(strPath);
		if (fileInfo.Exists && !AssetFileUtils.DeleteAsset(strPath))
		{
			return false;
		}
		try
		{
			if (!fileInfo.Exists)
			{
				Directory.CreateDirectory(fileInfo.DirectoryName);
			}
			AssetFileUtils.WriteFile(strPath, bytes);
			return true;
		}
		catch (Exception ex)
		{
			LogSystem.LogError(new object[]
			{
				"WriteLocalAsset",
				ex.ToString()
			});
		}
		return false;
	}

	public static void WriteFile(string filePath, object data)
	{
		FileStream fileStream = File.OpenWrite(filePath);
		fileStream.Position = 0L;
		fileStream.SetLength(0L);
		if (data is string)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data as string);
			fileStream.Write(bytes, 0, bytes.Length);
		}
		else
		{
			fileStream.Write(data as byte[], 0, (data as byte[]).Length);
		}
		fileStream.Flush();
		fileStream.Close();
	}
}
