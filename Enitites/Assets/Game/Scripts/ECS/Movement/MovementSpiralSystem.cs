using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

namespace Game
{
    public sealed class MovementSpiralSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities
                .ForEach((ref Translation trans, ref MovementSpiral movement, in Rotation rot) =>
                {
                    //trans.Value += moveForward.speed * deltaTime * math.forward(rot.Value);
                    movement.time += deltaTime;
                    //movement.angle += 0.001f;
                    var p = 1.0f / (2 * math.PI) * movement.angle;
                    trans.Value = new float3(p * math.cos(movement.angle), 0, p * math.sin(movement.angle));
                })
                .ScheduleParallel();
        }
    }
}