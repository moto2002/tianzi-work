using System;

public class Language
{
	public enum LanguageType
	{
		China,
		Russia,
		NorthAmerica,
		Thailand,
		Taiwan,
		Korea,
		Japan
	}

	public static Language.LanguageType LanguageVersion
	{
		get;
		private set;
	}

	public static void Init()
	{
		LogSystem.LogWarning(new object[]
		{
			"current language version : ",
			Language.LanguageVersion.ToString()
		});
	}
}
