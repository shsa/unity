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
        Random random;
        NativeArray<Entity> enemyPrefabs;

        [ReadOnly]
        static float minSpeed;
        [ReadOnly]
        static float maxSpeed;

        [ReadOnly]
        public static EntityArchetype enemyPrefab;
        [ReadOnly]
        public static EntityArchetype enemyTypePrefab;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            setup = EnemySpawner.Instance;

            minSpeed = setup.minSpeed;
            maxSpeed = setup.maxSpeed;

            var settings = GameObjectConversionSettings.FromWorld(World, null);

            enemyPrefabs = new NativeArray<Entity>(setup.enemyPrefabs.Length, Allocator.Persistent);
            for (int i = 0; i < setup.enemyPrefabs.Length; i++)
            {
                enemyPrefabs[i] = GameObjectConversionUtility.ConvertGameObjectHierarchy(setup.enemyPrefabs[i], settings);
            }
            random = new Random(1);

            enemyPrefab = CreateEnemyArchetype();
            enemyTypePrefab = CreateEnemyTypeArchetype();
        }

        public EntityArchetype CreateEnemyArchetype()
        {
            return EntityManager.CreateArchetype(
                typeof(Translation), 
                typeof(Rotation),
                typeof(LocalToWorld)
                );
        }

        public EntityArchetype CreateEnemyTypeArchetype()
        {
            return EntityManager.CreateArchetype(
                typeof(Translation),
                typeof(Rotation),
                typeof(LocalToWorld),
                typeof(LocalToParent),
                typeof(Parent),
                typeof(SubObject)
                );
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
                lastSpawnTime = float.MaxValue;

                var spawnCount = setup.spawnCount;
                var spawnRadius = setup.spawnRadius;
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
                        var jobIndex = 0;
                        for (int i = 0; i < spawnCount; i++)
                        {
                            var pos = RandomPointOnCircle(spawnRadius);
                            var enemy = ecb.CreateEntity(jobIndex, enemyPrefab);
                            ecb.AddComponent(jobIndex, enemy, new Translation { Value = pos });
                            AddMovement(ecb, jobIndex, enemy, rnd);
                            AddEnemyType(ecb, jobIndex, enemy, rnd);
                        }
                    })
                    .Schedule();
            }
        }

        static readonly int MovementEnumCount;
        static readonly int EnemyEnumCount;

        [BurstCompile]
        public static void AddMovement(in EntityCommandBuffer.Concurrent ecb, int jobIndex, in Entity enemy, in Random random)
        {
            var index = (MovementEnum)random.NextInt(0, MovementEnumCount);
            switch (index)
            {
                case MovementEnum.Linear:
                    break;
                case MovementEnum.Spiral:
                    ecb.AddComponent(jobIndex, enemy, new MovementSpiral 
                    { 
                        speed = random.NextFloat(minSpeed, maxSpeed)
                    });
                    break;
            }
        }

        [BurstCompile]
        public static Entity AddEnemyType(in EntityCommandBuffer.Concurrent ecb, int jobIndex, in Entity enemy, in Random random)
        {
            var subEnemy = ecb.CreateEntity(jobIndex, enemyTypePrefab);
            ecb.AddComponent(jobIndex, subEnemy, new Parent { Value = enemy });
            ecb.AddComponent(jobIndex, enemy, new SubObject { Value = subEnemy });

            var index = (EnemyEnum)random.NextInt(0, EnemyEnumCount);
            switch (index)
            {
                case EnemyEnum.Snake:
                    {
                        ecb.AddComponent(jobIndex, subEnemy, new Snake { });
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            return subEnemy;
        }

        static SpawnSystem()
        {
            MovementEnumCount = (int)Enum.GetValues(typeof(MovementEnum)).Cast<MovementEnum>().Max() + 1;
            EnemyEnumCount = (int)Enum.GetValues(typeof(EnemyEnum)).Cast<EnemyEnum>().Max() + 1;
        }
    }
}