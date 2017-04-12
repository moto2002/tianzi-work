using System;
using System.Collections.Generic;

public class TableParser
{
	public class TableStore
	{
		private string[] header;

		private string[] desc;

		private List<string[]> contents = new List<string[]>();

		public void SetHeader(string[] strHeaderGroup)
		{
			this.header = strHeaderGroup;
		}

		public void SetDesc(string[] strdescGroup)
		{
			this.desc = strdescGroup;
		}

		public void SetContent(string[] strContentGroup)
		{
			this.contents.Add(strContentGroup);
		}

		public int GetRows()
		{
			return this.contents.Count;
		}

		public int GetHeaderIndex(string strHeader)
		{
			for (int i = 0; i < this.header.Length; i++)
			{
				if (this.header[i].Equals(strHeader))
				{
					return i;
				}
			}
			return -1;
		}

		public int GetHeaderDesc(string strHeader)
		{
			int i = 0;
			while (i < this.header.Length)
			{
				if (this.header[i].Equals(strHeader))
				{
					if (i < this.desc.Length && !string.IsNullOrEmpty(this.desc[i]))
					{
						return int.Parse(this.desc[i]);
					}
					break;
				}
				else
				{
					i++;
				}
			}
			return -1;
		}

		public string GetText(string strHeader, int iRow)
		{
			int headerIndex = this.GetHeaderIndex(strHeader);
			return this.GetText(headerIndex, iRow);
		}

		public string GetText(int iHeader, int iRow)
		{
			if (iRow < 0 || iRow >= this.contents.Count)
			{
				return string.Empty;
			}
			string[] array = this.contents[iRow];
			if (iHeader < 0 || iHeader >= array.Length)
			{
				return string.Empty;
			}
			return array[iHeader];
		}
	}

	private static char[] strSplit = new char[]
	{
		'\r',
		'\n'
	};

	private static char chSpace = '\t';

	public static TableParser.TableStore Parse(string content)
	{
		TableParser.TableStore tableStore = new TableParser.TableStore();
		string[] array = content.Split(TableParser.strSplit);
		int num = array.Length;
		if (num > 2)
		{
			string[] array2 = array[0].Split(new char[]
			{
				TableParser.chSpace
			});
			tableStore.SetHeader(array2);
			array2 = array[1].Split(new char[]
			{
				TableParser.chSpace
			});
			tableStore.SetDesc(array2);
			for (int i = 2; i < num; i++)
			{
				array2 = array[i].Split(new char[]
				{
					TableParser.chSpace
				});
				tableStore.SetContent(array2);
			}
		}
		else
		{
			LogSystem.LogWarning(new object[]
			{
				"Config Parse Error"
			});
		}
		return tableStore;
	}
}
