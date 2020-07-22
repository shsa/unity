using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;

namespace Game
{
    [UpdateInGroup(typeof(MovementGroup1))]
    public sealed class MovementSnakeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            var up = new float3(0, 1, 0);
            var dir = new float3(0, 0, 0.5f);
            Entities
                .ForEach((ref Snake snake, ref Translation trans, ref Rotation rot) =>
                {
                    snake.step += deltaTime * 10f;
                    var dx = math.cos(snake.step);
                    var offset = new float3(dx, 0, 0);
                    offset = math.rotate(rot.Value, offset);
                    trans.Value += offset;
                    //rot.Value = math.mul(q, rot.Value);
                })
                .ScheduleParallel();
        }
    }
}