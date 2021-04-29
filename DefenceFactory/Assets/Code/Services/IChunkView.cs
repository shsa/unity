using DefenceFactory.Game.World;

namespace DefenceFactory
{
    interface IChunkView : IView
    {
        void UpdateBlocks(Chunk chunk);
    }
}
