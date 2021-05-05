using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World
{
    public enum Meta : long
    {
        None = 0
    }

    public struct BlockData
    {
        public BlockType id;
        public Meta meta;
    }

    public static class BlockDataExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlockType GetBlockId(this BlockData data)
        {
            return data.id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Block GetBlock(this BlockData data)
        {
            return Block.GetBlock(data.id);
        }

    }

    public static class BlockTypeExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlockData GetBlockData(this BlockType blockId)
        {
            return new BlockData
            {
                id = blockId,
                meta = 0
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlockData GetBlockData(this BlockType blockId, Int16 meta)
        {
            return new BlockData
            {
                id = blockId,
                meta = (Meta)meta
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlockData GetBlockData(this BlockType blockId, DirectionSet dirs)
        {
            return new BlockData
            {
                id = blockId,
                meta = (Meta)dirs
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Meta GetMeta(this BlockData blockData)
        {
            return blockData.meta;
        }
    }
}
