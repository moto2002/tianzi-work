using System;

public class NcDontActive : NcEffectBehaviour
{
	private void Awake()
	{
		DelegateProxy.GameDestory(base.gameObject);
	}

	private void OnEnable()
	{
	}
}
