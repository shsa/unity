using Unity.Entities;

namespace Game
{
    [GenerateAuthoringComponent]
    public struct Snake : IComponentData
    {
        public Entity tailPrefab;
    }
}