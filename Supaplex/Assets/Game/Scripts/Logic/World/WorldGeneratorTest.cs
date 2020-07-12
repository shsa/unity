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

        public override void Generate(ChunkGenerateEvent e)
        {
            var pos = e.pos;
            var chunk = e.chunk;
            var blockId = BlockType.Stone1;
            pos.Set(1, 1, 1);
            chunk.SetBlockData(pos, blockId.GetBlockData());
            pos.Set(2, 1, 1);
            chunk.SetBlockData(pos, blockId.GetBlockData());
            pos.Set(2, 2, 1);
            chunk.SetBlockData(pos, blockId.GetBlockData());
            pos.Set(1, 2, 1);
            chunk.SetBlockData(pos, blockId.GetBlockData());
        }
    }
}
