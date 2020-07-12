namespace Game.Logic.World
{
    public interface IWorldReader
    {
        BlockData GetBlockData(BlockPos pos);
    }
}
