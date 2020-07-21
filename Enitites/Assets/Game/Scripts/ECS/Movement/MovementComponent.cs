using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct Movement : IComponentData
    {
        public MovementEnum type;
        public float time;
        public float speed;
        public float3 offset;
        public quaternion rotation;
    }
}