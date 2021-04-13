using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LeoECS.Ecs
{
    sealed class PlayerViewUpdatePositionSystem : IEcsRunSystem
    {
        private readonly EcsFilter<View, Position, PositionUpdated> _views = default;

        void IEcsRunSystem.Run()
        {
            if (_views.IsEmpty())
                return;

            foreach (var i in _views)
            {
                var view = _views.Get1(i).Value;
                var pos = _views.Get2(i).Value;
                view.UpdatePosition(pos.X, pos.Y);
            }
        }
    }
}
