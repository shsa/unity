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

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            var setup = EnemySpawner.Instance.GetComponent<SnakeSetup>();
            var settings = GameObjectConversionSettings.FromWorld(World, null);
            tailPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(setup.snakeTail, settings);
        }

        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            var _tailPrefab = tailPrefab;
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
                    for (int i = 0; i < 0; i++)
                    {
                        var tail = ecb.Instantiate(entityInQueryIndex, _tailPrefab);
                        ecb.AddComponent(entityInQueryIndex, tail, new Snake 
                        {
                            time = time
                        });
                        pos -= dir;
                        time -= 0.01f;
                        ecb.AddComponent(entityInQueryIndex, tail, new Movement { 
                            speed = movement.speed,
                            pos = pos,
                            dir = dir
                        });
                        ecb.AddComponent(entityInQueryIndex, tail, new Chain { head = head });

                        head = tail;
                    }
                })
                .ScheduleParallel();
        }
    }
}