using Leopotam.Ecs;
using Leopotam.Ecs.Threads;
using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DefenceFactory.Ecs
{
    sealed class ThreadTestSystem : EcsMultiThreadSystem<EcsFilter<ThreadComponent>>
    {
        readonly EcsWorld _world = default;
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
            return Worker;
        }

        /// <summary>
        /// Our worker callback for processing entities.
        /// Important: better to use static methods as workers - you cant touch any instance data without additional sync.
        /// </summary>
        static void Worker(EcsMultiThreadWorkerDesc workerDesc)
        {
            foreach (var idx in workerDesc)
            {
                ref var c = ref workerDesc.Filter.Get1(idx);
                //c.Result = (float)System.Math.Sqrt(c.A + c.B + c.C + c.D + c.E + c.F + c.G + c.H + c.I + c.J);
                //c.Result = (float)System.Math.Sin(c.A + c.B + c.C + c.D + c.E + c.F + c.G + c.H + c.I + c.J);
                //c.Result = (float)System.Math.Cos(c.A + c.B + c.C + c.D + c.E + c.F + c.G + c.H + c.I + c.J);
                //c.Result = (float)System.Math.Tan(c.A + c.B + c.C + c.D + c.E + c.F + c.G + c.H + c.I + c.J);
                //c.Result = (float)System.Math.Log10(c.A + c.B + c.C + c.D + c.E + c.F + c.G + c.H + c.I + c.J);
                //c.Result = (float)System.Math.Sqrt(c.A + c.B + c.C + c.D + c.E + c.F + c.G + c.H + c.I + c.J);
                //c.Result = (float)System.Math.Sin(c.A + c.B + c.C + c.D + c.E + c.F + c.G + c.H + c.I + c.J);
                //c.Result = (float)System.Math.Cos(c.A + c.B + c.C + c.D + c.E + c.F + c.G + c.H + c.I + c.J);
                //c.Result = (float)System.Math.Tan(c.A + c.B + c.C + c.D + c.E + c.F + c.G + c.H + c.I + c.J);
                //c.Result = (float)System.Math.Log10(c.A + c.B + c.C + c.D + c.E + c.F + c.G + c.H + c.I + c.J);
            }
        }
    }
}
