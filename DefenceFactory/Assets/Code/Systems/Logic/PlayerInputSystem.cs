using DefenceFactory.World;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DefenceFactory.Ecs
{
    sealed class PlayerInputSystem : IEcsRunSystem
    {
        private readonly GameWorld _gameWorld = default;
        private readonly EcsFilter<Input> _input = default;
        private readonly EcsFilter<Position, PlayerTag> _filter = default;

        void IEcsRunSystem.Run()
        {
            if (_input.IsEmpty())
            {
                return;
            }

            if (_filter.IsEmpty())
            {
                return;
            }

            foreach (var j in _input)
            {
                ref var coord = ref _input.Get1(j).Coordinate;
                foreach (var i in _filter)
                {
                    ref var pos = ref _filter.Get1(i).Value;
                    pos.Set(coord.X, coord.Y);
                    _filter.GetEntity(i).Get<PositionUpdatedFlag>();
                }
            }
        }
    }
}
