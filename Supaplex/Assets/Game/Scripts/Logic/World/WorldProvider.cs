using System.Collections.Generic;

namespace Game.Logic.World
{
    public class WorldProvider : IWorld
    {
        public static int depth = 64;

        NoiseS3D noiseCore;
        Dictionary<ChunkPos, Chunk> chunks;
        Chunk[] chunkCash;

        public WorldProvider(int seed)
        {
            noiseCore = new NoiseS3D();
            noiseCore.seed = seed;
            chunks = new Dictionary<ChunkPos, Chunk>();
            chunkCash = new Chunk[16 * 16 * 16];
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

        /// <summary>
        /// return value x = {0, 1}
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        double GetNoise(double x, double y, double z)
        {
            var k = (noiseCore.Noise(x, y, z) + 1) * 0.5; // {0-1}
            return k;
        }

        float stoneScale = 0.05f;
        bool CalcStone(int x, int y, int z, out float k)
        {
            k = (float)GetNoise(x * stoneScale, y * stoneScale, z * stoneScale);
            var f = 0.7f;
            var n = k - z * 2f / depth;
            if (n < f)
            {
                return true;
            }
            return false;
        }

        BlockType CalcBlockId(BlockPos pos)
        {
            if (CalcStone(pos.x, pos.y, pos.z, out var k))
            {
                if (k > 0.8f)
                {
                    //return ObjectType.Stone4x4;
                }
                return BlockType.Stone;
            }
            return BlockType.Empty;
        }

        public bool IsStone(BlockPos pos)
        {
            return CalcBlockId(pos) == BlockType.Stone;
        }

        void Generate(Chunk chunk)
        {
            var min = chunk.position.min;
            var max = chunk.position.max;
            var pos = new BlockPos();
            for (int z = min.z; z <= max.z; z++)
            {
                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        pos.Set(x, y, z);
                        var blockId = CalcBlockId(pos);
                        var block = blockId.GetBlock();
                        chunk.SetBlockData(pos, blockId.GetBlockData(block.GetDefaultState()));
                    }
                }
            }
        }

        ChunkPos chunkPos = new ChunkPos(0, 0, 0);
        public BlockData GetBlockData(BlockPos pos)
        {
            chunkPos.Set(pos);
            var chunk = GetChunk(chunkPos);
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
