using DefenceFactory.World;
using Leopotam.Ecs;

namespace DefenceFactory
{
    sealed class CreateChunkSystem : IEcsRunSystem
    {
        readonly EcsWorld _world = default;
        private readonly GameWorld _gameWorld = default;

        void IEcsRunSystem.Run()
        {
            if (_gameWorld.newChunks.IsEmpty)
            {
                return;
            }

            Chunk chunk;
            while (_gameWorld.newChunks.TryPop(out chunk))
            {
                var e = _world.NewEntity();
                e.Get<Ecs.Chunk>().Value = chunk;
            }
        }
    }
}