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

        public static Block GetBlock(ObjectType objectType)
        {
            return REGISTER[(int)objectType];
        }

        public static void Register(ObjectType objectType, Block block)
        {
            REGISTER[(int)objectType] = block;
        }

        static Block()
        {
            REGISTER = new Block[255];
            Register(ObjectType.Stone, new Stone());
        }
    }
}
