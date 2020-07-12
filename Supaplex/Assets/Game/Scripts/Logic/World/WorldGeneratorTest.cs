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
            pos.Set(chunk.position);
            if ((pos.x >> 4) == 0 && (pos.y >> 4) == 0 && (pos.z >> 4) == 0)
            {
                var blockId = BlockType.Stone;
                pos.Set(e.chunk.position);
                pos.Add(1, 1, 1);
                chunk.SetBlockData(pos, blockId.GetBlockData());
                pos.Set(e.chunk.position);
                pos.Add(2, 1, 1);
                chunk.SetBlockData(pos, blockId.GetBlockData());
                pos.Set(e.chunk.position);
                pos.Add(2, 2, 1);
                chunk.SetBlockData(pos, blockId.GetBlockData());
                pos.Set(e.chunk.position);
                pos.Add(1, 2, 1);
                chunk.SetBlockData(pos, blockId.GetBlockData());
            }
        }
    }
}
