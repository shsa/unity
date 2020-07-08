using System;

namespace Game.Logic.World
{
    public class ChunkChangeEvent
    {
        public BlockPos position { get; private set; }
        public FacingSet sides { get; private set; }
        public BlockData data { get; private set; }

        public ChunkChangeEvent(BlockPos position, FacingSet sets, BlockData data)
        {
            this.position = position;
            this.sides = sets;
            this.data = data;
        }

        public void Set(BlockPos position, FacingSet sides, BlockData data)
        {
            this.position = position;
            this.sides = sides;
            this.data = data;
        }
    }

    public class Chunk
    {
        public WorldProvider world { get; private set; }
        public ChunkPos position { get; private set; }
        BlockData[] data;

        public event EventHandler<ChunkChangeEvent> cubeChanged;

        public Chunk(WorldProvider world, ChunkPos pos)
        {
            this.world = world;
            this.position = new ChunkPos(pos);
            data = new BlockData[16 * 16 * 16];
            //Array.Clear(data, 0, data.Length);
        }

        public BlockData GetBlockData(BlockPos pos)
        {
            return data[GetBlockIndex(pos)];
        }

        public void SetBlockData(BlockPos pos, BlockData value)
        {
            var index = GetBlockIndex(pos);
            var oldValue = data[index];
            if (oldValue != value)
            {
                data[index] = value;
                if (cubeChanged != null)
                {
                    var e = new ChunkChangeEvent(pos, FacingSet.All, value);
                    cubeChanged(this, e);
                    for (var side = Facing.First; side <= Facing.Last; side++)
                    {
                        e.Set(pos.Offset(side), side.Opposite().ToSet(), BlockData.None);
                        cubeChanged(this, e);
                    }
                }
            }
        }

        public bool IsEmpty(BlockPos pos)
        {
            switch (GetBlockData(pos).GetBlockId())
            {
                case BlockType.Empty:
                case BlockType.None:
                    return true;
                default:
                    return false;
            }
        }

        public static int GetBlockIndex(BlockPos pos)
        {
            return ((pos.y & 0xF) << 8) | ((pos.x & 0xF) << 4) | (pos.z & 0xF);
        }
    }
}
