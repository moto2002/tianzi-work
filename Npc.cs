using System;
using UnityEngine;

[ExecuteInEditMode]
public class Npc : MonoBehaviour
{
	public string npcName = string.Empty;

	public float radius = 1f;

	private void Update()
	{
		GameObjectUnit gameObjectUnit = GameScene.mainScene.FindUnit(base.gameObject.name);
		if (Application.isEditor)
		{
			gameObjectUnit.localScale = new Vector3(this.radius, this.radius, this.radius);
			if (gameObjectUnit.ins)
			{
				gameObjectUnit.ins.transform.localScale = new Vector3(this.radius, this.radius, this.radius);
			}
		}
	}
}
