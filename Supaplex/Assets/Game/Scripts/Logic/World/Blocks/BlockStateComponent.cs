using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logic.World.Blocks
{
    public enum StateEnum
    {
        Front
    }

    public class BlockStateComponent
    {
    }

    public class FrontComponent : BlockStateComponent
    {
        public Facing value;
    }
}
