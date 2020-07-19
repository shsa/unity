using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;

namespace Game
{
    public abstract class EntityCommandBufferSystem : SystemBase
    {
        EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            OnUpdate(endSimulationEcbSystem.CreateCommandBuffer().ToConcurrent());
            AddJobHandleForProducer(Dependency);
        }

        protected void AddJobHandleForProducer(JobHandle jobHandle)
        {
            endSimulationEcbSystem.AddJobHandleForProducer(jobHandle);
        }

        protected abstract void OnUpdate(EntityCommandBuffer.Concurrent ecb);
    }
}