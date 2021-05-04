using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World
{
    public interface IWorldReader
    {
        Block GetBlock(int x, int y, int z);
    }
}
