﻿using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;

namespace Game
{
    public class DestructionSystem : SystemBase
    {
        EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;
        private EntityQuery bulletQuery;
        float thresholdDistance = 2f;

        protected override void OnCreate()
        {
            base.OnCreate();
            endSimulationEcbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            bulletQuery = GetEntityQuery(
                ComponentType.ReadOnly<BulletTag>(),
                ComponentType.ReadOnly<Translation>());
        }

        protected override void OnUpdate()
        {
            var ecb = endSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
            var bulletPos = bulletQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var jobHandle1);
            Dependency = JobHandle.CombineDependencies(Dependency, jobHandle1);

            float3 playerPosition = (float3)GameManager.GetPlayerPosition();
            float dstSq = thresholdDistance * thresholdDistance;

            Dependency = Entities
                .WithAll<EnemyTag>()
                .WithDeallocateOnJobCompletion(bulletPos)
                .ForEach((Entity enemy, int entityInQueryIndex, in Translation enemyPos) =>
                {
                    playerPosition.y = enemyPos.Value.y;

                    if (math.distancesq(enemyPos.Value, playerPosition) <= dstSq)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, enemy);
                    }
                    else
                    {
                        for (int i = 0; i < bulletPos.Length; i++)
                        {
                            if (math.distancesq(bulletPos[i].Value, enemyPos.Value) < dstSq)
                            {
                                ecb.DestroyEntity(entityInQueryIndex, enemy);
                            }
                        }
                    }

                }).ScheduleParallel(Dependency);

            endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}