using System;
using UnityEngine;

public class testMemory : MonoBehaviour
{
	public Vector2 target = new Vector2(3f, 1f);

	private GameScene scene;

	private void Start()
	{
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(20f, 20f, 200f, 50f), "path find"))
		{
		}
	}
}
