using System;

namespace SceneCore
{
	public class ProbeResult
	{
		public int width;

		public int height;

		public Node root;

		public bool fitsInMaxSize;

		public float efficiency;

		public float squareness;

		public void Set(int width, int height, Node root, bool fits, float efficiency, float squareness)
		{
			this.width = width;
			this.height = height;
			this.root = root;
			this.fitsInMaxSize = fits;
			this.efficiency = efficiency;
			this.squareness = squareness;
		}

		public float GetScore()
		{
			float num = (!this.fitsInMaxSize) ? 0f : 1f;
			return this.squareness + 2f * this.efficiency + num;
		}
	}
}
