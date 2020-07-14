using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using System.Threading;

namespace Game
{
    public class LifetimeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity, ref Lifetime lifetime) =>
                {
                    lifetime.Value -= deltaTime;
                    if (lifetime.Value <= 0)
                    {
                        EntityManager.DestroyEntity(entity);
                    }
                }).Run();
        }
    }
}