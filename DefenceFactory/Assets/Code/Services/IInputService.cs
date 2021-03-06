﻿using DefenceFactory.Ecs;
using Leopotam.Ecs.Types;

namespace DefenceFactory
{
    interface IInputService
    {
        bool GetClickedCoordinate(out Int2 coord);
        bool GetCursorCoordinate(out Int2 coord);
        bool GetShift(out Float2 coord);
        bool GetDrag(out Float2 coord, out DragEnum state);
        bool GetClicked(out Float2 coord);
    }
}
