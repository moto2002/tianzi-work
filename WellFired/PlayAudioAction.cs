using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Play Audio Action"), USequencerFriendlyName("Play Audio Action")]
	public class PlayAudioAction : ActionBase
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
				Instance.Get<SequenceManager>().PlayAudio(Camera.main.gameObject, this.audioId);
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
				Instance.Get<SequenceManager>().StopAudio(Camera.main.gameObject, this.audioId);
			}
		}
	}
}
