using Leopotam.Ecs;
using UnityEngine;

namespace DefenceFactory.Ecs
{
    sealed class InputSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world = default;
        private readonly IInputService _input = default;
        private readonly IInventoryService _inventory = default;
        private readonly EcsFilter<InventoryItem> _inventoryFilter = default;

        void IEcsRunSystem.Run()
        {
            //if (_input.GetClickedCoordinate(out var coord))
            //{
            //    _world.NewEntity()
            //        .Replace(new Input { Coordinate = coord });
            //}

            if (_input.GetDrag(out var pos, out var state))
            {
                _world.NewEntity()
                    .Replace(new Drag
                    {
                        Position = pos,
                        State = state
                    });
            }

            if (_inventory.GetBlock(out var item))
            {
                var e = _world.NewEntity();
                e.Get<InventoryItem>();
            }

            if (_input.GetClicked(out pos))
            {
                foreach (var i in _inventoryFilter)
                {
                    var e = _inventoryFilter.GetEntity(i);
                    e.Get<PlaceItemFlag>();
                    e.Replace(new Position
                    {
                        Value = pos
                    });
                }
            }
        }
    }
}
