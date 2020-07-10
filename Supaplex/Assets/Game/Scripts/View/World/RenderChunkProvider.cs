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

        public RenderChunk GetChunk(BlockPos pos)
        {
            if (pos.z < 0)
            {
                return null;
            }

            var index = CalcIndex(pos);
            var renderChunk = renderChunks[index];
            if (renderChunk == null || !renderChunk.chunk.position.ChunkEquals(pos))
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

        public RenderChunk GetChunk(BlockPos playerPos, BlockPos pos)
        {
            if ((pos.z >> 4) > size.z)
            {
                return null;
            }
            if (Mathf.Abs((pos.x >> 4) - (playerPos.x >> 4)) > size.x)
            {
                return null;
            }
            if (Mathf.Abs((pos.y >> 4) - (playerPos.y >> 4)) > size.y)
            {
                return null;
            }

            return GetChunk(pos);
        }

        int CalcIndex(BlockPos pos)
        {
            return ((pos.x >> 4) & 0xF) << 8 | ((pos.y >> 4) & 0xF) << 4 | ((pos.z >> 4) & 0xF);
        }
    }
}
