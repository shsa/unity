using Game.Logic.World;
using System.Collections.Generic;
using UnityEngine;

namespace Game.View.World
{
    public sealed class RenderCalcMeshEvent : ChunkEvent
    {
        public RenderChunk render;
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();
        public List<Vector2> uv = new List<Vector2>();

        public override void Execute()
        {
            render.mesh.Clear();
            render.mesh.SetVertices(vertices);
            render.mesh.SetTriangles(triangles, 0);
            render.mesh.SetUVs(0, uv);
            render.mesh.RecalculateNormals();
            render.mesh.Optimize();

            render.meshVersion = render.visibilityVersion;
            render.calculating = false;
        }
    }
}
