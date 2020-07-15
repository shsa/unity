using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

namespace Game
{
    public sealed class MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Translation trans, in Rotation rot, in MoveForward moveForward) =>
            {
                trans.Value += moveForward.speed * deltaTime * math.forward(rot.Value);
            }).ScheduleParallel();
        }
    }
}