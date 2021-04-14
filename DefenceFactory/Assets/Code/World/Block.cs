using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.World
{
    public class Block
    {
        public static Block[] REGISTER;

        public BlockType id { get; private set; }

        public Block(string name)
        {

        }

        public static Block GetBlock(BlockType objectType)
        {
            return REGISTER[(int)objectType];
        }

        public static void Register(BlockType blockType, Block block)
        {
            REGISTER[(int)blockType] = block;
            block.id = blockType;
        }

        static Block()
        {
            var count = (int)Enum.GetValues(typeof(BlockType)).Cast<BlockType>().Max() + 1;
            REGISTER = new Block[count];

            //Register(BlockType.Empty, new BlockEmpty());
            //Register(BlockType.Stone, new BlockStone());
            //Register(BlockType.Rock, new BlockRPGMakerTileSet("Rock"));
            //Register(BlockType.Masonry, new BlockRPGMakerTileSet("Masonry"));
            //Register(BlockType.Cobblestone, new BlockRPGMakerTileSet("Cobblestone"));
        }
    }
}
