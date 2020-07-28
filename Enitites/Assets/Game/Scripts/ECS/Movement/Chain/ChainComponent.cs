using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct Chain : IComponentData
    {
        public float dist;
    }

    public struct ChainParent : IComponentData
    {
        public Entity Value;
    }

    public struct ChainChild : IComponentData
    {
        public Entity Value;
    }
}