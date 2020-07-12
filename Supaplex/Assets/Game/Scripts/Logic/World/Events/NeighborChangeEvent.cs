namespace Game.Logic.World
{
    public sealed class NeighborChangeEvent : ChunkEvent
    {
        public BlockData neighborBlockData;
        public BlockPos neighborBlockPos = new BlockPos();

        public BlockData blockData;
        public BlockPos blockPos = new BlockPos();
        public BlockPos pos = new BlockPos();
        public IWorldAccess world;

        public override void Execute()
        {
            var block = blockData.GetBlock();
            block.OnNeighborChange(this);
        }
    }
}
