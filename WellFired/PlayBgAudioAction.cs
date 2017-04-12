using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Play BackGround Audio Action"), USequencerFriendlyName("Play BackGround Audio Action")]
	public class PlayBgAudioAction : ActionBase
	{
		public string audioId = string.Empty;

		public AudioClip audioClip;

		public override void UpdateEvent()
		{
			if (this.audioClip != null)
			{
				base.Duration = this.audioClip.length;
			}
			this.audioClip = null;
		}

		public override void FireEvent()
		{
			if (Instance.Get<SequenceManager>().PlayAudio != null)
			{
				Instance.Get<SequenceManager>().PlayBgAudio(this.audioId);
			}
		}

		public override void StopEvent()
		{
			this.UndoEvent();
		}

		public override void EndEvent()
		{
			this.UndoEvent();
		}

		public override void UndoEvent()
		{
			if (Instance.Get<SequenceManager>().StopAudio != null)
			{
				Instance.Get<SequenceManager>().PlayBgSoundFunc(true);
			}
		}
	}
}
