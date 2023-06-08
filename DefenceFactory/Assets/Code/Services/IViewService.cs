using DefenceFactory.World;
using Leopotam.Ecs.Types;

namespace DefenceFactory
{
    interface IViewService
    {
        IView CreatePlayerView(int x, int y, Float3 color);
        IView CreateBlock(BlockPos pos, BlockData blockData);

        void SetPlayerPosition(int x, int y);
    }
}
