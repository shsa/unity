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

        public virtual BlockType CalcBlockId(BlockPos pos)
        {
            if (CalcStone(pos.x, pos.y, pos.z, out var k))
            {
                if (k > 0.8f)
                {
                    //return ObjectType.Stone4x4;
                }
                return BlockType.Stone1;
            }
            return BlockType.Empty;
        }

        public bool IsStone(BlockPos pos)
        {
            return CalcBlockId(pos) == BlockType.Stone;
        }

        public virtual void Generate(ChunkGenerateEvent e)
        {
            var pos = e.pos;
            var chunk = e.chunk;
            var min = chunk.position.min;
            var max = chunk.position.max;
            for (int z = min.z + 1; z < max.z; z++)
            {
                for (int x = min.x + 1; x < max.x; x++)
                {
                    for (int y = min.y + 1; y < max.y; y++)
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
