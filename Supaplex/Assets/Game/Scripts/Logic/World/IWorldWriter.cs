namespace Game.Logic.World
{
    public interface IWorldWriter : IWorldReader
    {
        void SetBlockData(BlockPos pos, BlockData blockData);
    }
}
