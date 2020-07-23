using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct Chain : IComponentData
    {
        public Entity head;
        public float3 pos;
        public float3 dir;
    }
}