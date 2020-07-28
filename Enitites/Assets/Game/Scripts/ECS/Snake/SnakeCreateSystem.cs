using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using System;
using Random = Unity.Mathematics.Random;

namespace Game
{
    public sealed class SnakeCreateSystem : EntityCommandBufferSystem
    {
        static readonly float3 front = new float3(0, 0, 1);

        SnakeSetup setup;

        [ReadOnly]
        static Entity headPrefab;
        [ReadOnly]
        static Entity tailPrefab;
        [ReadOnly]
        static int tailLength;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            setup = EnemySpawner.Instance.GetComponent<SnakeSetup>();
            var settings = GameObjectConversionSettings.FromWorld(World, null);
            headPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(setup.headPrefab, settings);
            tailPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(setup.tailPrefab, settings);
            tailLength = setup.tailLength;
        }

        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            var cdfePos = GetComponentDataFromEntity<Translation>(true);
            Entities
                .WithNone<CreatedTag>()
                .WithReadOnly(cdfePos)
                .ForEach((Entity entity, int entityInQueryIndex, ref Snake snake, in Parent parent, in Rotation rot) =>
                {
                    var time = 0.0f;

                    var dir = math.rotate(rot.Value, front);

                    var headModel = ecb.Instantiate(entityInQueryIndex, headPrefab);
                    ecb.AddComponent(entityInQueryIndex, headModel, new Translation { Value = float3.zero });
                    ecb.AddComponent(entityInQueryIndex, headModel, new Rotation { Value = quaternion.identity });
                    ecb.AddComponent(entityInQueryIndex, headModel, new Parent { Value = entity });
                    ecb.AddComponent(entityInQueryIndex, headModel, new LocalToParent { });

                    snake.time = time;
                    ecb.AddComponent<CreatedTag>(entityInQueryIndex, entity);
                    ecb.AddComponent(entityInQueryIndex, entity, new Model { Value = headModel });
                    ecb.AddComponent<EntityAliveTag>(entityInQueryIndex, entity);

                    var pos = cdfePos[parent.Value].Value;
                    var chainParent = parent.Value;
                    for (int i = 0; i < tailLength; i++)
                    {
                        time -= 0.1f;
                        pos -= dir * 1f;

                        var chain = ecb.CreateEntity(entityInQueryIndex, SpawnSystem.enemyPrefab);
                        var tail = ecb.CreateEntity(entityInQueryIndex, AddEnemyTypeSystem.enemyTypePrefab);
                        var tailModel = ecb.Instantiate(entityInQueryIndex, tailPrefab);

                        ecb.AddComponent(entityInQueryIndex, chainParent, new ChainChild { Value = chain });

                        ecb.AddComponent(entityInQueryIndex, chain, new Translation { Value = pos });
                        ecb.AddComponent(entityInQueryIndex, chain, new Rotation { Value = rot.Value });
                        ecb.AddComponent(entityInQueryIndex, chain, new Chain { dist = 1f });
                        ecb.AddComponent(entityInQueryIndex, chain, new ChainParent { Value = chainParent });
                        ecb.AddComponent(entityInQueryIndex, chain, new EnemyType { Value = tail });
                        ecb.AddComponent<EntityAliveTag>(entityInQueryIndex, chain);
                        ecb.AddComponent<MovementTypeTag>(entityInQueryIndex, chain);

                        ecb.AddComponent<CreatedTag>(entityInQueryIndex, tail);
                        ecb.AddComponent(entityInQueryIndex, tail, new Parent { Value = chain });
                        ecb.AddComponent(entityInQueryIndex, tail, new Snake { time = time });
                        ecb.AddComponent(entityInQueryIndex, tail, new Model { Value = tailModel });
                        ecb.AddComponent<EntityAliveTag>(entityInQueryIndex, tail);
                        ecb.AddComponent<CreatedTag>(entityInQueryIndex, tail);

                        ecb.AddComponent(entityInQueryIndex, tailModel, new Translation { Value = float3.zero });
                        ecb.AddComponent(entityInQueryIndex, tailModel, new Rotation { Value = quaternion.identity });
                        ecb.AddComponent(entityInQueryIndex, tailModel, new Parent { Value = tail });
                        ecb.AddComponent(entityInQueryIndex, tailModel, new LocalToParent { });

                        chainParent = chain;
                    }
                })
                .ScheduleParallel();
        }
    }
}