using Unity.Entities;

namespace Game
{
    public struct Movement : IComponentData
    {
        public MovementEnum type;
        public float time;
        public float speed;
    }
}