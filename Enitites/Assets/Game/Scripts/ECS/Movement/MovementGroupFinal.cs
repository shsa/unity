using Unity.Entities;

namespace Game
{
    [UpdateInGroup(typeof(MovementGroup), OrderLast = true)]
    [UpdateAfter(typeof(MovementGroup1))]
    public class MovementGroupFinal : ComponentSystemGroup
    {
    }
}