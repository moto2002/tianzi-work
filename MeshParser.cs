using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MeshParser : ParserBase
{
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

	private Material mat;

	private bool endStream;

	private GameObject curGo;

	private MeshFilter meshFilter;

	private Mesh mesh;

	private MeshRenderer mr;

	public override bool ProceedParsing()
	{
		if (this._startedParsing)
		{
			this.br = new BinaryReader(new MemoryStream(this._data));
			this._startedParsing = false;
			if (this.br.ReadString() != "model")
			{
				this.go = new GameObject("EmptyModel");
				this.endStream = true;
				return true;
			}
		}
		if (this.endStream)
		{
			return true;
		}
		string text = this.br.ReadString();
		string text2 = text;
		switch (text2)
		{
		case "gameobject":
			this.CreateGameObjects();
			break;
		case "mesh":
			this.ParseMesh();
			break;
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
		case "material":
			this.ParseMainMaterial();
			break;
		case "end":
			this.endStream = true;
			break;
		}
		return false;
	}

	private GameObject CreateGameObjects()
	{
		string name = this.br.ReadString();
		string key = this.br.ReadString();
		GameObject gameObject = new GameObject();
		this.curGo = gameObject;
		if (this.go == null)
		{
			this.go = gameObject;
		}
		gameObject.name = name;
		bool flag = this.br.ReadBoolean();
		if (flag)
		{
			GameObject gameObject2 = this.gos[this.br.ReadString()];
			gameObject.transform.parent = gameObject2.transform;
		}
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
		this.gos.Add(key, gameObject);
		return gameObject;
	}

	public void ParseMesh()
	{
		this.meshFilter = this.curGo.AddComponent<MeshFilter>();
		this.mesh = new Mesh();
		this.meshFilter.sharedMesh = this.mesh;
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
		this.mesh.vertices = this._vertices;
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
		this.mesh.uv = this._uvs;
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
		this.mesh.triangles = this._triangles;
	}

	public void ParseMainMaterial()
	{
		this.mr = this.curGo.AddComponent<MeshRenderer>();
		string name = this.br.ReadString();
		string name2 = this.br.ReadString();
		Material material = new Material(Shader.Find(name2));
		material.name = name;
		string text = this.br.ReadString();
		if (text == "_TintColor")
		{
			material.SetColor(text, new Color(this.br.ReadSingle(), this.br.ReadSingle(), this.br.ReadSingle(), this.br.ReadSingle()));
		}
		else if (text == "_Color")
		{
			material.SetColor(text, new Color(this.br.ReadSingle(), this.br.ReadSingle(), this.br.ReadSingle(), this.br.ReadSingle()));
		}
		string path = this.br.ReadString();
		Texture2D texture2D = AssetLibrary.Load(path, AssetType.Texture2D, LoadType.Type_Resources).texture2D;
		material.mainTexture = texture2D;
		this.mr.sharedMaterial = material;
	}
}
