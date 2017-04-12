using System;
using UnityEngine;

[AddComponentMenu("NGUI/TypeWriterEffect"), RequireComponent(typeof(UILabel))]
public class TypeWriterEffect : MonoBehaviour
{
	public Action WriteComplete;

	public int charsPerSecond = 20;

	public bool wait;

	private UILabel mLabel;

	private string[] mText;

	private int mOffset;

	private float mNextChar;

	private int index;

	private int maxCount = 1;

	private void Awake()
	{
	}

	public void Init(string text, Action WriteComplete, bool wait = false)
	{
		this.index = 0;
		this.maxCount = 1;
		this.wait = wait;
		this.mText = new string[1];
		this.mText[this.index] = text;
		this.WriteComplete = WriteComplete;
		this.mLabel = base.GetComponent<UILabel>();
		if (this.mLabel == null)
		{
			this.mLabel = base.gameObject.AddComponent<UILabel>();
		}
	}

	public void Init(string[] text, Action WriteComplete, bool wait = false)
	{
		this.index = 0;
		this.maxCount = text.Length;
		this.wait = wait;
		this.mText = text;
		this.WriteComplete = WriteComplete;
		this.mLabel = base.GetComponent<UILabel>();
		if (this.mLabel == null)
		{
			this.mLabel = base.gameObject.AddComponent<UILabel>();
		}
	}

	private void Update()
	{
		if (this.mLabel == null)
		{
			this.mLabel = base.GetComponent<UILabel>();
			this.index = 0;
			this.maxCount = 1;
			this.wait = false;
			this.mText = new string[1];
			this.mText[this.index] = this.mLabel.processedText;
		}
		if (this.mOffset < this.mText[this.index].Length)
		{
			if (this.mNextChar <= RealTime.time)
			{
				this.charsPerSecond = Mathf.Max(1, this.charsPerSecond);
				float num = 1f / (float)this.charsPerSecond;
				char c = this.mText[this.index][this.mOffset];
				if (c == '.' || c == '\n' || c == '!' || c == '?')
				{
					num *= 4f;
				}
				NGUIText.ParseSymbol(this.mText[this.index], ref this.mOffset);
				this.mNextChar = RealTime.time + num;
				this.mLabel.text = this.mText[this.index].Substring(0, ++this.mOffset);
			}
		}
		else if (!this.wait)
		{
			this.index++;
			if (this.index >= this.maxCount)
			{
				if (this.WriteComplete != null)
				{
					this.WriteComplete();
				}
				UnityEngine.Object.Destroy(this);
			}
			else
			{
				this.mOffset = 0;
			}
		}
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
		{
			if (this.mOffset < this.mText[this.index].Length)
			{
				this.mLabel.text = this.mText[this.index];
				this.mOffset = this.mText[this.index].Length;
			}
			else
			{
				this.index++;
				if (this.index >= this.maxCount)
				{
					if (this.WriteComplete != null)
					{
						this.WriteComplete();
					}
					UnityEngine.Object.Destroy(this);
				}
				else
				{
					this.mOffset = 0;
				}
			}
		}
	}
}
