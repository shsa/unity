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
    sealed class PlayerMoveSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Input> _inputs = default;
        private readonly EcsFilter<PlayerTag, Position, PositionUpdatedFlag> _positions = default;

        void IEcsRunSystem.Run()
        {
        }
    }
}
