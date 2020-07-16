using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;

namespace Game
{
    [UpdateAfter(typeof(MovementSystem))]
    public sealed class PrepareEnemyPartSystem : SystemBase
    {
        const float PI2 = math.PI * 2;

        EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;
        EntityQuery partsQuery;
        float angle = 0;
        float speed = math.PI * 2;

        protected override void OnCreate()
        {
            base.OnCreate();
            endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            partsQuery = GetEntityQuery(
                ComponentType.ReadOnly<EnemyPartTag>());
        }

        protected override void OnUpdate()
        {
            var ecb = endSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
            var c = GetComponentDataFromEntity<Translation>(true);
            float deltaTime = Time.DeltaTime;

            //var result = new NativeArray<float3>(partsQuery.CalculateEntityCount(), Allocator.TempJob);
            angle += speed * deltaTime;
            if (angle > PI2)
            {
                angle -= PI2;
            }
            var r = quaternion.Euler(0, angle, 0);
            Dependency = Entities
                .WithAll<EnemyPartTag>()
                .WithReadOnly(c)
                .ForEach((Entity part, int entityInQueryIndex, ref Rotation rot, in Parent parent) =>
                {
                    if (c.Exists(parent.Value))
                    {
                        rot.Value = r;
                    }
                    else
                    {
                        ecb.DestroyEntity(entityInQueryIndex, part);
                    }
                })
                .ScheduleParallel(Dependency);

            endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}