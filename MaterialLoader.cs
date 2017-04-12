using System;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class MaterialLoader : MonoBehaviour
{
	public delegate void CallLoadAsset(string strFileName, AssetCallBack callback);

	[SerializeField]
	public string strAssetName = string.Empty;

	public MonoBehaviour matchMono;

	private MaterialAsset oSelfAsset;

	public static MaterialLoader.CallLoadAsset monLoadAsset;

	private void Start()
	{
		if (!string.IsNullOrEmpty(this.strAssetName))
		{
			this.LoadAsset(this.strAssetName);
		}
	}

	private void LoadAsset(string strMeshName)
	{
		if (MaterialLoader.monLoadAsset != null)
		{
			MaterialLoader.monLoadAsset(strMeshName, new AssetCallBack(this.OnFileLoaded));
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

	public void CombineRender(MeshAsset oOtherAsset)
	{
		if (this.oSelfAsset == null)
		{
			return;
		}
		if (this.matchMono is SkinMeshLoader)
		{
			SkinnedMeshRenderer component = base.GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				component.material = this.oSelfAsset.material;
				component.sharedMesh = oOtherAsset.mesh;
				GameObjectUnit.ChangeShader(component);
			}
		}
		else if (this.matchMono is MeshLoader)
		{
			MeshRenderer component2 = base.GetComponent<MeshRenderer>();
			if (component2 != null)
			{
				component2.material = this.oSelfAsset.material;
				GameObjectUnit.ChangeShader(component2);
			}
			MeshFilter component3 = base.GetComponent<MeshFilter>();
			if (component3 != null)
			{
				component3.mesh = oOtherAsset.mesh;
			}
		}
		else if (this.matchMono == null)
		{
			MeshRenderer component4 = base.GetComponent<MeshRenderer>();
			if (component4 != null)
			{
				component4.material = this.oSelfAsset.material;
				GameObjectUnit.ChangeShader(component4);
			}
			else
			{
				SkinnedMeshRenderer component5 = base.GetComponent<SkinnedMeshRenderer>();
				if (component5 != null)
				{
					component5.material = this.oSelfAsset.material;
					GameObjectUnit.ChangeShader(component5);
				}
			}
		}
		if (this.matchMono != null)
		{
			UnityEngine.Object.Destroy(this.matchMono);
		}
		UnityEngine.Object.Destroy(this);
	}

	public static void SetLoadAssetCall(MaterialLoader.CallLoadAsset call)
	{
		MaterialLoader.monLoadAsset = call;
	}

	private void OnFileLoaded(params object[] args)
	{
		MaterialAsset x = args[0] as MaterialAsset;
		if (x == null)
		{
			return;
		}
		this.oSelfAsset = x;
		if (this.matchMono is SkinMeshLoader)
		{
			SkinMeshLoader skinMeshLoader = this.matchMono as SkinMeshLoader;
			if (skinMeshLoader != null)
			{
				skinMeshLoader.CombineRender(this.oSelfAsset);
			}
		}
		else if (this.matchMono is MeshLoader)
		{
			MeshLoader meshLoader = this.matchMono as MeshLoader;
			if (meshLoader != null)
			{
				meshLoader.CombineRender(this.oSelfAsset);
			}
		}
	}

	private void OnDestroy()
	{
		this.oSelfAsset = null;
	}
}
