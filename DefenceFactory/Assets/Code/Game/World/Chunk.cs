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
    public sealed class Chunk : IDisposable
    {
        static int _index = 0;

        public int index { get; }
        public int count = 0;
        public GameWorld World { get; private set; }
        public int x;
        public int y;
        public int z;
        public ChunkFlag flag;

        public HashSet<BlockPos> updateBlocks2 = new HashSet<BlockPos>();
        public ConcurrentStack<BlockPos> updateBlock = new ConcurrentStack<BlockPos>();

        public BlockData[] data = default;
        public BlockFlag[] data_update = default;

        public BlockPos min { get; private set; }
        public BlockPos max { get; private set; }

        public Chunk(GameWorld world, BlockPos pos)
        {
            index = _index++;
            World = world;
            x = (pos.x >> 4) << 4;
            y = (pos.y >> 4) << 4; 
            z = (pos.z >> 4) << 4;

            min = new BlockPos(x, y, z);
            max = new BlockPos(x | 0xF, y | 0xF, z | 0xF);

            data = new BlockData[4096]; // 16 * 16 * 16
            data_update = new BlockFlag[4096];
        }

        ~Chunk()
        {
            Dispose();
        }

        public void Dispose()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetDataIndex(int x, int y, int z)
        {
            return ((y & 0xF) << 8) | ((x & 0xF) << 4) | (z & 0xF);
        }

        public ref BlockData GetBlockData(int x, int y, int z)
        {
            return ref data[GetDataIndex(x, y, z)];
        }

        public ref BlockData GetBlockData(in BlockPos pos)
        {
            return ref data[pos.GetBlockIndex()];
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
            return data_update[GetDataIndex(x, y, z)];
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
            var index = GetDataIndex(x, y, z);
            data_update[index] |= value;
            ProcessFlag(value);
        }

        public void SetFlag(int x, int y, int z, BlockFlag value)
        {
            var index = GetDataIndex(x, y, z);
            data_update[index] = value;
            ProcessFlag(value);
        }

        public bool Equals(int x, int y, int z)
        {
            return this.x == (x & ~0xF) && this.y == (y & ~0xF) && this.z == (z & ~0xF);
        }

        public bool Equals(in BlockPos pos)
        {
            return Equals(pos.x, pos.y, pos.z);
        }

        public override bool Equals(object obj)
        {
            if (obj is Chunk c)
            {
                return x == c.x && y == c.y && z == c.z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
    }
}
