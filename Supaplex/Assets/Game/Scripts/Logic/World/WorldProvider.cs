using System.Collections.Generic;

namespace Game.Logic.World
{
    public class WorldProvider : IWorld, IWorldAccess
    {
        public static int depth = 64;

        WorldGenerator generator;
        Dictionary<ChunkPos, Chunk> chunks;
        Chunk[] chunkCash;

        public WorldProvider(int seed)
        {
            generator = new WorldGenerator(seed, depth);
            chunks = new Dictionary<ChunkPos, Chunk>();
            chunkCash = new Chunk[16 * 16 * 16];
        }

        public Chunk GetChunkOrNull(ChunkPos pos)
        {
            var index = ((pos.x & 0xF) << 8) | ((pos.y & 0xF) << 4) | (pos.z & 0xF);
            var chunk = chunkCash[index];
            if (chunk == null || chunk.position != pos)
            {
                if (!chunks.TryGetValue(pos, out chunk))
                {
                    return null;
                }
                chunkCash[index] = chunk;
            }
            return chunk;
        }

        public Chunk GetChunk(ChunkPos pos)
        {
            var index = ((pos.x & 0xF) << 8) | ((pos.y & 0xF) << 4) | (pos.z & 0xF);
            var chunk = chunkCash[index];
            if (chunk == null || chunk.position != pos)
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

        ChunkPos chunkPos = new ChunkPos(0, 0, 0);
        public BlockData GetBlockData(BlockPos pos)
        {
            chunkPos.Set(pos);
            var chunk = GetChunk(chunkPos);
            return chunk.GetBlockData(pos);
        }

        BlockData IWorldAccess.GetBlockData(BlockPos pos)
        {
            chunkPos.Set(pos);
            var chunk = GetChunkOrNull(chunkPos);
            if (chunk == null)
            {
                return BlockData.None;
            }
            return chunk.GetBlockData(pos);
        }

        public void SetBlockData(BlockPos pos, BlockData value)
        {
            chunkPos.Set(pos);
            var chunk = GetChunk(chunkPos);
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
