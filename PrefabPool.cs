using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class PrefabPool
{
	private const long mciDespawnTimeInterval = 300000000L;

	public Transform prefab;

	internal GameObject prefabGO;

	public int preloadAmount = 1;

	public bool preloadTime;

	public int preloadFrames = 2;

	public float preloadDelay;

	public bool limitInstances;

	public int limitAmount = 100;

	public bool limitFIFO;

	public bool cullDespawned;

	public int cullAbove;

	public int cullDelay = 60;

	public int cullMaxPerPass = 5;

	public bool _logMessages;

	private bool forceLoggingSilent;

	public SpawnPool spawnPool;

	private bool cullingActive;

	internal List<Transform> spawned = new List<Transform>();

	public List<Transform> despawned = new List<Transform>();

	internal Dictionary<Transform, long> mDespawnedTime = new Dictionary<Transform, long>();

	private bool _preloaded;

	public int fTimeSecondWhenEmpty;

	public bool logMessages
	{
		get
		{
			if (this.forceLoggingSilent)
			{
				return false;
			}
			if (this.spawnPool.logMessages)
			{
				return this.spawnPool.logMessages;
			}
			return this._logMessages;
		}
	}

	public int totalCount
	{
		get
		{
			int num = 0;
			num += this.spawned.Count;
			return num + this.despawned.Count;
		}
	}

	internal bool preloaded
	{
		get
		{
			return this._preloaded;
		}
		private set
		{
			this._preloaded = value;
		}
	}

	public PrefabPool(Transform prefab)
	{
		this.prefab = prefab;
		this.prefabGO = prefab.gameObject;
	}

	public PrefabPool()
	{
	}

	internal void inspectorInstanceConstructor()
	{
		this.prefabGO = this.prefab.gameObject;
		this.spawned = new List<Transform>();
		this.despawned = new List<Transform>();
	}

	internal void SelfDestruct()
	{
		this.prefab = null;
		this.prefabGO = null;
		this.spawnPool = null;
		for (int i = 0; i < this.despawned.Count; i++)
		{
			Transform transform = this.despawned[i];
			if (transform != null)
			{
				this.DestroyInstance(transform);
			}
		}
		for (int j = 0; j < this.spawned.Count; j++)
		{
			Transform transform2 = this.spawned[j];
			if (transform2 != null)
			{
				this.DestroyInstance(transform2);
			}
		}
		this.mDespawnedTime.Clear();
		this.spawned.Clear();
		this.despawned.Clear();
	}

	public void ProcessTimeOutInstance()
	{
		long ticks = DateTime.Now.Ticks;
		for (int i = 0; i < this.despawned.Count; i++)
		{
			Transform transform = this.despawned[i];
			if (this.mDespawnedTime.ContainsKey(transform))
			{
				long num = this.mDespawnedTime[transform];
				if (ticks - num > 300000000L)
				{
					this.mDespawnedTime.Remove(transform);
					this.DestroyInstance(transform);
					this.despawned.RemoveAt(i);
					i--;
				}
			}
		}
		if (this.despawned.Count == 0 && this.spawned.Count == 0)
		{
			this.fTimeSecondWhenEmpty = DateTime.Now.Second;
		}
	}

	private void DestroyInstance(Transform trans)
	{
		if (trans == null)
		{
			return;
		}
		if (this.mDespawnedTime.ContainsKey(trans))
		{
			this.mDespawnedTime.Remove(trans);
		}
		UnityEngine.Object.Destroy(trans.gameObject);
	}

	internal bool DespawnInstance(Transform xform)
	{
		if (this.logMessages)
		{
			UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): Despawning '{2}'", this.spawnPool.poolName, this.prefab.name, xform.name));
		}
		this.OnDespawnObject(xform);
		return true;
	}

	private void OnDespawnObject(Transform xform)
	{
		this.spawned.Remove(xform);
		this.despawned.Add(xform);
		long ticks = DateTime.Now.Ticks;
		if (this.mDespawnedTime.ContainsKey(xform))
		{
			this.mDespawnedTime[xform] = ticks;
		}
		else
		{
			this.mDespawnedTime.Add(xform, ticks);
		}
		xform.gameObject.BroadcastMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);
		PoolManagerUtils.SetActive(xform.gameObject, false);
		if (!this.cullingActive && this.cullDespawned && this.totalCount > this.cullAbove)
		{
			this.cullingActive = true;
			this.spawnPool.StartCoroutine(this.CullDespawned());
		}
	}

	private bool IsSpawnedObject(Transform trans)
	{
		return this.spawned.Contains(trans);
	}

	private void ReleraseTransTree(Transform xform)
	{
		for (int i = 0; i < xform.childCount; i++)
		{
			Transform child = xform.GetChild(i);
			if (child != null)
			{
				this.ReleraseTransTree(child);
			}
		}
		if (this.IsSpawnedObject(xform))
		{
			if (xform.parent != null)
			{
				xform.parent = null;
			}
			this.OnDespawnObject(xform);
		}
	}

	[DebuggerHidden]
	internal IEnumerator CullDespawned()
	{
		PrefabPool.<CullDespawned>c__IteratorE <CullDespawned>c__IteratorE = new PrefabPool.<CullDespawned>c__IteratorE();
		<CullDespawned>c__IteratorE.<>f__this = this;
		return <CullDespawned>c__IteratorE;
	}

	internal Transform SpawnInstance(Vector3 pos, Quaternion rot)
	{
		if (this.limitInstances && this.limitFIFO && this.spawned.Count >= this.limitAmount)
		{
			Transform transform = this.spawned[0];
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): LIMIT REACHED! FIFO=True. Calling despawning for {2}...", this.spawnPool.poolName, this.prefab.name, transform));
			}
			this.DespawnInstance(transform);
			this.spawnPool.spawned.Remove(transform);
		}
		this.TrimSpawnedIns();
		Transform transform2;
		if (this.despawned.Count == 0)
		{
			transform2 = this.SpawnNew(pos, rot);
		}
		else
		{
			transform2 = this.despawned[0];
			this.despawned.RemoveAt(0);
			this.spawned.Add(transform2);
			if (transform2 == null)
			{
				string message = "Make sure you didn't delete a despawned instance directly.";
				throw new MissingReferenceException(message);
			}
			SpawnPool.CheckEffectWatcher(transform2.gameObject);
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): respawning '{2}'.", this.spawnPool.poolName, this.prefab.name, transform2.name));
			}
			transform2.position = pos;
			transform2.rotation = rot;
			PoolManagerUtils.SetActive(transform2.gameObject, true);
		}
		if (transform2 != null)
		{
			transform2.gameObject.BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
		}
		return transform2;
	}

	private void TrimSpawnedIns()
	{
		for (int i = 0; i < this.despawned.Count; i++)
		{
			Transform x = this.despawned[i];
			if (x == null)
			{
				this.despawned.RemoveAt(i);
				i--;
			}
		}
	}

	public Transform SpawnNew()
	{
		return this.SpawnNew(Vector3.zero, Quaternion.identity);
	}

	public Transform SpawnNew(Vector3 pos, Quaternion rot)
	{
		if (this.fTimeSecondWhenEmpty > 0)
		{
			this.fTimeSecondWhenEmpty = 0;
		}
		if (this.limitInstances && this.totalCount >= this.limitAmount)
		{
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): LIMIT REACHED! Not creating new instances! (Returning null)", this.spawnPool.poolName, this.prefab.name));
			}
			return null;
		}
		if (pos == Vector3.zero)
		{
			pos = this.spawnPool.group.position;
		}
		if (rot == Quaternion.identity)
		{
			rot = this.spawnPool.group.rotation;
		}
		Transform transform = (Transform)UnityEngine.Object.Instantiate(this.prefab, pos, rot);
		this.nameInstance(transform);
		transform.parent = this.spawnPool.group;
		if (this.spawnPool.matchPoolScale)
		{
			transform.localScale = Vector3.one;
		}
		if (this.spawnPool.matchPoolLayer)
		{
			this.SetRecursively(transform, this.spawnPool.gameObject.layer);
		}
		this.spawned.Add(transform);
		if (this.logMessages)
		{
			UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): Spawned new instance '{2}'.", this.spawnPool.poolName, this.prefab.name, transform.name));
		}
		return transform;
	}

	private void SetRecursively(Transform xform, int layer)
	{
		xform.gameObject.layer = layer;
		foreach (Transform xform2 in xform)
		{
			this.SetRecursively(xform2, layer);
		}
	}

	internal void AddUnpooled(Transform inst, bool despawn)
	{
		this.nameInstance(inst);
		if (despawn)
		{
			PoolManagerUtils.SetActive(inst.gameObject, false);
			this.despawned.Add(inst);
			long ticks = DateTime.Now.Ticks;
			this.mDespawnedTime.Add(inst, ticks);
		}
		else
		{
			this.spawned.Add(inst);
		}
	}

	internal void PreloadInstances()
	{
		if (this.preloaded)
		{
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): Already preloaded! You cannot preload twice. If you are running this through code, make sure it isn't also defined in the Inspector.", this.spawnPool.poolName, this.prefab.name));
			}
			return;
		}
		if (this.prefab == null)
		{
			if (this.logMessages)
			{
				LogSystem.LogWarning(new object[]
				{
					string.Format("SpawnPool {0} ({1}): Prefab cannot be null.", this.spawnPool.poolName, this.prefab.name)
				});
			}
			return;
		}
		if (this.limitInstances && this.preloadAmount > this.limitAmount)
		{
			if (this.logMessages)
			{
				UnityEngine.Debug.LogWarning(string.Format("SpawnPool {0} ({1}): You turned ON 'Limit Instances' and entered a 'Limit Amount' greater than the 'Preload Amount'! Setting preload amount to limit amount.", this.spawnPool.poolName, this.prefab.name));
			}
			this.preloadAmount = this.limitAmount;
		}
		if (this.cullDespawned && this.preloadAmount > this.cullAbove && this.logMessages)
		{
			UnityEngine.Debug.LogWarning(string.Format("SpawnPool {0} ({1}): You turned ON Culling and entered a 'Cull Above' threshold greater than the 'Preload Amount'! This will cause the culling feature to trigger immediatly, which is wrong conceptually. Only use culling for extreme situations. See the docs.", this.spawnPool.poolName, this.prefab.name));
		}
		if (this.preloadTime)
		{
			if (this.preloadFrames > this.preloadAmount)
			{
				if (this.logMessages)
				{
					UnityEngine.Debug.LogWarning(string.Format("SpawnPool {0} ({1}): Preloading over-time is on but the frame duration is greater than the number of instances to preload. The minimum spawned per frame is 1, so the maximum time is the same as the number of instances. Changing the preloadFrames value...", this.spawnPool.poolName, this.prefab.name));
				}
				this.preloadFrames = this.preloadAmount;
			}
			this.spawnPool.StartCoroutine(this.PreloadOverTime());
		}
		else
		{
			this.forceLoggingSilent = true;
			while (this.totalCount < this.preloadAmount)
			{
				Transform xform = this.SpawnNew();
				this.DespawnInstance(xform);
			}
			this.forceLoggingSilent = false;
		}
	}

	[DebuggerHidden]
	private IEnumerator PreloadOverTime()
	{
		PrefabPool.<PreloadOverTime>c__IteratorF <PreloadOverTime>c__IteratorF = new PrefabPool.<PreloadOverTime>c__IteratorF();
		<PreloadOverTime>c__IteratorF.<>f__this = this;
		return <PreloadOverTime>c__IteratorF;
	}

	private void nameInstance(Transform instance)
	{
		instance.name += (this.totalCount + 1).ToString("#000");
	}
}
