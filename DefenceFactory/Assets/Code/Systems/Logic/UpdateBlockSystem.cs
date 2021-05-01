using DefenceFactory.Game.World;
using Leopotam.Ecs;

namespace DefenceFactory
{
    sealed class UpdateBlockSystem : IEcsRunSystem
    {
        private readonly GameWorld _gameWorld = default;
        readonly EcsFilter<Ecs.Chunk> _filter = default;

        void IEcsRunSystem.Run()
        {
            foreach (var i in _filter)
            {
                var chunk = _filter.Get1(i).Value;
                if (!chunk.updateBlock.IsEmpty)
                {
                    BlockPos blockPos;
                    while (chunk.updateBlock.TryPop(out blockPos))
                    {
                        var blockId = _gameWorld.GetBlockData(blockPos).GetBlockId();
                        var sets = _gameWorld.CalcNeighbors(blockPos);
                        _gameWorld.SetBlockData(blockPos, blockId.GetBlockData(sets));
                    }
                }
            }
        }
    }
}