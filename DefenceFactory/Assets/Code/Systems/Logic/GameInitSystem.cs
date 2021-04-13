using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using System;

namespace DefenceFactory.Ecs
{
    sealed class GameInitSystem : IEcsInitSystem
    {
        readonly EcsWorld _world = default;
        readonly Random _random = default;

        public void Init()
        {
            for (int i = 0; i < 1000; i++)
            {
                var entity = _world.NewEntity();
                entity.Get<Position>().Value = new Int2(_random.Next(-10, 10), _random.Next(-10, 10));
            }
        }
    }
}