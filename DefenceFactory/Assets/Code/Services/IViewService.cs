using DefenceFactory.Game.World;
using Leopotam.Ecs.Types;

namespace DefenceFactory
{
    interface IViewService
    {
        IView CreatePlayerView(float x, float y);
        IChunkView CreateChunk(Chunk chunk);
    }
}
