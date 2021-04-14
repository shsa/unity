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
        private readonly EcsFilter<Position, PlayerTag, PositionUpdatedFlag> _views = default;

        void IEcsRunSystem.Run()
        {
            if (_views.IsEmpty())
                return;

            foreach (var i in _views)
            {
                var pos = _views.Get1(i).Value;
                _views.UpdatePosition(pos.X, pos.Y);
            }
        }
    }
}
