using DefenceFactory.Game.World;
using Leopotam.Ecs;
using Leopotam.Ecs.Threads;
using Leopotam.Ecs.Types;
using UnityEngine;
using Random = System.Random;

namespace DefenceFactory.Ecs
{
    sealed class ThreadChunkSystem : EcsMultiThreadSystem<EcsFilter<ThreadChunk>>
    {
        readonly GameWorld _gameWorld = default;
        readonly EcsFilter<ThreadChunk> _filter = default;

        /// <summary>
        /// Returns filter for processing entities in it at background threads.
        /// </summary>
        protected override EcsFilter<ThreadChunk> GetFilter()
        {
            return _filter;
        }

        /// <summary>
        /// Returns minimal amount of entities for splitting to threads instead processing in one.
        /// </summary>
        protected override int GetMinJobSize()
        {
            return 1;
        }

        /// <summary>
        /// Returns background threads amount. Main thread will be used as additional worker (+1 thread).
        /// </summary>
        protected override int GetThreadsCount()
        {
            return System.Environment.ProcessorCount - 1;
        }

        /// <summary>
        /// Returns our worker callback.
        /// </summary>
        protected override EcsMultiThreadWorker GetWorker()
        {
            return (p) => Worker(_gameWorld, p);
        }

        /// <summary>
        /// Our worker callback for processing entities.
        /// Important: better to use static methods as workers - you cant touch any instance data without additional sync.
        /// </summary>
        static void Worker(GameWorld world, EcsMultiThreadWorkerDesc workerDesc)
        {
            foreach (var idx in workerDesc)
            {
                ref var chunk = ref workerDesc.Filter.Get1(idx);
                if ((chunk.flag & ChunkFlag.Generate) == ChunkFlag.Generate)
                {
                    Generate(world.generator, chunk);
                    chunk.flag |= ChunkFlag.Update;
                }
                if ((chunk.flag & ChunkFlag.Update) == ChunkFlag.Update)
                {
                    Update(world, chunk);
                }
            }
        }

        static void Generate(IWorldGenerator generator, in ThreadChunk chunk)
        {
            for (int x = 0; x <= 15; x++)
            {
                for (int y = 0; y <= 15; y++)
                {
                    var index = Game.World.Chunk.GetBlockIndex(x, y, 0);
                    //chunk.data[index] = generator.CalcBlockId(chunk.pos.x + x, chunk.pos.y + y, chunk.pos.z).GetBlockData();
                    //chunk.flags[index] = BlockFlag.Update;
                }
            }
        }

        static void Update(GameWorld world, in ThreadChunk chunk)
        {
            for (int i = 0; i <= 15; i++)
            {
                for (int j = 0; j <= 15; j++)
                {
                    var index = Game.World.Chunk.GetBlockIndex(i, j, 0);
                    //if (chunk.flags[index] == BlockFlag.Update)
                    //{
                    //    var blockId = chunk.data[index].GetBlockId();
                    //    var neighbors = DirectionSet.None;
                    //    for (var d = DirectionEnum.First; d <= DirectionEnum.Last; d++)
                    //    {
                    //        var neighborId = GetBlockId(world, chunk, i + d.GetVector2().X, j + d.GetVector2().Y, 0);
                    //        if (neighborId == blockId)
                    //        {
                    //            neighbors |= d.Set();
                    //        }
                    //    }
                    //}
                }
            }
        }

        static BlockType GetBlockId(GameWorld world, in ThreadChunk chunk, int x, int y, int z)
        {
            if ((x < 0) || (x > 15) || (y < 0) || (y > 15))
            {
                return world.GetBlockData(chunk.x + x, chunk.x + y, chunk.z + z).GetBlockId();
            }
            return chunk.data[Game.World.Chunk.GetBlockIndex(x, y, z)].GetBlockId();
        }
    }
}
