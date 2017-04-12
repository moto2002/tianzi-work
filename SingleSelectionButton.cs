using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Custom/SingleSelectionButton")]
public class SingleSelectionButton : MonoBehaviour
{
	private struct TAB
	{
		public UISprite back;

		public Vector3 perviousV3;

		public List<UISprite> auxSprite;

		public UILabel lbl;

		public UISprite tabLogo;

		public void Active(string spriteName, Color pressColor, Color lblClr, bool result)
		{
			this.back.spriteName = spriteName;
			if (this.tabLogo != null)
			{
				this.tabLogo.gameObject.SetActive(true);
			}
			if (result)
			{
				Vector3 zero = Vector3.zero;
				zero.x = 1f;
				zero.y = 1.01f;
				zero.z = 0f;
				this.back.transform.localScale = zero;
				float y = (float)this.back.height * 0.01f;
				zero.x = 0f;
				zero.y = y;
				zero.z = 0f;
				this.back.transform.localPosition = this.perviousV3 + zero;
			}
			if (this.auxSprite != null)
			{
				int count = this.auxSprite.Count;
				for (int i = 0; i < count; i++)
				{
					this.auxSprite[i].color = pressColor;
				}
			}
			if (this.lbl)
			{
				this.lbl.color = lblClr;
			}
		}

		public void Normal(string spriteName, Color lblClr, bool result)
		{
			this.back.spriteName = spriteName;
			if (this.tabLogo != null)
			{
				this.tabLogo.gameObject.SetActive(false);
			}
			if (result)
			{
				this.back.transform.localScale = Vector3.one;
				this.back.transform.localPosition = this.perviousV3;
			}
			if (this.auxSprite != null)
			{
				int count = this.auxSprite.Count;
				for (int i = 0; i < count; i++)
				{
					this.auxSprite[i].color = Color.white;
				}
			}
			if (this.lbl)
			{
				this.lbl.color = lblClr;
			}
		}
	}

	[Serializable]
	public class Sprite
	{
		public string normal;

		public string hover;
	}

	public TouchEvent touchEvent;

	public UIAtlas mAtlas;

	public Color pressColor = Color.white;

	public Color LabelNormalColor = Color.white;

	public Color LabelPressColor = Color.white;

	public SingleSelectionButton.Sprite[] spritesName;

	private bool scale;

	private int currentIndex;

	private int perviousIndex;

	private CustomList<GameObject> children;

	private SingleSelectionButton.TAB[] tabs;

	public void Init(bool result = false, string[] contents = null)
	{
		this.currentIndex = 0;
		this.perviousIndex = -1;
		this.scale = result;
		int childCount = base.transform.childCount;
		if (childCount <= 0)
		{
			throw new Exception("transform don't have child");
		}
		this.children = new CustomList<GameObject>();
		this.tabs = new SingleSelectionButton.TAB[childCount];
		for (int i = 0; i < childCount; i++)
		{
			this.children.Add(base.transform.GetChild(i).gameObject);
		}
		this.children.Sort();
		for (int j = 0; j < childCount; j++)
		{
			UISprite[] componentsInChildren = this.children[j].GetComponentsInChildren<UISprite>(true);
			UILabel[] componentsInChildren2 = this.children[j].GetComponentsInChildren<UILabel>(true);
			if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
			{
				this.tabs[j].lbl = componentsInChildren2[0];
				if (contents != null && !string.IsNullOrEmpty(contents[j]))
				{
					this.tabs[j].lbl.text = contents[j];
				}
			}
			int num = componentsInChildren.Length;
			this.tabs[j].auxSprite = new List<UISprite>();
			if (num == 1)
			{
				this.tabs[j].back = componentsInChildren[0];
				this.tabs[j].perviousV3 = componentsInChildren[0].transform.localPosition;
			}
			else
			{
				for (int k = 0; k < num; k++)
				{
					if (componentsInChildren[k].name.StartsWith("back"))
					{
						this.tabs[j].back = componentsInChildren[k];
						this.tabs[j].perviousV3 = componentsInChildren[k].transform.localPosition;
						componentsInChildren[k].atlas = this.mAtlas;
					}
					else if (componentsInChildren[k].name == "tabLogo")
					{
						this.tabs[j].tabLogo = componentsInChildren[k];
					}
					else
					{
						this.tabs[j].auxSprite.Add(componentsInChildren[k]);
					}
				}
			}
			UIEventListener.Get(this.children[j]).onClick = new UIEventListener.VoidDelegate(this.onTouch);
		}
		this.Normal();
		this.Active();
	}

	public void SelectBtn(GameObject go)
	{
		this.onTouch(go);
	}

	private void onTouch(GameObject go)
	{
		if (this.children == null || this.children.Length <= 1)
		{
			return;
		}
		int length = this.children.Length;
		int i = 0;
		while (i < length)
		{
			if (this.children[i].name == go.name)
			{
				if (i == this.currentIndex)
				{
					return;
				}
				this.Active(i);
				DelegateProxy.PlayUIAudio(15);
				if (this.touchEvent != null)
				{
					this.touchEvent(i, this.children[i]);
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	private void Normal()
	{
		int length = this.children.Length;
		for (int i = 0; i < length; i++)
		{
			this.tabs[i].Normal(this.spritesName[i].normal, this.LabelNormalColor, this.scale);
		}
	}

	private void Active()
	{
		this.tabs[0].Active(this.spritesName[0].hover, this.pressColor, this.LabelPressColor, this.scale);
	}

	public void Active(int index)
	{
		if (this.currentIndex == index)
		{
			return;
		}
		this.perviousIndex = this.currentIndex;
		this.currentIndex = index;
		this.tabs[this.currentIndex].Active(this.spritesName[this.currentIndex].hover, this.pressColor, this.LabelPressColor, this.scale);
		this.tabs[this.perviousIndex].Normal(this.spritesName[this.perviousIndex].normal, this.LabelNormalColor, this.scale);
	}

	private void OnDestroy()
	{
		if (this.touchEvent != null)
		{
			this.touchEvent = null;
		}
	}
}
