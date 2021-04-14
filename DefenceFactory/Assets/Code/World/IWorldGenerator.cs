namespace DefenceFactory.World
{
    public interface IWorldGenerator
    {
        BlockType CalcBlockId(BlockPos pos);
    }
}
