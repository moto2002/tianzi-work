using System;
using UnityEngine;
using WellFired;

public class SequenceManager
{
	public delegate void CGPlayComplete(string taskId, GameObject role);

	public delegate void PlayActionDelegate(AnimationProxy aGrounp, string strAnimation, float fSpeed);

	public delegate bool DelegateObjectAudio(GameObject oSource, string strAudioId);

	public delegate bool DelegateObjectBgAudio(string strAudioId);

	public delegate void DelegateObjectPlayBgSound(bool value);

	public delegate void DelegateLoadAsset(string strFileName, AssetCallBack callback);

	public const float UNITCULLINGDISTANCE = 20f;

	public const float TILECULLINGDISTANCE = 40f;

	private bool misEditor = true;

	public static readonly Vector3 CAMERAANGLE = new Vector3(40f, 54.6f, 0f);

	public static readonly Vector3 CAMERADISTANCE = new Vector3(0f, 0f, -26f);

	private Vector3 mAngle = SequenceManager.CAMERAANGLE;

	private Vector3 mDistance = SequenceManager.CAMERADISTANCE;

	private USSequencer mMovie;

	private int mMainRoleSex = 1;

	private int mMainRoleJob = 1;

	private GameObject RoleModel;

	private string mTaskId;

	private SequenceManager.CGPlayComplete mCallFunc;

	private Action mCallFunAction;

	private GameObject mCamera;

	private SequenceManager.PlayActionDelegate mPlayActionFunc;

	private SequenceManager.DelegateObjectAudio mPlayAudio;

	private SequenceManager.DelegateObjectBgAudio mPlayBgAudio;

	private SequenceManager.DelegateObjectAudio mStopAudio;

	private SequenceManager.DelegateObjectPlayBgSound mPlayBgSound;

	public Action<string> ShowDailogPanel;

	public Action HideDailogPanel;

	public Action ShowControllPanel;

	public Action HideControllPanel;

	public Action ShowMaskPanel;

	public Action<Vector3> UpdateScene;

	public bool isPlaying
	{
		get
		{
			return this.mMovie != null && this.mMovie.IsPlaying;
		}
	}

	public bool isEditor
	{
		get
		{
			return this.misEditor;
		}
		set
		{
			this.misEditor = value;
		}
	}

	public Vector3 cameraAngle
	{
		get
		{
			return this.mAngle;
		}
	}

	public Vector3 cameraDistance
	{
		get
		{
			return this.mDistance;
		}
	}

	public int mainRoleSex
	{
		get
		{
			return this.mMainRoleSex;
		}
		set
		{
			this.mMainRoleSex = value;
		}
	}

	public int mainRoleJob
	{
		get
		{
			return this.mMainRoleJob;
		}
		set
		{
			this.mMainRoleJob = value;
		}
	}

	public GameObject RealModel
	{
		get
		{
			return this.RoleModel;
		}
		set
		{
			this.RoleModel = value;
		}
	}

	public GameObject Camera
	{
		get
		{
			return this.mCamera;
		}
		set
		{
			this.mCamera = value;
		}
	}

	public SequenceManager.PlayActionDelegate PlayActionFunc
	{
		get
		{
			return this.mPlayActionFunc;
		}
		set
		{
			this.mPlayActionFunc = value;
		}
	}

	public SequenceManager.DelegateObjectAudio PlayAudio
	{
		get
		{
			return this.mPlayAudio;
		}
		set
		{
			this.mPlayAudio = value;
		}
	}

	public SequenceManager.DelegateObjectBgAudio PlayBgAudio
	{
		get
		{
			return this.mPlayBgAudio;
		}
		set
		{
			this.mPlayBgAudio = value;
		}
	}

	public SequenceManager.DelegateObjectAudio StopAudio
	{
		get
		{
			return this.mStopAudio;
		}
		set
		{
			this.mStopAudio = value;
		}
	}

	public SequenceManager.DelegateObjectPlayBgSound PlayBgSoundFunc
	{
		get
		{
			return this.mPlayBgSound;
		}
		set
		{
			this.mPlayBgSound = value;
		}
	}

	public SequenceManager.DelegateLoadAsset LoadAsset
	{
		get;
		set;
	}

	public SequenceManager()
	{
		Instance.Set<SequenceManager>(this, true);
	}

	public void Start(GameObject sequencer, GameObject mainRole, GameObject camera, SequenceManager.CGPlayComplete callFunc, string taskId = "")
	{
		if (this.mMovie == null)
		{
			this.mCallFunc = callFunc;
			this.RoleModel = mainRole;
			this.mCamera = camera;
			this.mTaskId = taskId;
			this.misEditor = false;
			if (sequencer != null)
			{
				USSequencer component = sequencer.GetComponent<USSequencer>();
				if (component != null)
				{
					this.PlayMovie(component);
				}
			}
		}
	}

	public void Start(GameObject sequencer, GameObject mainRole, GameObject camera, Action callFunc)
	{
		if (this.mMovie == null)
		{
			this.mCallFunAction = callFunc;
			this.RoleModel = mainRole;
			this.mCamera = camera;
			this.misEditor = false;
			if (sequencer != null)
			{
				USSequencer component = sequencer.GetComponent<USSequencer>();
				if (component != null)
				{
					this.PlayMovie(component);
				}
			}
		}
	}

	public void Stop()
	{
		if (this.mMovie.IsPlaying)
		{
			this.mMovie.Stop();
		}
	}

	private void Clear()
	{
		this.RealModel = null;
		this.Camera = null;
	}

	public void SetCameraPos(Vector3 angle, Vector3 distance)
	{
		this.mAngle = angle;
		this.mDistance = distance;
	}

	public void FllowTarget(Transform camera, Transform target, Vector3 angle, Vector3 distance, float speed)
	{
		this.mAngle = Vector3.Lerp(this.mAngle, angle, speed);
		this.mDistance = Vector3.Lerp(this.mDistance, distance, speed);
		Quaternion rotation = Quaternion.Euler(this.mAngle);
		Vector3 b = rotation * this.mDistance;
		camera.position = target.position + b;
		camera.rotation = rotation;
	}

	public void SwitchTarget(Transform camera, Vector3 angle, Vector3 pos, float speed)
	{
		this.mAngle = Vector3.Lerp(this.mAngle, angle, speed);
		Quaternion rotation = Quaternion.Euler(this.mAngle);
		camera.position = Vector3.Lerp(camera.position, pos, speed);
		camera.rotation = rotation;
	}

	private void PlayMovie(USSequencer movie)
	{
		this.mMovie = movie;
		this.mMovie.PlaybackFinished = new USSequencer.PlaybackDelegate(this.OnMoviePlayFinished);
		this.mMovie.PlaybackStopped = new USSequencer.PlaybackDelegate(this.OnMoviePlayFinished);
		this.mMovie.Play();
	}

	private void OnMoviePlayFinished(USSequencer sequencer)
	{
		this.misEditor = true;
		if (this.mCallFunc != null)
		{
			this.mCallFunc(this.mTaskId, this.RoleModel);
			this.mCallFunc = null;
		}
		if (this.mMovie != null)
		{
			UnityEngine.Object.Destroy(this.mMovie.gameObject);
			this.mMovie = null;
		}
		if (this.mCallFunAction != null)
		{
			this.mCallFunAction();
			this.mCallFunAction = null;
		}
		this.Clear();
	}
}
