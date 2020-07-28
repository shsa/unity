using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

namespace Game
{
    [UpdateInGroup(typeof(MovementGroup1))]
    public sealed class MovementSnakeSystem : EntityCommandBufferSystem
    {
        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            float deltaTime = Time.DeltaTime;

            Entities
                .ForEach((ref Snake snake, ref Translation trans) =>
                {
                    snake.time += deltaTime;
                    var step = snake.time * 10f;
                    var dx = math.cos(step) * 0.5f;
                    var offset = new float3(dx, 0, 0);
                    trans.Value = offset;
                })
                .ScheduleParallel();
        }
    }
}