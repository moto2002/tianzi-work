using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Grid")]
public class UIGrid : UIWidgetContainer
{
	public enum Arrangement
	{
		Horizontal,
		Vertical
	}

	public enum Sorting
	{
		None,
		Alphabetic,
		Horizontal,
		Vertical,
		Custom
	}

	public delegate void OnReposition();

	public delegate void OnUpdateDataRow(UIGridItem item);

	public delegate void OnClickSubItem(UIGridItem oItem, GameObject OSubItem);

	public delegate void OnSelectEvent(int iSelect, bool active, bool select);

	public UIGrid.Arrangement arrangement = UIGrid.Arrangement.Vertical;

	public UIGrid.Sorting sorting;

	public UIWidget.Pivot pivot;

	public int maxPerLine = 1;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	public bool animateSmoothly;

	public bool hideInactive = true;

	public bool keepWithinPanel;

	public UIGrid.OnReposition onReposition;

	[HideInInspector, SerializeField]
	private bool sorted;

	protected bool mReposition;

	protected UIPanel mPanel;

	protected bool mInitDone;

	private bool mIsStartOk;

	private Action FnCallbackStartOk;

	public bool mbCustomGrid;

	public List<Transform> mTransSource = new List<Transform>();

	public List<object> mDataSource = new List<object>();

	public List<Transform> mFreeTrans = new List<Transform>();

	public Dictionary<object, Transform> mDataToTrans = new Dictionary<object, Transform>();

	public GameObject mgridItem;

	public GameObject mSelectItem;

	private UIGrid.OnUpdateDataRow mfnOnChangeRow;

	private Vector3 mPanelPos;

	private Vector3 mPanelInitPos;

	private Vector2 mPanelInitOffest;

	private int mOffsetRows;

	public UIGrid.OnClickSubItem fnonClickSubItem;

	public UIGrid.OnSelectEvent SelectItem;

	public int miSelectIndex = -1;

	private BetterList<Transform> list = new BetterList<Transform>();

	public int OffestRow
	{
		get
		{
			return this.mOffsetRows;
		}
		set
		{
			this.mOffsetRows = value;
		}
	}

	public bool repositionNow
	{
		set
		{
			if (value)
			{
				this.mReposition = true;
				base.enabled = true;
			}
		}
	}

	public void BindCustomCallBack(UIGrid.OnUpdateDataRow fn_ChangeRow)
	{
		this.mfnOnChangeRow = fn_ChangeRow;
		this.mbCustomGrid = true;
	}

	public void OnClickItem(UIGridItem oItem, GameObject OSubItem)
	{
		if (this.fnonClickSubItem != null)
		{
			this.fnonClickSubItem(oItem, OSubItem);
		}
	}

	public void ClearCustomGrid()
	{
		this.ClearCustomData();
	}

	public void ClearCustomData()
	{
		this.mOffsetRows = 0;
		this.miSelectIndex = -1;
		this.PanelReset();
		int count = this.mFreeTrans.Count;
		this.mFreeTrans.InsertRange(this.mFreeTrans.Count, this.mDataToTrans.Values);
		for (int i = count; i < this.mFreeTrans.Count; i++)
		{
			Transform transform = this.mFreeTrans[i];
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
		}
		this.mDataToTrans.Clear();
		this.mDataSource.Clear();
	}

	public void DeleteCustomData(object oData)
	{
		if (oData == null)
		{
			return;
		}
		for (int i = 0; i < this.mDataSource.Count; i++)
		{
			object obj = this.mDataSource[i];
			if (obj == oData)
			{
				this.mDataSource.RemoveAt(i);
				if (this.mDataToTrans.ContainsKey(oData))
				{
					Transform transform = this.mDataToTrans[oData];
					this.mDataToTrans.Remove(oData);
					if (transform != null)
					{
						transform.gameObject.SetActive(false);
						this.mFreeTrans.Add(transform);
						this.UpdateCustomView();
					}
				}
				break;
			}
		}
	}

	public void UpdateCustomData(object odata)
	{
		if (odata == null)
		{
			return;
		}
		for (int i = 0; i < this.mDataSource.Count; i++)
		{
			object obj = this.mDataSource[i];
			if (obj == odata)
			{
				this.mDataSource[i] = odata;
				if (this.mDataToTrans.ContainsKey(odata))
				{
					Transform transform = this.mDataToTrans[odata];
					if (transform != null)
					{
						UIGridItem component = transform.gameObject.GetComponent<UIGridItem>();
						if (component != null)
						{
							component.setIndex(i);
							component.onClick = new UIGridItem.VoidDelegate(this.OnClick);
							component.oData = odata;
							this.mfnOnChangeRow(component);
						}
					}
				}
				break;
			}
		}
	}

	public void UpdateCustomDataList(List<object> list)
	{
		this.UpdateCustomDataListAll(list);
	}

	public void UpdateCustomDataListAll(List<object> list)
	{
		int count = list.Count;
		int count2 = this.mDataSource.Count;
		int num = Mathf.Max(count, count2);
		bool flag = false;
		for (int i = 0; i < num; i++)
		{
			if (i < count)
			{
				if (i >= count2)
				{
					this.AddCustomData(list[i]);
				}
				else if (this.mDataSource[i] != list[i])
				{
					object key = this.mDataSource[i];
					if (this.mDataToTrans.ContainsKey(key))
					{
						Transform transform = this.mDataToTrans[key];
						this.mDataToTrans.Remove(key);
						if (transform != null)
						{
							transform.gameObject.SetActive(false);
							this.mFreeTrans.Add(transform);
						}
					}
					this.mDataSource[i] = list[i];
				}
			}
			else if (i < count2)
			{
				flag = true;
			}
		}
		if (flag)
		{
			int num2 = count2 - count;
			for (int j = 0; j < num2; j++)
			{
				object key2 = this.mDataSource[count];
				if (this.mDataToTrans.ContainsKey(key2))
				{
					Transform transform2 = this.mDataToTrans[key2];
					this.mDataToTrans.Remove(key2);
					if (transform2 != null)
					{
						transform2.gameObject.SetActive(false);
						this.mFreeTrans.Add(transform2);
					}
				}
				this.mDataSource.RemoveAt(count);
			}
		}
		this.UpdateCustomView();
	}

	public void AddCustomDataList(List<object> list)
	{
		this.ClearCustomData();
		this.UpdateCustomDataListAll(list);
	}

	public void AddCustomData(object oData)
	{
		this.mDataSource.Add(oData);
	}

	private void OnCustomDrag()
	{
		int num = 0;
		if (this.arrangement == UIGrid.Arrangement.Vertical)
		{
			int num2 = (int)(this.mPanel.height / this.cellHeight) + 2;
			int num3 = Mathf.CeilToInt((float)this.mDataSource.Count / (float)this.maxPerLine);
			if (this.mDataSource.Count < this.maxPerLine * num2)
			{
				return;
			}
			num = (int)((this.mPanel.transform.localPosition.y - this.mPanelPos.y) / this.cellHeight);
			if (num > num3 - num2)
			{
				num = num3 - num2;
			}
			if (num < 0)
			{
				num = 0;
			}
		}
		else if (this.arrangement == UIGrid.Arrangement.Horizontal)
		{
			int num2 = Mathf.FloorToInt(this.mPanel.width / this.cellWidth) + 2;
			int num3 = Mathf.CeilToInt((float)this.mDataSource.Count / (float)this.maxPerLine);
			if (this.mDataSource.Count < this.maxPerLine * num2)
			{
				return;
			}
			num = (int)((this.mPanelPos.x - this.mPanel.transform.localPosition.x) / this.cellWidth);
			if (num > num3 - num2)
			{
				num = num3 - num2;
			}
			if (num < 0)
			{
				num = 0;
			}
		}
		if (this.mOffsetRows == num)
		{
			return;
		}
		this.mOffsetRows = num;
		this.UpdateCustomView();
	}

	private Transform GetFreeTrans()
	{
		if (this.mFreeTrans.Count > 0)
		{
			Transform result = this.mFreeTrans[0];
			this.mFreeTrans.RemoveAt(0);
			return result;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(this.mgridItem) as GameObject;
		if (gameObject != null)
		{
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			return gameObject.transform;
		}
		return null;
	}

	public void UpdateCustomView()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (this.arrangement == UIGrid.Arrangement.Vertical)
		{
			num3 = (int)(this.mPanel.height / this.cellHeight) + 2;
		}
		else if (this.arrangement == UIGrid.Arrangement.Horizontal)
		{
			num3 = Mathf.FloorToInt(this.mPanel.width / this.cellWidth) + 2;
		}
		int num4 = this.mOffsetRows * this.maxPerLine;
		int num5 = num4 + num3 * this.maxPerLine;
		for (int i = 0; i < this.mDataSource.Count; i++)
		{
			object obj = this.mDataSource[i];
			if (i < num4 || i > num5)
			{
				if (this.mDataToTrans.ContainsKey(obj))
				{
					Transform transform = this.mDataToTrans[obj];
					if (transform != null)
					{
						transform.gameObject.SetActive(false);
						this.mFreeTrans.Add(transform);
					}
					this.mDataToTrans.Remove(obj);
				}
				if (this.SelectItem != null && this.miSelectIndex == i)
				{
					this.SelectItem(this.miSelectIndex, false, false);
				}
			}
			else
			{
				int num6 = i - num4;
				Transform transform2;
				if (this.mDataToTrans.ContainsKey(obj))
				{
					transform2 = this.mDataToTrans[obj];
				}
				else
				{
					transform2 = this.GetFreeTrans();
					if (transform2 == null)
					{
						return;
					}
					this.mDataToTrans.Add(obj, transform2);
				}
				transform2.name = DelegateProxy.StringBuilder(new object[]
				{
					"Item",
					i
				});
				float z = transform2.localPosition.z;
				Vector3 zero = Vector3.zero;
				if (this.arrangement == UIGrid.Arrangement.Vertical)
				{
					zero.x = this.cellWidth * (float)num;
					zero.y = -this.cellHeight * (float)num2;
					zero.z = z;
				}
				else
				{
					zero.x = this.cellWidth * (float)num2;
					zero.y = -this.cellHeight * (float)num;
					zero.z = z;
				}
				transform2.localPosition = zero;
				if (!transform2.gameObject.activeSelf)
				{
					transform2.gameObject.SetActive(true);
				}
				UIGridItem component = transform2.gameObject.GetComponent<UIGridItem>();
				if (component != null)
				{
					component.setIndex(i);
					component.onClick = new UIGridItem.VoidDelegate(this.OnClick);
					component.oData = obj;
					this.mfnOnChangeRow(component);
				}
				if (this.SelectItem != null && this.miSelectIndex == i)
				{
					this.SelectItem(this.miSelectIndex, true, false);
				}
			}
			if (++num >= this.maxPerLine && this.maxPerLine > 0)
			{
				num = 0;
				num2++;
			}
		}
	}

	public void GoToPosition(int index)
	{
		if (index < 0 || index >= this.mDataSource.Count)
		{
			return;
		}
		int num = index / this.maxPerLine;
		if (this.arrangement == UIGrid.Arrangement.Vertical)
		{
			float num2 = this.cellHeight * (float)num;
			Vector3 zero = Vector3.zero;
			zero.x = this.mPanelInitPos.x;
			zero.y = this.mPanelInitPos.y + num2;
			zero.z = this.mPanelInitPos.z;
			this.mPanel.transform.localPosition = zero;
			zero.x = this.mPanelInitOffest.x;
			zero.y = this.mPanelInitOffest.y - num2;
			zero.z = 0f;
			this.mPanel.clipOffset = zero;
		}
		else if (this.arrangement == UIGrid.Arrangement.Horizontal)
		{
			float num3 = this.cellWidth * (float)num;
			Vector3 zero2 = Vector3.zero;
			zero2.x = this.mPanelInitPos.x - num3;
			zero2.y = this.mPanelInitPos.y;
			zero2.z = this.mPanelInitPos.z;
			this.mPanel.transform.localPosition = zero2;
			zero2.x = this.mPanelInitOffest.x + num3;
			zero2.y = this.mPanelInitOffest.y;
			zero2.z = 0f;
			this.mPanel.clipOffset = zero2;
		}
		this.OnCustomDrag();
		UIScrollView uIScrollView = NGUITools.FindInParents<UIScrollView>(base.gameObject);
		uIScrollView.DisableSpring();
		uIScrollView.UpdateScrollbars(true);
		uIScrollView.RestrictWithinBounds(false, uIScrollView.canMoveHorizontally, uIScrollView.canMoveVertically);
	}

	public void ScrollToPostion(int nScrollIndex, int nSelectIndex = -1)
	{
		if (!this.mIsStartOk)
		{
			this.FnCallbackStartOk = delegate
			{
				this.GoToPosition(nScrollIndex);
				this.SetSelect(nSelectIndex);
			};
			return;
		}
		this.GoToPosition(nScrollIndex);
		this.SetSelect(nSelectIndex);
	}

	public int GetSelectedIndex()
	{
		return this.miSelectIndex;
	}

	public void SetSelect(int iSelectIndex)
	{
		if (iSelectIndex < 0 || iSelectIndex >= this.mDataSource.Count)
		{
			return;
		}
		object select = this.mDataSource[iSelectIndex];
		this.SetSelect(select);
	}

	public void SetSelect(object oData)
	{
		if (oData == null)
		{
			return;
		}
		for (int i = 0; i < this.mDataSource.Count; i++)
		{
			if (this.mDataSource[i] != null && oData == this.mDataSource[i])
			{
				if (this.mSelectItem != null && this.SelectItem != null)
				{
					this.SelectItem(this.miSelectIndex, false, true);
				}
				if (this.mDataToTrans.ContainsKey(oData))
				{
					Transform transform = this.mDataToTrans[oData];
					if (transform != null)
					{
						this.mSelectItem = transform.gameObject;
					}
				}
				this.miSelectIndex = i;
				if (this.SelectItem != null)
				{
					this.SelectItem(this.miSelectIndex, true, true);
				}
				break;
			}
		}
	}

	public object GetSelected()
	{
		if (this.miSelectIndex < 0 || this.miSelectIndex >= this.mDataSource.Count)
		{
			return null;
		}
		return this.mDataSource[this.miSelectIndex];
	}

	public GameObject GetSelected(int index)
	{
		if (index < 0 || index >= this.mDataSource.Count)
		{
			return null;
		}
		object obj = this.mDataSource[index];
		if (obj == null)
		{
			return null;
		}
		if (this.mDataToTrans.ContainsKey(obj))
		{
			Transform transform = this.mDataToTrans[obj];
			if (transform != null)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public Transform GetSelectedItem()
	{
		if (this.mSelectItem == null)
		{
			return null;
		}
		return this.mSelectItem.transform;
	}

	public UIGridItem GetSelectedGridItem()
	{
		if (this.miSelectIndex < 0 || this.miSelectIndex >= this.mDataSource.Count)
		{
			return null;
		}
		UIGridItem result = null;
		if (this.mSelectItem != null)
		{
			result = this.mSelectItem.GetComponent<UIGridItem>();
		}
		return result;
	}

	public object GetCustomDataItem(int iIndex)
	{
		if (iIndex < 0 || iIndex > this.mDataSource.Count)
		{
			return null;
		}
		return this.mDataSource[iIndex];
	}

	public Transform GetCustomTransItem(int iIndex)
	{
		if (iIndex < 0 || iIndex > this.mDataSource.Count)
		{
			return null;
		}
		object key = this.mDataSource[iIndex];
		if (this.mDataToTrans.ContainsKey(key))
		{
			return this.mDataToTrans[key];
		}
		return null;
	}

	public UIGridItem GetCustomGridItem(int iIndex)
	{
		if (iIndex < 0 || iIndex > this.mDataSource.Count)
		{
			return null;
		}
		UIGridItem result = null;
		object key = this.mDataSource[iIndex];
		if (this.mDataToTrans.ContainsKey(key))
		{
			Transform transform = this.mDataToTrans[key];
			if (transform != null)
			{
				result = transform.GetComponent<UIGridItem>();
			}
			return result;
		}
		return result;
	}

	private void OnClick(UIGridItem item)
	{
		if (item == null)
		{
			return;
		}
		if (this.mSelectItem != null && this.miSelectIndex >= 0 && this.SelectItem != null)
		{
			this.SelectItem(this.miSelectIndex, false, true);
		}
		this.miSelectIndex = item.GetIndex();
		this.mSelectItem = item.gameObject;
		if (this.SelectItem != null && this.miSelectIndex >= 0 && this.SelectItem != null)
		{
			this.SelectItem(this.miSelectIndex, true, true);
		}
	}

	private void OnSubmit(UIGridItem item)
	{
	}

	private void OnDoubleClick(UIGridItem item)
	{
	}

	private void OnHover(UIGridItem item, bool isOver)
	{
	}

	private void OnPress(UIGridItem item, bool isPressed)
	{
	}

	private void OnSelect(UIGridItem item, bool selected)
	{
	}

	private void OnScroll(float delta)
	{
	}

	private void OnDrag(UIGridItem item, Vector2 delta)
	{
	}

	private void OnDrop(UIGridItem item, GameObject go)
	{
	}

	private void OnKey(UIGridItem item, KeyCode key)
	{
	}

	protected virtual void Init()
	{
		this.mInitDone = true;
		this.mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
		this.mPanelInitPos = this.mPanel.transform.localPosition;
		this.mPanelInitOffest = this.mPanel.clipOffset;
		if (this.maxPerLine < 1)
		{
			this.maxPerLine = 1;
		}
	}

	protected virtual void Start()
	{
		if (!this.mInitDone)
		{
			this.Init();
		}
		this.mPanelPos = this.mPanel.transform.localPosition;
		if (this.mgridItem != null)
		{
			this.mgridItem.SetActive(false);
		}
		bool flag = this.animateSmoothly;
		this.animateSmoothly = false;
		this.Reposition();
		this.animateSmoothly = flag;
		base.enabled = this.mbCustomGrid;
		this.mIsStartOk = true;
		if (this.FnCallbackStartOk != null)
		{
			this.FnCallbackStartOk();
			this.FnCallbackStartOk = null;
		}
	}

	public void StartCustom()
	{
		if (!this.mInitDone)
		{
			this.Init();
		}
		bool flag = this.animateSmoothly;
		this.animateSmoothly = false;
		this.PanelReset();
		this.Reposition();
		this.mPanelPos = this.mPanel.transform.localPosition;
		this.animateSmoothly = flag;
		base.enabled = this.mbCustomGrid;
	}

	private void PanelReset()
	{
		UIScrollView uIScrollView = NGUITools.FindInParents<UIScrollView>(base.gameObject);
		if (uIScrollView == null)
		{
			return;
		}
		uIScrollView.DisableSpring();
		if (this.mPanel != null)
		{
			this.mPanel.transform.localPosition = this.mPanelInitPos;
			this.mPanel.clipOffset = this.mPanelInitOffest;
		}
		uIScrollView.DisableSpring();
		uIScrollView.UpdateScrollbars(true);
	}

	protected virtual void Update()
	{
		if (this.mbCustomGrid)
		{
			this.OnCustomDrag();
		}
		else if (this.mReposition)
		{
			this.Reposition();
		}
		base.enabled = this.mbCustomGrid;
	}

	public static int SortByName(Transform a, Transform b)
	{
		return string.Compare(a.name, b.name);
	}

	public static int SortHorizontal(Transform a, Transform b)
	{
		return a.localPosition.x.CompareTo(b.localPosition.x);
	}

	public static int SortVertical(Transform a, Transform b)
	{
		return b.localPosition.y.CompareTo(a.localPosition.y);
	}

	protected virtual void Sort(BetterList<Transform> list)
	{
		list.Sort(new BetterList<Transform>.CompareFunc(UIGrid.SortByName));
	}

	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !this.mInitDone && NGUITools.GetActive(this))
		{
			this.mReposition = true;
			return;
		}
		if (!this.mInitDone)
		{
			this.Init();
		}
		this.mReposition = false;
		Transform transform = base.transform;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		if (this.sorting != UIGrid.Sorting.None || this.sorted)
		{
			this.list.Clear();
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child && (!this.hideInactive || NGUITools.GetActive(child.gameObject)))
				{
					this.list.Add(child);
				}
			}
			if (this.sorting == UIGrid.Sorting.Alphabetic)
			{
				this.list.Sort(new BetterList<Transform>.CompareFunc(UIGrid.SortByName));
			}
			else if (this.sorting == UIGrid.Sorting.Horizontal)
			{
				this.list.Sort(new BetterList<Transform>.CompareFunc(UIGrid.SortHorizontal));
			}
			else if (this.sorting == UIGrid.Sorting.Vertical)
			{
				this.list.Sort(new BetterList<Transform>.CompareFunc(UIGrid.SortVertical));
			}
			else
			{
				this.Sort(this.list);
			}
			int j = 0;
			int size = this.list.size;
			while (j < size)
			{
				Transform transform2 = this.list[j];
				if (NGUITools.GetActive(transform2.gameObject) || !this.hideInactive)
				{
					float z = transform2.localPosition.z;
					Vector3 zero = Vector3.zero;
					if (this.arrangement == UIGrid.Arrangement.Vertical)
					{
						zero.x = this.cellWidth * (float)num;
						zero.y = -this.cellHeight * (float)num2;
						zero.z = z;
					}
					else
					{
						zero.x = this.cellWidth * (float)num2;
						zero.y = -this.cellHeight * (float)num - this.cellHeight * (float)num2;
						zero.z = z;
					}
					if (this.animateSmoothly && Application.isPlaying)
					{
						SpringPosition.Begin(transform2.gameObject, zero, 15f).updateScrollView = true;
					}
					else
					{
						transform2.localPosition = zero;
					}
					num3 = Mathf.Max(num3, num);
					num4 = Mathf.Max(num4, num2);
					if (++num >= this.maxPerLine && this.maxPerLine > 0)
					{
						num = 0;
						num2++;
					}
				}
				j++;
			}
		}
		else
		{
			for (int k = 0; k < transform.childCount; k++)
			{
				Transform child2 = transform.GetChild(k);
				if (NGUITools.GetActive(child2.gameObject) || !this.hideInactive)
				{
					float z2 = child2.localPosition.z;
					Vector3 zero2 = Vector3.zero;
					if (this.arrangement == UIGrid.Arrangement.Vertical)
					{
						zero2.x = this.cellWidth * (float)num;
						zero2.y = -this.cellHeight * (float)num2;
						zero2.z = z2;
					}
					else
					{
						zero2.x = this.cellWidth * (float)num2;
						zero2.y = -this.cellHeight * (float)num;
						zero2.z = z2;
					}
					child2.localPosition = zero2;
					if (++num >= this.maxPerLine && this.maxPerLine > 0)
					{
						num = 0;
						num2++;
					}
				}
			}
		}
		if (this.pivot != UIWidget.Pivot.TopLeft)
		{
			Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.pivot);
			float num5;
			float num6;
			if (this.arrangement == UIGrid.Arrangement.Horizontal)
			{
				num5 = Mathf.Lerp(0f, (float)num3 * this.cellWidth, pivotOffset.x);
				num6 = Mathf.Lerp((float)(-(float)num4) * this.cellHeight, 0f, pivotOffset.y);
			}
			else
			{
				num5 = Mathf.Lerp(0f, (float)num4 * this.cellWidth, pivotOffset.x);
				num6 = Mathf.Lerp((float)(-(float)num3) * this.cellHeight, 0f, pivotOffset.y);
			}
			for (int l = 0; l < transform.childCount; l++)
			{
				Transform child3 = transform.GetChild(l);
				if (NGUITools.GetActive(child3.gameObject) || !this.hideInactive)
				{
					SpringPosition component = child3.GetComponent<SpringPosition>();
					if (component != null)
					{
						SpringPosition expr_47B_cp_0 = component;
						expr_47B_cp_0.target.x = expr_47B_cp_0.target.x - num5;
						SpringPosition expr_490_cp_0 = component;
						expr_490_cp_0.target.y = expr_490_cp_0.target.y - num6;
					}
					else
					{
						Vector3 localPosition = child3.localPosition;
						localPosition.x -= num5;
						localPosition.y -= num6;
						child3.localPosition = localPosition;
					}
				}
			}
		}
		if (this.keepWithinPanel && this.mPanel != null)
		{
			this.mPanel.ConstrainTargetToBounds(transform, true);
		}
		if (this.onReposition != null)
		{
			this.onReposition();
		}
	}

	public bool IsContainNowShow(object oData)
	{
		return this.mDataToTrans.ContainsKey(oData);
	}

	public UIGridItem GetItemByData(object oData)
	{
		if (this.mDataToTrans.ContainsKey(oData))
		{
			Transform transform = this.mDataToTrans[oData];
			if (transform != null)
			{
				UIGridItem component = transform.GetComponent<UIGridItem>();
				if (component != null)
				{
					return component;
				}
			}
		}
		return null;
	}

	private void OnDestroy()
	{
		int count = this.mFreeTrans.Count;
		this.mFreeTrans.InsertRange(this.mFreeTrans.Count, this.mDataToTrans.Values);
		for (int i = count; i < this.mFreeTrans.Count; i++)
		{
			Transform transform = this.mFreeTrans[i];
			if (transform != null)
			{
				UnityEngine.Object.DestroyImmediate(transform.gameObject);
			}
		}
		this.mFreeTrans.Clear();
		this.mDataToTrans.Clear();
		if (this.mDataSource != null)
		{
			this.mDataSource.Clear();
		}
		this.mgridItem = null;
		this.mSelectItem = null;
		this.mfnOnChangeRow = null;
		this.fnonClickSubItem = null;
		this.SelectItem = null;
	}
}
