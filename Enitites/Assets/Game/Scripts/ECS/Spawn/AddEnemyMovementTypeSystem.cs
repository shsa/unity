using Unity.Entities;
using System;
using Random = Unity.Mathematics.Random;
using System.Linq;
using Unity.Transforms;
using Unity.Mathematics;

namespace Game
{
    public sealed class AddEnemyMovementTypeSystem : EntityCommandBufferSystem
    {
        int MovementEnumCount;

        EnemySpawner setup = null;

        Random mainRandom;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            setup = EnemySpawner.Instance;

            mainRandom = new Random((uint)System.DateTime.Now.Millisecond);
            MovementEnumCount = (int)Enum.GetValues(typeof(MovementEnum)).Cast<MovementEnum>().Max() + 1;
        }

        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            var random = new Random(mainRandom.NextUInt(uint.MinValue, uint.MaxValue));
            var minSpeed = setup.minSpeed;
            var maxSpeed = setup.maxSpeed;
            var movementEnumCount = MovementEnumCount;

            Entities
                .WithAll<EnemyTag>()
                .WithNone<MovementTypeTag>()
                .ForEach((Entity entity, int entityInQueryIndex, ref Rotation rot) =>
                {
                    ecb.AddComponent<MovementTypeTag>(entityInQueryIndex, entity);
                    var i = random.NextInt(0, 1) * 2 - 1; // -1 or 1
                    rot.Value = quaternion.Euler(0, i * math.PI / 2, 0);

                    var index = (MovementEnum)random.NextInt(0, movementEnumCount);
                    index = MovementEnum.Spiral;
                    switch (index)
                    {
                        case MovementEnum.Linear:
                            break;
                        case MovementEnum.Spiral:
                            ecb.AddComponent(entityInQueryIndex, entity, new MovementSpiral
                            {
                                speed = random.NextFloat(minSpeed, maxSpeed)
                            });
                            break;
                    }

                })
                .ScheduleParallel();
        }
    }
}