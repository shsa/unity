using DefenceFactory.Game.World;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using System;

namespace DefenceFactory.Ecs
{
    sealed class GameInitSystem : IEcsInitSystem
    {
        readonly EcsWorld _world = default;
        readonly Random _random = default;
        readonly GameWorld _gameWorld = default;

        public void Init()
        {
            var player = _world.NewEntity();
            player.Get<PlayerTag>();
            player.Get<Position>().Value.Set(0, 0);
            player.Get<PositionUpdatedFlag>();

            //for (int i = 0; i < 1000; i++)
            //{
            //    var entity = _world.NewEntity();
            //    entity.Get<Position>().Value = new Int2(0, 0);
            //    entity.Get<Color>().Value = new Float3(
            //        (float)_random.NextDouble(), 
            //        (float)_random.NextDouble(), 
            //        (float)_random.NextDouble());
            //    entity.Get<ThreadComponent>();
            //}
        }
    }
}