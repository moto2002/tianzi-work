using System;
using System.Text.RegularExpressions;
using UnityEngine;

public static class AccountExtensions
{
	private static string[] names = new string[]
	{
		"Penetrator",
		"Kenneth",
		"Palette",
		"Mark",
		"Parmesian",
		"Spitfire",
		"Eva",
		"Alpro",
		"Redtail",
		"Infiltrator",
		"RearEnd",
		"White Mice",
		"Unconventional",
		"Penis Man",
		"The Bin Man",
		"The Evacuator",
		"Giant Lump",
		"Ejaculatory",
		"The Sperminator",
		"Jester",
		"Maverick",
		"Ice Man",
		"Slider",
		"Ironside",
		"T-Bag",
		"Sundown",
		"Max",
		"Wizard",
		"Merlin",
		"BARRY",
		"Vitamin C",
		"Zoolander",
		"Coaster",
		"Plank",
		"Neo1988",
		"n3o",
		"ETC",
		"Read End Connection",
		"Blind",
		"Z - Ray",
		"Egg-man",
		"AXIX",
		"Sergio Georgini"
	};

	public static bool IsValidEmailAddress(string email)
	{
		if (string.IsNullOrEmpty(email))
		{
			return false;
		}
		string pattern = "^(?!\\.)(\"([^\"\\r\\\\]|\\\\[\"\\r\\\\])*\"|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\\.)\\.)*)(?<!\\.)@[a-z0-9][\\w\\.-]*[a-z0-9]\\.[a-z][a-z\\.]*[a-z]$";
		Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
		return regex.IsMatch(email);
	}

	public static string GetRandomName()
	{
		return AccountExtensions.names[UnityEngine.Random.Range(0, AccountExtensions.names.Length - 1)];
	}
}
