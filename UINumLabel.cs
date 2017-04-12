using System;
using System.Text.RegularExpressions;
using UnityEngine;

[AddComponentMenu("NGUI/Custom/NumLabel")]
public class UINumLabel : UILabel
{
	private const string SupportPattern = "^[0-9|+|-|=|*|/|%|.|:|{|}|<|>|~|^|`|#| ]+$";

	private const string checkNum = "\\d\\d*";

	private MatchCollection matchs;

	private bool CanWrite(string str)
	{
		return Regex.IsMatch(str, "^[0-9|+|-|=|*|/|%|.|:|{|}|<|>|~|^|`|#| ]+$");
	}

	public void SpecialText(string content)
	{
		base.text = content;
	}

	public void WriteConstomNumText(string str, string flag, string postfix = "")
	{
		this.matchs = Regex.Matches(str, "\\d\\d*");
		if (this.matchs.Count > 0)
		{
			int count = this.matchs.Count;
			string[] array = new string[count];
			for (int i = 0; i < count; i++)
			{
				int length = this.matchs[i].Value.Length;
				for (int j = 0; j < length; j++)
				{
					string[] expr_62_cp_0 = array;
					int expr_62_cp_1 = i;
					expr_62_cp_0[expr_62_cp_1] += DelegateProxy.StringBuilder(new object[]
					{
						flag,
						this.matchs[i].Value[j]
					});
				}
			}
			string text = string.Empty;
			int num = 0;
			for (int k = 0; k < count; k++)
			{
				if (k > 0)
				{
					num += array[k - 1].Length - this.matchs[k - 1].Value.Length;
					text = text.Remove(this.matchs[k].Index + num, this.matchs[k].Value.Length);
					text = text.Insert(this.matchs[k].Index + num, array[k]);
				}
				else
				{
					text = str.Remove(this.matchs[k].Index, this.matchs[k].Value.Length);
					text = text.Insert(this.matchs[k].Index, array[k]);
				}
			}
			for (int l = 0; l < text.Length; l++)
			{
				if (text[l] == '+' || text[l] == '-' || text[l] == '=')
				{
					text = text.Insert(l, flag);
					int length2 = flag.Length;
					l += length2;
				}
			}
			text = DelegateProxy.StringBuilder(new object[]
			{
				text,
				postfix
			});
			base.text = text;
		}
	}
}
