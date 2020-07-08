using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logic.World
{
    public enum BlockData : int
    {
    }

    public enum BlockState: int
    {
    }

    public static class BlockDataExtension
    {
        public static BlockType GetBlockId(this BlockData data)
        {
            return (BlockType)((int)data & 0xFF);
        }

        public static BlockType GetBlockState(this BlockData data)
        {
            return (BlockType)((int)data >> 8);
        }

        public static BlockData GetBlockData(this BlockType blockId, BlockState state)
        {
            return (BlockData)((int)state << 8 | (int)blockId);
        }
    }
}
