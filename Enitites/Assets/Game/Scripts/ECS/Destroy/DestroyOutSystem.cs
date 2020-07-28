using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;

namespace Game
{
    public sealed class DestroyOutSystem : EntityCommandBufferSystem
    {
        private float _maxDistanceSq = 10000;
        private float _maxDistance = 100;
        public float maxDistance {
            get {
                return _maxDistance;
            }
            set {
                _maxDistance = value;
                _maxDistanceSq = value * value;
            }
        }

        protected override void OnUpdate(EntityCommandBuffer.Concurrent ecb)
        {
            float3 playerPos = GameManager.GetPlayerPosition();
            var maxDistanceSq = _maxDistanceSq;

            Entities
                .ForEach((Entity entity, int entityInQueryIndex, in Translation trans) =>
                {
                    if (math.lengthsq(playerPos - trans.Value) >= maxDistanceSq)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                })
                .ScheduleParallel();
        }
    }
}