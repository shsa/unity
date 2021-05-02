using DefenceFactory.Game.World;
using Leopotam.Ecs.Types;
using Unity.Collections;

namespace DefenceFactory.Ecs
{
    struct ThreadChunk
    {
        public int x;
        public int y;
        public int z;
        public ChunkFlag flag;

        public NativeArray<BlockData> data;

        [ReadOnly]
        public NativeArray<BlockData> N;
        [ReadOnly]
        public NativeArray<BlockData> NE;
        [ReadOnly]
        public NativeArray<BlockData> E;
        [ReadOnly]
        public NativeArray<BlockData> SE;
        [ReadOnly]
        public NativeArray<BlockData> S;
        [ReadOnly]
        public NativeArray<BlockData> SW;
        [ReadOnly]
        public NativeArray<BlockData> W;
        [ReadOnly]
        public NativeArray<BlockData> NW;
    }
}