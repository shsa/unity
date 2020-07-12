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
        static Vec3i[] neighbors;

        public WorldProvider world { get; private set; }
        public BlockPos position { get; private set; }
        public int version = 0;
        BlockData[] data;
        ChunkEventManager eventManager;
        ChunkEventProvider<BlockChangeEvent> blockChange;
        ChunkEventProvider<ChunkGenerateEvent> chunkGenerate;

        public Chunk(WorldProvider world, BlockPos pos)
        {
            eventManager = new ChunkLogicEventManager();
            blockChange = new ChunkEventProvider<BlockChangeEvent>(eventManager);
            chunkGenerate = new ChunkEventProvider<ChunkGenerateEvent>(eventManager);

            this.world = world;
            position = new BlockPos(pos);
            data = new BlockData[4096]; // 16 * 16 * 16
        }

        public BlockData GetBlockData(BlockPos pos)
        {
            return data[pos.GetIndex()];
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

        public void SetBlockData(BlockPos pos, BlockData value)
        {
            var index = pos.GetIndex();
            var oldValue = data[index];
            if (oldValue != value)
            {
                data[index] = value;
                version++;

                //if (oldValue.GetBlockId() != value.GetBlockId())
                {
                    if (eventManager.SetBit(pos))
                    {
                        var e = blockChange.Create();

                        for (int i = 0; i < neighbors.Length; i++)
                        {
                            e.pos.Set(pos, neighbors[i]);
                            var chunk = world.GetChunkOrNull(e.pos);
                            if (chunk != null)
                            {
                                chunk.RegisterBlockChange(e.pos);
                            }
                        }

                        e.blockPos.Set(pos);
                        e.blockData = GetBlockData(pos);
                        e.world = world;
                        e.Publish();
                    }
                }
            }
        }

        public void RegisterBlockChange(BlockPos pos)
        {
            var blockData = GetBlockData(pos);
            switch (blockData.GetBlockId())
            {
                case BlockType.None:
                    break;
                default:
                    if (eventManager.SetBit(pos))
                    {
                        var e = blockChange.Create();
                        e.blockPos.Set(pos);
                        e.blockData = blockData;
                        e.world = world;
                        e.Publish();
                    }
                    break;
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
            SetBlockData(pos, blockData);
        }

        #endregion

        static Chunk()
        {
            neighbors = new Vec3i[26];
            var index = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x != 0 || y != 0 || z != 0)
                        {
                            neighbors[index++] = new Vec3i(x, y, z);
                        }
                    }
                }
            }
        }
    }
}
