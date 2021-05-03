using DefenceFactory.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace DefenceFactory.Game.Jobs
{
    using BlockDataArray = NativeArray<BlockData>;

    public struct ChunkGenerateJob : IJob
    {
        public int x;
        public int y;
        public int z;

        public int seed;

        public BlockDataArray data;

        float CalcNoise(int x, int y, float scaleX, float scaleY)
        {
            var i = 1000000.0f + (seed + x * scaleX) / 0x10;
            var j = 1000000.0f + (seed + y * scaleY) / 0x10;
            return Mathf.PerlinNoise(i, j);
        }

        BlockType CalcBlockId(int x, int y, int z)
        {
            var k = CalcNoise(x, y, 1.0f, 1.0f);
            if (k > 0.5)
            {
                return BlockType.Stone;
            }

            return BlockType.Empty;
        }

        public void Execute()
        {
            for (int i = 0; i <= 0xF; i++)
            {
                for (int j = 0; j <= 0xF; j++)
                {
                    var index = Chunk.GetDataIndex(x + i, y + j, z);
                    data[index] = new BlockData
                    {
                        id = CalcBlockId(x + i, y + j, z),
                        meta = 0
                    };
                }
            }
        }
    }

    public class ChunkGenerateContainer : IDisposable
    {
        public static Unity.Mathematics.Random rnd = new Unity.Mathematics.Random(1);

        public Chunk chunk;
        public ChunkGenerateJob job;
        public JobHandle handle;

        ~ChunkGenerateContainer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (job.data != default)
            {
                job.data.Dispose();
            }
        }
    }
}
