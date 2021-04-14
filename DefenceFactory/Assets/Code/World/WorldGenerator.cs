using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.World
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

        float GetNoise(BlockPos pos, float scale, Int3 offset)
        {
            return (float)GetNoise(pos.x * scale + offset.X, pos.y * scale + offset.Y, pos.z * scale + offset.Z);
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
        Int3 masonryOffset = new Int3(0, 0, 100);
        float cobblestoneScale = 0.05f;
        Int3 cobblestoneOffset = new Int3(100, 0, 0);
        public virtual BlockType CalcBlockId(BlockPos pos)
        {
            if (CalcBlock(pos.x, pos.y, pos.z, out var k))
            {
                //k = GetNoise(pos, masonryScale, masonryOffset);
                //if (k > 0.5)
                //{
                //    return BlockType.Stone;
                //}

                //k = GetNoise(pos, cobblestoneScale, cobblestoneOffset);
                //if (k > 0.5)
                //{
                //    return BlockType.Stone;
                //}
                return BlockType.Stone;
            }
            return BlockType.Empty;
        }
    }
}
