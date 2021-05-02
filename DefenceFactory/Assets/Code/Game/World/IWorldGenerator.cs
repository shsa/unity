namespace DefenceFactory.Game.World
{
    public interface IWorldGenerator
    {
        BlockType CalcBlockId(int x, int y, int z);
    }
}
