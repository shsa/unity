using Leopotam.Ecs;

namespace DefenceFactory
{
    sealed class DestroyChunkViewSystem : IEcsRunSystem
    {
        readonly EcsWorld _world = default;
        readonly EcsFilter<Ecs.Chunk, Ecs.View, Ecs.DestroyedFlag> _filter = default;

        void IEcsRunSystem.Run()
        {
            if (_filter.IsEmpty())
            {
                return;
            }

            foreach (var i in _filter)
            {
                ref var chunk = ref _filter.Get1(i).Value;
                ref var view = ref _filter.Get2(i).Value;
                view.Destroy();
                _filter.GetEntity(i).Destroy();
            }
        }
    }
}