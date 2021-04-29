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
    sealed class PlayerViewPositionUpdatedSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Position, View, PositionUpdatedFlag, PlayerTag> _filter = default;

        void IEcsRunSystem.Run()
        {
            if (_filter.IsEmpty())
            {
                return;
            }

            foreach (var i in _filter)
            {
                ref var pos = ref _filter.Get1(i).Value;
                var view = _filter.Get2(i).Value;
                view.UpdatePosition(pos.X, pos.Y);
            }
        }
    }
}
