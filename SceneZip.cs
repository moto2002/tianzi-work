using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneZip
{
	public static int startRegX = -1;

	public static int startRegY = -1;

	public static int endRegX = 1;

	public static int endRegY = 1;

	public static string zipPath;

	public static string unZipPath = "D:\\test";

	private static MemoryStream sceneZipStream;

	private static BinaryWriter sceneZipWriter;

	private static BinaryReader sceneZipReader;

	public static void Zip(string scenePath, string zipfilePath)
	{
		SceneZip.zipPath = scenePath;
		SceneZip.sceneZipStream = new MemoryStream();
		SceneZip.sceneZipWriter = new BinaryWriter(SceneZip.sceneZipStream);
		SceneZip.ZipScene();
		SceneZip.ZipLightmap();
		for (int i = SceneZip.startRegX; i <= SceneZip.endRegX; i++)
		{
			for (int j = SceneZip.startRegY; j <= SceneZip.endRegY; j++)
			{
				SceneZip.ZipRegion(i, j);
			}
		}
		if (!QFileUtils.Exists(zipfilePath))
		{
			QFileUtils.CreateFile(zipfilePath);
		}
		QFileUtils.WriteBytes(zipfilePath, SceneZip.sceneZipStream.ToArray());
	}

	public static void UnZip(string zipfile, int sceneID)
	{
		SceneZip.unZipPath = Application.persistentDataPath + "/Resources/Scenes/" + sceneID;
		LogSystem.Log(new object[]
		{
			"unzip scene file into ->" + SceneZip.unZipPath
		});
		byte[] buffer = QFileUtils.ReadBinary(zipfile);
		SceneZip.sceneZipReader = new BinaryReader(new MemoryStream(buffer));
		SceneZip.UnZipScene();
		SceneZip.UnZipLightmap();
		while (SceneZip.sceneZipReader.BaseStream.Position < SceneZip.sceneZipReader.BaseStream.Length)
		{
			SceneZip.UnZipRegion();
		}
	}

	public static void ZipScene()
	{
		string fileName = SceneZip.zipPath + "/Scene.bytes";
		byte[] array = QFileUtils.ReadBinary(fileName);
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(array, 0, array.Length);
		memoryStream.Position = 0L;
		MemoryStream memoryStream2 = new MemoryStream();
		StreamZip.Zip(memoryStream, memoryStream2);
		byte[] array2 = memoryStream2.ToArray();
		SceneZip.sceneZipWriter.Write(array2.Length);
		SceneZip.sceneZipWriter.Write(array2);
	}

	public static void UnZipScene()
	{
		string text = SceneZip.unZipPath + "/Scene.bytes";
		int count = SceneZip.sceneZipReader.ReadInt32();
		byte[] array = SceneZip.sceneZipReader.ReadBytes(count);
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(array, 0, array.Length);
		memoryStream.Position = 0L;
		MemoryStream memoryStream2 = new MemoryStream();
		StreamZip.Unzip(memoryStream, memoryStream2);
		string dirName = text.Substring(0, text.LastIndexOf("/"));
		if (!QFileUtils.ExistsDir(dirName))
		{
			QFileUtils.CreateDir(dirName);
		}
		if (!QFileUtils.Exists(text))
		{
			QFileUtils.CreateFile(text);
		}
		QFileUtils.WriteBytes(text, memoryStream2.ToArray());
	}

	public static void ZipLightmap()
	{
		string path = SceneZip.zipPath + "/Lightmap";
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		FileInfo[] files = directoryInfo.GetFiles();
		List<string> list = new List<string>();
		for (int i = 0; i < files.Length; i++)
		{
			list.Add(files[i].DirectoryName + "\\" + files[i].Name);
		}
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter outStream = new BinaryWriter(memoryStream);
		StreamZip.MultiZip(list, outStream);
		byte[] array = memoryStream.ToArray();
		SceneZip.sceneZipWriter.Write(array.Length);
		SceneZip.sceneZipWriter.Write(array);
	}

	public static void UnZipLightmap()
	{
		int count = SceneZip.sceneZipReader.ReadInt32();
		byte[] zipbytes = SceneZip.sceneZipReader.ReadBytes(count);
		string dir = SceneZip.unZipPath + "\\Lightmap";
		StreamZip.MultiUnzip(zipbytes, dir);
	}

	public static void ZipRegion(int regX, int regY)
	{
		string path = string.Concat(new object[]
		{
			SceneZip.zipPath,
			"/",
			regX,
			"_",
			regY
		});
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		FileInfo[] files = directoryInfo.GetFiles();
		List<string> list = new List<string>();
		for (int i = 0; i < files.Length; i++)
		{
			list.Add(files[i].DirectoryName + "\\" + files[i].Name);
		}
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter outStream = new BinaryWriter(memoryStream);
		StreamZip.MultiZip(list, outStream);
		byte[] array = memoryStream.ToArray();
		SceneZip.sceneZipWriter.Write(regX);
		SceneZip.sceneZipWriter.Write(regY);
		SceneZip.sceneZipWriter.Write(array.Length);
		SceneZip.sceneZipWriter.Write(array);
	}

	public static void UnZipRegion()
	{
		int num = SceneZip.sceneZipReader.ReadInt32();
		int num2 = SceneZip.sceneZipReader.ReadInt32();
		int count = SceneZip.sceneZipReader.ReadInt32();
		byte[] zipbytes = SceneZip.sceneZipReader.ReadBytes(count);
		string dir = string.Concat(new object[]
		{
			SceneZip.unZipPath,
			"\\",
			num,
			"_",
			num2
		});
		StreamZip.MultiUnzip(zipbytes, dir);
	}
}
