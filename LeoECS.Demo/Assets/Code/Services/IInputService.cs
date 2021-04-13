using Leopotam.Ecs.Types;

namespace LeoECS
{
    interface IInputService
    {
        bool GetClickedCoordinate(out Int2 coord);
    }
}
