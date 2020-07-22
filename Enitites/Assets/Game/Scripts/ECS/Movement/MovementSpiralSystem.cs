using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using System.Diagnostics;

namespace Game
{
    [UpdateInGroup(typeof(MovementGroup0))]
    public sealed class MovementSpiralSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float3 playerPos = GameManager.GetPlayerPosition();
            float deltaTime = Time.DeltaTime;
            var up = new float3(0, 1, 0);
            Entities
                .ForEach((ref Translation trans, ref MovementSpiral movement, ref Rotation rot) =>
                {
                    var speed = 0.1f; // movement.speed
                    var step = 1f;
                    var offset = playerPos - trans.Value;
                    var l2 = math.length(offset) * 2;
                    step = step / (math.PI * l2 / step);
                    var a = math.acos(speed / l2);
                    var q = quaternion.AxisAngle(up, a);
                    offset = math.normalize(offset);
                    var dir = math.rotate(q, offset);
                    offset = dir * speed + offset * step;

                    trans.Value += offset;

                    rot.Value = quaternion.LookRotation(offset, math.up());
                })
                .ScheduleParallel();
        }
    }
}