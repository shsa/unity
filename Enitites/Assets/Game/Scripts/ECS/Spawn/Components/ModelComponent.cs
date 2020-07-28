using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct Model : ISystemStateComponentData
    {
        public Entity Value;
    }
}