using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;

namespace Game
{
    [UpdateAfter(typeof(MoveForwardSystem))]
    public sealed class PrepareEnemyPartSystem : SystemBase
    {
        EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;
        EntityQuery partsQuery;
        float angle = 0;
        float speed = 0.1f;
        float time = 0;

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
            time += deltaTime;

            //var result = new NativeArray<float3>(partsQuery.CalculateEntityCount(), Allocator.TempJob);
            angle += speed * time;
            if (angle > (1 * math.PI))
            {
                angle -= 1 * math.PI;
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