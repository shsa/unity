using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public enum ObjectType
    {
        None = 0,
        Empty = 1,
        Wall = 2,
        Stone = 3
    }

    public enum ObjectState
    {
        None,
        Init,
        Move
    }
}