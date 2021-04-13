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
    sealed class PlayerMoveSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Input> _inputs = default;
        private readonly EcsFilter<Position> _positions = default;

        void IEcsRunSystem.Run()
        {
            foreach (var i in _inputs)
            {
                var coordinate = _inputs.Get1(i).Coordinate;
                foreach (var p in _positions)
                {
                    var e = _positions.GetEntity(p);
                    e.Replace(new Position { Value = coordinate });
                    e.Get<PositionUpdated>();
                }
            }
        }
    }
}
