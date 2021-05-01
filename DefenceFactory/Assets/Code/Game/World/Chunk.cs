﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World
{
    public sealed class Chunk
    {
        public GameWorld World { get; private set; }
        public ChunkPos Position { get; private set; }
        public bool IsDestroyed { get; set; } = false;
        public bool IsChanged { get; set; }

        ImmutableHashSet<BlockPos> hashSet = ImmutableHashSet<BlockPos>.Empty;
        public ConcurrentStack<BlockPos> updateBlock = new ConcurrentStack<BlockPos>();

        BlockData[] data;

        public Chunk(GameWorld world, ChunkPos pos)
        {
            World = world;
            Position = new ChunkPos(pos.x, pos.y, pos.z);
            data = new BlockData[4096]; // 16 * 16 * 16
        }

        public BlockData GetBlockData(in BlockPos pos)
        {
            return data[pos.GetChunkIndex()];
        }

        public void SetBlockData(in BlockPos pos, BlockData value)
        {
            var index = pos.GetChunkIndex();
            var oldValue = data[index];
            if (oldValue != value)
            {
                data[index] = value;
                IsChanged = true;
            }
        }
    }
}
