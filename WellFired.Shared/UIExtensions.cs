using System;
using UnityEngine;

namespace WellFired.Shared
{
	public class UIExtensions
	{
		public static Texture2D CreateTexture(int width, int height, Color colour)
		{
			return UIExtensions.CreateTexture(width, height, colour, true);
		}

		public static Texture2D CreateTexture(int width, int height, Color colour, bool dontSave)
		{
			Color[] array = new Color[width * height];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = colour;
			}
			Texture2D texture2D = new Texture2D(width, height);
			texture2D.wrapMode = TextureWrapMode.Repeat;
			texture2D.SetPixels(array);
			texture2D.Apply();
			texture2D.hideFlags = ((!dontSave) ? HideFlags.None : HideFlags.HideAndDontSave);
			return texture2D;
		}

		public static Texture2D Create1x1Texture(Color colour)
		{
			return UIExtensions.Create1x1Texture(colour, true);
		}

		public static Texture2D Create1x1Texture(Color colour, bool dontSave)
		{
			return UIExtensions.CreateTexture(1, 1, colour, dontSave);
		}
	}
}
