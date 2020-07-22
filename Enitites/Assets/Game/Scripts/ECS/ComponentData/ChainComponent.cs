using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct Chain : IComponentData
    {
        public Entity head;
        public float3 finish;
        public float3 finishDir;
        public float3 start;
        public float3 startDir;

        public float3 current;
    }
}