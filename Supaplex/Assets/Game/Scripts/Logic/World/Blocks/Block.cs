using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic.World.Blocks
{
    public class Block
    {
        static Block[] REGISTER;

        public BlockType id { get; private set; }
        public readonly string name;

        public Block(string name)
        {
            this.name = name;
        }

        public virtual Facing GetFront(int state)
        {
            return (Facing)(state & 0xF);
        }

        public virtual int GetDefaultState()
        {
            return (int)Facing.South;
        }

        public static BlockType GetBlockID(int metadata)
        {
            return (BlockType)(metadata & 0xFF);
        }

        public static int GetState(int metadata)
        {
            return metadata >> 8;
        }

        public static int GetMetadata(BlockType blockId, int state)
        {
            return state << 8 | (int)blockId;
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
            Register(BlockType.Stone, new Stone());
        }
    }
}
