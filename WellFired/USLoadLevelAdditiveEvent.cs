using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Application/Load Level Additive"), USequencerEventHideDuration, USequencerFriendlyName("Load Level Additively")]
	public class USLoadLevelAdditiveEvent : USEventBase
	{
		public bool fireInEditor;

		public string levelName = string.Empty;

		public int levelIndex = -1;

		public override void FireEvent()
		{
			if (this.levelName.Length == 0 && this.levelIndex < 0)
			{
				LogSystem.LogWarning(new object[]
				{
					"You have a Load Level event in your sequence, however, you didn't give it a level to load."
				});
				return;
			}
			if (this.levelIndex >= Application.levelCount)
			{
				LogSystem.LogWarning(new object[]
				{
					"You tried to load a level that is invalid, the level index is out of range."
				});
				return;
			}
			if (!Application.isPlaying && !this.fireInEditor)
			{
				LogSystem.Log(new object[]
				{
					"Load Level Fired, but it wasn't processed, since we are in the editor. Please set the fire In Editor flag in the inspector if you require this behaviour."
				});
				return;
			}
			if (this.levelName.Length != 0)
			{
				Application.LoadLevelAdditive(this.levelName);
			}
			if (this.levelIndex != -1)
			{
				Application.LoadLevelAdditive(this.levelIndex);
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
		}
	}
}
