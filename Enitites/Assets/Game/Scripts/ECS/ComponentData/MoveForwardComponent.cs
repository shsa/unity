using Unity.Entities;

namespace Game
{
    [GenerateAuthoringComponent]
    public struct MoveForward : IComponentData
    {
        public float speed;
    }
}