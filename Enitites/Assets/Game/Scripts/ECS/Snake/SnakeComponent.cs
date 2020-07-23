using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    [GenerateAuthoringComponent]
    public struct Snake : IComponentData
    {
        public Entity head;
        public float3 pos;
        public float3 dir;
        public float time;
    }
}