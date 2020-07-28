using Unity.Entities;
using Unity.Collections;
using System;
using Random = Unity.Mathematics.Random;
using System.Linq;

namespace Game
{
    public sealed class AddEnemyMovementTypeSystem : EntityCommandBufferSystem
    {
        static readonly int MovementEnumCount;

        EnemySpawner setup = null;

        Random mainRandom;

        [ReadOnly]
        static float minSpeed;
        [ReadOnly]
        static float maxSpeed;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            setup = EnemySpawner.Instance;

            minSpeed = setup.minSpeed;
            maxSpeed = setup.maxSpeed;

            mainRandom = new Random(1);
        }

        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            var random = new Random(mainRandom.NextUInt(uint.MinValue, uint.MaxValue));
            
            Entities
                .WithAll<EnemyTag>()
                .WithNone<MovementTypeTag>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.AddComponent<MovementTypeTag>(entityInQueryIndex, entity);

                    var index = (MovementEnum)random.NextInt(0, MovementEnumCount);
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

        static AddEnemyMovementTypeSystem()
        {
            MovementEnumCount = (int)Enum.GetValues(typeof(MovementEnum)).Cast<MovementEnum>().Max() + 1;
        }
    }
}