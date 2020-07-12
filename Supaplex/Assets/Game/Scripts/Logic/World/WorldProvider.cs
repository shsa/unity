using System.Collections.Generic;

namespace Game.Logic.World
{
    public class WorldProvider : IWorld, IWorldAccess, IWorldWriter
    {
        IWorldGenerator generator;
        Dictionary<BlockPos, Chunk> chunks;
        Chunk[] chunkCash;

        public WorldProvider(int seed)
        {
            generator = new WorldGeneratorTest(seed);
            chunks = new Dictionary<BlockPos, Chunk>();
            chunkCash = new Chunk[16 * 16 * 16];
        }

        public Chunk GetChunkOrNull(BlockPos pos)
        {
            var index = (((pos.x >> 4) & 0xF) << 8) | (((pos.y >> 4) & 0xF) << 4) | ((pos.z >> 4) & 0xF);
            var chunk = chunkCash[index];
            if (chunk == null || !chunk.position.ChunkEquals(pos))
            {
                if (!chunks.TryGetValue(pos, out chunk))
                {
                    return null;
                }
                chunkCash[index] = chunk;
            }
            return chunk;
        }

        public Chunk GetChunk(BlockPos pos)
        {
            var index = (((pos.x >> 4) & 0xF) << 8) | (((pos.y >> 4) & 0xF) << 4) | ((pos.z >> 4) & 0xF);
            var chunk = chunkCash[index];
            if (chunk == null || !chunk.position.ChunkEquals(pos))
            {
                if (!chunks.TryGetValue(pos, out chunk))
                {
                    chunk = new Chunk(this, pos);
                    chunks[chunk.position] = chunk;
                    Generate(chunk);
                }
                chunkCash[index] = chunk;
            }
            return chunk;
        }

        void Generate(Chunk chunk)
        {
            chunk.Generate(generator);
        }

        public BlockData GetBlockData(BlockPos pos)
        {
            var chunk = GetChunk(pos);
            return chunk.GetBlockData(pos);
        }

        BlockData IWorldReader.GetBlockData(BlockPos pos)
        {
            var chunk = GetChunkOrNull(pos);
            if (chunk == null)
            {
                return BlockData.None;
            }
            return chunk.GetBlockData(pos);
        }

        BlockData IWorldAccess.GetBlockData(BlockPos pos)
        {
            var chunk = GetChunkOrNull(pos);
            if (chunk == null)
            {
                return BlockData.None;
            }
            return chunk.GetBlockData(pos);
        }

        void IWorldAccess.SetBlockData(BlockPos pos, BlockData value)
        {
            var chunk = GetChunkOrNull(pos);
            if (chunk != null)
            {
                chunk.SetBlockData(pos, value);
            }
        }

        public void SetBlockData(BlockPos pos, BlockData value)
        {
            var chunk = GetChunk(pos);
            chunk.SetBlockData(pos, value);
        }

        public bool IsEmpty(BlockPos pos)
        {
            switch (GetBlockData(pos).GetBlockId())
            {
                case BlockType.Empty:
                case BlockType.None:
                    return true;
                default:
                    return false;
            }
        }
    }
}
