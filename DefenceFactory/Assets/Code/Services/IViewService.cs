using DefenceFactory.Game.World;
using Leopotam.Ecs.Types;

namespace DefenceFactory
{
    interface IViewService
    {
        IView CreatePlayerView(float x, float y);
        IView CreateBlock(BlockPos pos, BlockData blockData);
        IView CreateChunk(Chunk chunk);
    }
}
