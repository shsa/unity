using DefenceFactory.World;
using Leopotam.Ecs;

namespace DefenceFactory
{
    sealed class DestroyChunkSystem : IEcsRunSystem
    {
        readonly EcsWorld _world = default;
        private readonly GameWorld _gameWorld = default;
        readonly EcsFilter<Ecs.Chunk> _filter = default;

        void IEcsRunSystem.Run()
        {
            if (_filter.IsEmpty())
            {
                return;
            }

            foreach (var i in _filter)
            {
                ref var chunk = ref _filter.Get1(i).Value;
                if (chunk.IsDestroyed)
                {
                    _filter.GetEntity(i).Get<Ecs.DestroyedFlag>();
                }
            }
        }
    }
}