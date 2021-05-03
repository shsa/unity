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

    public struct ChunkUpdateJob : IJob
    {
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

        BlockDataArray GetMap(int x, int y, int z)
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

        public void Execute()
        {
            for (int i = 0; i <= 0xF; i++)
            {
                for (int j = 0; j <= 0xF; j++)
                {
                    var index = Chunk.GetDataIndex(i, j, 0);
                    var block = data[index];
                    if ((data_update[index] & BlockFlag.Update) == BlockFlag.Update)
                    {
                        var dirs = DirectionSet.None;
                        for (var d = DirectionEnum.First; d <= DirectionEnum.Last; d++)
                        {
                            ref var dir = ref d.GetVector2();
                            int _x = i + dir.X;
                            int _y = j + dir.Y;
                            var m = GetMap(_x, _y, 0);
                            var neighbourId = m[Chunk.GetDataIndex(_x & 0xF, _y & 0xF, 0)].id;
                            if (neighbourId == block.id)
                            {
                                dirs |= d.Set();
                            }
                        }
                        block.meta = (Meta)dirs;
                        data[index] = block;
                    }
                }
            }
        }
    }
}
