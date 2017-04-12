using System;
using UnityEngine;

public class EffectWatcher : MonoBehaviour
{
	private NcDuplicator item;

	private NcSpriteAnimation[] list1;

	private NcCurveAnimation[] list2;

	private NcDelayActive[] list3;

	private NcUvAnimation[] list4;

	private ParticleSystem[] list5;

	private Animation[] list6;

	private DestroyForTime[] list8;

	private bool init;

	private void Awake()
	{
		this.init = true;
		NcDuplicator componentInChildren = base.gameObject.GetComponentInChildren<NcDuplicator>();
		if (componentInChildren != null)
		{
			LogSystem.LogWarning(new object[]
			{
				base.gameObject.name,
				" NcDuplicator cannot be replayed."
			});
			return;
		}
		this.list1 = base.gameObject.GetComponentsInChildren<NcSpriteAnimation>(true);
		this.list2 = base.gameObject.GetComponentsInChildren<NcCurveAnimation>(true);
		this.list3 = base.gameObject.GetComponentsInChildren<NcDelayActive>(true);
		this.list4 = base.gameObject.GetComponentsInChildren<NcUvAnimation>(true);
		this.list5 = base.gameObject.GetComponentsInChildren<ParticleSystem>(true);
		this.list6 = base.gameObject.GetComponentsInChildren<Animation>(true);
		this.list8 = base.gameObject.GetComponentsInChildren<DestroyForTime>(true);
	}

	public void ResetEffect()
	{
		if (!this.init)
		{
			this.Awake();
		}
		if (this.item != null)
		{
			LogSystem.LogWarning(new object[]
			{
				base.gameObject.name,
				" NcDuplicator cannot be replayed."
			});
			return;
		}
		if (this.list8 != null)
		{
			try
			{
				for (int i = 0; i < this.list8.Length; i++)
				{
					DestroyForTime destroyForTime = this.list8[i];
					if (!(destroyForTime == null))
					{
						destroyForTime.Reset();
					}
				}
			}
			catch (Exception ex)
			{
				LogSystem.LogError(new object[]
				{
					ex.ToString()
				});
			}
		}
		if (this.list1 != null)
		{
			try
			{
				for (int j = 0; j < this.list1.Length; j++)
				{
					if (this.list1[j] != null)
					{
						this.list1[j].ResetAnimation();
					}
				}
			}
			catch (Exception ex2)
			{
				LogSystem.LogError(new object[]
				{
					ex2.ToString()
				});
			}
		}
		if (this.list2 != null)
		{
			try
			{
				for (int k = 0; k < this.list2.Length; k++)
				{
					if (this.list2[k] != null)
					{
						this.list2[k].ResetAnimation();
					}
				}
			}
			catch (Exception ex3)
			{
				LogSystem.LogError(new object[]
				{
					ex3.ToString()
				});
			}
		}
		if (this.list3 != null)
		{
			try
			{
				for (int l = 0; l < this.list3.Length; l++)
				{
					if (this.list3[l] != null)
					{
						this.list3[l].ResetAnimation();
					}
				}
			}
			catch (Exception ex4)
			{
				LogSystem.LogError(new object[]
				{
					ex4.ToString()
				});
			}
		}
		if (this.list4 != null)
		{
			try
			{
				for (int m = 0; m < this.list4.Length; m++)
				{
					if (this.list4[m] != null)
					{
						this.list4[m].ResetAnimation();
					}
				}
			}
			catch (Exception ex5)
			{
				LogSystem.LogError(new object[]
				{
					ex5.ToString()
				});
			}
		}
		if (this.list5 != null)
		{
			try
			{
				for (int n = 0; n < this.list5.Length; n++)
				{
					ParticleSystem particleSystem = this.list5[n];
					if (particleSystem != null)
					{
						particleSystem.Stop();
						particleSystem.Clear();
						particleSystem.time = 0f;
						particleSystem.Play();
					}
				}
			}
			catch (Exception ex6)
			{
				LogSystem.LogError(new object[]
				{
					ex6.ToString()
				});
			}
		}
		if (this.list6 != null)
		{
			try
			{
				for (int num = 0; num < this.list6.Length; num++)
				{
					Animation animation = this.list6[num];
					if (!(animation == null))
					{
						foreach (AnimationState animationState in animation)
						{
							animationState.time = 0f;
						}
						animation.Play();
					}
				}
			}
			catch (Exception ex7)
			{
				LogSystem.LogError(new object[]
				{
					ex7.ToString()
				});
			}
		}
	}

	public void OnSpawned()
	{
	}

	private void OnDestroy()
	{
		this.list1 = null;
		this.list2 = null;
		this.list3 = null;
		this.list4 = null;
		this.list5 = null;
		this.list6 = null;
		this.list8 = null;
	}
}
