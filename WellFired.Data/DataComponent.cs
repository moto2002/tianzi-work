using System;
using UnityEngine;

namespace WellFired.Data
{
	public class DataComponent : MonoBehaviour
	{
		protected DataBaseEntry Data
		{
			get;
			set;
		}

		public void InitFromData(DataBaseEntry data)
		{
			this.Data = data;
		}

		protected virtual void Start()
		{
			if (this.Data == null)
			{
				Debug.LogError("Component has started without being Initialized", base.gameObject);
			}
		}
	}
}
