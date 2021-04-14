using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.World
{
    public sealed class BlockPos
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public int GetChunkIndex()
        {
            return ((y & 0xF) << 8) | ((x & 0xF) << 4) | (z & 0xF);
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
            return $"({x}, {y}, {z}), ({x >> 4}.{x & 0xF}, {y >> 4}.{y & 0xF}, {z >> 4}.{z & 0xF})";
        }

    }
}
