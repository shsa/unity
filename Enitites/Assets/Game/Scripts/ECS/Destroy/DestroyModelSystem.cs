using Unity.Entities;
using Unity.Collections;
using System;
using Random = Unity.Mathematics.Random;
using System.Linq;

namespace Game
{
    public sealed class DestroyModelSystem : EntityCommandBufferSystem
    {
        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            Entities
                .WithNone<EntityAliveTag>()
                .ForEach((Entity entity, int entityInQueryIndex, in Model model) =>
                {
                    ecb.DestroyEntity(entityInQueryIndex, model.Value);
                })
                .ScheduleParallel();
        }
    }
}