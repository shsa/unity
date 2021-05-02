using DefenceFactory.Game.World;
using Leopotam.Ecs;
using System;
using Unity.Collections;
using UnityEngine;

namespace DefenceFactory.Ecs
{
    sealed class InputSystem : IEcsRunSystem, IDisposable
    {
        private readonly EcsWorld _world = default;
        private readonly IInputService _input = default;
        private readonly IInventoryService _inventory = default;
        private readonly EcsFilter<InventoryItem> _inventoryFilter = default;
        NativeArray<BlockData> data;
        NativeArray<BlockType> types;
        NativeArray<BlockFlag> flags;
        NativeArray<Int32> metas;

        public InputSystem()
        {
            //data = new NativeArray<BlockData>(4096, Allocator.Persistent);
            //types = new NativeArray<BlockType>(4096, Allocator.Persistent);
            //flags = new NativeArray<BlockFlag>(4096, Allocator.Persistent);
            //metas = new NativeArray<Int32>(4096, Allocator.Persistent);
        }

        public void Dispose()
        {
            //data.Dispose();
            //types.Dispose();
            //flags.Dispose();
            //metas.Dispose();
        }

        void IEcsRunSystem.Run()
        {
            //var rnd = new System.Random();
            //for (int j = 0; j < 100; j++)
            //{
            //    for (int i = 0; i < 4096; i++)
            //    {
            //        var d = data[i];
            //        //var d = metas[i];
            //    }
            //}


            //for (int j = 0; j < 1; j++)
            //{
            //    for (int i = 0; i < 4096; i++)
            //    {
            //        var d = data[i];
            //        d.meta = rnd.Next(Int32.MaxValue);
            //        data[i] = d;
            //    }
            //}

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
                e.Get<InventoryItem>().Value = item;
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
