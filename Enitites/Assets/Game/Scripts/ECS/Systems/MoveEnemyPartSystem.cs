using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

namespace Game
{
    [UpdateAfter(typeof(MovementSystem))]
    public sealed class PrepareEnemyPartSystem : SystemBase
    {
        EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = endSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
            var c = GetComponentDataFromEntity<Translation>(true);
            Dependency = Entities
                .WithReadOnly(c)
                .ForEach((Entity part, int entityInQueryIndex, ref EnemyPartTag partTag) =>
                {
                    if (c.Exists(partTag.parent))
                    {
                        var pos = c[partTag.parent];
                        partTag.pos = pos.Value;
                    }
                    else
                    {
                        ecb.DestroyEntity(entityInQueryIndex, part);
                    }
                })
                .ScheduleParallel(Dependency);


            Dependency = Entities
                .ForEach((ref Translation pos, in EnemyPartTag partTag) =>
                {
                    pos.Value = partTag.pos;
                })
                .ScheduleParallel(Dependency);

            endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}