﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.World
{
    public sealed class ChunkPos
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public ChunkPos(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public BlockPos MinBlockPos()
        {
            return new BlockPos(x << 4, y << 4, z << 4);
        }

        public BlockPos MaxBlockPos()
        {
            return new BlockPos(x << 4 | 0xF, y << 4 | 0xF, z << 4 | 0xF);
        }

        public void Set(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public override bool Equals(object obj)
        {
            if (obj is ChunkPos pos)
            {
                return x == pos.x && y == pos.y && z == pos.z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // from unity Vector3.GetHashCode()
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);

            //unchecked // Overflow is fine, just wrap
            //{
            //    int hash = 17;
            //    // Suitable nullity checks etc, of course :)
            //    hash = hash * 23 + x.GetHashCode();
            //    hash = hash * 23 + y.GetHashCode();
            //    hash = hash * 23 + z.GetHashCode();
            //    return hash;
            //}
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

    }
}
