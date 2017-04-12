using System;

namespace SceneCore
{
	public class PixRect
	{
		public int x;

		public int y;

		public int width;

		public int height;

		public PixRect()
		{
		}

		public PixRect(int x, int y, int w, int h)
		{
			this.x = x;
			this.y = y;
			this.width = w;
			this.height = h;
		}
	}
}
