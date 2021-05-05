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
    using Meta = Int32;

    public struct ChunkUpdateJob : IJob, IWorldReader
    {
        public int count;

        public int x;
        public int y;
        public int z;

        public BlockDataArray data;
        [ReadOnly] public NativeArray<BlockFlag> data_update;

        [ReadOnly] public BlockDataArray N;
        [ReadOnly] public BlockDataArray NE;
        [ReadOnly] public BlockDataArray E;
        [ReadOnly] public BlockDataArray SE;
        [ReadOnly] public BlockDataArray S;
        [ReadOnly] public BlockDataArray SW;
        [ReadOnly] public BlockDataArray W;
        [ReadOnly] public BlockDataArray NW;

        BlockDataArray GetMap(int x, int y)
        {
            if (x < 0)
            {
                if (y < 0)
                {
                    return SW;
                }
                if (y > 0xF)
                {
                    return NW;
                }
                return W;
            }
            if (x > 0xF)
            {
                if (y < 0)
                {
                    return SE;
                }
                if (y > 0xF)
                {
                    return NE;
                }
                return E;
            }
            if (y < 0)
            {
                return S;
            }
            if (y > 0xF)
            {
                return N;
            }
            return data;
        }

        Block IWorldReader.GetBlock(int x, int y, int z)
        {
            var m = GetMap(x, y);
            return m[Chunk.GetDataIndex(x & 0xF, y & 0xF, z & 0xF)].GetBlock();
        }

        public void Execute()
        {
            for (int _z = (int)WorldLayerEnum.First; _z <= (int)WorldLayerEnum.Last; _z++)
            {
                for (int i = 0; i <= 0xF; i++)
                {
                    for (int j = 0; j <= 0xF; j++)
                    {
                        var index = Chunk.GetDataIndex(i, j, _z);
                        var block = data[index];
                        if (data_update[index].HasFlag(BlockFlag.Update))
                        {
                            block.meta = block.GetBlock().GetMeta(this, i, j, _z);
                            data[index] = block;
                        }
                    }
                }
            }
        }
    }
}
