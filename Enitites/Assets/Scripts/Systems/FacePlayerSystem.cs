using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

public class FacePlayerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        float3 playerPos = (float3)GameManager.GetPlayerPosition();

        Entities.WithAll<EnemyTag>().ForEach((Entity entity, ref Translation trans, ref Rotation rot) =>
        {
            float3 direction = playerPos - trans.Value;
            direction.y = 0f;

            rot.Value = quaternion.LookRotation(direction, math.up());
        });
    }
}
