using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World
{
    public sealed class BlockPos
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public BlockPos()
        {
        }

        public BlockPos(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Set(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Set(in BlockPos pos, in Int3 offset)
        {
            this.x = pos.x + offset.X;
            this.y = pos.y + offset.Y;
            this.z = pos.z + offset.Z;
        }

        public void Set(in BlockPos pos, in Int2 offset)
        {
            this.x = pos.x + offset.X;
            this.y = pos.y + offset.Y;
            this.z = pos.z;
        }

        public ChunkPos ChunkPos {
            get {
                return new ChunkPos(x >> 4, y >> 4, z >> 4);
            }
        }

        public int GetChunkIndex()
        {
            return ((y & 0xF) << 8) | ((x & 0xF) << 4) | (z & 0xF);
        }

        public static BlockPos From(in Float2 pos)
        {
            return new BlockPos
            {
                x = (int)Math.Round(pos.X),
                y = (int)Math.Round(pos.Y)
            };
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
