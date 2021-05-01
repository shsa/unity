using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World
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
            //return ((chunkPos.x & 0xF) << 8) | ((chunkPos.y & 0xF) << 4) | (chunkPos.z & 0xF);
            return ((chunkPos.x & 0x7) << 8) | ((chunkPos.y & 0x7) << 4) | (chunkPos.z & 0x7);
        }

        int CacheIndex(in BlockPos blockPos)
        {
            return (((blockPos.x >> 4) & 0x7) << 8) | (((blockPos.y >> 4) & 0x7) << 4) | ((blockPos.z >> 4) & 0x7);
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

        public Chunk GetChunk(in BlockPos blockPos)
        {
            var index = CacheIndex(blockPos);
            var chunk = chunkCache[index];
            if (chunk == null || !chunk.Position.Equals(blockPos))
            {
                return null;
            }

            return chunk;
        }

        public bool TryGetChunk(in ChunkPos chunkPos, out Chunk chunk)
        {
            chunk = GetChunk(chunkPos);
            return chunk != default;
        }

        public bool TryGetChunk(in BlockPos blockPos, out Chunk chunk)
        {
            chunk = GetChunk(blockPos);
            return chunk != default;
        }

        Chunk CreateChunk(in ChunkPos chunkPos, in int index)
        {
            if (!chunks.TryGetValue(chunkPos, out var chunk))
            {
                chunk = new Chunk(this, chunkPos);
                Generate(chunk);
                chunks.Add(new ChunkPos(chunkPos.x, chunkPos.y, chunkPos.z), chunk);
                chunk.IsChanged = true;
            }
            chunk.IsDestroyed = false;
            newChunks.Push(chunk);
            chunkCache[index] = chunk;
            return chunk;
        }

        public Chunk GetOrCreateChunk(in ChunkPos chunkPos)
        {
            var index = CacheIndex(chunkPos);
            var chunk = chunkCache[index];
            if (chunk == default)
            {
                return CreateChunk(chunkPos, index);
            }
            if (!chunk.Position.Equals(chunkPos))
            {
                chunk.IsDestroyed = true;
                return CreateChunk(chunkPos, index);
            }

            return chunk;
        }

        public Chunk GetOrCreateChunk(in BlockPos blockPos)
        {
            var index = CacheIndex(blockPos);
            var chunk = chunkCache[index];
            if (chunk == null)
            {
                return CreateChunk(blockPos.ChunkPos, index);
            }
            if (!chunk.Position.Equals(blockPos))
            {
                chunk.IsDestroyed = true;
                return CreateChunk(blockPos.ChunkPos, index);
            }

            return chunk;
        }

        public BlockData GetBlockData(in BlockPos pos)
        {
            if (TryGetChunk(pos, out var chunk))
            {
                return chunk.GetBlockData(pos);
            }
            return BlockType.None.GetBlockData();
        }

        public void SetBlockData(in BlockPos blockPos, BlockData value)
        {
            var chunk = GetOrCreateChunk(blockPos);
            chunk.SetBlockData(blockPos, value);
            var tempPos = new BlockPos();
            for (var d = DirectionEnum.First; d <= DirectionEnum.Last; d++)
            {
                tempPos.Set(blockPos, d.GetDirection());
                UpdateBlock(tempPos);
            }
        }

        public void UpdateBlock(in BlockPos pos)
        {
            if (TryGetChunk(pos, out var chunk))
            {
                chunk.updateBlock.Push(pos);
            }
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
