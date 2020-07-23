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
            var dir = new float3(0, 0, 0.5f);

            var cdfe = GetComponentDataFromEntity<Movement>(true);
            Entities
                .WithReadOnly(cdfe)
                .ForEach((Entity entity, int entityInQueryIndex, ref Chain chain, in Movement movement) =>
                {
                    if (cdfe.Exists(chain.head))
                    {
                        var m = cdfe[chain.head];
                        var l = math.length(m.pos - movement.pos);
                        var speed = movement.speed * deltaTime;
                        var k = speed / l;
                        chain.dir = math.normalize(math.lerp(movement.dir, m.dir, k));
                        chain.pos += chain.dir * speed;
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
                    rot.Value = quaternion.LookRotation(movement.dir, math.up()); ;
                })
                .ScheduleParallel();
        }
    }
}