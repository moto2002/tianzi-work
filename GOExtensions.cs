using System;
using UnityEngine;
using WellFired.Data;

public class GOExtensions
{
	public static T Construct<T>() where T : Component
	{
		GameObject gameObject = new GameObject(typeof(T).ToString());
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.AddComponent(typeof(T));
		return gameObject.GetComponent(typeof(T)) as T;
	}

	public static T ConstructPersistant<T>() where T : Component
	{
		T t = GOExtensions.Construct<T>();
		UnityEngine.Object.DontDestroyOnLoad(t.gameObject);
		return t.GetComponent(typeof(T)) as T;
	}

	public static T ConstructFromResource<T>(string name) where T : Component
	{
		GameObject original = Resources.Load(name) as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		return gameObject.GetComponent(typeof(T)) as T;
	}

	public static T ConstructFromResourcePersistant<T>(string name) where T : Component
	{
		GameObject original = Resources.Load(name) as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		UnityEngine.Object.DontDestroyOnLoad(gameObject.gameObject);
		return gameObject.GetComponent(typeof(T)) as T;
	}

	public static T AddDisabledComponent<T>(GameObject gameObject) where T : MonoBehaviour
	{
		T result = gameObject.AddComponent(typeof(T)) as T;
		result.enabled = false;
		return result;
	}

	public static T AddComponentWithData<T>(GameObject gameObject, DataBaseEntry data) where T : DataComponent
	{
		T result = gameObject.AddComponent(typeof(T)) as T;
		result.InitFromData(data);
		return result;
	}

	public static T AddDisabledComponentWithData<T>(GameObject gameObject, DataBaseEntry data) where T : DataComponent
	{
		T result = gameObject.AddComponent(typeof(T)) as T;
		result.InitFromData(data);
		result.enabled = false;
		return result;
	}
}
