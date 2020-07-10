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
        public BlockPos position { get; private set; }
        BlockData[] data;

        public event EventHandler<ChunkChangeEvent> cubeChanged;

        public Chunk(WorldProvider world, BlockPos pos)
        {
            this.world = world;
            this.position = new BlockPos(pos);
            data = new BlockData[16 * 16 * 16];
        }

        public BlockData GetBlockData(BlockPos pos)
        {
            return data[pos.GetIndex()];
        }

        public void SetBlockData(BlockPos pos, BlockData value)
        {
            var index = pos.GetIndex();
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

        public void Generate(WorldGenerator generator)
        {
            var ee = Events.blockPlaced.Create();
            var min = position.min;
            var max = position.max;
            var pos = new BlockPos();
            for (int z = min.z + 1; z < max.z; z++)
            {
                for (int x = min.x + 1; x < max.x; x++)
                {
                    for (int y = min.y + 1; y < max.y; y++)
                    {
                        pos.Set(x, y, z);
                        var blockId = generator.CalcBlockId(pos);
                        var e = ee.Add();
                        e.world = world;
                        e.pos.Set(pos);
                        e.blockData = blockId.GetBlockData(BlockState.None);
                        SetBlockDataInternal(pos, e.blockData);
                    }
                }
            }
            Events.blockPlaced.Enqueue(ee);
        }

        bool SetBlockDataInternal(BlockPos pos, BlockData value)
        {
            var index = pos.GetIndex();
            var oldValue = data[index];
            if (oldValue != value)
            {
                data[index] = value;
                return true;
            }
            return false;
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
    }
}
