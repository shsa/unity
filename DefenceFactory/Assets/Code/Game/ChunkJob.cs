using DefenceFactory.Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;

namespace DefenceFactory.Game
{
    using BlockDataArray = NativeArray<BlockData>;

    public struct ChunkJob : IJob
    {
        public int x;
        public int y;
        public int z;
        public ChunkFlag flag;

        public BlockDataArray data;

        [ReadOnly]
        public BlockDataArray N;
        [ReadOnly]
        public BlockDataArray NE;
        [ReadOnly]
        public BlockDataArray E;
        [ReadOnly]
        public BlockDataArray SE;
        [ReadOnly]
        public BlockDataArray S;
        [ReadOnly]
        public BlockDataArray SW;
        [ReadOnly]
        public BlockDataArray W;
        [ReadOnly]
        public BlockDataArray NW;

        int GetIndex(DirectionEnum dir)
        {
            ref var v = ref dir.GetVector2();
            return (v.Y + 1) * 3 + (v.X + 1);
        }

        public void Execute()
        {
            BlockDataArray[] map = new BlockDataArray[9];
            map[GetIndex(DirectionEnum.N)] = N;
        }
    }
}
