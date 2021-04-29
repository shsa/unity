using DefenceFactory.Game.World;
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
        private readonly EcsFilter<Input> _input = default;
        private readonly EcsFilter<Drag> _drag = default;
        private readonly EcsFilter<Position, PlayerTag> _filter = default;

        Float2 startDrag;
        Float2 startPos;
        void IEcsRunSystem.Run()
        {
            if (_filter.IsEmpty())
            {
                return;
            }

            if (!_input.IsEmpty())
            {
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

            if (!_drag.IsEmpty())
            {
                foreach (var j in _drag)
                {
                    ref var currentDrag = ref _drag.Get1(j).Position;
                    var state = _drag.Get1(j).State;
                    foreach (var i in _filter)
                    {
                        ref var pos = ref _filter.Get1(i).Value;

                        if (state == DragEnum.Begin)
                        {
                            startDrag = currentDrag;
                            startPos = pos;
                        }
                        else
                        {
                            var offset = currentDrag - startDrag;

                            pos.Set(startPos.X - offset.X, startPos.Y - offset.Y);
                            _filter.GetEntity(i).Get<PositionUpdatedFlag>();
                        }
                    }
                }
            }
        }
    }
}
