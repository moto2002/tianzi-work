using System;

namespace SceneCore
{
	public class Node
	{
		public Node[] child = new Node[2];

		public PixRect pixRect;

		public Image img;

		private bool isLeaf()
		{
			return this.child[0] == null || this.child[1] == null;
		}

		public Node Insert(Image img, bool handed)
		{
			int num = 0;
			int num2 = 0;
			if (handed)
			{
				num = 0;
				num2 = 1;
			}
			else if (!handed)
			{
				num = 1;
				num2 = 0;
			}
			if (!this.isLeaf())
			{
				Node node = this.child[num].Insert(img, handed);
				if (node != null)
				{
					return node;
				}
				return this.child[num2].Insert(img, handed);
			}
			else
			{
				if (this.img != null)
				{
					return null;
				}
				if (this.pixRect.width < img.width || this.pixRect.height < img.height)
				{
					return null;
				}
				if (this.pixRect.width == img.width && this.pixRect.height == img.height)
				{
					this.img = img;
					return this;
				}
				this.child[num] = new Node();
				this.child[num2] = new Node();
				int num3 = this.pixRect.width - img.width;
				int num4 = this.pixRect.height - img.height;
				if (num3 > num4)
				{
					this.child[num].pixRect = new PixRect(this.pixRect.x, this.pixRect.y, img.width, this.pixRect.height);
					this.child[num2].pixRect = new PixRect(this.pixRect.x + img.width, this.pixRect.y, this.pixRect.width - img.width, this.pixRect.height);
				}
				else
				{
					this.child[num].pixRect = new PixRect(this.pixRect.x, this.pixRect.y, this.pixRect.width, img.height);
					this.child[num2].pixRect = new PixRect(this.pixRect.x, this.pixRect.y + img.height, this.pixRect.width, this.pixRect.height - img.height);
				}
				return this.child[num].Insert(img, handed);
			}
		}
	}
}
