using Leopotam.Ecs;
using Leopotam.Ecs.Threads;
using Leopotam.Ecs.Types;
using UnityEngine;
using Random = System.Random;

namespace DefenceFactory.Ecs
{
    sealed class ThreadTestSystem : EcsMultiThreadSystem<EcsFilter<ThreadComponent>>
    {
        readonly EcsFilter<ThreadComponent> _filter = default;

        /// <summary>
        /// Returns filter for processing entities in it at background threads.
        /// </summary>
        protected override EcsFilter<ThreadComponent> GetFilter()
        {
            return _filter;
        }

        /// <summary>
        /// Returns minimal amount of entities for splitting to threads instead processing in one.
        /// </summary>
        protected override int GetMinJobSize()
        {
            return 100;
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
            return (p) => Worker(1, p);
        }

        /// <summary>
        /// Our worker callback for processing entities.
        /// Important: better to use static methods as workers - you cant touch any instance data without additional sync.
        /// </summary>
        static void Worker(int param, EcsMultiThreadWorkerDesc workerDesc)
        {
            var rnd = new Random();

            foreach (var idx in workerDesc)
            {
                ref var c = ref workerDesc.Filter.Get1(idx);
                c.Value = new Int2(rnd.Next(-10, 10), rnd.Next(-10, 10));
            }
        }
    }
}
