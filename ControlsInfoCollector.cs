using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public class ControlsInfoCollector : MonoBehaviour
{
	private enum CheckUpdateType
	{
		Click,
		UIEvent
	}

	public class ControlInfo
	{
		public int instanceId;

		public string name;

		public string text;

		public string showText;

		public int top;

		public int left;

		public int width;

		public int height;
	}

	public class ControlInfoList
	{
		public int screenWidth;

		public int screenHeight;

		public double fps;

		public double scaleRatio;

		public List<ControlsInfoCollector.ControlInfo> controls;
	}

	public UIRoot root;

	public DebugDraw draw;

	public float firstCheckInterval = 1f;

	public float clickUpdateDelay = 0.8f;

	public float eventUpdateDelay = 0.3f;

	private int needUpdate;

	private bool isProcessing;

	private Dictionary<int, GameObject> _lastgameObject = new Dictionary<int, GameObject>();

	private StringBuilder _sb = new StringBuilder();

	public void Start()
	{
	}

	private void OnClick()
	{
		this.needUpdate |= 1;
	}

	private void OnSelect()
	{
		this.needUpdate |= 1;
	}

	private void OnDragEnd()
	{
		this.needUpdate |= 1;
	}

	private void LateUpdate()
	{
		if (this.root != null && ((this.needUpdate & 1) != 0 || (this.needUpdate & 2) != 0) && !this.isProcessing)
		{
			this.isProcessing = true;
			base.StartCoroutine(this.UpdateCachedControls(this.needUpdate));
		}
	}

	[DebuggerHidden]
	private IEnumerator UpdateCachedControls(int updateType)
	{
		ControlsInfoCollector.<UpdateCachedControls>c__Iterator18 <UpdateCachedControls>c__Iterator = new ControlsInfoCollector.<UpdateCachedControls>c__Iterator18();
		<UpdateCachedControls>c__Iterator.<>f__this = this;
		return <UpdateCachedControls>c__Iterator;
	}

	private void OnOpenWindow(string id, Transform t)
	{
		this.needUpdate |= 2;
	}

	private void OnCloseWindow(string id)
	{
		this.needUpdate |= 2;
	}

	private void OnCloseAll(List<string> ids)
	{
	}

	private void CachePanel(List<Component> panels)
	{
		this._lastgameObject.Clear();
		int num = 0;
		while (panels != null && num < panels.Count)
		{
			this._lastgameObject[panels[num].GetInstanceID()] = panels[num].gameObject;
			num++;
		}
	}

	private void GetFirstPanelInEveryBranch(Component root, List<Component> panels)
	{
		if (root == null || !root.gameObject.activeInHierarchy)
		{
			return;
		}
		UIPanel component = root.GetComponent<UIPanel>();
		if (component)
		{
			panels.Add(component);
		}
		else
		{
			int childCount = root.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = root.transform.GetChild(i);
				this.GetFirstPanelInEveryBranch(child, panels);
			}
		}
	}

	private void SendInfo(List<Component> panels)
	{
		List<ControlsInfoCollector.ControlInfo> list = new List<ControlsInfoCollector.ControlInfo>();
		for (int i = 0; i < panels.Count; i++)
		{
			List<ControlsInfoCollector.ControlInfo> allControlsInfo = ControlsInfoCollector.GetAllControlsInfo(panels[i]);
			int num = 0;
			while (allControlsInfo != null && num < allControlsInfo.Count)
			{
				list.Add(allControlsInfo[num]);
				num++;
			}
		}
		this.SendInfo(list);
	}

	private void SendInfo(List<ControlsInfoCollector.ControlInfo> controls)
	{
		if (controls == null || controls.Count == 0)
		{
			return;
		}
		if (this.draw)
		{
			this.draw.BeginDraw(new Color(0f, 0f, 1f, 0.5f));
			for (int i = 0; i < controls.Count; i++)
			{
				ControlsInfoCollector.ControlInfo controlInfo = controls[i];
				if (this.draw)
				{
					Rect rect = new Rect((float)controlInfo.left, (float)ControlsInfoCollector.getAbsTop(controlInfo.height, controlInfo.top), (float)controlInfo.width, (float)controlInfo.height);
				}
			}
		}
		string idsMsg = JsonMapper.ToJson(new ControlsInfoCollector.ControlInfoList
		{
			fps = -1.0,
			scaleRatio = 1.0,
			screenWidth = Screen.width,
			screenHeight = Screen.height,
			controls = controls
		});
		UICollectorBridge.GetInstance().SendUIList(idsMsg);
	}

	public static int getAbsTop(int height, int top)
	{
		return Screen.height - (height + top);
	}

	private string EncodeUIName(List<ControlsInfoCollector.ControlInfo> controls)
	{
		if (controls == null)
		{
			return null;
		}
		this._sb.Remove(0, this._sb.Length);
		for (int i = 0; i < controls.Count; i++)
		{
			this._sb.Append(",");
			this._sb.Append(controls[i].instanceId);
		}
		return this._sb.ToString();
	}

	private static List<ControlsInfoCollector.ControlInfo> GetAllControlsInfo(Component root)
	{
		List<ControlsInfoCollector.ControlInfo> list = new List<ControlsInfoCollector.ControlInfo>();
		Component[] componentsInChildren = root.GetComponentsInChildren(typeof(BoxCollider), false);
		int num = 0;
		while (componentsInChildren != null && num != componentsInChildren.Length)
		{
			BoxCollider boxCollider = componentsInChildren[num] as BoxCollider;
			if (boxCollider)
			{
				ControlsInfoCollector.ControlInfo controlInfo = new ControlsInfoCollector.ControlInfo();
				controlInfo.instanceId = componentsInChildren[num].gameObject.GetInstanceID();
				controlInfo.name = componentsInChildren[num].gameObject.name;
				UILabel componentInChildren = componentsInChildren[num].gameObject.GetComponentInChildren<UILabel>();
				if (componentInChildren)
				{
					controlInfo.showText = componentInChildren.text;
				}
				else
				{
					controlInfo.showText = string.Empty;
				}
				Rect areaInScreenByBoxCollider = ControlsInfoCollector.GetAreaInScreenByBoxCollider(boxCollider);
				controlInfo.top = (int)areaInScreenByBoxCollider.yMin;
				controlInfo.left = (int)areaInScreenByBoxCollider.xMin;
				controlInfo.width = (int)areaInScreenByBoxCollider.width;
				controlInfo.height = (int)areaInScreenByBoxCollider.height;
				list.Add(controlInfo);
			}
			num++;
		}
		return list;
	}

	private static Rect GetAreaInScreenByBoxCollider(BoxCollider boxCollider)
	{
		Vector3 vector = boxCollider.center - 0.5f * boxCollider.size;
		Vector3 vector2 = vector + new Vector3(0f, boxCollider.size.y, 0f);
		Vector3 localPos = vector2 + new Vector3(boxCollider.size.x, 0f, 0f);
		Vector3 loacalToScreen = ControlsInfoCollector.GetLoacalToScreen(boxCollider.transform, vector);
		Vector3 loacalToScreen2 = ControlsInfoCollector.GetLoacalToScreen(boxCollider.transform, vector2);
		Vector3 loacalToScreen3 = ControlsInfoCollector.GetLoacalToScreen(boxCollider.transform, localPos);
		return new Rect(loacalToScreen.x, loacalToScreen.y, Mathf.Abs(loacalToScreen3.x - loacalToScreen2.x), Mathf.Abs(loacalToScreen.y - loacalToScreen2.y));
	}

	private static Vector3 GetLoacalToScreen(Transform transform, Vector3 localPos)
	{
		return UICamera.mainCamera.WorldToScreenPoint(transform.TransformPoint(localPos));
	}
}
