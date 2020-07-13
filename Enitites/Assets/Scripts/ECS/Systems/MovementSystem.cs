﻿using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.WithAll<EnemyTag>().ForEach((ref Translation trans, ref Rotation rot, ref MoveForward moveForward) =>
        {
            trans.Value += moveForward.speed * deltaTime * math.forward(rot.Value);
        }).ScheduleParallel();
    }
}
