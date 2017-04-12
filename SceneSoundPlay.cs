using System;
using UnityEngine;

public class SceneSoundPlay : MonoBehaviour
{
	public delegate bool PlayObjectAudioDelegate(GameObject oSource, string strAudioId);

	public static SceneSoundPlay.PlayObjectAudioDelegate PlayObjectAudio;

	public string mstrSoundID;

	public float mfDelay;

	private void Awake()
	{
	}

	private void StartSound()
	{
		if (SceneSoundPlay.PlayObjectAudio != null)
		{
			SceneSoundPlay.PlayObjectAudio(base.gameObject, this.mstrSoundID);
		}
	}

	private void Update()
	{
		if (this.mfDelay > 0f)
		{
			this.mfDelay -= Time.deltaTime;
			return;
		}
		this.StartSound();
		UnityEngine.Object.Destroy(this);
	}
}
