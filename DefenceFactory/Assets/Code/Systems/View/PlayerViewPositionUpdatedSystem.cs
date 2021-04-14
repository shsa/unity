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
        private readonly IViewService _viewService = default;
        private readonly EcsFilter<Position, PositionUpdatedFlag, PlayerTag> _filter = default;

        void IEcsRunSystem.Run()
        {
            if (_filter.IsEmpty())
            {
                return;
            }

            foreach (var i in _filter)
            {
                var pos = _filter.Get1(i).Value;
                _viewService.SetPlayerPosition(pos.X, pos.Y);
            }
        }
    }
}
