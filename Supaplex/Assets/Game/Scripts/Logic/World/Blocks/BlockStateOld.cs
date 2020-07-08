namespace Game.Logic.World.Blocks
{
    public partial class BlockStateOld
    {
        public Facing Front {
            get {
                if (HasComponent(StateEnum.Front))
                {
                    return GetComponent<FrontComponent>(StateEnum.Front).value;
                }
                return Facing.South;
            }
            set {
               if (Front != value)
                {
                    var component = CreateComponent<FrontComponent>(StateEnum.Front);
                    component.value = value;
                    AddComponent(StateEnum.Front, component);
                }
            }
        }
    }
}
