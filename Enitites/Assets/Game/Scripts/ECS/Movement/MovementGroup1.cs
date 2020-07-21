using Unity.Entities;

namespace Game
{
    [UpdateInGroup(typeof(MovementGroup))]
    [UpdateAfter(typeof(MovementGroup0))]
    public class MovementGroup1 : ComponentSystemGroup
    {
    }
}