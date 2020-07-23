using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct Movement : IComponentData
    {
        public MovementEnum type;
        public float speed;
        public float3 pos;
        public float3 dir;
    }
}