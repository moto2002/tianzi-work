using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WellFired.Shared
{
	public static class StringExtensions
	{
		[DebuggerHidden]
		public static IEnumerable<string> Split(this string str, Func<char, bool> controller)
		{
			StringExtensions.<Split>c__Iterator0 <Split>c__Iterator = new StringExtensions.<Split>c__Iterator0();
			<Split>c__Iterator.str = str;
			<Split>c__Iterator.controller = controller;
			<Split>c__Iterator.<$>str = str;
			<Split>c__Iterator.<$>controller = controller;
			StringExtensions.<Split>c__Iterator0 expr_23 = <Split>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static IEnumerable<string> SplitCommandLine(string commandLine)
		{
			bool inQuotes = false;
			return from arg in commandLine.Split(delegate(char c)
			{
				if (c == '"')
				{
					inQuotes = !inQuotes;
				}
				return !inQuotes && c == ' ';
			})
			select arg.Trim().TrimMatchingQuotes('"') into arg
			where !string.IsNullOrEmpty(arg)
			select arg;
		}

		public static string TrimMatchingQuotes(this string input, char quote)
		{
			if (input.Length >= 2 && input[0] == quote && input[input.Length - 1] == quote)
			{
				return input.Substring(1, input.Length - 2);
			}
			return input;
		}
	}
}
