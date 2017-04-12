using System;
using UnityEngine;

public class VCCollideableObject : MonoBehaviour
{
	protected Camera _colliderCamera;

	protected Collider _collider;

	private Vector3 _tempVec;

	protected void InitCollider(GameObject colliderGo)
	{
		this._collider = colliderGo.collider;
		this._colliderCamera = VCUtils.GetCamera(colliderGo);
	}

	public bool AABBContains(Vector2 pos)
	{
		if (this._collider == null)
		{
			return false;
		}
		this._tempVec = this._colliderCamera.WorldToScreenPoint(this._collider.bounds.min);
		if (pos.x < this._tempVec.x || pos.y < this._tempVec.y)
		{
			return false;
		}
		this._tempVec = this._colliderCamera.WorldToScreenPoint(this._collider.bounds.max);
		return pos.x <= this._tempVec.x && pos.y <= this._tempVec.y;
	}
}
