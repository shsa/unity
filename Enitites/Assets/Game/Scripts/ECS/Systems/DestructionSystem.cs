using Unity.Transforms;
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
        private EntityQuery enemyQuery;
        float thresholdDistance = 2f;

        protected override void OnCreate()
        {
            base.OnCreate();
            endSimulationEcbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            bulletQuery = GetEntityQuery(
                ComponentType.ReadOnly<BulletTag>(),
                ComponentType.ReadOnly<Translation>());

            enemyQuery = GetEntityQuery(
                ComponentType.ReadOnly<EnemyTag>(),
                ComponentType.ReadOnly<Translation>());
        }

        // https://forum.unity.com/threads/collection-safety-error-in-ecs-for-using-property-that-was-also-requested.897299/
        protected override void OnUpdate()
        {
            var ecb = endSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
            //var translationType = GetArchetypeChunkComponentType<Translation>();
            //var entityType = GetArchetypeChunkEntityType();
            //var chunks = bulletQuery.CreateArchetypeChunkArrayAsync(Allocator.TempJob, out var handle1);
            var bulletPos = bulletQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var handle1);

            Dependency = JobHandle.CombineDependencies(Dependency, handle1);

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


    //public class DestructionSystem : SystemBase
    //{
    //    EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;
    //    private EntityQuery bulletQuery;
    //    float thresholdDistance = 2f;

    //    protected override void OnCreate()
    //    {
    //        base.OnCreate();
    //        endSimulationEcbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    //        bulletQuery = GetEntityQuery(
    //            ComponentType.ReadOnly<BulletTag>(),
    //            ComponentType.ReadOnly<Translation>());
    //    }

    //    protected override void OnUpdate()
    //    {
    //        // https://forum.unity.com/threads/looping-over-two-separate-entity-types.852580/#post-5657860
    //        var ecb = endSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
    //        var translationType = GetArchetypeChunkComponentType<Translation>();
    //        var entityType = GetArchetypeChunkEntityType();
    //        var query = GetEntityQuery(
    //            ComponentType.ReadOnly<BulletTag>(),
    //            ComponentType.ReadOnly<Translation>());

    //        var chunks = query.CreateArchetypeChunkArray(Allocator.TempJob);
    //        ArchetypeChunk[] list = new ArchetypeChunk[10];
    //        float dst = thresholdDistance;
    //        float3 playerPosition = (float3)GameManager.GetPlayerPosition();

    //        Entities
    //            .WithAll<EnemyTag>()
    //            .WithDeallocateOnJobCompletion(chunks)
    //            //.WithStructuralChanges()
    //            .ForEach((Entity enemy, int entityInQueryIndex, in Translation enemyPos) =>
    //            {
    //                playerPosition.y = enemyPos.Value.y;

    //                if (math.distance(enemyPos.Value, playerPosition) <= dst)
    //                {
    //                    //FXManager.Instance.CreateExplosion(enemyPos.Value);
    //                    //FXManager.Instance.CreateExplosion(playerPosition);
    //                    //GameManager.EndGame();
    //                    ecb.DestroyEntity(entityInQueryIndex, enemy);
    //                    //EntityManager.DestroyEntity(enemy);
    //                }
    //            }).ScheduleParallel();

    //        //float3 enemyPosition = enemyPos.Value;

    //        //Entities.WithAll<BulletTag>()
    //        //    .WithStructuralChanges()
    //        //    .ForEach((Entity bullet, ref Translation bulletPos) =>
    //        //    {
    //        //        if (math.distance(enemyPosition, bulletPos.Value) <= thresholdDistance)
    //        //        {
    //        //            EntityManager.DestroyEntity(enemy);
    //        //            EntityManager.DestroyEntity(bullet);

    //        //            FXManager.Instance.CreateExplosion(enemyPosition);
    //        //            GameManager.AddScore(1);
    //        //        }
    //        //    }).Run();
    //        //}).Run();
    //    }
    //}
}