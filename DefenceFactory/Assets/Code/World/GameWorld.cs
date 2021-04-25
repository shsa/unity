using System;
using System.Collections.Concurrent;
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
        IWorldGenerator generator;

        public ConcurrentStack<Chunk> newChunks = new ConcurrentStack<Chunk>();
        public ConcurrentStack<Chunk> destroyedChunks = new ConcurrentStack<Chunk>();

        public GameWorld()
        {
            generator = new WorldGenerator(0);
            chunkCache = new Chunk[16 * 16 * 16];
        }

        int CacheIndex(in ChunkPos chunkPos)
        {
            return ((chunkPos.x & 0xF) << 8) | ((chunkPos.y & 0xF) << 4) | (chunkPos.z & 0xF);
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

        Chunk CreateChunk(in ChunkPos chunkPos, in int index)
        {
            var chunk = new Chunk(this, chunkPos);
            chunkCache[index] = chunk;
            Generate(chunk);
            newChunks.Push(chunk);
            return chunk;
        }

        public Chunk GetOrCreateChunk(in ChunkPos chunkPos)
        {
            var index = CacheIndex(chunkPos);
            var chunk = chunkCache[index];
            if (chunk == null)
            {
                return CreateChunk(chunkPos, index);
            }
            if (!chunk.Position.Equals(chunkPos))
            {
                //destroyedChunks.Push(chunk);
                chunk.IsDestroyed = true;
                return CreateChunk(chunkPos, index);
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
                    chunk.SetBlockData(pos, generator.CalcBlockId(pos).GetBlockData());
                }
            }

        }
    }
}
