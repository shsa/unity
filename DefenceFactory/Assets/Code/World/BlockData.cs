using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.World
{
    using BT = System.Int64;

    public enum BlockData : BT
    {
        None = 0
    }

    public static class BlockDataExtension
    {
        public const int BlockIdSize = 16;
        public const BT BlockIdMask = 0xFFFF;

        public static BlockType GetBlockId(this BlockData data)
        {
            return (BlockType)((BT)data & BlockIdMask);
        }

        public static Block GetBlock(this BlockData data)
        {
            return Block.GetBlock(data.GetBlockId());
        }

    }
}
