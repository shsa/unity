using Leopotam.Ecs;

namespace DefenceFactory.Ecs
{
    sealed class InputSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world = default;
        private readonly IInputService _input = default;

        void IEcsRunSystem.Run()
        {
            //if (_input.GetClickedCoordinate(out var coord))
            //{
            //    _world.NewEntity()
            //        .Replace(new Input { Coordinate = coord });
            //}

            if (_input.GetCursorCoordinate(out var coord))
            {
                _world.NewEntity()
                    .Replace(new Input { Coordinate = coord });
            }
        }
    }
}
