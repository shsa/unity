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
                .WithAll<Snake, CreatedTag>()
                .ForEach((Entity entity, int entityInQueryIndex, in Translation pos) =>
                {
                    ecb.RemoveComponent<CreatedTag>(entityInQueryIndex, entity);

                    var head = entity;
                    for (int i = 0; i < 3; i++)
                    {
                        var tail = ecb.Instantiate(entityInQueryIndex, _tailPrefab);
                        ecb.AddComponent(entityInQueryIndex, tail, pos);
                        ecb.AddComponent(entityInQueryIndex, tail, new Chain { head = head, start = pos.Value, finish = pos.Value });
                        //ecb.AddComponent(entityInQueryIndex, tail, move);
                        //ecb.AddComponent(entityInQueryIndex, tail, )

                        head = tail;
                    }
                })
                .ScheduleParallel();
        }
    }
}