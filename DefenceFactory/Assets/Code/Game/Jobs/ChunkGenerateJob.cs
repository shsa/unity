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

        float scale;
        float CalcNoise(int x, int y)
        {
            var i = seed + x * scale;
            var j = seed + y * scale;

            return Mathf.PerlinNoise(i, j);
        }

        BlockType CalcMainBlockId(int x, int y, int z)
        {
            if (y > 0)
            {
                return BlockType.Empty;
            }

            var k = CalcNoise(x, y);

            var t = Math.Min(1.0f, Math.Abs(y) / 10.0f);
            var k2 = Vector2.Lerp(new Vector2(1.0f, 0), new Vector2(0.5f, 0), t).x;

            if (k < k2)
            {
                return BlockType.Stone;
            }

            return BlockType.None;
        }

        BlockType CalcBackgroundBlockId(int x, int y, int z)
        {
            if (y > 0)
            {
                return BlockType.Empty;
            }

            var k = CalcNoise(x, y);

            var t = Math.Min(1.0f, Math.Abs(y) / 10.0f);
            var k2 = Vector2.Lerp(new Vector2(0.0f, 0), new Vector2(0.5f, 0), t).x;

            if (k > k2)
            {
                return BlockType.Cobblestone;
            }

            return BlockType.Sand;
        }

        public void Execute()
        {
            scale = 1.0f / 0xF;
            seed = 10000;
            var _z = (int)WorldLayerEnum.Main;
            for (int i = 0; i <= 0xF; i++)
            {
                for (int j = 0; j <= 0xF; j++)
                {
                    var index = Chunk.GetDataIndex(x + i, y + j, _z);
                    data[index] = new BlockData
                    {
                        id = CalcMainBlockId(x + i, y + j, _z),
                        meta = 0
                    };
                }
            }

            scale = 1.0f / 0x8;
            seed = 10000;
            _z = (int)WorldLayerEnum.Background;
            for (int i = 0; i <= 0xF; i++)
            {
                for (int j = 0; j <= 0xF; j++)
                {
                    var index = Chunk.GetDataIndex(x + i, y + j, _z);
                    data[index] = new BlockData
                    {
                        id = CalcBackgroundBlockId(x + i, y + j, _z),
                        meta = 0
                    };
                }
            }
        }
    }

    public class ChunkGenerateContainer : IDisposable
    {
        public Chunk chunk;
        public BlockDataArray data;
        public JobHandle handle;

        ~ChunkGenerateContainer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (data != default)
            {
                data.Dispose();
            }
        }
    }
}
