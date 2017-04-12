using System;
using UnityEngine;

public class AngleItem : MonoBehaviour
{
	private float MAXDISTANCE;

	private UIPanel panel;

	private AngleScrollView scrollView;

	private object data;

	private static int MaxDepth;

	public static object SelectSecretItem;

	private static object TempSecretItem;

	private void Start()
	{
		this.scrollView = NGUITools.FindInParents<AngleScrollView>(base.gameObject);
		this.panel = base.transform.GetComponent<UIPanel>();
		this.MAXDISTANCE = this.scrollView.panel.width / 2f;
	}

	private void Update()
	{
		Vector3 zero = Vector3.zero;
		zero.x = base.transform.position.x;
		zero.y = base.transform.position.y;
		Vector3 a = UICamera.currentCamera.transform.InverseTransformPoint(zero);
		float x = Vector3.Distance(a, Vector3.zero);
		float parabola = this.GetParabola(x);
		base.transform.localScale = Vector3.one * parabola;
		int num = Mathf.RoundToInt(parabola * 10f) + this.scrollView.panel.depth;
		this.panel.depth = num;
		this.panel.alpha = Mathf.Pow(parabola, 2f) * 1.2f;
		int num2 = Mathf.Max(AngleItem.MaxDepth, num);
		if (num2 > AngleItem.MaxDepth)
		{
			AngleItem.MaxDepth = num2;
			AngleItem.TempSecretItem = this.data;
		}
	}

	private void LateUpdate()
	{
		if (AngleItem.TempSecretItem != null)
		{
			AngleItem.SelectSecretItem = AngleItem.TempSecretItem;
			AngleItem.TempSecretItem = null;
			AngleItem.MaxDepth = 0;
		}
	}

	public void SetSecretItem(object data)
	{
		this.data = data;
	}

	private float GetParabola(float x)
	{
		float num = 1f / this.MAXDISTANCE;
		float num2 = Mathf.Max(this.MAXDISTANCE - Mathf.Abs(x), 0f);
		return Mathf.Sqrt(num2 * num);
	}
}
