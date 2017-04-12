using System;
using UnityEngine;

namespace WellFired
{
	[USequencerEvent("Custom Event/Play Main Audio Action"), USequencerFriendlyName("Play Main Audio Action")]
	public class PlayMainAudioAction : ActionBase
	{
		public AudioClip lady_audioClip;

		public AudioClip man_audioClip;

		public string lady_audioId = string.Empty;

		public string man_audioId = string.Empty;

		public float ladyDuration;

		public float manDuration;

		public bool loop;

		private bool wasPlaying;

		public override void UpdateEvent()
		{
			if (!this.loop && this.lady_audioClip)
			{
				this.ladyDuration = this.lady_audioClip.length;
			}
			if (!this.loop && this.man_audioClip)
			{
				this.manDuration = this.man_audioClip.length;
			}
			if (Instance.Get<SequenceManager>().mainRoleSex == 1)
			{
				base.Duration = this.manDuration;
			}
			else
			{
				base.Duration = this.ladyDuration;
			}
			this.lady_audioClip = null;
			this.man_audioClip = null;
		}

		public override void FireEvent()
		{
			string strAudioId;
			if (Instance.Get<SequenceManager>().mainRoleSex == 1)
			{
				strAudioId = this.man_audioId;
			}
			else
			{
				strAudioId = this.lady_audioId;
			}
			if (Instance.Get<SequenceManager>().PlayAudio != null)
			{
				Instance.Get<SequenceManager>().PlayAudio(Camera.main.gameObject, strAudioId);
			}
		}

		public override void ProcessEvent(float deltaTime)
		{
		}

		public override void ManuallySetTime(float deltaTime)
		{
		}

		public override void ResumeEvent()
		{
		}

		public override void PauseEvent()
		{
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
			string strAudioId;
			if (Instance.Get<SequenceManager>().mainRoleSex == 1)
			{
				strAudioId = this.man_audioId;
			}
			else
			{
				strAudioId = this.lady_audioId;
			}
			if (Instance.Get<SequenceManager>().StopAudio != null)
			{
				Instance.Get<SequenceManager>().StopAudio(Camera.main.gameObject, strAudioId);
			}
		}
	}
}
