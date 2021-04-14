using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.World
{
    public sealed class Chunk
    {
        public GameWorld World { get; private set; }
        public ChunkPos Position { get; private set; }
        BlockData[] data;

        public Chunk(GameWorld world, ChunkPos pos)
        {
            World = world;
            Position = pos;
            data = new BlockData[256];
        }

        public BlockData GetBlockData(BlockPos pos)
        {
            return data[pos.GetChunkIndex()];
        }

        public void SetBlockData(BlockPos pos, BlockData value)
        {
            var index = pos.GetChunkIndex();
            var oldValue = data[index];
            if (oldValue != value)
            {
                data[index] = value;
            }
        }
    }
}
