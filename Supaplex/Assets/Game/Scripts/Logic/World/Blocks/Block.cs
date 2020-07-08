using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic.World.Blocks
{
    public class Block
    {
        static Block[] REGISTER;

        public string texture;
        public string normalMap;
        public string heightMap;

        public static Block GetBlock(BlockType objectType)
        {
            return REGISTER[(int)objectType];
        }

        public static void Register(BlockType objectType, Block block)
        {
            REGISTER[(int)objectType] = block;
        }

        static Block()
        {
            var count = (int)Enum.GetValues(typeof(BlockType)).Cast<BlockType>().Max() + 1;
            REGISTER = new Block[count];
            Register(BlockType.Stone, new Stone());
        }
    }
}
