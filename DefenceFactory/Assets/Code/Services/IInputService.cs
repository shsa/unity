using Leopotam.Ecs.Types;

namespace DefenceFactory
{
    interface IInputService
    {
        bool GetClickedCoordinate(out Int2 coord);
        bool GetCursorCoordinate(out Int2 coord);
    }
}
