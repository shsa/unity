using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Game.Logic.World
{
    using BT = System.Int64;

    public enum BlockData : BT
    {
        None = 0
    }

    public enum BlockState: BT
    {
        None = 0
    }

    public static class BlockDataExtension
    {
        public const int BlockIdSize = 16;
        public const BT BlockIdMask = 0xFFFF;

        public static Block GetBlock(this BlockData data)
        {
            return Block.GetBlock(data.GetBlockId());
        }

        public static Block GetBlock(this BlockType blockId)
        {
            return Block.GetBlock(blockId);
        }

        public static BlockType GetBlockId(this BlockData data)
        {
            return (BlockType)((BT)data & BlockIdMask);
        }

        public static BlockState GetBlockState(this BlockData data)
        {
            return (BlockState)((BT)data >> BlockIdSize);
        }

        public static BlockData GetBlockData(this BlockType blockId)
        {
            return GetBlockData(blockId, Block.GetBlock(blockId).GetDefaultState());
        }

        public static BlockData GetBlockData(this BlockType blockId, BlockState state)
        {
            return (BlockData)((BT)state << BlockIdSize | (BT)blockId);
        }

        public static bool IsEmpty(this BlockType blockId)
        {
            switch (blockId)
            {
                case BlockType.Empty:
                case BlockType.None:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsEmpty(this BlockData blockData)
        {
            return IsEmpty(blockData.GetBlockId());
        }
    }
}
