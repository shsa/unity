﻿using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using TMPro;

namespace Game
{
    public sealed class ChainMoveSystem : EntityCommandBufferSystem
    {
        const float dist = 0.1f;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            //Enabled = false;
        }

        float time = 0;
        float maxTime = 0.1f;
        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            var deltaTime = Time.DeltaTime;
            time += deltaTime;
            var k = 1.0f;
            if (time > maxTime)
            {
                time -= maxTime;
            }
            else
            {
                k = time / maxTime;
            }
            var cdfe = GetComponentDataFromEntity<Translation>(true);
            Entities
                .WithReadOnly(cdfe)
                .ForEach((Entity entity, int entityInQueryIndex, ref Chain chain) =>
                {
                    if (cdfe.Exists(chain.head))
                    {
                        chain.current = math.lerp(chain.start, chain.finish, k);
                        if (k == 1.0f)
                        {
                            chain.start = chain.current;
                            chain.finish = cdfe[chain.head].Value;
                        }
                    }
                    else
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                })
                .ScheduleParallel();

            Entities
                .ForEach((ref Translation pos, in Chain chain) =>
                {
                    pos.Value = chain.current;
                })
                .ScheduleParallel();
        }
    }
}