using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
public sealed class SpawnPool : MonoBehaviour, IEnumerable, IList<Transform>, ICollection<Transform>, IEnumerable<Transform>
{
	public delegate void OnCacheDestroy(Transform trans);

	public delegate void OnDestroyFinished();

	public string poolName = string.Empty;

	public bool matchPoolScale;

	public bool matchPoolLayer;

	public bool dontDestroyOnLoad;

	public bool logMessages;

	public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();

	public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();

	[HideInInspector]
	public float maxParticleDespawnTime = 60f;

	public PrefabsDict prefabs = new PrefabsDict();

	public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

	private List<PrefabPool> _prefabPools = new List<PrefabPool>();

	internal List<Transform> spawned = new List<Transform>();

	private static int miEffectLayer = LayerMask.NameToLayer("Effect");

	private static int miUIEffectLayer = LayerMask.NameToLayer("UIEffect");

	private static int miModelLayer = LayerMask.NameToLayer("UIModel");

	private static SpawnPool.OnCacheDestroy monCacheDestroy = null;

	private static SpawnPool.OnDestroyFinished mOnDestroyFinished = null;

	public Transform group
	{
		get;
		private set;
	}

	public Dictionary<string, PrefabPool> prefabPools
	{
		get
		{
			Dictionary<string, PrefabPool> dictionary = new Dictionary<string, PrefabPool>();
			foreach (PrefabPool current in this._prefabPools)
			{
				dictionary[current.prefabGO.name] = current;
			}
			return dictionary;
		}
	}

	public Transform this[int index]
	{
		get
		{
			return this.spawned[index];
		}
		set
		{
			throw new NotImplementedException("Read-only.");
		}
	}

	public int Count
	{
		get
		{
			return this.spawned.Count;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	[DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
	{
		SpawnPool.GetEnumerator>c__Iterator9 getEnumerator>c__Iterator = new SpawnPool.GetEnumerator>c__Iterator9();
		getEnumerator>c__Iterator.<>f__this = this;
		return getEnumerator>c__Iterator;
	}

	bool ICollection<Transform>.Remove(Transform item)
	{
		throw new NotImplementedException();
	}

	private void Awake()
	{
		if (this.dontDestroyOnLoad)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		this.group = base.transform;
		if (this.poolName == string.Empty)
		{
			this.poolName = this.group.name.Replace("Pool", string.Empty);
			this.poolName = this.poolName.Replace("(Clone)", string.Empty);
		}
		if (this.logMessages)
		{
			UnityEngine.Debug.Log(string.Format("SpawnPool {0}: Initializing..", this.poolName));
		}
		foreach (PrefabPool current in this._perPrefabPoolOptions)
		{
			if (current.prefab == null)
			{
				if (this.logMessages)
				{
					UnityEngine.Debug.LogWarning(string.Format("Initialization Warning: Pool '{0}' contains a PrefabPool with no prefab reference. Skipping.", this.poolName));
				}
			}
			else
			{
				current.inspectorInstanceConstructor();
				this.CreatePrefabPool(current);
			}
		}
		CachePoolManager.Pools.Add(this);
	}

	public void ClearSpawnCache()
	{
		CachePoolManager.Pools.Remove(this);
		base.StopAllCoroutines();
		this.spawned.Clear();
		for (int i = 0; i < this._prefabPools.Count; i++)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			Transform prefab = prefabPool.prefab;
			prefabPool.SelfDestruct();
			if (SpawnPool.monCacheDestroy != null)
			{
				SpawnPool.monCacheDestroy(prefab);
			}
		}
		this._prefabPools.Clear();
		if (SpawnPool.mOnDestroyFinished != null)
		{
			SpawnPool.mOnDestroyFinished();
		}
	}

	private void OnDestroy()
	{
	}

	public void ClearSpawnPool()
	{
		this.spawned.Clear();
		for (int i = 0; i < this._prefabPools.Count; i++)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			Transform prefab = prefabPool.prefab;
			prefabPool.SelfDestruct();
			if (SpawnPool.monCacheDestroy != null)
			{
				SpawnPool.monCacheDestroy(prefab);
			}
		}
		this._prefabPools.Clear();
	}

	public void ForceClearMemory()
	{
		int second = DateTime.Now.Second;
		for (int i = 0; i < this._prefabPools.Count; i++)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			prefabPool.ProcessTimeOutInstance();
			if (prefabPool.despawned.Count == 0 && prefabPool.spawned.Count == 0)
			{
				Transform prefab = prefabPool.prefab;
				prefabPool.SelfDestruct();
				if (SpawnPool.monCacheDestroy != null)
				{
					SpawnPool.monCacheDestroy(prefab);
				}
				this._prefabPools.RemoveAt(i);
				i--;
			}
		}
	}

	public void RemoveEmptyPrefabPools()
	{
		for (int i = 0; i < this._prefabPools.Count; i++)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			if (prefabPool != null)
			{
				prefabPool.ProcessTimeOutInstance();
				if (prefabPool.despawned.Count == 0 && prefabPool.spawned.Count == 0 && prefabPool.fTimeSecondWhenEmpty > 0)
				{
					Transform prefab = prefabPool.prefab;
					prefabPool.SelfDestruct();
					if (SpawnPool.monCacheDestroy != null)
					{
						SpawnPool.monCacheDestroy(prefab);
					}
					this._prefabPools.RemoveAt(i);
					i--;
				}
			}
		}
	}

	public static void CheckEffectWatcher(GameObject oEffect)
	{
		if (oEffect == null)
		{
			return;
		}
		if (oEffect.layer == SpawnPool.miEffectLayer || oEffect.layer == SpawnPool.miUIEffectLayer || oEffect.layer == SpawnPool.miModelLayer)
		{
			EffectWatcher effectWatcher = oEffect.GetComponent<EffectWatcher>();
			if (effectWatcher == null)
			{
				effectWatcher = oEffect.AddComponent<EffectWatcher>();
			}
			effectWatcher.ResetEffect();
		}
	}

	public static void ResetEffect(GameObject oEffect)
	{
		if (oEffect == null)
		{
			return;
		}
		if (oEffect.layer == SpawnPool.miEffectLayer || oEffect.layer == SpawnPool.miUIEffectLayer || oEffect.layer == SpawnPool.miModelLayer)
		{
			NcDuplicator componentInChildren = oEffect.GetComponentInChildren<NcDuplicator>();
			if (componentInChildren != null)
			{
				LogSystem.LogWarning(new object[]
				{
					oEffect.name,
					" NcDuplicator cannot be replayed."
				});
				return;
			}
			NcSpriteAnimation[] componentsInChildren = oEffect.GetComponentsInChildren<NcSpriteAnimation>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] != null)
				{
					componentsInChildren[i].ResetAnimation();
				}
			}
			NcCurveAnimation[] componentsInChildren2 = oEffect.GetComponentsInChildren<NcCurveAnimation>(true);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (componentsInChildren2[j] != null)
				{
					componentsInChildren2[j].ResetAnimation();
				}
			}
			NcDelayActive[] componentsInChildren3 = oEffect.GetComponentsInChildren<NcDelayActive>(true);
			for (int k = 0; k < componentsInChildren3.Length; k++)
			{
				if (componentsInChildren3[k] != null)
				{
					componentsInChildren3[k].ResetAnimation();
				}
			}
			NcUvAnimation[] componentsInChildren4 = oEffect.GetComponentsInChildren<NcUvAnimation>(true);
			for (int l = 0; l < componentsInChildren4.Length; l++)
			{
				if (componentsInChildren4[l] != null)
				{
					componentsInChildren4[l].ResetAnimation();
				}
			}
			ParticleSystem[] componentsInChildren5 = oEffect.GetComponentsInChildren<ParticleSystem>(true);
			for (int m = 0; m < componentsInChildren5.Length; m++)
			{
				ParticleSystem particleSystem = componentsInChildren5[m];
				if (particleSystem != null)
				{
					particleSystem.Stop();
					particleSystem.Clear();
					particleSystem.time = 0f;
					particleSystem.Play();
				}
			}
			Animation[] componentsInChildren6 = oEffect.GetComponentsInChildren<Animation>(true);
			for (int n = 0; n < componentsInChildren6.Length; n++)
			{
				Animation animation = componentsInChildren6[n];
				if (!(animation == null))
				{
					foreach (AnimationState animationState in animation)
					{
						animationState.time = 0f;
					}
					animation.Play();
				}
			}
			DestroyForTime[] componentsInChildren7 = oEffect.GetComponentsInChildren<DestroyForTime>(true);
			for (int num = 0; num < componentsInChildren7.Length; num++)
			{
				DestroyForTime destroyForTime = componentsInChildren7[num];
				if (!(destroyForTime == null))
				{
					destroyForTime.Reset();
				}
			}
		}
	}

	public static void SetCacheDestroy(SpawnPool.OnCacheDestroy onCacheDestroy)
	{
		SpawnPool.monCacheDestroy = onCacheDestroy;
	}

	public static void SetDestroyFinished(SpawnPool.OnDestroyFinished callfunc)
	{
		SpawnPool.mOnDestroyFinished = callfunc;
	}

	public void CreatePrefabPool(PrefabPool prefabPool)
	{
		if (this.GetPrefab(prefabPool.prefab) == null)
		{
			prefabPool.spawnPool = this;
			this._prefabPools.Add(prefabPool);
		}
		if (!prefabPool.preloaded)
		{
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0}: Preloading {1} {2}", this.poolName, prefabPool.preloadAmount, prefabPool.prefab.name));
			}
			prefabPool.PreloadInstances();
		}
	}

	public void Add(Transform instance, string prefabName, bool despawn, bool parent)
	{
		foreach (PrefabPool current in this._prefabPools)
		{
			if (current.prefabGO == null)
			{
				if (this.logMessages)
				{
					LogSystem.LogWarning(new object[]
					{
						"Unexpected Error: PrefabPool.prefabGO is null"
					});
				}
				return;
			}
			if (current.prefabGO.name == prefabName)
			{
				current.AddUnpooled(instance, despawn);
				if (this.logMessages)
				{
					UnityEngine.Debug.Log(string.Format("SpawnPool {0}: Adding previously unpooled instance {1}", this.poolName, instance.name));
				}
				if (parent)
				{
					instance.parent = this.group;
				}
				if (!despawn)
				{
					this.spawned.Add(instance);
				}
				return;
			}
		}
		if (this.logMessages)
		{
			LogSystem.LogWarning(new object[]
			{
				string.Format("SpawnPool {0}: PrefabPool {1} not found.", this.poolName, prefabName)
			});
		}
	}

	public void Add(Transform item)
	{
		string message = "Use SpawnPool.Spawn() to properly add items to the pool.";
		throw new NotImplementedException(message);
	}

	public void Remove(Transform item)
	{
		string message = "Use Despawn() to properly manage items that should remain in the pool but be deactivated.";
		throw new NotImplementedException(message);
	}

	public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
	{
		int i = 0;
		Transform transform;
		while (i < this._prefabPools.Count)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			if (prefabPool.prefabGO == prefab.gameObject)
			{
				transform = prefabPool.SpawnInstance(pos, rot);
				if (transform == null)
				{
					return null;
				}
				if (transform.parent != this.group)
				{
					transform.parent = this.group;
				}
				this.spawned.Add(transform);
				return transform;
			}
			else
			{
				i++;
			}
		}
		PrefabPool prefabPool2 = new PrefabPool(prefab);
		this.CreatePrefabPool(prefabPool2);
		transform = prefabPool2.SpawnInstance(pos, rot);
		transform.parent = this.group;
		this.spawned.Add(transform);
		return transform;
	}

	public bool IsCacheObject(GameObject obj)
	{
		return this.spawned != null && obj != null && this.spawned.Contains(obj.transform);
	}

	public bool IsCachePrefab(UnityEngine.Object obj)
	{
		GameObject gameObject = obj as GameObject;
		if (gameObject == null)
		{
			return false;
		}
		for (int i = 0; i < this._prefabPools.Count; i++)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			if (prefabPool.prefabGO == gameObject)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsInDespawn(Transform trans)
	{
		for (int i = 0; i < this._prefabPools.Count; i++)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			if (prefabPool.despawned.Contains(trans))
			{
				return true;
			}
		}
		return false;
	}

	public Transform Spawn(Transform prefab)
	{
		return this.Spawn(prefab, Vector3.zero, Quaternion.identity);
	}

	public ParticleEmitter Spawn(ParticleEmitter prefab, Vector3 pos, Quaternion quat)
	{
		Transform transform = this.Spawn(prefab.transform, pos, quat);
		if (transform == null)
		{
			return null;
		}
		ParticleAnimator component = transform.GetComponent<ParticleAnimator>();
		if (component != null)
		{
			component.autodestruct = false;
		}
		ParticleEmitter component2 = transform.GetComponent<ParticleEmitter>();
		component2.emit = true;
		base.StartCoroutine(this.ListenForEmitDespawn(component2));
		return component2;
	}

	public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion quat)
	{
		Transform transform = this.Spawn(prefab.transform, pos, quat);
		if (transform == null)
		{
			return null;
		}
		ParticleSystem component = transform.GetComponent<ParticleSystem>();
		base.StartCoroutine(this.ListenForEmitDespawn(component));
		return component;
	}

	public ParticleEmitter Spawn(ParticleEmitter prefab, Vector3 pos, Quaternion quat, string colorPropertyName, Color color)
	{
		Transform transform = this.Spawn(prefab.transform, pos, quat);
		if (transform == null)
		{
			return null;
		}
		ParticleAnimator component = transform.GetComponent<ParticleAnimator>();
		if (component != null)
		{
			component.autodestruct = false;
		}
		ParticleEmitter component2 = transform.GetComponent<ParticleEmitter>();
		component2.renderer.material.SetColor(colorPropertyName, color);
		component2.emit = true;
		base.StartCoroutine(this.ListenForEmitDespawn(component2));
		return component2;
	}

	private void DespawnTree(Transform xform)
	{
		for (int i = xform.childCount - 1; i >= 0; i--)
		{
			Transform child = xform.GetChild(i);
			if (!(child == null))
			{
				this.DespawnTree(child);
			}
		}
		if (this.IsSpawned(xform))
		{
			xform.parent = base.transform;
			this.DespawnObject(xform);
		}
	}

	private void DespawnObject(Transform xform)
	{
		bool flag = false;
		for (int i = 0; i < this._prefabPools.Count; i++)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			if (prefabPool.spawned.Contains(xform))
			{
				flag = prefabPool.DespawnInstance(xform);
				break;
			}
			if (prefabPool.despawned.Contains(xform))
			{
				if (this.logMessages)
				{
					LogSystem.LogWarning(new object[]
					{
						string.Format("SpawnPool {0}: {1} has already been despawned. You cannot despawn something more than once!", this.poolName, xform.name)
					});
				}
				return;
			}
		}
		if (!flag)
		{
			if (this.logMessages)
			{
				LogSystem.LogWarning(new object[]
				{
					string.Format("SpawnPool {0}: {1} not found in SpawnPool", this.poolName, xform.name)
				});
			}
			return;
		}
		this.spawned.Remove(xform);
	}

	public void Despawn(Transform xform)
	{
		if (xform == null)
		{
			return;
		}
		this.DespawnTree(xform);
	}

	public void Despawn(Transform instance, float seconds)
	{
		base.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds));
	}

	[DebuggerHidden]
	private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds)
	{
		SpawnPool.<DoDespawnAfterSeconds>c__IteratorA <DoDespawnAfterSeconds>c__IteratorA = new SpawnPool.<DoDespawnAfterSeconds>c__IteratorA();
		<DoDespawnAfterSeconds>c__IteratorA.seconds = seconds;
		<DoDespawnAfterSeconds>c__IteratorA.instance = instance;
		<DoDespawnAfterSeconds>c__IteratorA.<$>seconds = seconds;
		<DoDespawnAfterSeconds>c__IteratorA.<$>instance = instance;
		<DoDespawnAfterSeconds>c__IteratorA.<>f__this = this;
		return <DoDespawnAfterSeconds>c__IteratorA;
	}

	public void DespawnAll()
	{
		for (int i = 0; i < this.spawned.Count; i++)
		{
			Transform xform = this.spawned[i];
			this.Despawn(xform);
		}
	}

	public bool IsSpawned(Transform instance)
	{
		return this.spawned.Contains(instance);
	}

	public Transform GetPrefab(Transform prefab)
	{
		foreach (PrefabPool current in this._prefabPools)
		{
			if (current.prefabGO == null && this.logMessages)
			{
				LogSystem.LogWarning(new object[]
				{
					string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", this.poolName)
				});
			}
			if (current.prefabGO == prefab.gameObject)
			{
				return current.prefab;
			}
		}
		return null;
	}

	public GameObject GetPrefab(GameObject prefab)
	{
		foreach (PrefabPool current in this._prefabPools)
		{
			if (current.prefabGO == null && this.logMessages)
			{
				LogSystem.LogWarning(new object[]
				{
					string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", this.poolName)
				});
			}
			if (current.prefabGO == prefab)
			{
				return current.prefabGO;
			}
		}
		return null;
	}

	[DebuggerHidden]
	private IEnumerator ListenForEmitDespawn(ParticleEmitter emitter)
	{
		SpawnPool.<ListenForEmitDespawn>c__IteratorB <ListenForEmitDespawn>c__IteratorB = new SpawnPool.<ListenForEmitDespawn>c__IteratorB();
		<ListenForEmitDespawn>c__IteratorB.emitter = emitter;
		<ListenForEmitDespawn>c__IteratorB.<$>emitter = emitter;
		<ListenForEmitDespawn>c__IteratorB.<>f__this = this;
		return <ListenForEmitDespawn>c__IteratorB;
	}

	[DebuggerHidden]
	private IEnumerator ListenForEmitDespawn(ParticleSystem emitter)
	{
		SpawnPool.<ListenForEmitDespawn>c__IteratorC <ListenForEmitDespawn>c__IteratorC = new SpawnPool.<ListenForEmitDespawn>c__IteratorC();
		<ListenForEmitDespawn>c__IteratorC.emitter = emitter;
		<ListenForEmitDespawn>c__IteratorC.<$>emitter = emitter;
		<ListenForEmitDespawn>c__IteratorC.<>f__this = this;
		return <ListenForEmitDespawn>c__IteratorC;
	}

	public override string ToString()
	{
		List<string> list = new List<string>();
		foreach (Transform current in this.spawned)
		{
			list.Add(current.name);
		}
		return string.Join(", ", list.ToArray());
	}

	public string ToPrefabString()
	{
		List<string> list = new List<string>();
		foreach (PrefabPool current in this._prefabPools)
		{
			list.Add(current.prefab.name);
		}
		return string.Join(", ", list.ToArray());
	}

	public bool Contains(Transform item)
	{
		string message = "Use IsSpawned(Transform instance) instead.";
		throw new NotImplementedException(message);
	}

	public void CopyTo(Transform[] array, int arrayIndex)
	{
		this.spawned.CopyTo(array, arrayIndex);
	}

	[DebuggerHidden]
	public IEnumerator<Transform> GetEnumerator()
	{
		SpawnPool.<GetEnumerator>c__IteratorD <GetEnumerator>c__IteratorD = new SpawnPool.<GetEnumerator>c__IteratorD();
		<GetEnumerator>c__IteratorD.<>f__this = this;
		return <GetEnumerator>c__IteratorD;
	}

	public int IndexOf(Transform item)
	{
		throw new NotImplementedException();
	}

	public void Insert(int index, Transform item)
	{
		throw new NotImplementedException();
	}

	public void RemoveAt(int index)
	{
		throw new NotImplementedException();
	}

	public void Clear()
	{
		throw new NotImplementedException();
	}
}
