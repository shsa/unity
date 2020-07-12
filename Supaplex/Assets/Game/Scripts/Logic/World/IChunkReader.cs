namespace Game.Logic.World
{
    public interface IChunkReader
    {
        int version { get; }
        IBlockPos position { get; }
        BlockData GetBlockData(BlockPos pos);
    }
}
