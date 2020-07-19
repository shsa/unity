using Unity.Entities;

namespace Game
{
    public struct MovementSpiral : IComponentData
    {
        public float time;
        public float speed;
        public float angle;
    }
}