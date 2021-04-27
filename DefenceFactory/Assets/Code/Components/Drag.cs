using Leopotam.Ecs.Types;

namespace DefenceFactory.Ecs
{
    public enum DragEnum
    {
        None,
        Begin,
        Current
    }

    struct Drag
    {
        public DragEnum State;
        public Float2 Position;
    }
}
