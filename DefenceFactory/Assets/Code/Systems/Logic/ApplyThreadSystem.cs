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
    sealed class ApplyThreadSystem : IEcsRunSystem
    {
        private readonly EcsFilter<ThreadComponent, Position> _list = default;

        void IEcsRunSystem.Run()
        {
            foreach (var i in _list)
            {
                ref var t = ref _list.Get1(i);
                ref var pos = ref _list.Get2(i);
                pos.Value.Set(t.Value.X, t.Value.Y);
                _list.GetEntity(i).Get<PositionUpdatedFlag>();
            }
        }
    }
}
