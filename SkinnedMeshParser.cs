using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkinnedMeshParser : ParserBase
{
	public Mesh mesh;

	public GameObject go;

	private Dictionary<string, GameObject> gos = new Dictionary<string, GameObject>();

	private bool _startedParsing = true;

	private Vector3[] _vertices;

	private Vector2[] _uvs;

	private Vector3[] _normals;

	private int[] _triangles;

	private BoneWeight[] _weights;

	private Matrix4x4[] _bindPoses;

	private Transform[] _bones;

	private BinaryReader br;

	private string rootName = string.Empty;

	private MeshRenderer skinnedMeshRenderer;

	private Material mat;

	private bool endStream;

	private int buffIndex;

	private static Shader skmShader = Shader.Find("Snail/Bumped Specular Point Light");

	public override bool ProceedParsing()
	{
		if (this._startedParsing)
		{
			this.br = new BinaryReader(new MemoryStream(this._data));
			this._startedParsing = false;
			if (this.br.ReadString() != "skeletonModel")
			{
				this.go = new GameObject("EmptyModel");
				this.endStream = true;
				return true;
			}
		}
		if (!this.endStream)
		{
			string text = this.br.ReadString();
			string text2 = text;
			switch (text2)
			{
			case "vertexs":
				this.ParseVertex();
				break;
			case "uvs":
				this.ParseUV();
				break;
			case "nomals":
				this.ParseVertexNormal();
				break;
			case "triangles":
				this.ParseFace();
				break;
			case "weights":
				this.ParseWeights();
				break;
			case "bindPoses":
				this.ParseBindPoses();
				break;
			case "bones":
				this.ParseBones();
				break;
			case "gos":
				this.rootName = this.br.ReadString();
				this.CreateGameObjects();
				break;
			case "material":
				this.ParseMainMaterial();
				break;
			case "end":
				this.endStream = true;
				break;
			}
			return false;
		}
		this.buffIndex++;
		if (this.buffIndex == 1)
		{
			this.CreateVertexBuffer();
			return false;
		}
		if (this.buffIndex == 2)
		{
			this.CreateUVBuffer();
			return false;
		}
		if (this.buffIndex == 3)
		{
			this.CreateTriBuffer();
			return false;
		}
		this.mesh.RecalculateNormals();
		this.skinnedMeshRenderer.material = this.mat;
		MeshFilter meshFilter = this.gos[this.rootName].AddComponent<MeshFilter>();
		meshFilter.sharedMesh = this.mesh;
		return true;
	}

	public void ParseVertex()
	{
		int num = this.br.ReadInt32();
		this._vertices = new Vector3[num];
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = default(Vector3);
			vector.x = this.br.ReadSingle();
			vector.y = this.br.ReadSingle();
			vector.z = this.br.ReadSingle();
			this._vertices[i] = vector;
		}
	}

	public void ParseUV()
	{
		int num = this.br.ReadInt32();
		this._uvs = new Vector2[num];
		for (int i = 0; i < num; i++)
		{
			Vector2 vector = default(Vector2);
			vector.x = this.br.ReadSingle();
			vector.y = this.br.ReadSingle();
			this._uvs[i] = vector;
		}
	}

	public void ParseVertexNormal()
	{
		this.br.ReadInt32();
	}

	public void ParseFace()
	{
		int num = this.br.ReadInt32();
		this._triangles = new int[num];
		for (int i = 0; i < num; i++)
		{
			this._triangles[i] = this.br.ReadInt32();
		}
	}

	public void ParseWeights()
	{
		int num = this.br.ReadInt32();
		this._weights = new BoneWeight[num];
		for (int i = 0; i < num; i++)
		{
			BoneWeight boneWeight = default(BoneWeight);
			boneWeight.weight0 = this.br.ReadSingle();
			boneWeight.weight1 = this.br.ReadSingle();
			boneWeight.weight2 = this.br.ReadSingle();
			boneWeight.weight3 = this.br.ReadSingle();
			boneWeight.boneIndex0 = this.br.ReadInt32();
			boneWeight.boneIndex1 = this.br.ReadInt32();
			boneWeight.boneIndex2 = this.br.ReadInt32();
			boneWeight.boneIndex3 = this.br.ReadInt32();
			this._weights[i] = boneWeight;
		}
	}

	public void ParseBindPoses()
	{
		int num = this.br.ReadInt32();
		this._bindPoses = new Matrix4x4[num];
		for (int i = 0; i < num; i++)
		{
			Matrix4x4 matrix4x = default(Matrix4x4);
			matrix4x[0] = this.br.ReadSingle();
			matrix4x[1] = this.br.ReadSingle();
			matrix4x[2] = this.br.ReadSingle();
			matrix4x[3] = this.br.ReadSingle();
			matrix4x[4] = this.br.ReadSingle();
			matrix4x[5] = this.br.ReadSingle();
			matrix4x[6] = this.br.ReadSingle();
			matrix4x[7] = this.br.ReadSingle();
			matrix4x[8] = this.br.ReadSingle();
			matrix4x[9] = this.br.ReadSingle();
			matrix4x[10] = this.br.ReadSingle();
			matrix4x[11] = this.br.ReadSingle();
			matrix4x[12] = this.br.ReadSingle();
			matrix4x[13] = this.br.ReadSingle();
			matrix4x[14] = this.br.ReadSingle();
			matrix4x[15] = this.br.ReadSingle();
			this._bindPoses[i] = matrix4x;
		}
	}

	private GameObject CreateGameObjects()
	{
		string text = this.br.ReadString();
		GameObject gameObject = new GameObject();
		if (this.go == null)
		{
			this.go = gameObject;
		}
		gameObject.name = text;
		Vector3 localPosition = default(Vector3);
		localPosition.x = this.br.ReadSingle();
		localPosition.y = this.br.ReadSingle();
		localPosition.z = this.br.ReadSingle();
		Quaternion localRotation = default(Quaternion);
		localRotation.x = this.br.ReadSingle();
		localRotation.y = this.br.ReadSingle();
		localRotation.z = this.br.ReadSingle();
		localRotation.w = this.br.ReadSingle();
		Vector3 localScale = default(Vector3);
		localScale.x = this.br.ReadSingle();
		localScale.y = this.br.ReadSingle();
		localScale.z = this.br.ReadSingle();
		gameObject.transform.localPosition = localPosition;
		gameObject.transform.localRotation = localRotation;
		gameObject.transform.localScale = localScale;
		bool flag = this.br.ReadBoolean();
		if (flag)
		{
		}
		this.gos.Add(text, gameObject);
		int num = this.br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject2 = this.CreateGameObjects();
			gameObject2.transform.parent = gameObject.transform;
		}
		return gameObject;
	}

	public void ParseBones()
	{
		int num = this.br.ReadInt32();
		this._bones = new Transform[num];
		for (int i = 0; i < num; i++)
		{
			string text = this.br.ReadString();
			if (!this.gos.ContainsKey(text))
			{
				Debug.Log("miss " + text);
			}
			this._bones[i] = this.gos[text].transform;
			Transform transform = this._bones[i];
			Vector3 localPosition = transform.localPosition;
			Quaternion localRotation = transform.localRotation;
			localPosition.x = this.br.ReadSingle();
			localPosition.y = this.br.ReadSingle();
			localPosition.z = this.br.ReadSingle();
			localRotation.x = this.br.ReadSingle();
			localRotation.y = this.br.ReadSingle();
			localRotation.z = this.br.ReadSingle();
			localRotation.w = this.br.ReadSingle();
			this.gos[text].transform.localPosition = localPosition;
			this.gos[text].transform.localRotation = localRotation;
		}
	}

	public void ParseMainMaterial()
	{
		string path = this.br.ReadString();
		Texture2D texture2D = AssetLibrary.Load(path, AssetType.Texture2D, LoadType.Type_Resources).texture2D;
		this.mat = new Material(SkinnedMeshParser.skmShader);
		this.mat.mainTexture = texture2D;
	}

	public void CreateVertexBuffer()
	{
		GameObject gameObject = this.gos[this.rootName];
		if (gameObject != null)
		{
			this.skinnedMeshRenderer = gameObject.AddComponent<MeshRenderer>();
		}
		this.mesh = new Mesh();
		this.mesh.vertices = this._vertices;
	}

	public void CreateUVBuffer()
	{
		this.mesh.uv = this._uvs;
	}

	public void CreateTriBuffer()
	{
		this.mesh.triangles = this._triangles;
	}

	public void CreateWeightsBuffer()
	{
		this.mesh.boneWeights = this._weights;
	}

	public void CreateBindposes()
	{
		this.mesh.bindposes = this._bindPoses;
	}

	public void CreateBones()
	{
	}
}
