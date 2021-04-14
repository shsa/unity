using Leopotam.Ecs.Types;

namespace DefenceFactory
{
    interface IViewService
    {
        IView CreatePlayerView(int x, int y, Float3 color);
    }
}
