using DefenceFactory.Game.World;

namespace DefenceFactory
{
    interface IChunkView : IView
    {
        void Update(Chunk chunk);
    }
}
