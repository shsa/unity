using DefenceFactory.World;
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
    sealed class PlayerMoveSystem : IEcsRunSystem
    {
        private readonly GameWorld _gameWorld = default;
        private readonly EcsFilter<Position, PositionUpdatedFlag, PlayerTag> _filter = default;

        void IEcsRunSystem.Run()
        {
            if (_filter.IsEmpty())
            {
                return;
            }

            foreach (var i in _filter)
            {
                ref var pos = ref _filter.Get1(i).Value;
                var blockPos = new BlockPos(pos.X, pos.Y, 0);
                var chunkPos = blockPos.ChunkPos;
                var cp = new ChunkPos(0, 0, 0);
                for (int x = -3; x <= 3; x++)
                {
                    for (int y = -3; y <= 3; y++)
                    {
                        cp.Set(chunkPos.x + x, chunkPos.y + y, 0);
                        _gameWorld.GetOrCreateChunk(cp);
                    }
                }
            }
        }
    }
}
