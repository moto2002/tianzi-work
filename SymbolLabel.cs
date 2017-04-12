using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class SymbolLabel : MonoBehaviour
{
	private List<string> m_Symbols = new List<string>
	{
		"{00}",
		"{01}",
		"{02}",
		"{03}",
		"{04}",
		"{05}",
		"{06}",
		"{07}",
		"{08}",
		"{09}",
		"{10}",
		"{11}",
		"{12}",
		"{13}",
		"{14}",
		"{15}",
		"{16}",
		"{17}",
		"{18}",
		"{19}",
		"{20}",
		"{21}",
		"{22}",
		"{23}",
		"{24}",
		"{25}",
		"{26}",
		"{27}",
		"{28}",
		"{29}",
		"{30}",
		"{31}",
		"{32}",
		"{33}",
		"{34}",
		"{35}",
		"{36}",
		"{37}",
		"{38}",
		"{39}",
		"{40}",
		"{41}",
		"{42}",
		"{43}",
		"{44}",
		"{45}",
		"{46}",
		"{47}",
		"{48}",
		"{49}",
		"{50}",
		"{51}",
		"{52}",
		"{53}",
		"{54}",
		"{55}",
		"{56}",
		"{57}",
		"{58}",
		"{59}",
		"{60}",
		"{61}",
		"{62}",
		"{63}",
		"{64}",
		"{65}",
		"{66}",
		"{67}",
		"{68}",
		"{69}",
		"{70}",
		"{71}",
		"{72}",
		"{73}",
		"{74}",
		"{75}",
		"{76}",
		"{77}",
		"{78}",
		"{79}",
		"{80}",
		"{81}",
		"{82}",
		"{83}",
		"{84}",
		"{85}",
		"{86}",
		"{87}",
		"{88}",
		"{89}",
		"{90}",
		"{91}",
		"{92}",
		"{93}",
		"{94}",
		"{95}",
		"{96}",
		"{97}"
	};

	private string m_Text;

	private string m_realText;

	private Vector2 m_textSize;

	public UIFont uifont;

	public int fontSize = 26;

	public int symbolSize = 40;

	public int spacingY;

	public int width = 100;

	public int depth;

	public int maxLine;

	public int textHeight = 24;

	public UILabel.Overflow overflowMethod = UILabel.Overflow.ResizeHeight;

	public NGUIText.Alignment alignment = NGUIText.Alignment.Left;

	public UIWidget.Pivot pivot;

	private UILabel m_TextLabel;

	private UILabel m_SymbolLabel;

	private MatchCollection m_matchs;

	private MatchCollection m_spaceMatchs;

	private List<Match> m_realMatchs;

	private StringBuilder sString = new StringBuilder();

	public string realText
	{
		get
		{
			return this.m_realText;
		}
	}

	public Vector2 textSize
	{
		get
		{
			return this.m_textSize;
		}
	}

	public int height
	{
		get
		{
			return this.m_TextLabel.height;
		}
	}

	public UILabel labelText
	{
		get
		{
			return this.m_TextLabel;
		}
	}

	public UILabel labelSymbol
	{
		get
		{
			return this.m_SymbolLabel;
		}
	}

	public string text
	{
		get
		{
			return this.m_Text;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				this.m_Text = string.Empty;
				this.m_TextLabel.text = null;
				this.m_SymbolLabel.text = null;
				this.m_realMatchs.Clear();
				return;
			}
			this.m_realMatchs.Clear();
			this.m_Text = value;
			string text = this.m_TextLabel.processedText;
			this.m_TextLabel.UpdateNGUIText();
			NGUIText.fontSize = this.fontSize;
			NGUIText.maxLines = this.maxLine;
			NGUIText.rectWidth = this.width;
			NGUIText.rectHeight = 100000;
			if (this.overflowMethod == UILabel.Overflow.ResizeHeight)
			{
				text = this.m_Text;
			}
			else
			{
				NGUIText.WrapSymbolText(this.m_Text, out text, this.symbolSize);
			}
			this.m_textSize = NGUIText.CalculatePrintedSize(value);
			this.m_realText = NGUIText.StripSymbols(text);
			this.m_matchs = Regex.Matches(this.m_realText, "\\{\\w\\w\\}");
			this.m_spaceMatchs = Regex.Matches(this.m_realText, " ");
			if (this.sString.Length > 0)
			{
				this.sString.Remove(0, this.sString.Length);
			}
			if (this.m_matchs.Count > 0)
			{
				for (int i = 0; i < this.m_matchs.Count; i++)
				{
					Match match = this.m_matchs[i];
					if (this.m_Symbols.IndexOf(match.Value) > -1)
					{
						this.m_realMatchs.Add(match);
						this.sString.Append(match.Value);
					}
				}
			}
			this.m_TextLabel.text = value;
			this.m_SymbolLabel.text = this.sString.ToString();
			this.m_SymbolLabel.width = this.m_TextLabel.width;
			this.m_SymbolLabel.height = this.m_TextLabel.height;
			this.m_SymbolLabel.MarkAsChanged();
		}
	}

	private void Awake()
	{
		this.m_realMatchs = new List<Match>();
		this.m_TextLabel = NGUITools.AddChild<UILabel>(base.gameObject);
		this.m_TextLabel.name = "textLabel";
		if (Config.SnailFont != null)
		{
			this.m_TextLabel.trueTypeFont = Config.SnailFont.dynamicFont;
		}
		this.m_TextLabel.spacingY = this.spacingY;
		this.m_TextLabel.fontSize = this.fontSize;
		this.m_TextLabel.overflowMethod = this.overflowMethod;
		this.m_TextLabel.alignment = this.alignment;
		this.m_TextLabel.pivot = this.pivot;
		this.m_TextLabel.width = this.width;
		this.m_TextLabel.depth = this.depth;
		this.m_TextLabel.transform.localPosition = Vector3.zero;
		this.m_TextLabel.SetSymbolOffset(new Action(this.SymbolOffset));
		if (this.overflowMethod == UILabel.Overflow.ClampContent)
		{
			this.m_TextLabel.height = this.textHeight;
			this.m_TextLabel.maxLineCount = this.maxLine;
		}
		this.m_SymbolLabel = NGUITools.AddChild<UILabel>(base.gameObject);
		this.m_SymbolLabel.name = "symbolLabel";
		this.m_SymbolLabel.bitmapFont = this.uifont;
		this.m_SymbolLabel.fontSize = this.symbolSize;
		this.m_SymbolLabel.overflowMethod = this.overflowMethod;
		this.m_SymbolLabel.alignment = this.alignment;
		this.m_SymbolLabel.pivot = this.pivot;
		this.m_SymbolLabel.depth = this.depth + 1;
		Vector3 zero = Vector3.zero;
		zero.x = 0f;
		zero.y = -3f;
		zero.z = 0f;
		this.m_SymbolLabel.transform.localPosition = zero;
		this.m_SymbolLabel.SetSymbolOffset(new Action(this.SymbolOffset));
		UIWidget arg_213_0 = this.m_SymbolLabel;
		int height = 10;
		this.m_SymbolLabel.height = height;
		arg_213_0.width = height;
	}

	public void SetWidth(int width)
	{
		this.width = width;
		this.m_TextLabel.width = width;
		this.m_SymbolLabel.width = width;
	}

	private void SymbolOffset()
	{
		BetterList<Vector3> verts = this.m_TextLabel.geometry.verts;
		BetterList<Vector3> verts2 = this.m_SymbolLabel.geometry.verts;
		Vector3 zero = Vector3.zero;
		zero.x = 0f;
		zero.y = 0f;
		Vector3 b = zero;
		if (verts.size > 0 && verts2.size > 0)
		{
			for (int i = 0; i < this.m_realMatchs.Count; i++)
			{
				Match match = this.m_realMatchs[i];
				int num = this.GetIndex(match.Index) * 4;
				int num2 = num + (match.Length - 1) * 4 + 3;
				int num3 = i * 4;
				if (num3 + 3 >= verts2.buffer.Length)
				{
					break;
				}
				float num4 = Mathf.Abs(verts2.buffer[num3].x - verts2.buffer[num3 + 3].x);
				float x;
				if (verts.buffer[num].y == verts.buffer[num2].y)
				{
					float num5 = Mathf.Abs(verts.buffer[num].x - verts.buffer[num2].x);
					x = (num5 - num4) / 2f;
				}
				else
				{
					x = 1f;
				}
				b.x = x;
				Vector2 pivotOffset = this.m_TextLabel.pivotOffset;
				float num6 = Mathf.Lerp(0f, (float)(-(float)NGUIText.rectWidth), pivotOffset.x);
				float num7 = Mathf.Lerp((float)NGUIText.rectHeight, 0f, pivotOffset.y) + Mathf.Lerp(this.m_TextLabel.printedSize.y - (float)NGUIText.rectHeight, 0f, pivotOffset.y);
				num6 = Mathf.Round(num6);
				num7 = Mathf.Round(num7);
				Vector3 b2 = verts.buffer[num] - verts2.buffer[num3];
				verts2.buffer[num3] = verts.buffer[num] + b;
				Vector3[] expr_23D_cp_0 = verts2.buffer;
				int expr_23D_cp_1 = num3;
				expr_23D_cp_0[expr_23D_cp_1].x = expr_23D_cp_0[expr_23D_cp_1].x - num6;
				Vector3[] arg_258_0 = verts2.buffer;
				int expr_253 = num3++;
				arg_258_0[expr_253].y = arg_258_0[expr_253].y - num7;
				verts2.buffer[num3] = verts2.buffer[num3] + b2 + b;
				Vector3[] expr_2A9_cp_0 = verts2.buffer;
				int expr_2A9_cp_1 = num3;
				expr_2A9_cp_0[expr_2A9_cp_1].x = expr_2A9_cp_0[expr_2A9_cp_1].x - num6;
				Vector3[] arg_2C4_0 = verts2.buffer;
				int expr_2BF = num3++;
				arg_2C4_0[expr_2BF].y = arg_2C4_0[expr_2BF].y - num7;
				verts2.buffer[num3] = verts2.buffer[num3] + b2 + b;
				Vector3[] expr_315_cp_0 = verts2.buffer;
				int expr_315_cp_1 = num3;
				expr_315_cp_0[expr_315_cp_1].x = expr_315_cp_0[expr_315_cp_1].x - num6;
				Vector3[] arg_330_0 = verts2.buffer;
				int expr_32B = num3++;
				arg_330_0[expr_32B].y = arg_330_0[expr_32B].y - num7;
				verts2.buffer[num3] = verts2.buffer[num3] + b2 + b;
				Vector3[] expr_381_cp_0 = verts2.buffer;
				int expr_381_cp_1 = num3;
				expr_381_cp_0[expr_381_cp_1].x = expr_381_cp_0[expr_381_cp_1].x - num6;
				Vector3[] arg_39C_0 = verts2.buffer;
				int expr_397 = num3++;
				arg_39C_0[expr_397].y = arg_39C_0[expr_397].y - num7;
				for (int j = 0; j < match.Length; j++)
				{
					if (this.m_TextLabel.geometry.cols.size >= num + 4)
					{
						this.m_TextLabel.geometry.cols[num++] = Color.clear;
						this.m_TextLabel.geometry.cols[num++] = Color.clear;
						this.m_TextLabel.geometry.cols[num++] = Color.clear;
						this.m_TextLabel.geometry.cols[num++] = Color.clear;
					}
				}
			}
		}
	}

	private int GetIndex(int itemIndex)
	{
		int num = 0;
		for (int i = 0; i < this.m_spaceMatchs.Count; i++)
		{
			Match match = this.m_spaceMatchs[i];
			if (match.Index < itemIndex)
			{
				num++;
			}
		}
		return itemIndex - num;
	}
}
