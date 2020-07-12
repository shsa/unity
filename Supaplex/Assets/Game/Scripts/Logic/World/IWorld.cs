namespace Game.Logic.World
{
    public interface IWorld : IWorldReader
    {
        Chunk GetChunk(BlockPos pos);
    }
}
