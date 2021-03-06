using DefenceFactory.Ecs;
using DefenceFactory.Game.World;
using Leopotam.Ecs;
using System.Linq;
using UnityEngine;

namespace DefenceFactory
{
    sealed class UpdateChunkViewSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Ecs.Chunk, View> _filter = default;

        void IEcsRunSystem.Run()
        {
            if (_filter.IsEmpty())
                return;

            foreach (var i in _filter)
            {
                ref var chunk = ref _filter.Get1(i).Value;
                if ((chunk.flag & Game.World.ChunkFlag.Redraw) == Game.World.ChunkFlag.Redraw)
                {
                    var view = _filter.Get2(i).Value as IChunkView;
                    if (view != default)
                    {
                        view.UpdateBlocks(chunk);
                    }
                    chunk.flag &= ~Game.World.ChunkFlag.Redraw;
                }
            }
        }
    }
}