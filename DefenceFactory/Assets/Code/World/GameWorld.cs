using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.World
{
    public class GameWorld
    {
        Dictionary<ChunkPos, Chunk> chunks = new Dictionary<ChunkPos, Chunk>();
        Chunk[] chunkCache;

        public GameWorld()
        {
            chunkCache = new Chunk[16 * 16 * 16];
        }

        int CacheIndex(in ChunkPos chunkPos)
        {
            return (((chunkPos.x >> 4) & 0xF) << 8) | (((chunkPos.y >> 4) & 0xF) << 4) | ((chunkPos.z >> 4) & 0xF);
        }

        public Chunk GetChunk(in ChunkPos chunkPos)
        {
            var index = CacheIndex(chunkPos);
            var chunk = chunkCache[index];
            if (chunk == null || !chunk.Position.Equals(chunkPos))
            {
                return null;
            }

            return chunk;
        }

        public Chunk GetOrCreateChunk(in ChunkPos chunkPos)
        {
            var index = CacheIndex(chunkPos);
            var chunk = chunkCache[index];
            if (chunk == null || !chunk.Position.Equals(chunkPos))
            {
                chunk = new Chunk(this, chunkPos);
                chunkCache[index] = chunk;
            }

            return chunk;
        }

        void Generate(in Chunk chunk)
        {
            var pos = new BlockPos();
            var min = chunk.Position.MinBlockPos();
            var max = chunk.Position.MaxBlockPos();
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    pos.Set(x, y, min.z);
                    chunk.SetBlockData(pos, BlockType.Stone.GetBlockData());
                }
            }

        }
    }
}
