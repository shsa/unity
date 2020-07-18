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
        protected override void OnUpdate(in EntityCommandBuffer entityCommandBuffer)
        {
            var ecb = entityCommandBuffer.ToConcurrent();
            Entities
                .WithChangeFilter<Snake>()
                .ForEach((Entity entity, int entityInQueryIndex, in Snake snake, in Translation pos, in MoveForward move) =>
                {
                    var head = entity;
                    for (int i = 0; i < 3; i++)
                    {
                        var tail = ecb.Instantiate(entityInQueryIndex, snake.tailPrefab);
                        ecb.AddComponent(entityInQueryIndex, tail, pos);
                        ecb.AddComponent(entityInQueryIndex, tail, new Chain { head = head, start = pos.Value, finish = pos.Value });
                        ecb.AddComponent(entityInQueryIndex, tail, move);

                        head = tail;
                    }
                })
                .ScheduleParallel();
        }
    }
}