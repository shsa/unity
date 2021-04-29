namespace DefenceFactory.Game.World
{
    public interface IWorldGenerator
    {
        BlockType CalcBlockId(BlockPos pos);
    }
}
