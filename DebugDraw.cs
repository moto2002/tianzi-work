using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugDraw : MonoBehaviour
{
	private Material mat;

	private List<Rect> _rectList = new List<Rect>();

	private Color _col;

	private void Start()
	{
		if (this.mat == null)
		{
			string contents = "Shader \"Alpha Additive\" {SubShader {\tTags { \"Queue\" = \"Overlay\" }\tPass {       Blend SrcAlpha OneMinusSrcAlpha       ColorMaterial Emission        Lighting Off\t}\t}}";
			this.mat = new Material(contents);
		}
	}

	public void BeginDraw(Color col)
	{
		this._col = col;
		this._rectList.Clear();
	}

	public void DrawRect(Rect rect)
	{
		this._rectList.Add(rect);
	}

	public void EndDraw()
	{
	}

	private void OnPostRender()
	{
		if (!this.mat)
		{
			LogSystem.LogWarning(new object[]
			{
				"Please Assign a material on the inspector"
			});
			return;
		}
		GL.PushMatrix();
		this.mat.SetPass(0);
		GL.LoadOrtho();
		for (int i = 0; i < this._rectList.Count; i++)
		{
			Rect rect = this._rectList[i];
			GL.Begin(4);
			GL.Color(this._col);
			GL.Vertex(this.GetNormalizePos(new Vector3(rect.xMax, rect.yMin, 0f)));
			GL.Vertex(this.GetNormalizePos(new Vector3(rect.xMin, rect.yMin, 0f)));
			GL.Vertex(this.GetNormalizePos(new Vector3(rect.xMax, rect.yMax, 0f)));
			GL.Vertex(this.GetNormalizePos(new Vector3(rect.xMin, rect.yMax, 0f)));
			GL.Vertex(this.GetNormalizePos(new Vector3(rect.xMax, rect.yMax, 0f)));
			GL.Vertex(this.GetNormalizePos(new Vector3(rect.xMin, rect.yMin, 0f)));
			GL.End();
		}
		GL.PopMatrix();
	}

	private void Example()
	{
	}

	private Vector3 GetNormalizePos(Vector3 screenPos)
	{
		return new Vector3(screenPos.x / (float)Screen.width, screenPos.y / (float)Screen.height, 0f);
	}
}
