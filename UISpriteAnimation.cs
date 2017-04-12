using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite Animation"), ExecuteInEditMode, RequireComponent(typeof(UISprite))]
public class UISpriteAnimation : MonoBehaviour
{
	[HideInInspector, SerializeField]
	private int mFPS = 30;

	[HideInInspector, SerializeField]
	private string mPrefix = string.Empty;

	[HideInInspector, SerializeField]
	private bool mLoop = true;

	private UISprite mSprite;

	private float mDelta;

	private int mIndex;

	private bool mActive = true;

	private List<string> mSpriteNames = new List<string>();

	private int loopTime = 1;

	private int loopCounter;

	public Action m_callBack;

	public bool perfect = true;

	public int frames
	{
		get
		{
			return this.mSpriteNames.Count;
		}
	}

	public int framesPerSecond
	{
		get
		{
			return this.mFPS;
		}
		set
		{
			this.mFPS = value;
		}
	}

	public string namePrefix
	{
		get
		{
			return this.mPrefix;
		}
		set
		{
			if (this.mPrefix != value)
			{
				this.mPrefix = value;
				this.RebuildSpriteList();
			}
		}
	}

	public bool loop
	{
		get
		{
			return this.mLoop;
		}
		set
		{
			this.mLoop = value;
		}
	}

	public bool isPlaying
	{
		get
		{
			return this.mActive;
		}
	}

	private void Start()
	{
		this.RebuildSpriteList();
	}

	private void Update()
	{
		if (this.mActive && this.mSpriteNames.Count > 1 && Application.isPlaying && (float)this.mFPS > 0f)
		{
			this.mDelta += RealTime.deltaTime;
			float num = 1f / (float)this.mFPS;
			if (num < this.mDelta)
			{
				this.mDelta = ((num <= 0f) ? 0f : (this.mDelta - num));
				if (++this.mIndex >= this.mSpriteNames.Count)
				{
					this.mIndex = 0;
					if (!this.loop)
					{
						this.loopCounter++;
						if (this.loopCounter >= this.loopTime)
						{
							this.mActive = false;
							if (this.m_callBack != null)
							{
								this.m_callBack();
							}
						}
					}
					else
					{
						this.mActive = true;
					}
				}
				if (this.mActive)
				{
					this.mSprite.spriteName = this.mSpriteNames[this.mIndex];
					if (this.perfect)
					{
						this.mSprite.MakePixelPerfect();
					}
				}
			}
		}
	}

	private void RebuildSpriteList()
	{
		if (this.mSprite == null)
		{
			this.mSprite = base.GetComponent<UISprite>();
		}
		this.mSpriteNames.Clear();
		if (this.mSprite != null && this.mSprite.atlas != null)
		{
			List<UISpriteData> spriteList = this.mSprite.atlas.spriteList;
			int i = 0;
			int count = spriteList.Count;
			while (i < count)
			{
				UISpriteData uISpriteData = spriteList[i];
				if (string.IsNullOrEmpty(this.mPrefix) || uISpriteData.name.StartsWith(this.mPrefix))
				{
					this.mSpriteNames.Add(uISpriteData.name);
				}
				i++;
			}
			this.mSpriteNames.Sort();
		}
	}

	public void Reset(int loopTime = 1)
	{
		this.loopCounter = 0;
		this.loopTime = loopTime;
		this.mActive = true;
		this.mIndex = 0;
		if (this.mSprite != null && this.mSpriteNames.Count > 0)
		{
			this.mSprite.spriteName = this.mSpriteNames[this.mIndex];
			if (this.perfect)
			{
				this.mSprite.MakePixelPerfect();
			}
		}
	}

	public void Replay()
	{
		this.loopCounter = 0;
		this.mActive = true;
		this.mIndex = 0;
		if (this.mSprite != null && this.mSpriteNames.Count > 0)
		{
			this.mSprite.spriteName = this.mSpriteNames[this.mIndex];
			if (this.perfect)
			{
				this.mSprite.MakePixelPerfect();
			}
		}
	}
}
