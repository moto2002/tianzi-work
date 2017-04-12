using System;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveRenderQueue : MonoBehaviour
{
	private GameObject mGameObject;

	private UIPanel mPanel;

	public int mEffectDepth;

	private int mDrawCalls;

	private int mRenderQueue;

	private List<Renderer> rendererList = new List<Renderer>();

	private ModelLoadEffect[] effects;

	public int GetDrawCalls
	{
		get
		{
			return this.rendererList.Count;
		}
	}

	private void Awake()
	{
		this.mGameObject = base.gameObject;
	}

	private void OnEnable()
	{
		this.Start();
	}

	private void Start()
	{
		this.effects = this.mGameObject.GetComponentsInChildren<ModelLoadEffect>(true);
		this.Init();
	}

	private void OnDisable()
	{
		if (this.mPanel != null)
		{
			this.mPanel.RemoveRenderQuene(this);
			this.rendererList.Clear();
			this.mPanel = null;
		}
		if (this.effects != null)
		{
			int num = this.effects.Length;
			for (int i = 0; i < num; i++)
			{
				if (this.effects[i] != null)
				{
					this.effects[i].gameObjectActiveCallBack = null;
				}
			}
		}
		this.rendererList.Clear();
	}

	private void Init()
	{
		if (this.mPanel != null)
		{
			this.mPanel.RemoveRenderQuene(this);
		}
		this.mPanel = this.GetRootPanel(this.mGameObject);
		if (this.mPanel != null)
		{
			this.mPanel.AddRenderQuene(this);
			this.RefreshRenderer();
		}
		if (this.effects != null)
		{
			int num = this.effects.Length;
			for (int i = 0; i < num; i++)
			{
				if (this.effects[i] != null)
				{
					this.effects[i].gameObjectActiveCallBack = new Action(this.RefreshRenderer);
				}
			}
		}
	}

	public void RefreshRenderer()
	{
		if (this.mPanel != null)
		{
			this.rendererList.Clear();
			Renderer[] componentsInChildren = this.mGameObject.GetComponentsInChildren<Renderer>(true);
			int num = componentsInChildren.Length;
			for (int i = 0; i < num; i++)
			{
				if (componentsInChildren[i] != null && componentsInChildren[i].material != null && !this.rendererList.Contains(componentsInChildren[i]))
				{
					this.rendererList.Add(componentsInChildren[i]);
				}
			}
			ParticleSystem[] componentsInChildren2 = this.mGameObject.GetComponentsInChildren<ParticleSystem>(true);
			num = componentsInChildren2.Length;
			for (int j = 0; j < num; j++)
			{
				if (componentsInChildren2[j].GetComponent<Renderer>() != null && componentsInChildren2[j].GetComponent<Renderer>().material != null && !this.rendererList.Contains(componentsInChildren2[j].GetComponent<Renderer>()))
				{
					this.rendererList.Add(componentsInChildren2[j].GetComponent<Renderer>());
				}
			}
			this.rendererList.Sort(new Comparison<Renderer>(this.RendererCompareFunc));
			this.OnUpdate(this.mPanel.startingRenderQueue, this.mPanel.drawCalls.size, true);
		}
	}

	public void OnUpdate(int renderQuene, int drawCalls, bool bnow = false)
	{
		if (bnow)
		{
			this.mRenderQueue = renderQuene;
			this.mDrawCalls = drawCalls;
			this.SetRenderQuene();
		}
		else if (renderQuene != this.mRenderQueue || drawCalls != this.mDrawCalls)
		{
			this.mRenderQueue = renderQuene;
			this.mDrawCalls = drawCalls;
			this.SetRenderQuene();
		}
	}

	private void OnDestroy()
	{
		if (this.mPanel != null)
		{
			this.mPanel.RemoveRenderQuene(this);
			this.rendererList.Clear();
		}
		if (this.effects != null)
		{
			int num = this.effects.Length;
			if (num <= 0)
			{
				return;
			}
			for (int i = num - 1; i >= 0; i--)
			{
				if (this.effects[i] != null)
				{
					this.effects[i].gameObjectActiveCallBack = null;
				}
			}
		}
	}

	private UIPanel GetRootPanel(GameObject root)
	{
		if (root == null)
		{
			return null;
		}
		UIPanel component = root.GetComponent<UIPanel>();
		if (!(component == null))
		{
			return component;
		}
		Transform parent = root.transform.parent;
		if (parent == null)
		{
			return null;
		}
		if (parent.tag == "UIEvent")
		{
			return parent.GetComponent<UIPanel>();
		}
		return this.GetRootPanel(parent.gameObject);
	}

	private void SetRenderQuene()
	{
		if (this.rendererList != null && this.rendererList.Count > 0)
		{
			int num = 3000;
			int num2 = 0;
			for (int i = 0; i < this.rendererList.Count; i++)
			{
				if (this.rendererList[i].material.renderQueue >= 3000)
				{
					if (this.rendererList[i].material.renderQueue > num)
					{
						num = this.rendererList[i].material.renderQueue;
						num2++;
					}
					this.rendererList[i].material.renderQueue = this.mRenderQueue + this.mDrawCalls + this.mEffectDepth + num2 + 1;
				}
			}
		}
	}

	public static int RenderQueueCompareFunc(AdaptiveRenderQueue left, AdaptiveRenderQueue right)
	{
		if (left.mEffectDepth < right.mEffectDepth)
		{
			return -1;
		}
		if (left.mEffectDepth > right.mEffectDepth)
		{
			return 1;
		}
		return 0;
	}

	private int RendererCompareFunc(Renderer renA, Renderer renB)
	{
		return renA.material.shader.renderQueue.CompareTo(renB.material.shader.renderQueue);
	}
}
