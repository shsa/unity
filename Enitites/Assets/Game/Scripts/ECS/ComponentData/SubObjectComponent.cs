using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct SubObject : IComponentData
    {
        public Entity Value;
    }
}