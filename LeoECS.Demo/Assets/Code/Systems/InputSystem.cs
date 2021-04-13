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
    sealed class InputSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world = default;
        private readonly IInputService _input = default;

        void IEcsRunSystem.Run()
        {
            //if (_input.GetClickedCoordinate(out var coord))
            //{
            //    _world.NewEntity()
            //        .Replace(new Input { Coordinate = coord });
            //}

            if (_input.GetCursorCoordinate(out var coord))
            {
                _world.NewEntity()
                    .Replace(new Input { Coordinate = coord });
            }
        }
    }
}
