using DefenceFactory.Ecs;
using DefenceFactory.Game.World;
using Leopotam.Ecs;

namespace DefenceFactory
{
    sealed class PlaceItemSystem : IEcsRunSystem
    {
        private readonly GameWorld _gameWorld = default;
        readonly EcsFilter<InventoryItem, Position, PlaceItemFlag> _filter = default;

        void IEcsRunSystem.Run()
        {
            if (_filter.IsEmpty())
            {
                return;
            }

            foreach (var i in _filter)
            {
                var item = _filter.Get1(i).Value;
                ref var pos = ref _filter.Get2(i).Value;
                var blockPos = BlockPos.From(pos);
                var chunk = _gameWorld.GetOrCreateChunk(blockPos.ChunkPos);
                chunk.SetBlockData(blockPos, item.GetBlockData());
            }
        }
    }
}