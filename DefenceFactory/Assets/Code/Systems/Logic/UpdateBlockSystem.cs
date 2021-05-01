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
                    BlockPos tempPos = new BlockPos();
                    while (chunk.updateBlock.TryPop(out blockPos))
                    {
                        var blockId = _gameWorld.GetBlockData(blockPos).GetBlockId();
                        var sets = DirectionSet.None;
                        for (var d = DirectionEnum.First; d <= DirectionEnum.Last; d++)
                        {
                            ref var offset = ref d.GetDirection();
                            tempPos.Set(blockPos, offset);
                            if (_gameWorld.GetBlockData(tempPos).GetBlockId() == blockId)
                            {
                                sets |= d.Set();
                            }
                        }
                        _gameWorld.SetBlockData(blockPos, blockId.GetBlockData());
                    }
                }
            }
        }
    }
}