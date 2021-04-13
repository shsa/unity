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
    sealed class DemoInitSystem : IEcsInitSystem
    {
        private readonly EcsWorld _world = default;

        void IEcsInitSystem.Init()
        {
            var entity = _world.NewEntity();
            entity.Get<Position>().Value = new Int2(0, 0);
        }
    }
}
