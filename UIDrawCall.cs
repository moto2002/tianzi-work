using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Draw Call"), ExecuteInEditMode]
public class UIDrawCall : MonoBehaviour
{
	public enum Clipping
	{
		None,
		AlphaClip = 2,
		SoftClip,
		ConstrainButDontClip
	}

	private const int maxIndexBufferCache = 20;

	private static BetterList<UIDrawCall> mActiveList = new BetterList<UIDrawCall>();

	private static BetterList<UIDrawCall> mInactiveList = new BetterList<UIDrawCall>();

	[HideInInspector]
	[NonSerialized]
	public int depthStart = 2147483647;

	[HideInInspector]
	[NonSerialized]
	public int depthEnd = -2147483648;

	[HideInInspector]
	[NonSerialized]
	public UIPanel manager;

	[HideInInspector]
	[NonSerialized]
	public UIPanel panel;

	[HideInInspector]
	[NonSerialized]
	public bool alwaysOnScreen;

	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector3> verts = new BetterList<Vector3>();

	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector3> norms = new BetterList<Vector3>();

	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector4> tans = new BetterList<Vector4>();

	[HideInInspector]
	[NonSerialized]
	public BetterList<Vector2> uvs = new BetterList<Vector2>();

	[HideInInspector]
	[NonSerialized]
	public BetterList<Color32> cols = new BetterList<Color32>();

	private Material mMaterial;

	private Texture mTexture;

	private Shader mShader;

	private UIDrawCall.Clipping mClipping;

	private Vector4 mClipRange;

	private Vector2 mClipSoft;

	private Transform mTrans;

	private Mesh mMesh;

	private MeshFilter mFilter;

	private MeshRenderer mRenderer;

	private Material mDynamicMat;

	private int[] mIndices;

	private bool mRebuildMat = true;

	private bool mReset = true;

	private int mRenderQueue = 3000;

	private UIDrawCall.Clipping mLastClip;

	private int mTriangles;

	[NonSerialized]
	public bool isDirty;

	private static List<int[]> mCache = new List<int[]>(20);

	[Obsolete("Use UIDrawCall.activeList")]
	public static BetterList<UIDrawCall> list
	{
		get
		{
			return UIDrawCall.mActiveList;
		}
	}

	public static BetterList<UIDrawCall> activeList
	{
		get
		{
			return UIDrawCall.mActiveList;
		}
	}

	public static BetterList<UIDrawCall> inactiveList
	{
		get
		{
			return UIDrawCall.mInactiveList;
		}
	}

	public int renderQueue
	{
		get
		{
			return this.mRenderQueue;
		}
		set
		{
			if (this.mRenderQueue != value)
			{
				this.mRenderQueue = value;
				if (this.mDynamicMat != null)
				{
					this.mDynamicMat.renderQueue = value;
				}
			}
		}
	}

	public int sortingOrder
	{
		get
		{
			return (!(this.mRenderer != null)) ? 0 : this.mRenderer.sortingOrder;
		}
		set
		{
			if (this.mRenderer != null && this.mRenderer.sortingOrder != value)
			{
				this.mRenderer.sortingOrder = value;
			}
		}
	}

	public int finalRenderQueue
	{
		get
		{
			return (!(this.mDynamicMat != null)) ? this.mRenderQueue : this.mDynamicMat.renderQueue;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}

	public Material baseMaterial
	{
		get
		{
			return this.mMaterial;
		}
		set
		{
			if (this.mMaterial != value)
			{
				this.mMaterial = value;
				this.mRebuildMat = true;
			}
		}
	}

	public Material dynamicMaterial
	{
		get
		{
			return this.mDynamicMat;
		}
	}

	public Texture mainTexture
	{
		get
		{
			return this.mTexture;
		}
		set
		{
			this.mTexture = value;
			if (this.mDynamicMat != null)
			{
				this.mDynamicMat.mainTexture = value;
			}
		}
	}

	public Shader shader
	{
		get
		{
			return this.mShader;
		}
		set
		{
			if (this.mShader != value)
			{
				this.mShader = value;
				this.mRebuildMat = true;
			}
		}
	}

	public int triangles
	{
		get
		{
			return (!(this.mMesh != null)) ? 0 : this.mTriangles;
		}
	}

	public bool isClipped
	{
		get
		{
			return this.mClipping != UIDrawCall.Clipping.None;
		}
	}

	public UIDrawCall.Clipping clipping
	{
		get
		{
			return this.mClipping;
		}
		set
		{
			if (this.mClipping != value)
			{
				this.mClipping = value;
				this.mReset = true;
			}
		}
	}

	public Vector4 clipRange
	{
		get
		{
			return this.mClipRange;
		}
		set
		{
			this.mClipRange = value;
		}
	}

	public Vector2 clipSoftness
	{
		get
		{
			return this.mClipSoft;
		}
		set
		{
			this.mClipSoft = value;
		}
	}

	private void CreateMaterial()
	{
		string text = (!(this.mShader != null)) ? ((!(this.mMaterial != null)) ? "Unlit/Transparent Colored" : this.mMaterial.shader.name) : this.mShader.name;
		text = text.Replace("GUI/Text Shader", "Unlit/Text");
		text = text.Replace(" (AlphaClip)", string.Empty);
		text = text.Replace(" (SoftClip)", string.Empty);
		Shader shader;
		if (this.mClipping == UIDrawCall.Clipping.SoftClip)
		{
			shader = Shader.Find(text + " (SoftClip)");
		}
		else if (this.mClipping == UIDrawCall.Clipping.AlphaClip)
		{
			shader = Shader.Find(text + " (AlphaClip)");
		}
		else
		{
			shader = Shader.Find(text);
		}
		if (this.mMaterial != null)
		{
			this.mDynamicMat = new Material(this.mMaterial);
			this.mDynamicMat.hideFlags = (HideFlags.DontSave | HideFlags.NotEditable);
			this.mDynamicMat.CopyPropertiesFromMaterial(this.mMaterial);
		}
		else
		{
			this.mDynamicMat = new Material(shader);
			this.mDynamicMat.hideFlags = (HideFlags.DontSave | HideFlags.NotEditable);
		}
		if (shader != null)
		{
			this.mDynamicMat.shader = shader;
		}
		else if (this.mClipping != UIDrawCall.Clipping.None)
		{
			LogSystem.LogWarning(new object[]
			{
				text,
				" doesn't have a clipped shader version for ",
				this.mClipping
			});
			this.mClipping = UIDrawCall.Clipping.None;
		}
	}

	private Material RebuildMaterial()
	{
		NGUITools.DestroyImmediate(this.mDynamicMat);
		this.CreateMaterial();
		this.mDynamicMat.renderQueue = this.mRenderQueue;
		this.mLastClip = this.mClipping;
		if (this.mTexture != null)
		{
			this.mDynamicMat.mainTexture = this.mTexture;
		}
		if (this.mRenderer != null)
		{
			this.mRenderer.sharedMaterials = new Material[]
			{
				this.mDynamicMat
			};
		}
		return this.mDynamicMat;
	}

	private void UpdateMaterials()
	{
		if (this.mRebuildMat || this.mDynamicMat == null || this.mClipping != this.mLastClip)
		{
			this.RebuildMaterial();
			this.mRebuildMat = false;
		}
		else if (this.mRenderer.sharedMaterial != this.mDynamicMat)
		{
			this.mRenderer.sharedMaterials = new Material[]
			{
				this.mDynamicMat
			};
		}
	}

	public void UpdateGeometry()
	{
		int size = this.verts.size;
		if (size > 0 && size == this.uvs.size && size == this.cols.size && size % 4 == 0)
		{
			if (this.mFilter == null)
			{
				this.mFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			if (this.mFilter == null)
			{
				this.mFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (this.verts.size < 65000)
			{
				int num = (size >> 1) * 3;
				bool flag = this.mIndices == null || this.mIndices.Length != num;
				if (this.mMesh == null)
				{
					this.mMesh = new Mesh();
					this.mMesh.hideFlags = HideFlags.DontSave;
					this.mMesh.name = ((!(this.mMaterial != null)) ? "Mesh" : this.mMaterial.name);
					this.mMesh.MarkDynamic();
					flag = true;
				}
				bool flag2 = this.uvs.buffer.Length != this.verts.buffer.Length || this.cols.buffer.Length != this.verts.buffer.Length || (this.norms.buffer != null && this.norms.buffer.Length != this.verts.buffer.Length) || (this.tans.buffer != null && this.tans.buffer.Length != this.verts.buffer.Length);
				if (!flag2 && this.panel.renderQueue != UIPanel.RenderQueue.Automatic)
				{
					flag2 = (this.mMesh == null || this.mMesh.vertexCount != this.verts.buffer.Length);
				}
				if (!flag2 && this.verts.size << 1 < this.verts.buffer.Length)
				{
					flag2 = true;
				}
				this.mTriangles = this.verts.size >> 1;
				if (flag2 || this.verts.buffer.Length > 65000)
				{
					if (flag2 || this.mMesh.vertexCount != this.verts.size)
					{
						this.mMesh.Clear();
						flag = true;
					}
					this.mMesh.vertices = this.verts.ToArray();
					this.mMesh.uv = this.uvs.ToArray();
					this.mMesh.colors32 = this.cols.ToArray();
					if (this.norms != null)
					{
						this.mMesh.normals = this.norms.ToArray();
					}
					if (this.tans != null)
					{
						this.mMesh.tangents = this.tans.ToArray();
					}
				}
				else
				{
					if (this.mMesh.vertexCount != this.verts.buffer.Length)
					{
						this.mMesh.Clear();
						flag = true;
					}
					this.mMesh.vertices = this.verts.buffer;
					this.mMesh.uv = this.uvs.buffer;
					this.mMesh.colors32 = this.cols.buffer;
					if (this.norms != null)
					{
						this.mMesh.normals = this.norms.buffer;
					}
					if (this.tans != null)
					{
						this.mMesh.tangents = this.tans.buffer;
					}
				}
				if (flag)
				{
					this.mIndices = this.GenerateCachedIndexBuffer(size, num);
					this.mMesh.triangles = this.mIndices;
				}
				if (flag2 || !this.alwaysOnScreen)
				{
					this.mMesh.RecalculateBounds();
				}
				this.mFilter.mesh = this.mMesh;
			}
			else
			{
				this.mTriangles = 0;
				if (this.mFilter.mesh != null)
				{
					this.mFilter.mesh.Clear();
				}
				LogSystem.LogWarning(new object[]
				{
					"Too many vertices on one panel: ",
					this.verts.size
				});
			}
			if (this.mRenderer == null)
			{
				this.mRenderer = base.gameObject.GetComponent<MeshRenderer>();
			}
			if (this.mRenderer == null)
			{
				this.mRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
			this.UpdateMaterials();
		}
		else
		{
			if (this.mFilter.mesh != null)
			{
				this.mFilter.mesh.Clear();
			}
			LogSystem.LogWarning(new object[]
			{
				"UIWidgets must fill the buffer with 4 vertices per quad. Found ",
				size
			});
		}
		this.verts.Clear();
		this.uvs.Clear();
		this.cols.Clear();
		this.norms.Clear();
		this.tans.Clear();
	}

	private int[] GenerateCachedIndexBuffer(int vertexCount, int indexCount)
	{
		int i = 0;
		int count = UIDrawCall.mCache.Count;
		while (i < count)
		{
			int[] array = UIDrawCall.mCache[i];
			if (array != null && array.Length == indexCount)
			{
				return array;
			}
			i++;
		}
		int[] array2 = new int[indexCount];
		int num = 0;
		for (int j = 0; j < vertexCount; j += 4)
		{
			array2[num++] = j;
			array2[num++] = j + 1;
			array2[num++] = j + 2;
			array2[num++] = j + 2;
			array2[num++] = j + 3;
			array2[num++] = j;
		}
		UIDrawCall.mCache.Add(array2);
		if (UIDrawCall.mCache.Count > 20)
		{
			UIDrawCall.mCache.RemoveAt(0);
		}
		return array2;
	}

	private void OnWillRenderObject()
	{
		if (this.mReset)
		{
			this.mReset = false;
			this.UpdateMaterials();
		}
		if (this.mDynamicMat != null && this.isClipped && this.mClipping != UIDrawCall.Clipping.ConstrainButDontClip)
		{
			Vector2 zero = Vector2.zero;
			zero.x = -this.mClipRange.x / this.mClipRange.z;
			zero.y = -this.mClipRange.y / this.mClipRange.w;
			this.mDynamicMat.mainTextureOffset = zero;
			zero.x = 1f / this.mClipRange.z;
			zero.y = 1f / this.mClipRange.w;
			this.mDynamicMat.mainTextureScale = zero;
			Vector2 zero2 = Vector2.zero;
			zero2.x = 1000f;
			zero2.y = 1000f;
			if (this.mClipSoft.x > 0f)
			{
				zero2.x = this.mClipRange.z / this.mClipSoft.x;
			}
			if (this.mClipSoft.y > 0f)
			{
				zero2.y = this.mClipRange.w / this.mClipSoft.y;
			}
			this.mDynamicMat.SetVector("_ClipSharpness", zero2);
		}
	}

	private void OnEnable()
	{
		this.mRebuildMat = true;
	}

	private void OnDisable()
	{
		this.depthStart = 2147483647;
		this.depthEnd = -2147483648;
		this.panel = null;
		this.manager = null;
		this.mMaterial = null;
		this.mTexture = null;
		NGUITools.DestroyImmediate(this.mDynamicMat);
		this.mDynamicMat = null;
	}

	private void OnDestroy()
	{
		NGUITools.DestroyImmediate(this.mMesh);
		this.mTrans = null;
		this.mMesh = null;
		this.mFilter = null;
		this.mRenderer = null;
		this.mDynamicMat = null;
	}

	public static UIDrawCall Create(UIPanel panel, Material mat, Texture tex, Shader shader)
	{
		return UIDrawCall.Create(null, panel, mat, tex, shader);
	}

	private static UIDrawCall Create(string name, UIPanel pan, Material mat, Texture tex, Shader shader)
	{
		UIDrawCall uIDrawCall = UIDrawCall.Create(name);
		uIDrawCall.gameObject.layer = pan.cachedGameObject.layer;
		uIDrawCall.clipping = pan.clipping;
		uIDrawCall.baseMaterial = mat;
		uIDrawCall.mainTexture = tex;
		uIDrawCall.shader = shader;
		uIDrawCall.renderQueue = pan.startingRenderQueue;
		uIDrawCall.sortingOrder = pan.sortingOrder;
		uIDrawCall.manager = pan;
		return uIDrawCall;
	}

	private static UIDrawCall Create(string name)
	{
		if (UIDrawCall.mInactiveList.size > 0)
		{
			UIDrawCall uIDrawCall = UIDrawCall.mInactiveList.Pop();
			UIDrawCall.mActiveList.Add(uIDrawCall);
			if (name != null)
			{
				uIDrawCall.name = name;
			}
			NGUITools.SetActive(uIDrawCall.gameObject, true);
			return uIDrawCall;
		}
		GameObject gameObject = new GameObject(name);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		UIDrawCall uIDrawCall2 = gameObject.AddComponent<UIDrawCall>();
		UIDrawCall.mActiveList.Add(uIDrawCall2);
		return uIDrawCall2;
	}

	public static void ClearAll()
	{
		bool isPlaying = Application.isPlaying;
		int i = UIDrawCall.mActiveList.size;
		while (i > 0)
		{
			UIDrawCall uIDrawCall = UIDrawCall.mActiveList[--i];
			if (uIDrawCall)
			{
				if (isPlaying)
				{
					NGUITools.SetActive(uIDrawCall.gameObject, false);
				}
				else
				{
					NGUITools.DestroyImmediate(uIDrawCall.gameObject);
				}
			}
		}
		UIDrawCall.mActiveList.Clear();
	}

	public static void ReleaseAll()
	{
		UIDrawCall.ClearAll();
		UIDrawCall.ReleaseInactive();
	}

	public static void ReleaseInactive()
	{
		int i = UIDrawCall.mInactiveList.size;
		while (i > 0)
		{
			UIDrawCall uIDrawCall = UIDrawCall.mInactiveList[--i];
			if (uIDrawCall)
			{
				NGUITools.DestroyImmediate(uIDrawCall.gameObject);
			}
		}
		UIDrawCall.mInactiveList.Clear();
	}

	public static int Count(UIPanel panel)
	{
		int num = 0;
		for (int i = 0; i < UIDrawCall.mActiveList.size; i++)
		{
			if (UIDrawCall.mActiveList[i].manager == panel)
			{
				num++;
			}
		}
		return num;
	}

	public static void Destroy(UIDrawCall dc)
	{
		if (dc)
		{
			if (Application.isPlaying)
			{
				if (UIDrawCall.mActiveList.Remove(dc))
				{
					NGUITools.SetActive(dc.gameObject, false);
					UIDrawCall.mInactiveList.Add(dc);
				}
			}
			else
			{
				UIDrawCall.mActiveList.Remove(dc);
				NGUITools.DestroyImmediate(dc.gameObject);
			}
		}
	}
}
