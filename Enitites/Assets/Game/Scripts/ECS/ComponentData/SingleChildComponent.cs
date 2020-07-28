using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct SingleChild : IComponentData
    {
        public Entity Value;
    }
}