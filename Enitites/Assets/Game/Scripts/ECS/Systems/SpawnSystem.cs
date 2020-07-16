using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;

namespace Game
{
    public sealed class SpawnSystem : SystemBase
    {
        EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;

        public int spawnCount = 100;
        public float spawnRadius = 20;
        public float spawnInterval = 3;
        public float minSpeed = 3;
        public float maxSpeed = 5;

        public Entity[] enemyPrefabs;

        float lastSpawnTime = 0;

        protected override void OnCreate()
        {
            base.OnCreate();
            endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            lastSpawnTime -= Time.DeltaTime;
            if (lastSpawnTime <= 0)
            {
                lastSpawnTime = spawnInterval;

                var ecb = endSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
                var randomPos = new NativeArray<float3>(spawnCount, Allocator.TempJob);
                var randomSpeeds = new NativeArray<float>(spawnCount, Allocator.TempJob);
                var randomEnemy = new NativeArray<Entity>(spawnCount, Allocator.TempJob);
                for (int i = 0; i < spawnCount; i++)
                {
                    randomPos[i] = RandomPointOnCircle(spawnRadius);
                    randomSpeeds[i] = UnityEngine.Random.Range(minSpeed, maxSpeed);
                    randomEnemy[i] = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
                }
                var _spawnCount = spawnCount;
                Dependency = Job
                    .WithDeallocateOnJobCompletion(randomPos)
                    .WithDeallocateOnJobCompletion(randomSpeeds)
                    .WithDeallocateOnJobCompletion(randomEnemy)
                    .WithCode(() =>
                    {
                        var index = 0;
                        for (int i = 0; i < _spawnCount; i++)
                        {
                            index++;
                            var enemy = ecb.Instantiate(index, randomEnemy[0]);
                            ecb.AddComponent(index, enemy, new Translation { Value = randomPos[i] });
                            ecb.AddComponent(index, enemy, new MoveForward { speed = randomSpeeds[i] });
                            ecb.AddComponent(index, enemy, new Lifetime { Value = 5 });
                            ecb.AddComponent<EnemyTag>(index, enemy);
                        }
                    })
                    .Schedule(Dependency);

                endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
            }
        }

        float3 RandomPointOnCircle(float spawnRaduis)
        {
            var pos = UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;
            return new float3(pos.x, 0, pos.y) + (float3)GameManager.GetPlayerPosition();
        }
    }
}