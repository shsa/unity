using UnityEngine;
using Game.Logic;
using Game.Logic.World;

namespace Game.View.World
{
    public class RenderChunkProvider
    {
        RenderChunk[] renderChunks;
        IWorld world;
        Vector3Int size;
        Vector3Int sizeSqr;

        public RenderChunkProvider(IWorld world, Vector3Int size)
        {
            this.world = world;
            renderChunks = new RenderChunk[4096]; // 16 * 16 * 16
            this.size = size;
            sizeSqr = size * size;
        }

        public RenderChunk GetChunk(ChunkPos pos)
        {
            if (pos.z < 0)
            {
                return null;
            }

            var index = CalcIndex(pos);
            var renderChunk = renderChunks[index];
            if (renderChunk == null || renderChunk.chunk.position != pos)
            {
                if (renderChunk != null)
                {
                    renderChunk.Dispose();
                }
                var chunk = world.GetChunk(pos);
                renderChunk = new RenderChunk(chunk);
                renderChunks[index] = renderChunk;
            }
            return renderChunk;
        }

        public RenderChunk GetChunk(Vector3 playerPos, ChunkPos pos)
        {
            if (pos.z > size.z)
            {
                return null;
            }
            var p0 = ChunkPos.From(Vector3Int.FloorToInt(playerPos));
            if (Mathf.Abs(pos.x - p0.x) > size.x)
            {
                return null;
            }
            if (Mathf.Abs(pos.y - p0.y) > size.y)
            {
                return null;
            }

            return GetChunk(pos);
        }

        int CalcIndex(ChunkPos pos)
        {
            return (pos.x & 0xF) << 8 | (pos.y & 0xF) << 4 | (pos.z & 0xF);
        }
    }
}
