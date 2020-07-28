using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using System;
using Random = Unity.Mathematics.Random;
using Unity.Burst;
using System.Linq;

namespace Game
{
    public sealed class SpawnSystem : EntityCommandBufferSystem
    {
        EnemySpawner setup = null;

        float lastSpawnTime = 0;
        Random mainRandom;

        public static EntityArchetype enemyPrefab;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            setup = EnemySpawner.Instance;

            mainRandom = new Random(1);

            enemyPrefab = EntityManager.CreateArchetype(
                typeof(EnemyTag),
                typeof(Translation),
                typeof(Rotation),
                typeof(LocalToWorld)
                );
        }

        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            lastSpawnTime -= Time.DeltaTime;
            if (lastSpawnTime <= 0)
            {
                lastSpawnTime = setup.spawnInterval;
                lastSpawnTime = float.MaxValue;

                var spawnCount = setup.spawnCount;
                var spawnRadius = setup.spawnRadius;
                var playerPos = (float3)GameManager.GetPlayerPosition();
                var random = new Random(mainRandom.NextUInt(uint.MinValue, uint.MaxValue));
                var enemyArchetype = enemyPrefab;

                float3 RandomPointOnCircle(float spawnRaduis)
                {
                    var pos = random.NextFloat2Direction() * spawnRaduis;
                    return new float3(pos.x, 0, pos.y) + playerPos;
                }

                Job
                    .WithCode(() => 
                    {
                        var jobIndex = 0;
                        for (int i = 0; i < spawnCount; i++)
                        {
                            var enemy = ecb.CreateEntity(jobIndex, enemyArchetype);
                            ecb.AddComponent(jobIndex, enemy, new Translation { Value = RandomPointOnCircle(spawnRadius) });
                        }
                    })
                    .Schedule();
            }
        }
    }
}