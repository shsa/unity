namespace Game.Logic.World
{
    public interface IWorldGenerator
    {
        BlockType CalcBlockId(BlockPos pos);
        void Generate(ChunkGenerateEvent e);
    }
}
