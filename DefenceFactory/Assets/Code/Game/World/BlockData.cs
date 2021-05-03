using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World
{
    using Meta = Int32;

    public struct BlockData
    {
        public BlockType id;
        public Meta meta;
    }

    public static class BlockDataExtension
    {
        public static BlockType GetBlockId(this BlockData data)
        {
            return data.id;
        }

        public static Block GetBlock(this BlockData data)
        {
            return Block.GetBlock(data.GetBlockId());
        }

    }

    public static class BlockTypeExtension
    {
        public static BlockData GetBlockData(this BlockType blockId)
        {
            return new BlockData
            {
                id = blockId,
                meta = 0
            };
        }

        public static BlockData GetBlockData(this BlockType blockId, Int16 meta)
        {
            return new BlockData
            {
                id = blockId,
                meta = meta
            };
        }

        public static BlockData GetBlockData(this BlockType blockId, DirectionSet dirs)
        {
            return new BlockData
            {
                id = blockId,
                meta = (Meta)dirs
            };
        }

        public static Meta GetMeta(this BlockData blockData)
        {
            return blockData.meta;
        }
    }
}
