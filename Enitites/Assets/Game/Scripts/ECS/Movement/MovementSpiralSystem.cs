using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;

namespace Game
{
    [UpdateInGroup(typeof(MovementGroup0))]
    public sealed class MovementSpiralSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float3 playerPos = GameManager.GetPlayerPosition();
            float deltaTime = Time.DeltaTime;
            Entities
                .ForEach((ref Translation trans, ref MovementSpiral movement, ref Rotation rot) =>
                {
                    var offset = trans.Value - playerPos;
                    var dir = math.normalize(math.cross(offset, new float3(0, 1, 0)));
                    dir *= 0.5f;
                    var axis = math.normalizesafe(math.cross(offset, dir));
                    var q = quaternion.AxisAngle(axis, 15 * (math.PI / 180));
                    dir = math.rotate(q, dir);
                    trans.Value += dir; ;

                    rot.Value = quaternion.LookRotation(dir, math.up());
                })
                .ScheduleParallel();
        }
    }
}