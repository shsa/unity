﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic.World
{
    public class Block
    {
        public static Block[] REGISTER;

        public BlockType id { get; private set; }
        public readonly string name;
        public readonly ModelType model;

        public Block(string name, ModelType model)
        {
            this.name = name;
            this.model = model;
        }

        public virtual Facing GetFront(BlockState state)
        {
            return (Facing)((int)state & 0xF);
        }

        public virtual BlockState GetDefaultState()
        {
            return (int)Facing.South;
        }

        /// <summary>
        /// Called when a block on a side of this block changes is created or is destroyed.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnNeighborChange(NeighborChangeEvent e)
        {
        }

        public virtual void OnBlockPlaced(BlockPlacedEvent e)
        {
        }

        public virtual void OnBlockChange(BlockChangeEvent e)
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

            Register(BlockType.Empty, new BlockEmpty());
            Register(BlockType.Stone, new BlockStone());
            Register(BlockType.Stone1, new BlockRPGMakerTileSet());
        }
    }
}
