using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace DefenceFactory.Game.World
{
    public enum ChunkFlag
    {
        None        = 0x00,
        Load        = 0x01,
        Redraw      = 0x02,
        Destroy     = 0x04,

        Thread      = 0x10,
        Generate    = 0x12,
        Update      = 0x14,

        ThreadAll   = Thread | Generate | Update,
        NotThreadAll = ~ThreadAll
    }

    public sealed class Chunk : IDisposable
    {
        public GameWorld World { get; private set; }
        public int x;
        public int y;
        public int z;
        public ChunkFlag flag;

        public HashSet<BlockPos> updateBlocks2 = new HashSet<BlockPos>();
        public ConcurrentStack<BlockPos> updateBlock = new ConcurrentStack<BlockPos>();

        public NativeArray<BlockData> data = default;

        public BlockPos min { get; private set; }
        public BlockPos max { get; private set; }

        public Chunk(GameWorld world, BlockPos pos)
        {
            World = world;
            x = (pos.x >> 4) << 4;
            y = (pos.y >> 4) << 4; 
            z = (pos.z >> 4) << 4;

            min = new BlockPos(x, y, z);
            max = new BlockPos(x | 0xF, y | 0xF, z | 0xF);

            data = new NativeArray<BlockData>(4096, Allocator.Persistent); // 16 * 16 * 16
        }

        ~Chunk()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (data != default)
            {
                data.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBlockIndex(int x, int y, int z)
        {
            return ((y & 0xF) << 8) | ((x & 0xF) << 4) | (z & 0xF);
        }

        public BlockData GetBlockData(int x, int y, int z)
        {
            return data[GetBlockIndex(x, y, z)];
        }

        public BlockData GetBlockData(in BlockPos pos)
        {
            return data[pos.GetBlockIndex()];
        }

        public void SetBlockData(in BlockPos pos, in BlockData value)
        {
            var index = pos.GetBlockIndex();
            var oldValue = data[index];
            if (oldValue.id != value.id || oldValue.meta != value.meta)
            {
                data[index] = value;
                flag |= ChunkFlag.Redraw;
            }
        }

        public BlockFlag GetFlag(int x, int y, int z)
        {
            return data[GetBlockIndex(x, y, z)].flag;
        }

        void ProcessFlag(BlockFlag value)
        {
            if ((value & BlockFlag.Update) == BlockFlag.Update)
            {
                flag |= ChunkFlag.Update;
            }
        }

        public void AddFlag(int x, int y, int z, BlockFlag value)
        {
            var index = GetBlockIndex(x, y, z);
            var item = data[index];
            item.flag |= value;
            data[index] = item;
            ProcessFlag(value);
        }

        public void SetFlag(int x, int y, int z, BlockFlag value)
        {
            var index = GetBlockIndex(x, y, z);
            var item = data[index];
            item.flag = value;
            data[index] = item;
            ProcessFlag(value);
        }

        public bool Equals(int x, int y, int z)
        {
            return (this.x >> 4) == (x >> 4) && (this.y >> 4) == (y >> 4) && (this.z >> 4) == (z >> 4);
        }

        public bool Equals(in BlockPos pos)
        {
            return Equals(pos.x, pos.y, pos.z);
        }
    }
}
