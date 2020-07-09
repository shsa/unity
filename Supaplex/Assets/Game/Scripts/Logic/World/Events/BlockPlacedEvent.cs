namespace Game.Logic.World
{
    public sealed class BlockPlacedEvent : Event
    {
        public BlockData blockData;
        public BlockPos pos;
        public IWorldAccess world;

        public BlockPlacedEvent()
        {
            pos = new BlockPos();
        }

        public override void Raise()
        {
            var block = blockData.GetBlock();
            block.OnBlockPlaced(this);
        }
    }
}
