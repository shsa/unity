namespace Game.Logic.World
{
    public interface IChunkWriter : IChunkReader
    {
        void SetBlockData(BlockPos pos, BlockData blockData);
    }
}
