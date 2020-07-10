using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public enum BlockType
    {
        None,
        Empty,
        OffsetDown,
        OffsetSouth,
        OffsetWest,
        Wall,
        Stone,
        Stone1,
        Stone2x2,
        Stone3x3,
        Stone4x4
    }

    public enum ObjectState
    {
        None,
        Init,
        Move
    }
}