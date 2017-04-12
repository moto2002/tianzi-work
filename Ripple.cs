using System;
using System.Collections.Generic;
using UnityEngine;

public class Ripple : MonoBehaviour
{
	public static Mesh mesh;

	public Vector3[] vertices;

	public Vector3[] normals;

	public Vector2[] uvs;

	public int[] triangles;

	public int segmentsW = 1;

	public int segmentsH = 1;

	private Material matrial;

	public MeshRenderer rippleRenderer;

	private float tick;

	public float life = 70f;

	public static List<Ripple> destroyRipples = new List<Ripple>();

	private Shader shader = Shader.Find("Snail/Ripple");

	private float scale = 1f;

	public static Ripple CreateRippleGameObject(Vector3 worldPostion)
	{
		Ripple ripple;
		GameObject gameObject;
		if (Ripple.destroyRipples.Count > 0)
		{
			ripple = Ripple.destroyRipples[0];
			gameObject = ripple.gameObject;
			ripple.gameObject.SetActive(true);
			Ripple.destroyRipples.RemoveAt(0);
		}
		else
		{
			gameObject = new GameObject();
			ripple = gameObject.AddComponent<Ripple>();
			gameObject.isStatic = false;
			gameObject.name = "Ripple";
			if (Ripple.mesh == null)
			{
				ripple.BuildShareWaterMesh();
			}
			gameObject.AddComponent<MeshFilter>().sharedMesh = Ripple.mesh;
			ripple.rippleRenderer = gameObject.AddComponent<MeshRenderer>();
			ripple.rippleRenderer.castShadows = false;
			ripple.rippleRenderer.receiveShadows = false;
			ripple.gameObject.layer = GameLayer.Layer_Water;
			ripple.BuildMaterial();
		}
		gameObject.transform.position = worldPostion;
		ripple.Reset();
		return ripple;
	}

	public void Reset()
	{
		this.scale = 1f + UnityEngine.Random.value * 0.2f;
		this.tick = 0f;
	}

	public void BuildShareWaterMesh()
	{
		this.BuildGeometry();
		this.BuildUVs();
	}

	public void BuildGeometry()
	{
		if (Ripple.mesh == null)
		{
			Ripple.mesh = new Mesh();
		}
		int num = this.segmentsW;
		int num2 = this.segmentsH;
		int num3 = 0;
		int num4 = num + 1;
		int num5 = (num2 + 1) * num4;
		this.vertices = new Vector3[num5];
		this.triangles = new int[num2 * num * 6];
		this.normals = new Vector3[num5];
		num5 = 0;
		for (int i = 0; i <= num2; i++)
		{
			for (int j = 0; j <= num; j++)
			{
				float x = ((float)j / (float)num - 0.5f) * 1.5f;
				float z = ((float)i / (float)num2 - 0.5f) * 1.5f;
				float y = 0f;
				this.vertices[num5] = new Vector3(x, y, z);
				num5++;
				if (j != num && i != num2)
				{
					int num6 = j + i * num4;
					this.triangles[num3++] = num6;
					this.triangles[num3++] = num6 + num4;
					this.triangles[num3++] = num6 + num4 + 1;
					this.triangles[num3++] = num6;
					this.triangles[num3++] = num6 + num4 + 1;
					this.triangles[num3++] = num6 + 1;
				}
			}
		}
		Ripple.mesh.vertices = this.vertices;
		Ripple.mesh.triangles = this.triangles;
	}

	public void BuildUVs()
	{
		int num = (this.segmentsH + 1) * (this.segmentsW + 1);
		this.uvs = new Vector2[num];
		num = 0;
		for (int i = 0; i <= this.segmentsH; i++)
		{
			for (int j = 0; j <= this.segmentsW; j++)
			{
				this.uvs[num++] = new Vector2((float)j / (float)this.segmentsW, 1f - (float)i / (float)this.segmentsH);
			}
		}
		Ripple.mesh.uv = this.uvs;
	}

	public void BuildMaterial()
	{
		this.matrial = new Material(this.shader);
		base.renderer.material = this.matrial;
		Texture2D texture = Resources.Load("Textures/Shader/ripple", typeof(Texture2D)) as Texture2D;
		this.matrial.SetTexture("_MainTex", texture);
		this.rippleRenderer.material = this.matrial;
	}

	private void Update()
	{
		if (!base.renderer)
		{
			return;
		}
		if (this.tick > this.life)
		{
			this.Reset();
			base.gameObject.SetActive(false);
			Ripple.destroyRipples.Add(this);
			return;
		}
		Material sharedMaterial = base.renderer.sharedMaterial;
		if (!sharedMaterial)
		{
			return;
		}
		this.scale += 0.08f;
		sharedMaterial.SetFloat("_Scale", this.scale);
		sharedMaterial.SetVector("_Color", new Color(0.5f, 0.5f, 0.5f, (this.life - this.tick) / this.life));
		this.tick += 2f;
	}
}
