using System;
using System.Linq;
using UnityEngine;

namespace Game.View
{
	public class ViewFrustum
	{
		Plane[] planes;

		public ViewFrustum()
		{
			planes = new Plane[6];
		}

		public void Update(Camera camera)
		{
			GeometryUtility.CalculateFrustumPlanes(camera, planes);
		}

		public bool Intersect(Bounds bounds)
		{
			return GeometryUtility.TestPlanesAABB(planes, bounds);
		}
	}
}
