using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

namespace Game
{
    [UpdateInGroup(typeof(MovementGroup0))]
    public sealed class MovementSystem : EntityCommandBufferSystem
    {
        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            return;
            float deltaTime = Time.DeltaTime;
            Entities
                .WithChangeFilter<Movement>()
                .ForEach((Entity entity, int entityInQueryIndex, in Movement movement, in Translation trans) =>
                {
                    switch (movement.type)
                    {
                        case MovementEnum.Spiral:
                            {
                                ecb.AddComponent(entityInQueryIndex, entity, new MovementSpiral { });
                            }
                            break;
                    }
                }).ScheduleParallel();
        }
    }
}