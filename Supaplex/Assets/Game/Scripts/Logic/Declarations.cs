using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public enum ObjectType
    {
        Empty,
        Wall,
        Stone
    }

    public enum ObjectState
    {
        None,
        Init,
        Move
    }
}