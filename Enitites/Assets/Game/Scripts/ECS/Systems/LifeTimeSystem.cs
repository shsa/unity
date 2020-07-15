using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using System.Threading;

namespace Game
{
    public sealed class LifetimeSystem : SystemBase
    {
        EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var ecb = endSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();

            Entities
                .ForEach((Entity entity, int entityInQueryIndex, ref Lifetime lifetime) =>
                {
                    lifetime.Value -= deltaTime;
                    if (lifetime.Value <= 0)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel();

            endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}