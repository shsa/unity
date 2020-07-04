using UnityEngine;
using Game.Logic;

namespace Game.View
{
    public class RenderChunkProvider
    {
        RenderChunk[] renderChunks;
        World world;

        public RenderChunkProvider(World world)
        {
            this.world = world;
            renderChunks = new RenderChunk[4096]; // 16 * 16 * 16
        }

        public RenderChunk this[ChunkPosition pos] {
            get {
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

        public CubeSideSet this[Vector3Int pos]
        {
            get {
                var renderChunk = this[ChunkPosition.From(pos)];
                return renderChunk[pos];
            }
        }
    }
}
