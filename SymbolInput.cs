using System;
using System.Text;
using UnityEngine;

public class SymbolInput : UIInput
{
	public SymbolLabel newLabel;

	private string mCached = string.Empty;

	public new string defaultText
	{
		get
		{
			return this.mDefaultText;
		}
		set
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			this.mDefaultText = value;
			this.UpdateLabel();
		}
	}

	[Obsolete("Use UIInput.value instead")]
	public new string text
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	public new string value
	{
		get
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			return this.mValue;
		}
		set
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			UIInput.mDrawStart = 0;
			if (Application.platform == RuntimePlatform.BB10Player)
			{
				value = value.Replace("\\b", "\b");
			}
			value = this.Validate(value);
			if (this.isSelected && UIInput.mKeyboard != null && this.mCached != value)
			{
				if (Config.bANYSDK)
				{
				}
				UIInput.mKeyboard.text = value;
				this.mCached = value;
			}
			if (this.mValue != value)
			{
				this.mValue = value;
				if (!this.isSelected)
				{
					this.SaveToPlayerPrefs(value);
				}
				this.UpdateLabel();
				this.ExecuteOnChange();
			}
		}
	}

	[Obsolete("Use UIInput.isSelected instead")]
	public new bool selected
	{
		get
		{
			return this.isSelected;
		}
		set
		{
			this.isSelected = value;
		}
	}

	public new bool isSelected
	{
		get
		{
			return UIInput.selection == this;
		}
		set
		{
			if (!value)
			{
				if (this.isSelected)
				{
					UICamera.selectedObject = null;
				}
			}
			else
			{
				UICamera.selectedObject = base.gameObject;
			}
		}
	}

	public new int cursorPosition
	{
		get
		{
			return this.value.Length;
		}
		set
		{
		}
	}

	public new int selectionStart
	{
		get
		{
			return this.value.Length;
		}
		set
		{
		}
	}

	public new int selectionEnd
	{
		get
		{
			return this.value.Length;
		}
		set
		{
		}
	}

	public new string Validate(string val)
	{
		if (string.IsNullOrEmpty(val))
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(val.Length);
		for (int i = 0; i < val.Length; i++)
		{
			char c = val[i];
			if (this.onValidate != null)
			{
				c = this.onValidate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			else if (this.validation != UIInput.Validation.None)
			{
				c = this.Validate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			if (c != '\0')
			{
				stringBuilder.Append(c);
			}
		}
		if (this.characterLimit > 0 && stringBuilder.Length > this.characterLimit)
		{
			return stringBuilder.ToString(0, this.characterLimit);
		}
		return stringBuilder.ToString();
	}

	private void Start()
	{
		if (string.IsNullOrEmpty(this.mValue))
		{
			if (!string.IsNullOrEmpty(this.savedAs) && PlayerPrefs.HasKey(this.savedAs))
			{
				this.value = PlayerPrefs.GetString(this.savedAs);
			}
		}
		else
		{
			this.value = this.mValue.Replace("\\n", "\n");
		}
	}

	protected new void Init()
	{
		if (this.mDoInit && this.newLabel != null)
		{
			this.mDoInit = false;
			this.mDefaultText = this.newLabel.text;
			this.mDefaultColor = this.newLabel.labelText.color;
			if (this.newLabel.labelText.alignment == NGUIText.Alignment.Justified)
			{
				this.newLabel.labelText.alignment = NGUIText.Alignment.Left;
				LogSystem.LogWarning(new object[]
				{
					"Input fields using labels with justified alignment are not supported at this time",
					this
				});
			}
			this.mPivot = this.newLabel.labelText.pivot;
			this.mPosition = this.newLabel.labelText.cachedTransform.localPosition.x;
			this.UpdateLabel();
		}
	}

	protected new void SaveToPlayerPrefs(string val)
	{
		if (!string.IsNullOrEmpty(this.savedAs))
		{
			if (string.IsNullOrEmpty(val))
			{
				PlayerPrefs.DeleteKey(this.savedAs);
			}
			else
			{
				PlayerPrefs.SetString(this.savedAs, val);
			}
		}
	}

	protected override void OnSelect(bool isSelected)
	{
		if (isSelected)
		{
			this.OnSelectEvent();
		}
		else
		{
			this.OnDeselectEvent();
		}
	}

	protected new void OnSelectEvent()
	{
		UIInput.selection = this;
		if (this.mDoInit)
		{
			this.Init();
		}
		LogSystem.LogWarning(new object[]
		{
			"keyBoard open 1"
		});
		if (this.newLabel != null && NGUITools.GetActive(this))
		{
			this.newLabel.labelText.color = this.activeTextColor;
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				UIInput.mKeyboard = ((this.inputType != UIInput.InputType.Password) ? TouchScreenKeyboard.Open(this.mValue, (TouchScreenKeyboardType)this.keyboardType, this.inputType == UIInput.InputType.AutoCorrect, this.newLabel.labelText.multiLine, false, false, this.defaultText) : TouchScreenKeyboard.Open(this.mValue, TouchScreenKeyboardType.Default, false, false, true));
				if (Config.bANYSDK)
				{
				}
			}
			else
			{
				Vector2 compositionCursorPos = (!(UICamera.current != null) || !(UICamera.current.cachedCamera != null)) ? this.newLabel.labelText.worldCorners[0] : UICamera.current.cachedCamera.WorldToScreenPoint(this.newLabel.labelText.worldCorners[0]);
				compositionCursorPos.y = (float)ResolutionConstrain.Instance.height - compositionCursorPos.y;
				Input.imeCompositionMode = IMECompositionMode.On;
				Input.compositionCursorPos = compositionCursorPos;
				UIInput.mDrawStart = 0;
			}
			this.UpdateLabel();
		}
	}

	protected new void OnDeselectEvent()
	{
		if (this.mDoInit)
		{
			this.Init();
		}
		if (this.newLabel != null && NGUITools.GetActive(this))
		{
			this.mValue = this.value;
			if (UIInput.mKeyboard != null)
			{
				UIInput.mKeyboard.active = false;
				UIInput.mKeyboard = null;
				if (Config.bANYSDK)
				{
				}
			}
			if (string.IsNullOrEmpty(this.mValue))
			{
				this.newLabel.text = this.mDefaultText;
				this.newLabel.labelText.color = this.mDefaultColor;
			}
			else
			{
				this.newLabel.text = this.mValue;
			}
			if (Config.bANYSDK)
			{
			}
			Input.imeCompositionMode = IMECompositionMode.Auto;
			this.RestoreLabelPivot();
		}
		UIInput.selection = null;
		this.UpdateLabel();
	}

	private void Update()
	{
		if (UIInput.mKeyboard != null && this.isSelected)
		{
			string text = UIInput.mKeyboard.text;
			if (this.mCached != text)
			{
				this.mCached = text;
				this.value = text;
			}
			if (UIInput.mKeyboard.done)
			{
				if (!UIInput.mKeyboard.wasCanceled)
				{
					this.Submit();
				}
				UIInput.mKeyboard = null;
				this.isSelected = false;
				this.mCached = string.Empty;
			}
		}
	}

	public new void Submit()
	{
		if (NGUITools.GetActive(this))
		{
			UIInput.current = this;
			this.mValue = this.value;
			EventDelegate.Execute(this.onSubmit);
			this.SaveToPlayerPrefs(this.mValue);
			UIInput.current = null;
		}
	}

	public new void UpdateLabel()
	{
		if (this.newLabel != null)
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			bool isSelected = this.isSelected;
			string value = this.value;
			bool flag = string.IsNullOrEmpty(value) && string.IsNullOrEmpty(Input.compositionString);
			this.newLabel.labelText.color = ((!flag || isSelected) ? this.activeTextColor : this.mDefaultColor);
			string text;
			if (flag)
			{
				text = ((!isSelected) ? this.mDefaultText : string.Empty);
				this.RestoreLabelPivot();
			}
			else
			{
				if (this.inputType == UIInput.InputType.Password)
				{
					text = string.Empty;
					int i = 0;
					int length = value.Length;
					while (i < length)
					{
						text += "*";
						i++;
					}
				}
				else
				{
					text = value;
				}
				int num = Mathf.Min(text.Length, this.cursorPosition);
				string str = text.Substring(0, num);
				if (isSelected)
				{
					str += Input.compositionString;
				}
				text = str + text.Substring(num, text.Length - num);
				if (this.newLabel.labelText.overflowMethod == UILabel.Overflow.ClampContent)
				{
					int num2 = this.newLabel.labelText.CalculateOffsetToFit(text);
					if (num2 == 0)
					{
						UIInput.mDrawStart = 0;
						this.RestoreLabelPivot();
					}
					else if (num < UIInput.mDrawStart)
					{
						UIInput.mDrawStart = num;
						this.SetPivotToLeft();
					}
					else if (num2 < UIInput.mDrawStart)
					{
						UIInput.mDrawStart = num2;
						this.SetPivotToLeft();
					}
					else
					{
						num2 = this.newLabel.labelText.CalculateOffsetToFit(text.Substring(0, num));
						if (num2 > UIInput.mDrawStart)
						{
							UIInput.mDrawStart = num2;
							this.SetPivotToRight();
						}
					}
					if (UIInput.mDrawStart != 0)
					{
						text = text.Substring(UIInput.mDrawStart, text.Length - UIInput.mDrawStart);
						int num3 = text.IndexOf('}', 0, 3);
						if (num3 > -1)
						{
							text = text.Substring(num3 + 1);
						}
						int num4 = text.LastIndexOf('{', text.Length - 1, 3);
						if (num4 > -1)
						{
							text = text.Substring(0, num4);
						}
					}
				}
				else
				{
					UIInput.mDrawStart = 0;
					this.RestoreLabelPivot();
				}
			}
			this.newLabel.text = text;
		}
	}

	protected new void SetPivotToLeft()
	{
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.mPivot);
		pivotOffset.x = 0f;
		this.newLabel.labelText.pivot = NGUIMath.GetPivot(pivotOffset);
	}

	protected new void SetPivotToRight()
	{
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.mPivot);
		pivotOffset.x = 1f;
		this.newLabel.labelText.pivot = NGUIMath.GetPivot(pivotOffset);
	}

	protected new void RestoreLabelPivot()
	{
		if (this.newLabel != null && this.newLabel.labelText.pivot != this.mPivot)
		{
			this.newLabel.labelText.pivot = this.mPivot;
		}
	}

	protected new char Validate(string text, int pos, char ch)
	{
		if (this.validation == UIInput.Validation.None || !base.enabled)
		{
			return ch;
		}
		if (this.validation == UIInput.Validation.Integer)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && pos == 0 && !text.Contains("-"))
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Float)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && pos == 0 && !text.Contains("-"))
			{
				return ch;
			}
			if (ch == '.' && !text.Contains("."))
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Alphanumeric)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return ch;
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Username)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return ch - 'A' + 'a';
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Name)
		{
			char c = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
			char c2 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
			if (ch >= 'a' && ch <= 'z')
			{
				if (c == ' ')
				{
					return ch - 'a' + 'A';
				}
				return ch;
			}
			else if (ch >= 'A' && ch <= 'Z')
			{
				if (c != ' ' && c != '\'')
				{
					return ch - 'A' + 'a';
				}
				return ch;
			}
			else if (ch == '\'')
			{
				if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
				{
					return ch;
				}
			}
			else if (ch == ' ' && c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
			{
				return ch;
			}
		}
		return '\0';
	}

	protected new void ExecuteOnChange()
	{
		if (EventDelegate.IsValid(this.onChange))
		{
			UIInput.current = this;
			EventDelegate.Execute(this.onChange);
			UIInput.current = null;
		}
	}
}
