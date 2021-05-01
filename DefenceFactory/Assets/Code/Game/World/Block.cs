using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World
{
    public class Block
    {
        public static Block[] REGISTER;

        public BlockType id { get; private set; }
        public string name { get; private set; }
        public Model model { get; private set; }

        public Block(string name, Model model)
        {
            this.name = name;
            this.model = model;
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

            Register(BlockType.Empty, new Blocks.BlockEmpty());
            Register(BlockType.Stone, new Blocks.BlockTileSet(ModelEnum.Stone));
        }
    }
}
