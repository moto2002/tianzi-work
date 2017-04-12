using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VCTouchController : MonoBehaviour
{
	private const string JoyFront = "JoyFront";

	private const int kMaxTouches = 5;

	public bool multitouch = true;

	public bool ignoreMultitouchErrorMovement;

	public float multiTouchErrorSqrMagnitudeMax = 1000f;

	[HideInInspector]
	public List<VCTouchWrapper> touches;

	private List<VCTouchWrapper> _activeTouchesCache;

	public static VCTouchController Instance
	{
		get;
		private set;
	}

	public List<VCTouchWrapper> ActiveTouches
	{
		get
		{
			if (this._activeTouchesCache == null)
			{
				this._activeTouchesCache = (from x in this.touches
				where x.Active
				select x).ToList<VCTouchWrapper>();
			}
			return this._activeTouchesCache;
		}
	}

	public static void ResetInstance()
	{
		VCTouchController.Instance = null;
	}

	private void Awake()
	{
		base.useGUILayout = false;
		if (VCTouchController.Instance != null)
		{
			VCUtils.DestroyWithError(base.gameObject, "Only one VCTouchController can be in a scene!  Destroying the gameObject with this component.");
			return;
		}
		VCTouchController.Instance = this;
		this.touches = new List<VCTouchWrapper>();
		for (int i = 0; i < 5; i++)
		{
			this.touches.Add(new VCTouchWrapper());
		}
	}

	private void Update()
	{
		if (this.touches == null)
		{
			return;
		}
		try
		{
			Input.multiTouchEnabled = this.multitouch;
			if (Input.touches != null && Input.touchCount > 0)
			{
				Touch[] array = Input.touches;
				int i = 0;
				Touch t;
				while (i < array.Length)
				{
					t = array[i];
					if (t.phase != TouchPhase.Began || !UICamera.Raycast(t.position, out UICamera.lastHit))
					{
						goto IL_C6;
					}
					if (!(UICamera.lastHit.collider == null))
					{
						if (UICamera.lastHit.collider.gameObject.name.Equals("JoyFront"))
						{
							goto IL_C6;
						}
					}
					IL_1B4:
					i++;
					continue;
					IL_C6:
					VCTouchWrapper vCTouchWrapper = this.touches.FirstOrDefault((VCTouchWrapper x) => x.fingerId == t.fingerId);
					if (vCTouchWrapper == null)
					{
						VCTouchWrapper vCTouchWrapper2 = this.touches.FirstOrDefault((VCTouchWrapper x) => x.fingerId == -1);
						if (vCTouchWrapper2 != null)
						{
							vCTouchWrapper2.Set(t);
						}
						goto IL_1B4;
					}
					vCTouchWrapper.visited = true;
					if (this.ignoreMultitouchErrorMovement && Input.touchCount > 1 && (vCTouchWrapper.position - t.position).sqrMagnitude > this.multiTouchErrorSqrMagnitudeMax)
					{
						goto IL_1B4;
					}
					vCTouchWrapper.deltaPosition = t.position - vCTouchWrapper.position;
					vCTouchWrapper.position = t.position;
					vCTouchWrapper.phase = t.phase;
					goto IL_1B4;
				}
			}
			if (this.touches.Count > 0)
			{
				foreach (VCTouchWrapper current in this.touches)
				{
					if (current != null)
					{
						if (!current.visited)
						{
							current.Reset();
						}
						else
						{
							current.visited = false;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogSystem.LogError(new object[]
			{
				"Update VCTouchController",
				ex.ToString()
			});
		}
		this._activeTouchesCache = null;
	}

	public VCTouchWrapper GetTouch(int fingerId)
	{
		return this.touches.FirstOrDefault((VCTouchWrapper x) => x.fingerId == fingerId);
	}
}
