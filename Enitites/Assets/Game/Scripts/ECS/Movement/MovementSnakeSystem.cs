using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

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
            var pi_2 = math.PI / 2;
            Entities
                .ForEach((ref Snake snake, ref Translation trans, ref Rotation rot) =>
                {
                    snake.angle += snake.step;
                    if (snake.angle < -pi_2)
                    {
                        snake.angle = -pi_2 - (snake.angle + pi_2);
                        snake.step = -snake.step;
                    }
                    else if (snake.angle > pi_2)
                    {
                        snake.angle = pi_2 - (snake.angle - pi_2);
                        snake.step = -snake.step;
                    }

                    var q = quaternion.AxisAngle(up, snake.angle);
                    var offset = math.rotate(q, dir);
                    offset = math.rotate(rot.Value, offset);
                    trans.Value += offset;
                })
                .ScheduleParallel();
        }
    }
}