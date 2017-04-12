using System;
using System.Collections.Generic;
using System.IO;

public class AssetLibrary
{
	private static Dictionary<string, List<string>> dependsMap;

	public static string downloadPath = string.Empty;

	public static string streamAssetsPath = string.Empty;

	public static AssetLibraryBundle getBundle()
	{
		return AssetLibraryBundle.getInstance("default");
	}

	private static void AddDepends(string sceneDicPath)
	{
		if (!Directory.Exists(sceneDicPath))
		{
			return;
		}
		string[] files = Directory.GetFiles(sceneDicPath);
		string[] array = files;
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			if (text.IndexOf(".unity3d") > 0)
			{
				int num = text.IndexOf(".unity3d") - text.LastIndexOf("/");
				string text2 = text.Substring(text.LastIndexOf("/") + 1, num - 1);
				if (text2.Equals("terrainTexture"))
				{
					text2 = "Textures/Terrain";
				}
				if (!AssetLibrary.dependsMap.ContainsKey(text2))
				{
					List<string> list = new List<string>();
					list.Add(text);
					AssetLibrary.dependsMap.Add(text2, list);
				}
			}
		}
	}

	public static List<string> FindDepends(string key)
	{
		if (AssetLibrary.dependsMap == null)
		{
			AssetLibrary.dependsMap = new Dictionary<string, List<string>>();
			AssetLibrary.AddDepends(AssetLibrary.downloadPath + "/Scenes/");
			AssetLibrary.AddDepends(AssetLibrary.streamAssetsPath + "/Scenes/");
		}
		foreach (KeyValuePair<string, List<string>> current in AssetLibrary.dependsMap)
		{
			if (key.Contains(current.Key))
			{
				return current.Value;
			}
		}
		return null;
	}

	public static void InsertTerrainDepends(string fileName)
	{
		if (AssetLibrary.dependsMap == null)
		{
			AssetLibrary.dependsMap = new Dictionary<string, List<string>>();
		}
		List<string> list = new List<string>();
		list.Add(fileName);
		AssetLibrary.dependsMap.Add("Textures/Terrain", list);
	}

	public static void InsertSceneDepends(int sceneID, string fileName)
	{
		if (AssetLibrary.dependsMap == null)
		{
			AssetLibrary.dependsMap = new Dictionary<string, List<string>>();
		}
		if (!AssetLibrary.dependsMap.ContainsKey(sceneID + string.Empty))
		{
			List<string> list = new List<string>();
			list.Add(fileName);
			AssetLibrary.dependsMap.Add(sceneID + string.Empty, list);
		}
	}

	public static void InsertDepends(Dictionary<string, List<string>> data)
	{
		AssetLibrary.dependsMap = data;
	}

	public static Asset Load(string path, AssetType type, LoadType loadType = LoadType.Type_Resources)
	{
		Asset asset = AssetLibrary.getBundle().GetAsset(path);
		if (asset == null)
		{
			asset = new Asset(path, loadType, type);
			AssetLibrary.getBundle().AddAsset(asset);
			return asset;
		}
		return asset;
	}

	public static void AddAsset(string path, Asset asset)
	{
		Asset asset2 = AssetLibrary.getBundle().GetAsset(path);
		if (asset2 != null)
		{
			return;
		}
		AssetLibrary.getBundle().AddAsset(asset);
	}

	public static void RemoveAsset(string path)
	{
		Asset asset = AssetLibrary.getBundle().GetAsset(path);
		if (asset != null)
		{
			AssetLibrary.getBundle().RemoveAsset(asset);
		}
	}

	public static void RemoveAsset(Asset asset)
	{
		if (asset != null)
		{
			AssetLibrary.getBundle().RemoveAsset(asset);
		}
	}

	public static void RemoveAllAsset()
	{
		AssetLibrary.getBundle().RemoveAllAssets();
	}
}
