using DefenceFactory.Ecs;
using Leopotam.Ecs;

namespace DefenceFactory
{
    sealed class CreateChunkViewSystem : IEcsRunSystem
    {
        private readonly IViewService _viewService = default;

        private readonly EcsFilter<Chunk>.Exclude<View> _views = default;

        void IEcsRunSystem.Run()
        {
            if (_views.IsEmpty())
                return;

            foreach (var i in _views)
            {
                ref var chunk = ref _views.Get1(i).Value;

                var view = _viewService.CreateChunk(chunk);

                _views.GetEntity(i).Get<View>().Value = view;
            }
        }
    }
}