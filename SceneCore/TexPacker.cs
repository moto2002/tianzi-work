using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneCore
{
	public class TexPacker
	{
		private class ImgIDComparer : IComparer<Image>
		{
			public int Compare(Image x, Image y)
			{
				if (x.id > y.id)
				{
					return 1;
				}
				if (x.id == y.id)
				{
					return 0;
				}
				return -1;
			}
		}

		private class ImageHeightComparer : IComparer<Image>
		{
			public int Compare(Image x, Image y)
			{
				if (x.height > y.height)
				{
					return -1;
				}
				if (x.height == y.height)
				{
					return 0;
				}
				return 1;
			}
		}

		private class ImageWidthComparer : IComparer<Image>
		{
			public int Compare(Image x, Image y)
			{
				if (x.width > y.width)
				{
					return -1;
				}
				if (x.width == y.width)
				{
					return 0;
				}
				return 1;
			}
		}

		private class ImageAreaComparer : IComparer<Image>
		{
			public int Compare(Image x, Image y)
			{
				int num = x.width * x.height;
				int num2 = y.width * y.height;
				if (num > num2)
				{
					return -1;
				}
				if (num == num2)
				{
					return 0;
				}
				return 1;
			}
		}

		public int atlasCount;

		private void GetExtent(Node r, ref int x, ref int y)
		{
			if (r.img != null)
			{
				if (r.pixRect.x + r.img.width > x)
				{
					x = r.pixRect.x + r.img.width;
				}
				if (r.pixRect.y + r.img.height > y)
				{
					y = r.pixRect.y + r.img.height;
				}
			}
			if (r.child[0] != null)
			{
				this.GetExtent(r.child[0], ref x, ref y);
			}
			if (r.child[1] != null)
			{
				this.GetExtent(r.child[1], ref x, ref y);
			}
		}

		private bool Probe(Image[] imgsToAdd, int idealAtlasW, int idealAtlasH, float imgArea, int maxAtlasDimension, ProbeResult pr)
		{
			Node node = new Node();
			node.pixRect = new PixRect(0, 0, idealAtlasW, idealAtlasH);
			for (int i = 0; i < imgsToAdd.Length; i++)
			{
				if (node.Insert(imgsToAdd[i], false) == null)
				{
					return false;
				}
				if (i == imgsToAdd.Length - 1)
				{
					int num = 0;
					int num2 = 0;
					this.GetExtent(node, ref num, ref num2);
					float efficiency = 1f - ((float)(num * num2) - imgArea) / (float)(num * num2);
					float squareness;
					if (num < num2)
					{
						squareness = (float)num / (float)num2;
					}
					else
					{
						squareness = (float)num2 / (float)num;
					}
					bool fits = num <= maxAtlasDimension && num2 <= maxAtlasDimension;
					pr.Set(num, num2, node, fits, efficiency, squareness);
					return true;
				}
			}
			return false;
		}

		public Image[] Pack(List<Vector2> imgWidthHeights, int maxDimension, int padding)
		{
			this.atlasCount = 1;
			List<Image> list = new List<Image>();
			int num = 0;
			int num2 = 0;
			Image[] array = new Image[imgWidthHeights.Count];
			int num3 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				Image image = array[i] = new Image(i, (int)imgWidthHeights[i].x, (int)imgWidthHeights[i].y, padding);
				num = Mathf.Max(num, image.width);
				num2 = Mathf.Max(num2, image.height);
			}
			if ((float)num2 / (float)num > 2f)
			{
				Array.Sort<Image>(array, new TexPacker.ImageHeightComparer());
			}
			else if ((double)((float)num2 / (float)num) < 0.5)
			{
				Array.Sort<Image>(array, new TexPacker.ImageWidthComparer());
			}
			else
			{
				Array.Sort<Image>(array, new TexPacker.ImageAreaComparer());
			}
			List<Image> list2 = new List<Image>();
			Node node = new Node();
			node.pixRect = new PixRect(0, 0, maxDimension, maxDimension);
			for (int j = 0; j < array.Length; j++)
			{
				Image image2 = array[j];
				if (node.Insert(array[j], false) == null)
				{
					node = new Node();
					node.pixRect = new PixRect(0, 0, maxDimension, maxDimension);
					node.Insert(array[j], false);
					num3++;
					this.atlasCount++;
					int num4 = 0;
					int num5 = 0;
					Image[] rects = this.GetRects(list2.ToArray(), maxDimension, padding, out num4, out num5);
					list.AddRange(rects);
					list2.Clear();
				}
				image2.atlasIndex = num3;
				list2.Add(image2);
			}
			int num6 = 0;
			int num7 = 0;
			Image[] rects2 = this.GetRects(list2.ToArray(), maxDimension, padding, out num6, out num7);
			list.AddRange(rects2);
			return list.ToArray();
		}

		public Image[] GetRects(Image[] imgsToAdd, int maxDimension, int padding, out int outW, out int outH)
		{
			ProbeResult probeResult = null;
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < imgsToAdd.Length; i++)
			{
				Image image = imgsToAdd[i];
				num += (float)(image.width * image.height);
				num2 = Mathf.Max(num2, image.width);
				num3 = Mathf.Max(num3, image.height);
			}
			int num4 = (int)Mathf.Sqrt(num);
			int num5 = num4;
			int num6 = num4;
			if (num2 > num4)
			{
				num5 = num2;
				num6 = Mathf.Max(Mathf.CeilToInt(num / (float)num2), num3);
			}
			if (num3 > num4)
			{
				num5 = Mathf.Max(Mathf.CeilToInt(num / (float)num3), num2);
				num6 = num3;
			}
			if (num5 == 0)
			{
				num5 = 1;
			}
			if (num6 == 0)
			{
				num6 = 1;
			}
			int num7 = (int)((float)num5 * 0.15f);
			int num8 = (int)((float)num6 * 0.15f);
			if (num7 == 0)
			{
			}
			if (num8 == 0)
			{
			}
			ProbeResult probeResult2 = new ProbeResult();
			if (this.Probe(imgsToAdd, maxDimension, maxDimension, num, maxDimension, probeResult2))
			{
				probeResult = probeResult2;
			}
			outW = 0;
			outH = 0;
			if (probeResult == null)
			{
				return null;
			}
			outW = probeResult.width;
			outH = probeResult.height;
			List<Image> list = new List<Image>();
			TexPacker.flattenTree(probeResult.root, list);
			list.Sort(new TexPacker.ImgIDComparer());
			if (list.Count != imgsToAdd.Length)
			{
				LogSystem.LogWarning(new object[]
				{
					"Result images not the same lentgh as source"
				});
			}
			return list.ToArray();
		}

		private static void printTree(Node r, string spc)
		{
			if (r.child[0] != null)
			{
				TexPacker.printTree(r.child[0], spc + "  ");
			}
			if (r.child[1] != null)
			{
				TexPacker.printTree(r.child[1], spc + "  ");
			}
		}

		private static void flattenTree(Node r, List<Image> putHere)
		{
			if (r.img != null)
			{
				r.img.x = r.pixRect.x;
				r.img.y = r.pixRect.y;
				putHere.Add(r.img);
			}
			if (r.child[0] != null)
			{
				TexPacker.flattenTree(r.child[0], putHere);
			}
			if (r.child[1] != null)
			{
				TexPacker.flattenTree(r.child[1], putHere);
			}
		}
	}
}
