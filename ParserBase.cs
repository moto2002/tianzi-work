using System;

public class ParserBase
{
	public delegate void ParseCompleteListener(ParserBase parser);

	public ParserBase.ParseCompleteListener parseCompleteListener;

	private bool _parsingFailure;

	private bool _parsingComplete;

	protected byte[] _data;

	protected int _frameLimit;

	public bool parsingFailure
	{
		get
		{
			return this._parsingFailure;
		}
		set
		{
			this._parsingFailure = value;
		}
	}

	public bool parsingComplete
	{
		get
		{
			return this._parsingComplete;
		}
	}

	public void ParseAsync(byte[] data, int frameLimit = 30)
	{
		this._data = data;
		this.StartParsing(frameLimit);
	}

	protected void StartParsing(int frameLimit)
	{
		this._frameLimit = frameLimit;
		GameScene.mainScene.parsers.Add(this);
	}

	public void Update()
	{
		if (this.ProceedParsing() && !this._parsingFailure)
		{
			this.FinishParsing();
		}
	}

	public virtual bool ProceedParsing()
	{
		return true;
	}

	protected void FinishParsing()
	{
		this._parsingComplete = true;
		if (this.parseCompleteListener != null)
		{
			this.parseCompleteListener(this);
		}
	}
}
