using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World
{
    public class GameWorld : IDisposable
    {
        Dictionary<BlockPos, Chunk> chunks = new Dictionary<BlockPos, Chunk>();
        Chunk[] chunkCache;
        public IWorldGenerator generator { get; private set; }

        public ConcurrentStack<Chunk> newChunks = new ConcurrentStack<Chunk>();
        public ConcurrentStack<Chunk> destroyedChunks = new ConcurrentStack<Chunk>();

        public GameWorld()
        {
            generator = new WorldGenerator(0);
            chunkCache = new Chunk[16 * 16 * 16];
        }
        
        ~GameWorld()
        {
            Dispose();
        }

        public void Dispose()
        {
            foreach (var chunk in chunks.Values)
            {
                chunk.Dispose();
            }
            chunks.Clear();
        }

        int CacheIndex(int x, int y, int z)
        {
            //return ((chunkPos.x & 0xF) << 8) | ((chunkPos.y & 0xF) << 4) | (chunkPos.z & 0xF);
            return (((x >> 4) & 0x7) << 8) | (((y >> 4) & 0x7) << 4) | ((z >> 4) & 0x7);
        }

        int CacheIndex(in BlockPos pos)
        {
            return CacheIndex(pos.x, pos.y, pos.z);
        }

        public Chunk GetChunk(in BlockPos pos)
        {
            return GetChunk(pos.x, pos.y, pos.z);
        }

        public Chunk GetChunk(int x, int y, int z)
        {
            var index = CacheIndex(x, y, z);
            var chunk = chunkCache[index];
            if (chunk == null || !chunk.Equals(x, y, z))
            {
                return null;
            }

            return chunk;
        }

        public bool TryGetChunk(in BlockPos pos, out Chunk chunk)
        {
            chunk = GetChunk(pos);
            return chunk != default;
        }

        public bool TryGetChunk(int x, int y, int z, out Chunk chunk)
        {
            chunk = GetChunk(x, y, z);
            return chunk != default;
        }

        Chunk CreateChunk(int x, int y, int z)
        {
            var pos = BlockPos.CreateChunkPos(x, y, z);
            if (!chunks.TryGetValue(pos, out var chunk))
            {
                chunk = new Chunk(this, pos);
                for (int i = 0; i < chunk.data.Length; i++)
                {
                    chunk.data[i] = BlockType.Empty.GetBlockData();
                }
                //Generate(chunk);
                chunk.flag = ChunkFlag.Generate;
                chunks.Add(pos, chunk);
            }
            chunkCache[CacheIndex(pos)] = chunk;
            newChunks.Push(chunk);
            return chunk;
        }

        public Chunk GetOrCreateChunk(int x, int y, int z)
        {
            var index = CacheIndex(x, y, z);
            var chunk = chunkCache[index];
            if (chunk == default)
            {
                return CreateChunk(x, y, z);
            }
            if (!chunk.Equals(x, y, z))
            {
                chunk.flag |= ChunkFlag.Destroy;
                return CreateChunk(x, y, z);
            }

            return chunk;
        }

        public Chunk GetOrCreateChunk(in BlockPos pos)
        {
            return GetOrCreateChunk(pos.x, pos.y, pos.z);
        }

        public BlockData GetBlockData(int x, int y, int z)
        {
            if (TryGetChunk(x, y, z, out var chunk))
            {
                return chunk.GetBlockData(x, y, z);
            }
            return BlockType.None.GetBlockData();
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
                tempPos.Set(blockPos, d.GetVector2());
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
            return;
            var pos = new BlockPos();
            for (int x = chunk.min.x; x <= chunk.max.x; x++)
            {
                for (int y = chunk.min.y; y <= chunk.max.y; y++)
                {
                    pos.Set(x, y, chunk.min.z);
                    chunk.SetBlockData(pos, generator.CalcBlockId(pos.x, pos.y, pos.z).GetBlockData());
                }
            }

            for (int x = chunk.min.x; x <= chunk.max.x; x++)
            {
                for (int y = chunk.min.y; y <= chunk.max.y; y++)
                {
                    pos.Set(x, y, chunk.min.z);
                    var blockId = chunk.GetBlockData(pos).GetBlockId();
                    chunk.SetBlockData(pos, blockId.GetBlockData(CalcNeighbors(pos)));
                }
            }
        }

        public DirectionSet CalcNeighbors(in BlockPos blockPos)
        {
            var meta = DirectionSet.None;
            var blockId = GetBlockData(blockPos).GetBlockId();
            var t = new BlockPos();
            for (var i = DirectionEnum.First; i <= DirectionEnum.Last; i++)
            {
                t.Set(blockPos, i.GetVector2());
                if (GetBlockData(t).GetBlockId() == blockId)
                {
                    meta |= i.Set();
                }
            }
            return meta;
        }
    }
}
