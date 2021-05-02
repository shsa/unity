using DefenceFactory.Game;
using DefenceFactory.Game.World;
using Leopotam.Ecs;
using Unity.Collections;

namespace DefenceFactory
{
    sealed class StartThreadChunkSystem : IEcsRunSystem
    {
        readonly EcsFilter<Ecs.Chunk> _filter = default;

        NativeArray<BlockData> Offset(Chunk chunk, DirectionEnum dir)
        {
            ref var v = ref dir.GetVector2();
            if (chunk.World.TryGetChunk(chunk.x + v.X, chunk.y + v.Y, chunk.z, out var c))
            {
                return c.data;
            }
            return default;
        }

        void IEcsRunSystem.Run()
        {
            if (_filter.IsEmpty())
            {
                return;
            }

            foreach (var i in _filter)
            {
                ref var chunk = ref _filter.Get1(i).Value;
                if ((chunk.flag & ChunkFlag.Thread) == ChunkFlag.Thread)
                {
                    var job = new ChunkJob
                    {
                        data = chunk.data,
                        N = Offset(chunk, DirectionEnum.N)
                    };
                    
                    chunk.flag &= ChunkFlag.NotThreadAll;
                }
            }
        }
    }
}