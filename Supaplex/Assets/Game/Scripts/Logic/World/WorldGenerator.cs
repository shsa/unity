using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logic.World
{
    public class WorldGenerator : IWorldGenerator
    {
        protected NoiseS3D noiseCore;
        public static int depth = 64;

        public int Seed {
            get {
                return noiseCore.seed;
            }
            set {
                noiseCore.seed = value;
            }
        }

        public WorldGenerator(int seed)
        {
            noiseCore = new NoiseS3D();
            noiseCore.seed = seed;
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

        float GetNoise(BlockPos pos, float scale, Vec3i offset)
        {
            return (float)GetNoise(pos.x * scale + offset.x, pos.y * scale + offset.y, pos.z * scale + offset.z);
        }

        float stoneScale = 0.05f;
        bool CalcBlock(int x, int y, int z, out float k)
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

        float masonryScale = 0.05f;
        Vec3i masonryOffset = new Vec3i(0, 0, 100);
        public virtual BlockType CalcBlockId(BlockPos pos)
        {
            if (CalcBlock(pos.x, pos.y, pos.z, out var k))
            {
                k = GetNoise(pos, masonryScale, masonryOffset);
                if (k > 0.5)
                {
                    return BlockType.Masonry;
                }
                return BlockType.Rock;
            }
            return BlockType.Empty;
        }

        public virtual void Generate(ChunkGenerateEvent e)
        {
            var pos = e.pos;
            var chunk = e.chunk;
            var min = chunk.position.min;
            var max = chunk.position.max;
            for (int z = min.z; z <= max.z; z++)
            {
                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        pos.Set(x, y, z);
                        var blockId = CalcBlockId(pos);
                        chunk.SetBlockData(pos, blockId.GetBlockData());
                    }
                }
            }
        }
    }
}
