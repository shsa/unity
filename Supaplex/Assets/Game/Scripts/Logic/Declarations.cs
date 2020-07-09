using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public enum Compas
    {
        N = 0,
        NE = 1,
        E = 2,
        SE = 3,
        S = 4,
        SW = 5,
        W = 6,
        NW = 7
    }

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