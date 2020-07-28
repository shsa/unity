using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using TMPro;

namespace Game
{
    [UpdateInGroup(typeof(MovementGroup1))]
    public sealed class MovementChainSystem : EntityCommandBufferSystem
    {
        static readonly float3 up = new float3(0, 1, 0);

        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            float deltaTime = Time.DeltaTime;

            Entities
                .WithAll<Chain>()
                .WithNone<TempTranslation>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.AddComponent(entityInQueryIndex, entity, new TempTranslation { });
                })
                .ScheduleParallel();

            var cdfe = GetComponentDataFromEntity<Translation>(true);
            Entities
                .WithReadOnly(cdfe)
                .ForEach((Entity entity, int entityInQueryIndex, ref TempTranslation tempTrans, ref Rotation rot, in Chain chain, in ChainParent parent) =>
                {
                    if (cdfe.Exists(parent.Value))
                    {
                        var m = cdfe[parent.Value];
                        var dir = math.normalize(m.Value - tempTrans.Value);
                        tempTrans.Value = m.Value - dir * chain.dist;
                        rot.Value = quaternion.LookRotation(dir, up);
                    }
                    else
                    {
                        //ecb.DestroyEntity(entityInQueryIndex, entity);
                        ecb.RemoveComponent<Chain>(entityInQueryIndex, entity);
                        ecb.RemoveComponent<ChainParent>(entityInQueryIndex, entity);
                        ecb.RemoveComponent<MovementTypeTag>(entityInQueryIndex, entity);
                    }
                })
                .ScheduleParallel();

            Entities
                .WithAll<Chain>()
                .ForEach((ref Translation trans, in TempTranslation tempTrans) =>
                {
                    trans.Value = tempTrans.Value;
                })
                .ScheduleParallel();
        }
    }
}