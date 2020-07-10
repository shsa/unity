using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logic.World
{
    public class WorldGeneratorTest : WorldGenerator
    {
        public WorldGeneratorTest(int seed, int depth) : base(seed, depth)
        {
        }

        public override BlockType CalcBlockId(BlockPos pos)
        {
            //if ((pos.x & 0xF) == 8 && (pos.y & 0xF) == 8 && (pos.z & 0xF) == 0)
            if ((pos.x & 0xF) == 0 && (pos.y & 0xF) == 0 && (pos.z & 0xF) == 0)
            {
                return BlockType.Stone1;
            }
            return BlockType.Empty;
        }
    }
}
