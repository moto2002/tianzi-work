using System;
using System.Text;

public class XMLParser
{
	private char LT = '<';

	private char GT = '>';

	private char SPACE = ' ';

	private char QUOTE = '"';

	private char QUOTE2 = '\'';

	private char SLASH = '/';

	private char QMARK = '?';

	private char EQUALS = '=';

	private char EXCLAMATION = '!';

	private char DASH = '-';

	private char SQR = ']';

	public XMLNode Parse(string content)
	{
		XMLNode xMLNode = new XMLNode();
		xMLNode["_text"] = new StringBuilder();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		StringBuilder stringBuilder3 = new StringBuilder();
		StringBuilder stringBuilder4 = new StringBuilder();
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		XMLNodeList xMLNodeList = new XMLNodeList();
		XMLNode xMLNode2 = xMLNode;
		for (int i = 0; i < content.Length; i++)
		{
			char c = content[i];
			char c2 = '~';
			char c3 = '~';
			char c4 = '~';
			if (i + 1 < content.Length)
			{
				c2 = content[i + 1];
			}
			if (i + 2 < content.Length)
			{
				c3 = content[i + 2];
			}
			if (i > 0)
			{
				c4 = content[i - 1];
			}
			if (flag6)
			{
				if (c == this.QMARK && c2 == this.GT)
				{
					flag6 = false;
					i++;
				}
			}
			else if (!flag5 && c == this.LT && c2 == this.QMARK)
			{
				flag6 = true;
			}
			else if (flag7)
			{
				if (c4 == this.DASH && c == this.DASH && c2 == this.GT)
				{
					flag7 = false;
					i++;
				}
			}
			else if (!flag5 && c == this.LT && c2 == this.EXCLAMATION)
			{
				if (content.Length > i + 9 && content.Substring(i, 9) == "<![CDATA[")
				{
					flag8 = true;
					i += 8;
				}
				else
				{
					flag7 = true;
				}
			}
			else if (flag8)
			{
				if (c == this.SQR && c2 == this.SQR && c3 == this.GT)
				{
					flag8 = false;
					i += 2;
				}
				else
				{
					stringBuilder4.Append(c);
				}
			}
			else if (flag)
			{
				if (flag2)
				{
					if (c == this.SPACE)
					{
						flag2 = false;
					}
					else if (c == this.GT)
					{
						flag2 = false;
						flag = false;
					}
					if (!flag2 && stringBuilder.Length > 0)
					{
						if (stringBuilder[0] == this.SLASH)
						{
							if (stringBuilder4.Length > 0)
							{
							}
							stringBuilder4.Remove(0, stringBuilder4.Length);
							stringBuilder.Remove(0, stringBuilder.Length);
							xMLNode2 = xMLNodeList.Pop();
						}
						else
						{
							if (stringBuilder4.Length > 0)
							{
							}
							stringBuilder4.Remove(0, stringBuilder4.Length);
							string text = stringBuilder.ToString();
							XMLNode xMLNode3 = new XMLNode();
							xMLNode3["_text"] = new StringBuilder();
							xMLNode3["_name"] = text;
							if (xMLNode2[text] == null)
							{
								xMLNode2[text] = new XMLNodeList();
							}
							XMLNodeList xMLNodeList2 = (XMLNodeList)xMLNode2[text];
							xMLNodeList2.Push(xMLNode3);
							xMLNodeList.Push(xMLNode2);
							xMLNode2 = xMLNode3;
							stringBuilder.Remove(0, stringBuilder.Length);
						}
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				else if (!flag5 && c == this.SLASH && c2 == this.GT)
				{
					flag = false;
					flag3 = false;
					flag4 = false;
					if (stringBuilder2.Length > 0)
					{
						if (stringBuilder3.Length > 0)
						{
							xMLNode2[stringBuilder2.Insert(0, '@').ToString()] = stringBuilder3.ToString();
						}
						else
						{
							xMLNode2[stringBuilder2.Insert(0, '@').ToString()] = true;
						}
					}
					i++;
					xMLNode2 = xMLNodeList.Pop();
					stringBuilder2.Remove(0, stringBuilder2.Length);
					stringBuilder3.Remove(0, stringBuilder3.Length);
				}
				else if (!flag5 && c == this.GT)
				{
					flag = false;
					flag3 = false;
					flag4 = false;
					if (stringBuilder2.Length > 0)
					{
						xMLNode2[stringBuilder2.Insert(0, '@').ToString()] = stringBuilder3.ToString();
					}
					stringBuilder2.Remove(0, stringBuilder2.Length);
					stringBuilder3.Remove(0, stringBuilder3.Length);
				}
				else if (flag3)
				{
					if (c == this.SPACE || c == this.EQUALS)
					{
						flag3 = false;
						flag4 = true;
					}
					else
					{
						stringBuilder2.Append(c);
					}
				}
				else if (flag4)
				{
					if (c == this.QUOTE || c == this.QUOTE2)
					{
						if (flag5)
						{
							flag4 = false;
							xMLNode2[stringBuilder2.Insert(0, '@').ToString()] = stringBuilder3.ToString();
							stringBuilder2.Remove(0, stringBuilder2.Length);
							stringBuilder3.Remove(0, stringBuilder3.Length);
							flag5 = false;
						}
						else
						{
							flag5 = true;
						}
					}
					else if (flag5)
					{
						stringBuilder3.Append(c);
					}
					else if (c == this.SPACE)
					{
						flag4 = false;
						xMLNode2[stringBuilder2.Insert(0, '@').ToString()] = stringBuilder3.ToString();
						stringBuilder2.Remove(0, stringBuilder2.Length);
						stringBuilder3.Remove(0, stringBuilder3.Length);
					}
				}
				else if (c != this.SPACE)
				{
					flag3 = true;
					stringBuilder2.Remove(0, stringBuilder2.Length).Append(c);
					stringBuilder3.Remove(0, stringBuilder3.Length);
					flag5 = false;
				}
			}
			else if (c == this.LT)
			{
				flag = true;
				flag2 = true;
			}
			else
			{
				stringBuilder4.Append(c);
			}
		}
		return xMLNode;
	}
}
