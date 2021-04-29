using DefenceFactory.Ecs;
using Leopotam.Ecs;

namespace DefenceFactory
{
    sealed class UpdateChunkViewSystem : IEcsRunSystem
    {
        private readonly EcsFilter<Chunk, View> _filter = default;

        void IEcsRunSystem.Run()
        {
            if (_filter.IsEmpty())
                return;

            foreach (var i in _filter)
            {
                ref var chunk = ref _filter.Get1(i).Value;
                if (chunk.IsChanged)
                {
                    var view = _filter.Get2(i).Value as IChunkView;
                    if (view != default)
                    {
                        view.UpdateBlocks(chunk);
                    }
                    chunk.IsChanged = false;
                }
            }
        }
    }
}