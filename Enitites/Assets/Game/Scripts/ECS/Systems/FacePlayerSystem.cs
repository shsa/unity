using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

namespace Game
{
    public class FacePlayerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            return;
            float3 playerPos = (float3)GameManager.GetPlayerPosition();

            Entities.WithAll<EnemyTag>().ForEach((Entity entity, ref Translation trans, ref Rotation rot) =>
            {
                float3 direction = playerPos - trans.Value;
                direction.y = 0f;

                rot.Value = quaternion.LookRotation(direction, math.up());
            }).ScheduleParallel();
        }
    }
}