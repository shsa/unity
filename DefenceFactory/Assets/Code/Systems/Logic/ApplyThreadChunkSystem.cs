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
    sealed class ApplyThreadChunkSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Chunk, ThreadChunk> _filter = default;

        void IEcsRunSystem.Run()
        {
            if (_filter.IsEmpty())
            {
                return;
            }

            foreach (var i in _filter)
            {
                ref var chunk = ref _filter.Get1(i).Value;
                ref var thread = ref _filter.Get2(i);
                //chunk.data = thread.data;
                //chunk.flags = thread.flags;
                chunk.flag &= Game.World.ChunkFlag.NotThreadAll;
                chunk.flag |= Game.World.ChunkFlag.Redraw;
                //_filter.GetEntity(i).Del<ThreadChunk>();
            }
        }
    }
}
