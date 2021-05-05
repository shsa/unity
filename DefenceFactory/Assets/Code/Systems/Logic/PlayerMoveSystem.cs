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
                var blockPos = new BlockPos((int)pos.X, (int)pos.Y, 0);
                var chunkPos = blockPos.ChunkMinPos;

                //_gameWorld.GetOrCreateChunk(chunkPos.x, chunkPos.y, 0);
                int offset = 1;
                for (int x = -offset; x <= offset; x++)
                {
                    for (int y = -offset; y <= offset; y++)
                    {
                        _gameWorld.GetOrCreateChunk(chunkPos.x + x * 0x10, chunkPos.y + y * 0x10, 0);
                    }
                }
            }
        }
    }
}
