using System;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class SkinMeshLoader : MonoBehaviour
{
	public delegate void CallLoadAsset(string strFileName, AssetCallBack callback);

	[SerializeField]
	public string strAssetName = string.Empty;

	public MaterialLoader matchMono;

	private MeshAsset oSelfAsset;

	public static SkinMeshLoader.CallLoadAsset monLoadAsset;

	private void Start()
	{
		if (!string.IsNullOrEmpty(this.strAssetName))
		{
			this.LoadAsset(this.strAssetName);
		}
	}

	private void LoadAsset(string strMeshName)
	{
		if (SkinMeshLoader.monLoadAsset != null)
		{
			SkinMeshLoader.monLoadAsset(strMeshName, new AssetCallBack(this.OnFileLoaded));
		}
		else
		{
			UnityEngine.Object @object = Resources.Load(this.strAssetName);
			this.OnFileLoaded(new object[]
			{
				@object
			});
		}
	}

	public void CombineRender(MaterialAsset oOtherAsset)
	{
		if (this.oSelfAsset == null)
		{
			return;
		}
		SkinnedMeshRenderer component = base.GetComponent<SkinnedMeshRenderer>();
		if (component != null)
		{
			component.material = oOtherAsset.material;
			component.sharedMesh = this.oSelfAsset.mesh;
			GameObjectUnit.ChangeShader(component);
		}
		if (this.matchMono != null)
		{
			UnityEngine.Object.Destroy(this.matchMono);
		}
		UnityEngine.Object.Destroy(this);
	}

	public static void SetLoadAssetCall(SkinMeshLoader.CallLoadAsset call)
	{
		SkinMeshLoader.monLoadAsset = call;
	}

	private void OnFileLoaded(params object[] args)
	{
		MeshAsset x = args[0] as MeshAsset;
		if (x == null)
		{
			return;
		}
		this.oSelfAsset = x;
		if (this.matchMono != null)
		{
			this.matchMono.CombineRender(this.oSelfAsset);
		}
		else
		{
			SkinnedMeshRenderer component = base.GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				component.sharedMesh = this.oSelfAsset.mesh;
			}
		}
	}

	private void OnDestroy()
	{
		this.oSelfAsset = null;
	}
}
