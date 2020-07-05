using UnityEngine;
using Game.Logic;

namespace Game.View
{
    public class RenderChunkProvider
    {
        RenderChunk[] renderChunks;
        World world;
        Vector3Int size;
        Vector3Int sizeSqr;

        public RenderChunkProvider(World world, Vector3Int size)
        {
            this.world = world;
            renderChunks = new RenderChunk[4096]; // 16 * 16 * 16
            this.size = size;
            sizeSqr = size * size;
        }

        public RenderChunk GetChunk(Vector3 playerPos, ChunkPosition pos)
        {
            if (pos.z < 0)
            {
                return null;
            }
            if (pos.z > size.z)
            {
                return null;
            }
            var p0 = ChunkPosition.From(Vector3Int.FloorToInt(playerPos));
            if (Mathf.Abs(pos.x - p0.x) > size.x)
            {
                return null;
            }
            if (Mathf.Abs(pos.y - p0.y) > size.y)
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
                renderChunk.CalcVisibility();
                renderChunks[index] = renderChunk;
            }
            return renderChunk;
        }

        public RenderChunk this[ChunkPosition pos] {
            get {
                if (pos.z < 0)
                {
                    return null;
                }
                if (pos.z > size.z)
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
                    renderChunk.CalcVisibility();
                    renderChunks[index] = renderChunk;
                }
                return renderChunk;
            }
        }

        int CalcIndex(ChunkPosition pos)
        {
            return (pos.x & 0xF) << 8 | (pos.y & 0xF) << 4 | (pos.z & 0xF);
        }

        public FacingSet this[Vector3Int pos]
        {
            get {
                var renderChunk = this[ChunkPosition.From(pos)];
                return renderChunk[pos];
            }
        }
    }
}
