using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    [GenerateAuthoringComponent]
    public struct EnemyPartTag : IComponentData
    {
        public Entity parent;
        public float3 pos;
    }
}