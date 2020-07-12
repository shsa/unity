namespace Game.Logic.World
{
    public sealed class BlockPlacedEvent : ChunkEvent
    {
        public BlockData blockData;
        public BlockPos pos = new BlockPos();
        public IWorldAccess world;

        public override void Execute()
        {
            var block = blockData.GetBlock();
            block.OnBlockPlaced(this);
        }
    }
}
