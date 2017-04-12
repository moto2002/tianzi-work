using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class _CameraShake : MonoBehaviour
{
	internal class ShakeState
	{
		internal readonly Vector3 startPosition;

		internal readonly Quaternion startRotation;

		internal readonly Vector2 guiStartPosition;

		internal Vector3 shakePosition;

		internal Quaternion shakeRotation;

		internal Vector2 guiShakePosition;

		internal ShakeState(Vector3 position, Quaternion rotation, Vector2 guiPosition)
		{
			this.startPosition = position;
			this.startRotation = rotation;
			this.guiStartPosition = guiPosition;
			this.shakePosition = position;
			this.shakeRotation = rotation;
			this.guiShakePosition = guiPosition;
		}
	}

	private const bool checkForMinimumValues = true;

	private const float minShakeValue = 0.001f;

	private const float minRotationValue = 0.001f;

	public List<Camera> cameras = new List<Camera>();

	public int numberOfShakes = 2;

	public Vector3 shakeAmount = Vector3.one;

	public Vector3 rotationAmount = Vector3.one;

	public float distance = 0.1f;

	public float speed = 50f;

	public float decay = 0.2f;

	public float guiShakeModifier = 1f;

	public bool multiplyByTimeScale = true;

	private Rect shakeRect;

	private bool shaking;

	private bool cancelling;

	private Dictionary<Camera, List<_CameraShake.ShakeState>> states = new Dictionary<Camera, List<_CameraShake.ShakeState>>();

	private Dictionary<Camera, int> shakeCount = new Dictionary<Camera, int>();

	public static _CameraShake instance;

	private List<Vector3> offsetCache = new List<Vector3>(10);

	private List<Quaternion> rotationCache = new List<Quaternion>(10);

	public event Action cameraShakeStarted
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.cameraShakeStarted = (Action)Delegate.Combine(this.cameraShakeStarted, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.cameraShakeStarted = (Action)Delegate.Remove(this.cameraShakeStarted, value);
		}
	}

	public event Action allCameraShakesCompleted
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.allCameraShakesCompleted = (Action)Delegate.Combine(this.allCameraShakesCompleted, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.allCameraShakesCompleted = (Action)Delegate.Remove(this.allCameraShakesCompleted, value);
		}
	}

	public static bool isShaking
	{
		get
		{
			return _CameraShake.instance.IsShaking();
		}
	}

	public static bool isCancelling
	{
		get
		{
			return _CameraShake.instance.IsCancelling();
		}
	}

	private void OnEnable()
	{
		if (this.cameras.Count < 1 && base.GetComponent<Camera>())
		{
			this.cameras.Add(base.GetComponent<Camera>());
		}
		if (this.cameras.Count < 1 && Camera.main)
		{
			this.cameras.Add(Camera.main);
		}
		if (this.cameras.Count < 1)
		{
			LogSystem.LogWarning(new object[]
			{
				"Camera Shake: No cameras assigned in the inspector!"
			});
		}
		_CameraShake.instance = this;
	}

	public static void Shake()
	{
		_CameraShake.instance.DoShake();
	}

	public static void Shake(int numberOfShakes, Vector3 shakeAmount, Vector3 rotationAmount, float distance, float speed, float decay, float guiShakeModifier, bool multiplyByTimeScale)
	{
		_CameraShake.instance.DoShake(numberOfShakes, shakeAmount, rotationAmount, distance, speed, decay, guiShakeModifier, multiplyByTimeScale);
	}

	public static void Shake(Action callback)
	{
		_CameraShake.instance.DoShake(callback);
	}

	public static void Shake(int numberOfShakes, Vector3 shakeAmount, Vector3 rotationAmount, float distance, float speed, float decay, float guiShakeModifier, bool multiplyByTimeScale, Action callback)
	{
		_CameraShake.instance.DoShake(numberOfShakes, shakeAmount, rotationAmount, distance, speed, decay, guiShakeModifier, multiplyByTimeScale, callback);
	}

	public static void CancelShake()
	{
		_CameraShake.instance.DoCancelShake();
	}

	public static void CancelShake(float time)
	{
		_CameraShake.instance.DoCancelShake(time);
	}

	public static void BeginShakeGUI()
	{
		_CameraShake.instance.DoBeginShakeGUI();
	}

	public static void EndShakeGUI()
	{
		_CameraShake.instance.DoEndShakeGUI();
	}

	public static void BeginShakeGUILayout()
	{
		_CameraShake.instance.DoBeginShakeGUILayout();
	}

	public static void EndShakeGUILayout()
	{
		_CameraShake.instance.DoEndShakeGUILayout();
	}

	public bool IsShaking()
	{
		return this.shaking;
	}

	public bool IsCancelling()
	{
		return this.cancelling;
	}

	public void DoShake()
	{
		Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
		foreach (Camera current in this.cameras)
		{
			base.StartCoroutine(this.DoShake_Internal(current, insideUnitSphere, this.numberOfShakes, this.shakeAmount, this.rotationAmount, this.distance, this.speed, this.decay, this.guiShakeModifier, this.multiplyByTimeScale, null));
		}
	}

	public void DoShake(int numberOfShakes, Vector3 shakeAmount, Vector3 rotationAmount, float distance, float speed, float decay, float guiShakeModifier, bool multiplyByTimeScale)
	{
		Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
		foreach (Camera current in this.cameras)
		{
			base.StartCoroutine(this.DoShake_Internal(current, insideUnitSphere, numberOfShakes, shakeAmount, rotationAmount, distance, speed, decay, guiShakeModifier, multiplyByTimeScale, null));
		}
	}

	public void DoShake(Action callback)
	{
		Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
		foreach (Camera current in this.cameras)
		{
			base.StartCoroutine(this.DoShake_Internal(current, insideUnitSphere, this.numberOfShakes, this.shakeAmount, this.rotationAmount, this.distance, this.speed, this.decay, this.guiShakeModifier, this.multiplyByTimeScale, callback));
		}
	}

	public void DoShake(int numberOfShakes, Vector3 shakeAmount, Vector3 rotationAmount, float distance, float speed, float decay, float guiShakeModifier, bool multiplyByTimeScale, Action callback)
	{
		Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
		foreach (Camera current in this.cameras)
		{
			base.StartCoroutine(this.DoShake_Internal(current, insideUnitSphere, numberOfShakes, shakeAmount, rotationAmount, distance, speed, decay, guiShakeModifier, multiplyByTimeScale, callback));
		}
	}

	public void DoCancelShake()
	{
		if (this.shaking && !this.cancelling)
		{
			this.shaking = false;
			base.StopAllCoroutines();
			foreach (Camera current in this.cameras)
			{
				if (this.shakeCount.ContainsKey(current))
				{
					this.shakeCount[current] = 0;
				}
				this.ResetState(current.transform, current);
			}
		}
	}

	public void DoCancelShake(float time)
	{
		if (this.shaking && !this.cancelling)
		{
			base.StopAllCoroutines();
			base.StartCoroutine(this.DoResetState(this.cameras, this.shakeCount, time));
		}
	}

	public void DoBeginShakeGUI()
	{
		this.CheckShakeRect();
		GUI.BeginGroup(this.shakeRect);
	}

	public void DoEndShakeGUI()
	{
		GUI.EndGroup();
	}

	public void DoBeginShakeGUILayout()
	{
		this.CheckShakeRect();
		GUILayout.BeginArea(this.shakeRect);
	}

	public void DoEndShakeGUILayout()
	{
		GUILayout.EndArea();
	}

	private void OnDrawGizmosSelected()
	{
		foreach (Camera current in this.cameras)
		{
			if (current)
			{
				if (this.IsShaking())
				{
					Vector3 vector = current.worldToCameraMatrix.GetColumn(3);
					vector.z *= -1f;
					vector = current.transform.position + current.transform.TransformPoint(vector);
					Quaternion q = _CameraShake.QuaternionFromMatrix(current.worldToCameraMatrix.inverse * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, 1f, -1f)));
					Matrix4x4 matrix = Matrix4x4.TRS(vector, q, current.transform.lossyScale);
					Gizmos.matrix = matrix;
				}
				else
				{
					Matrix4x4 matrix2 = Matrix4x4.TRS(current.transform.position, current.transform.rotation, current.transform.lossyScale);
					Gizmos.matrix = matrix2;
				}
				Gizmos.DrawWireCube(Vector3.zero, this.shakeAmount);
				Gizmos.color = Color.cyan;
				if (current.orthographic)
				{
					Vector3 center = new Vector3(0f, 0f, (current.nearClipPlane + current.farClipPlane) / 2f);
					Vector3 size = new Vector3(current.orthographicSize / current.aspect, current.orthographicSize * 2f, current.farClipPlane - current.nearClipPlane);
					Gizmos.DrawWireCube(center, size);
				}
				else
				{
					Gizmos.DrawFrustum(Vector3.zero, current.fieldOfView, current.farClipPlane, current.nearClipPlane, 0.7f / current.aspect);
				}
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator DoShake_Internal(Camera cam, Vector3 seed, int numberOfShakes, Vector3 shakeAmount, Vector3 rotationAmount, float distance, float speed, float decay, float guiShakeModifier, bool multiplyByTimeScale, Action callback)
	{
		_CameraShake.<DoShake_Internal>c__Iterator1D <DoShake_Internal>c__Iterator1D = new _CameraShake.<DoShake_Internal>c__Iterator1D();
		<DoShake_Internal>c__Iterator1D.seed = seed;
		<DoShake_Internal>c__Iterator1D.cam = cam;
		<DoShake_Internal>c__Iterator1D.numberOfShakes = numberOfShakes;
		<DoShake_Internal>c__Iterator1D.distance = distance;
		<DoShake_Internal>c__Iterator1D.multiplyByTimeScale = multiplyByTimeScale;
		<DoShake_Internal>c__Iterator1D.guiShakeModifier = guiShakeModifier;
		<DoShake_Internal>c__Iterator1D.rotationAmount = rotationAmount;
		<DoShake_Internal>c__Iterator1D.shakeAmount = shakeAmount;
		<DoShake_Internal>c__Iterator1D.speed = speed;
		<DoShake_Internal>c__Iterator1D.decay = decay;
		<DoShake_Internal>c__Iterator1D.callback = callback;
		<DoShake_Internal>c__Iterator1D.<$>seed = seed;
		<DoShake_Internal>c__Iterator1D.<$>cam = cam;
		<DoShake_Internal>c__Iterator1D.<$>numberOfShakes = numberOfShakes;
		<DoShake_Internal>c__Iterator1D.<$>distance = distance;
		<DoShake_Internal>c__Iterator1D.<$>multiplyByTimeScale = multiplyByTimeScale;
		<DoShake_Internal>c__Iterator1D.<$>guiShakeModifier = guiShakeModifier;
		<DoShake_Internal>c__Iterator1D.<$>rotationAmount = rotationAmount;
		<DoShake_Internal>c__Iterator1D.<$>shakeAmount = shakeAmount;
		<DoShake_Internal>c__Iterator1D.<$>speed = speed;
		<DoShake_Internal>c__Iterator1D.<$>decay = decay;
		<DoShake_Internal>c__Iterator1D.<$>callback = callback;
		<DoShake_Internal>c__Iterator1D.<>f__this = this;
		return <DoShake_Internal>c__Iterator1D;
	}

	private Vector3 GetGeometricAvg(List<_CameraShake.ShakeState> states, bool position)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = (float)states.Count;
		foreach (_CameraShake.ShakeState current in states)
		{
			if (position)
			{
				num -= current.shakePosition.x;
				num2 -= current.shakePosition.y;
				num3 -= current.shakePosition.z;
			}
			else
			{
				num += current.guiShakePosition.x;
				num2 += current.guiShakePosition.y;
			}
		}
		return new Vector3(num / num4, num2 / num4, num3 / num4);
	}

	private Quaternion GetAvgRotation(List<_CameraShake.ShakeState> states)
	{
		Quaternion shakeRotation = new Quaternion(0f, 0f, 0f, 0f);
		foreach (_CameraShake.ShakeState current in states)
		{
			if (Quaternion.Dot(current.shakeRotation, shakeRotation) > 0f)
			{
				shakeRotation.x += current.shakeRotation.x;
				shakeRotation.y += current.shakeRotation.y;
				shakeRotation.z += current.shakeRotation.z;
				shakeRotation.w += current.shakeRotation.w;
			}
			else
			{
				shakeRotation.x += -current.shakeRotation.x;
				shakeRotation.y += -current.shakeRotation.y;
				shakeRotation.z += -current.shakeRotation.z;
				shakeRotation.w += -current.shakeRotation.w;
			}
		}
		float num = Mathf.Sqrt(shakeRotation.x * shakeRotation.x + shakeRotation.y * shakeRotation.y + shakeRotation.z * shakeRotation.z + shakeRotation.w * shakeRotation.w);
		if (num > 0.0001f)
		{
			shakeRotation.x /= num;
			shakeRotation.y /= num;
			shakeRotation.z /= num;
			shakeRotation.w /= num;
		}
		else
		{
			shakeRotation = states[0].shakeRotation;
		}
		return shakeRotation;
	}

	private void CheckShakeRect()
	{
		if ((float)ResolutionConstrain.Instance.width != this.shakeRect.width || (float)ResolutionConstrain.Instance.height != this.shakeRect.height)
		{
			this.shakeRect.width = (float)ResolutionConstrain.Instance.width;
			this.shakeRect.height = (float)ResolutionConstrain.Instance.height;
		}
	}

	private float GetPixelWidth(Transform cachedTransform, Camera cachedCamera)
	{
		Vector3 position = cachedTransform.position;
		Vector3 a = cachedCamera.WorldToScreenPoint(position - cachedTransform.forward * 0.01f);
		Vector3 position2 = Vector3.zero;
		if (a.x > 0f)
		{
			position2 = a - Vector3.right;
		}
		else
		{
			position2 = a + Vector3.right;
		}
		if (a.y > 0f)
		{
			position2 = a - Vector3.up;
		}
		else
		{
			position2 = a + Vector3.up;
		}
		position2 = cachedCamera.ScreenToWorldPoint(position2);
		return 1f / (cachedTransform.InverseTransformPoint(position) - cachedTransform.InverseTransformPoint(position2)).magnitude;
	}

	private void ResetState(Transform cachedTransform, Camera cam)
	{
		cam.ResetWorldToCameraMatrix();
		this.shakeRect.x = 0f;
		this.shakeRect.y = 0f;
		this.states[cam].Clear();
	}

	[DebuggerHidden]
	private IEnumerator DoResetState(List<Camera> cameras, Dictionary<Camera, int> shakeCount, float time)
	{
		_CameraShake.<DoResetState>c__Iterator1E <DoResetState>c__Iterator1E = new _CameraShake.<DoResetState>c__Iterator1E();
		<DoResetState>c__Iterator1E.cameras = cameras;
		<DoResetState>c__Iterator1E.shakeCount = shakeCount;
		<DoResetState>c__Iterator1E.time = time;
		<DoResetState>c__Iterator1E.<$>cameras = cameras;
		<DoResetState>c__Iterator1E.<$>shakeCount = shakeCount;
		<DoResetState>c__Iterator1E.<$>time = time;
		<DoResetState>c__Iterator1E.<>f__this = this;
		return <DoResetState>c__Iterator1E;
	}

	private static Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{
		return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
	}

	private static void NormalizeQuaternion(ref Quaternion q)
	{
		float num = 0f;
		for (int i = 0; i < 4; i++)
		{
			num += q[i] * q[i];
		}
		float num2 = 1f / Mathf.Sqrt(num);
		for (int j = 0; j < 4; j++)
		{
			int index;
			int expr_43 = index = j;
			float num3 = q[index];
			q[expr_43] = num3 * num2;
		}
	}
}
