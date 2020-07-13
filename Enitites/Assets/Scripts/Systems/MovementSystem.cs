﻿using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

public class MovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<MoveForward>().ForEach((ref Translation trans, ref Rotation rot, ref MoveForward moveForward) =>
        {
            trans.Value += moveForward.speed * Time.DeltaTime * math.forward(rot.Value);
        });
    }
}
