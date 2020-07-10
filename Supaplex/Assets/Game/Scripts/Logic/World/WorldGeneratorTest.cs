using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logic.World
{
    public class WorldGeneratorTest : WorldGenerator
    {
        public WorldGeneratorTest(int seed) : base(seed)
        {
        }

        public override BlockType CalcBlockId(BlockPos pos)
        {
            var x = pos.x & 0xF;
            var y = pos.y & 0xF;
            var z = pos.z & 0xF;
            //if ((pos.x & 0xF) == 8 && (pos.y & 0xF) == 8 && (pos.z & 0xF) == 0)
            if (x == 1 && y == 1 && z == 1)
            {
                return BlockType.Stone1;
            }
            if (x == 2 && y == 1 && z == 1)
            {
                return BlockType.Stone1;
            }
            if (x == 2 && y == 2 && z == 1)
            {
                return BlockType.Stone1;
            }
            if (x == 1 && y == 2 && z == 1)
            {
                return BlockType.Stone1;
            }
            return BlockType.Empty;
        }
    }
}
