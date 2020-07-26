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
        Entity tailPrefab;
        SnakeSetup setup;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            setup = EnemySpawner.Instance.GetComponent<SnakeSetup>();
            var settings = GameObjectConversionSettings.FromWorld(World, null);
            tailPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(setup.snakeTail, settings);
        }

        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            var _tailPrefab = tailPrefab;
            var tailLength = setup.tailLength;
            Entities
                .WithAll<CreatedTag>()
                .ForEach((Entity entity, int entityInQueryIndex, in Snake snake, in Movement movement) =>
                {
                    ecb.RemoveComponent<CreatedTag>(entityInQueryIndex, entity);
                    ecb.AddComponent(entityInQueryIndex, entity, new MovementSpiral { });

                    var dir = math.normalize(movement.dir);
                    var pos = movement.pos;
                    var time = snake.time;
                    var head = entity;
                    for (int i = 0; i < tailLength; i++)
                    {
                        pos -= dir;
                        time -= 0.1f;

                        var tail = ecb.Instantiate(entityInQueryIndex, _tailPrefab);
                        ecb.AddComponent<EnemyTag>(entityInQueryIndex, tail);
                        ecb.AddComponent(entityInQueryIndex, tail, new Snake 
                        {
                            time = time
                        });
                        ecb.AddComponent(entityInQueryIndex, tail, new Movement 
                        { 
                            speed = movement.speed,
                            pos = pos,
                            dir = dir
                        });
                        ecb.AddComponent(entityInQueryIndex, tail, new Chain 
                        {
                            head = head,
                            pos = pos,
                            dir = dir,
                            dist = 1f
                        });

                        head = tail;
                    }
                })
                .ScheduleParallel();
        }
    }
}