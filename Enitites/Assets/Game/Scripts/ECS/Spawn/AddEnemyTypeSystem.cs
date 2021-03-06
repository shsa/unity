﻿using Unity.Entities;
using Unity.Collections;
using System;
using Random = Unity.Mathematics.Random;
using System.Linq;
using Unity.Transforms;

namespace Game
{
    public sealed class AddEnemyTypeSystem : EntityCommandBufferSystem
    {
        public static EntityArchetype enemyTypePrefab;

        EnemySpawner setup = null;
        Random mainRandom;
        int EnemyEnumCount;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            setup = EnemySpawner.Instance;

            enemyTypePrefab = EntityManager.CreateArchetype(
                typeof(Translation),
                typeof(Rotation),
                typeof(LocalToWorld),
                typeof(LocalToParent),
                typeof(Parent)
                );

            mainRandom = new Random(1);
            EnemyEnumCount = (int)Enum.GetValues(typeof(EnemyEnum)).Cast<EnemyEnum>().Max() + 1;
        }

        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            var random = new Random(mainRandom.NextUInt(uint.MinValue, uint.MaxValue));
            var enemyTypeArchetype = enemyTypePrefab;
            var enemyEnumCount = EnemyEnumCount;

            Entities
                .WithAll<EnemyTag>()
                .WithNone<EnemyType>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    var enemyType = ecb.CreateEntity(entityInQueryIndex, enemyTypeArchetype);

                    ecb.AddComponent(entityInQueryIndex, enemyType, new Parent { Value = entity });
                    ecb.AddComponent(entityInQueryIndex, entity, new EnemyType { Value = enemyType });
                    ecb.AddComponent<EntityAliveTag>(entityInQueryIndex, entity);

                    var index = (EnemyEnum)random.NextInt(0, enemyEnumCount);
                    switch (index)
                    {
                        case EnemyEnum.Snake:
                            {
                                ecb.AddComponent(entityInQueryIndex, enemyType, new Snake { });
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                })
                .ScheduleParallel();
        }
    }
}