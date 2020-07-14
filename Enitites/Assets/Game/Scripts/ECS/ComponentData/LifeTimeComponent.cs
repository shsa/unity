using Unity.Entities;

namespace Game
{
    [GenerateAuthoringComponent]
    public struct Lifetime : IComponentData
    {
        public float Value;
    }
}