using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using TMPro;

namespace Game
{
    [UpdateInGroup(typeof(MovementGroup1))]
    public sealed class MovementChainSystem : EntityCommandBufferSystem
    {
        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            float deltaTime = Time.DeltaTime;
            var up = new float3(0, 1, 0);

            var cdfe = GetComponentDataFromEntity<Movement>(true);
            Entities
                .WithReadOnly(cdfe)
                .ForEach((Entity entity, int entityInQueryIndex, ref Chain chain, in Movement movement) =>
                {
                    if (cdfe.Exists(chain.head))
                    {
                        var m = cdfe[chain.head];
                        chain.dir = math.normalize(m.pos - movement.pos);
                        chain.pos = m.pos - chain.dir * chain.dist;
                    }
                    else
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                })
                .ScheduleParallel();

            Entities
                .ForEach((ref Movement movement, ref Translation trans, ref Rotation rot, in Chain chain) =>
                {
                    movement.pos = chain.pos;
                    movement.dir = chain.dir;
                    trans.Value = movement.pos;
                    rot.Value = quaternion.LookRotation(movement.dir, up); ;
                })
                .ScheduleParallel();
        }
    }
}