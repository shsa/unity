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

        public Entity enemyPartPrefab;
        public EntityArchetype enemyPrefab;

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
                for (int i = 0; i < spawnCount; i++)
                {
                    randomPos[i] = RandomPointOnCircle(spawnRadius);
                    randomSpeeds[i] = UnityEngine.Random.Range(minSpeed, maxSpeed);
                }

                var _spawnCount = spawnCount;
                var _enemyPrefab = enemyPrefab;
                var _enemyPartPrefab = enemyPartPrefab;
                Dependency = Job
                    .WithDeallocateOnJobCompletion(randomPos)
                    .WithDeallocateOnJobCompletion(randomSpeeds)
                    .WithCode(() => 
                    {
                        var index = 0;
                        for (int i = 0; i < _spawnCount; i++)
                        {
                            index++;
                            var enemy = ecb.CreateEntity(index, _enemyPrefab);
                            ecb.AddComponent(index, enemy, new Translation { Value = randomPos[i] });
                            ecb.AddComponent(index, enemy, new MoveForward { speed = randomSpeeds[i] });
                            ecb.AddComponent(index, enemy, new Lifetime { Value = 5 });
                            ecb.AddComponent<EnemyTag>(index, enemy);

                            index++;
                            var part = ecb.Instantiate(index, _enemyPartPrefab);
                            ecb.AddComponent(index, part, new EnemyPartTag { });
                            ecb.AddComponent(index, part, new Parent { Value = enemy });
                            ecb.AddComponent(index, part, new LocalToParent { });
                            ecb.AddComponent(index, part, new Translation { Value = new float3(0, 0, 0) });
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