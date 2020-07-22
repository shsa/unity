using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct MovementSpiral : IComponentData
    {
        public float time;
        public float speed;
        public float angle;
        public float3 pos;
    }
}