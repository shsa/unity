namespace Game.Logic.World
{
    public interface IWorldAccess
    {
        BlockData GetBlockData(BlockPos pos);
        void SetBlockData(BlockPos pos, BlockData blockData);
    }
}
