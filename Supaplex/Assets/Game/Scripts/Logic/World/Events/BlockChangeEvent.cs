namespace Game.Logic.World
{
    public sealed class BlockChangeEvent : ChunkEvent
    {
        public BlockData blockData;
        public BlockPos blockPos = new BlockPos();
        public BlockPos pos = new BlockPos();
        public IWorldAccess world;

        public override void Execute()
        {
            var block = blockData.GetBlock();
            block.OnBlockChange(this);
        }
    }
}
