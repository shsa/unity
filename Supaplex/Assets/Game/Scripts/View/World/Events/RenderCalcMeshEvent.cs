using Game.Logic.World;

namespace Game.View.World
{
    public sealed class RenderCalcMeshEvent : ChunkEvent
    {
        public RenderChunk render;

        public override void Execute()
        {
            render.mesh.SetVertices(render.vertices);
            render.mesh.SetTriangles(render.triangles, 0);
            render.mesh.SetUVs(0, render.uv);
            render.mesh.RecalculateNormals();
            render.mesh.Optimize();

            render.meshVersion = render.visibilityVersion;
            render.calculating = false;
        }
    }
}
