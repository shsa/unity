using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using System;
using Random = Unity.Mathematics.Random;

namespace Game
{
    public sealed class SpawnSystem : EntityCommandBufferSystem
    {
        EnemySpawner setup = null;

        float lastSpawnTime = 0;
        Random random;
        NativeArray<Entity> enemyPrefabs;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            setup = EnemySpawner.Instance;

            var settings = GameObjectConversionSettings.FromWorld(World, null);

            enemyPrefabs = new NativeArray<Entity>(setup.enemyPrefabs.Length, Allocator.Persistent);
            for (int i = 0; i < setup.enemyPrefabs.Length; i++)
            {
                enemyPrefabs[i] = GameObjectConversionUtility.ConvertGameObjectHierarchy(setup.enemyPrefabs[i], settings);
            }
            random = new Random(1);
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            enemyPrefabs.Dispose();
        }

        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            lastSpawnTime -= Time.DeltaTime;
            if (lastSpawnTime <= 0)
            {
                lastSpawnTime = setup.spawnInterval;

                var spawnCount = setup.spawnCount;
                var spawnRadius = setup.spawnRadius;
                var minSpeed = setup.minSpeed;
                var maxSpeed = setup.maxSpeed;
                var playerPos = (float3)GameManager.GetPlayerPosition();
                var rnd = new Random(random.NextUInt(uint.MinValue, uint.MaxValue));

                float3 RandomPointOnCircle(float spawnRaduis)
                {
                    var pos = rnd.NextFloat2Direction() * spawnRaduis;
                    return new float3(pos.x, 0, pos.y) + playerPos;
                }

                var prefabs = enemyPrefabs;
                Job
                    .WithCode(() => 
                    {
                        var index = 0;
                        for (int i = 0; i < spawnCount; i++)
                        {
                            var enemy = ecb.Instantiate(index, prefabs[rnd.NextInt(0, prefabs.Length)]);
                            ecb.AddComponent(index, enemy, new Translation { Value = RandomPointOnCircle(spawnRadius) });
                            ecb.AddComponent(index, enemy, new Movement { 
                                speed = rnd.NextFloat(minSpeed, maxSpeed),
                                time = 0,
                                type = MovementEnum.Spiral
                            });
                            //ecb.AddComponent(index, enemy, new MoveForward { speed = rnd.NextFloat(minSpeed, maxSpeed) });
                            ecb.AddComponent(index, enemy, new Lifetime { Value = 30 });
                            ecb.AddComponent<EnemyTag>(index, enemy);
                            //ecb.AddComponent<IsCreated>(index, enemy);
                        }
                    })
                    .Schedule();
            }
        }
    }
}