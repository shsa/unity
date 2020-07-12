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

    public class Chunk : IChunkWriter
    {
        public WorldProvider world { get; private set; }
        public BlockPos position { get; private set; }
        public int version = 0;
        BlockData[] data;
        ChunkEventManager eventManager;
        ChunkEventProvider<BlockPlacedEvent> blockPlaced;
        ChunkEventProvider<ChunkGenerateEvent> chunkGenerate;

        public event EventHandler<ChunkChangeEvent> cubeChanged;

        public Chunk(WorldProvider world, BlockPos pos)
        {
            eventManager = new ChunkLogicEventManager();
            blockPlaced = new ChunkEventProvider<BlockPlacedEvent>(eventManager);
            chunkGenerate = new ChunkEventProvider<ChunkGenerateEvent>(eventManager);

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

        public void Generate(IWorldGenerator generator)
        {
            var e = chunkGenerate.Create();
            e.chunk = this;
            e.pos.Set(position);
            e.world = this.world;
            e.generator = generator;
            e.Publish();
        }

        bool SetBlockDataInternal(BlockPos pos, BlockData value)
        {
            var index = pos.GetIndex();
            var oldValue = data[index];
            if (oldValue != value)
            {
                data[index] = value;
                version++;
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

        #region IChunkReader

        int IChunkReader.version {
            get {
                return version;
            }
        }

        IBlockPos IChunkReader.position {
            get {
                return position;
            }
        }

        BlockData IChunkReader.GetBlockData(BlockPos pos)
        {
            return data[pos.GetIndex()];
        }

        #endregion

        #region IChunkWriter

        void IChunkWriter.SetBlockData(BlockPos pos, BlockData blockData)
        {
            if (SetBlockDataInternal(pos, blockData))
            {
                var e = blockPlaced.Create();
                e.pos.Set(pos);
                e.blockData = blockData;
                e.world = this.world;
                e.Publish();
            }
        }

        #endregion
    }
}
