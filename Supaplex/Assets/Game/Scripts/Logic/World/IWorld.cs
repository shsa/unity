namespace Game.Logic.World
{
    public interface IWorld
    {
        Chunk GetChunk(ChunkPos pos);
    }
}
