using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectFrame : MonoBehaviour
{
	private List<Transform> frameList;

	private Transform mTrans;

	private void Awake()
	{
		this.mTrans = base.transform;
		this.frameList = new List<Transform>();
		Transform item = this.mTrans.Find("TopLeft");
		this.frameList.Add(item);
		item = this.mTrans.Find("BottomLeft");
		this.frameList.Add(item);
		item = this.mTrans.Find("TopRight");
		this.frameList.Add(item);
		item = this.mTrans.Find("BottomRight");
		this.frameList.Add(item);
	}

	public void ShowSkillSelectFrame(Transform parent, int spriteDepth, float scale, float positionInterval = 6f, float duation = 0.5f)
	{
		this.mTrans.gameObject.SetActive(true);
		this.mTrans.parent = parent;
		this.mTrans.localScale = Vector3.one;
		this.mTrans.localPosition = Vector3.zero;
		int i = 0;
		int count = this.frameList.Count;
		while (i < count)
		{
			UISprite component = this.frameList[i].GetComponent<UISprite>();
			if (component)
			{
				component.depth = spriteDepth;
			}
			TweenPosition component2 = this.frameList[i].GetComponent<TweenPosition>();
			if (component2)
			{
				Vector3 from = component2.from;
				Vector3 to = component2.to;
				from.x = Mathf.Sign(from.x) * (scale / 2f + positionInterval);
				from.y = Mathf.Sign(from.y) * (scale / 2f + positionInterval);
				component2.from = from;
				to.x = Mathf.Sign(to.x) * scale / 2f;
				to.y = Mathf.Sign(to.y) * scale / 2f;
				component2.to = to;
				component2.duration = duation;
			}
			i++;
		}
	}
}
