using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Spawn/Spawn Prefab"), USequencerEventHideDuration, USequencerFriendlyName("Spawn Prefab")]
	public class USSpawnPrefabEvent : USEventBase
	{
		public GameObject spawnPrefab;

		public Transform spawnTransform;

		public override void FireEvent()
		{
			if (!this.spawnPrefab)
			{
				Debug.Log("Attempting to spawn a prefab, but you haven't given a prefab to the event from USSpawnPrefabEvent::FireEvent");
				return;
			}
			if (this.spawnTransform)
			{
				UnityEngine.Object.Instantiate(this.spawnPrefab, this.spawnTransform.position, this.spawnTransform.rotation);
			}
			else
			{
				UnityEngine.Object.Instantiate(this.spawnPrefab, Vector3.zero, Quaternion.identity);
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
		}

		private void OnDestroy()
		{
			this.spawnPrefab = null;
			this.spawnTransform = null;
		}
	}
}
