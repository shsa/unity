using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct TempTranslation : IComponentData
    {
        public float3 Value;
    }
}