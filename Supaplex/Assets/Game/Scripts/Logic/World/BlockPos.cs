using UnityEngine;

namespace Game.Logic.World
{
    public sealed class BlockPos
    {
        public int x;
        public int y;
        public int z;

        public BlockPos()
        {
        }

        public BlockPos(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public BlockPos(BlockPos pos)
        {
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }

        public void Set(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Set(BlockPos pos)
        {
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
        }

        public void Add(Vec3i offset)
        {
            x = x + offset.x;
            y = y + offset.y;
            z = z + offset.z;
        }

        public void SetChunk(int x, int y, int z)
        {
            this.x = x << 4;
            this.y = y << 4;
            this.z = z << 4;
        }

        public void AddChunk(int x, int y, int z)
        {
            this.x += x << 4;
            this.y += y << 4;
            this.z += z << 4;
        }

        public void AddChunk(Vec3i pos)
        {
            this.x += pos.x << 4;
            this.y += pos.y << 4;
            this.z += pos.z << 4;
        }

        public int GetIndex()
        {
            return ((y & 0xF) << 8) | ((x & 0xF) << 4) | (z & 0xF);
        }

        public Vector3 ToVector(Vector3 v)
        {
            return new Vector3(x + v.x, y + v.y, z + v.z);
        }

        public Vector3 ToVector()
        {
            return new Vector3(x, y, z);
        }

        public Vector3Int min {
            get {
                return new Vector3Int((x >> 4) << 4, (y >> 4) << 4, (z >> 4) << 4);
            }
        }

        public Vector3Int max {
            get {
                return new Vector3Int(((x >> 4) << 4) | 0xF, ((y >> 4) << 4) | 0xF, ((z >> 4) << 4) | 0xF);
            }
        }

        public Bounds bounds {
            get {
                return new Bounds((Vector3)(min + max) * 0.5f, Vector3.one * 16);
            }
        }

        public bool ChunkEquals(BlockPos pos)
        {
            return (x >> 4) == (pos.x >> 4) && (y >> 4) == (pos.y >> 4) && (z >> 4) == (pos.z >> 4);
        }

        public bool BlockEquals(BlockPos pos)
        {
            return x == pos.x && y == pos.y && z == pos.z;
        }

        public override bool Equals(object obj)
        {
            var pos = obj as BlockPos;
            if (pos == null)
            {
                return false;
            }
            // for chunk hashing
            return (x >> 4) == (pos.x >> 4) && (y >> 4) == (pos.y >> 4) && (z >> 4) == (pos.z >> 4);
        }

        public override int GetHashCode()
        {
            // from unity Vector3.GetHashCode()
            return (x >> 4).GetHashCode() ^ ((y >> 4).GetHashCode() << 2) ^ ((z >> 4).GetHashCode() >> 2);

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

        public static BlockPos operator +(BlockPos a, Vector3Int offset)
        {
            return new BlockPos(a.x + offset.x, a.y + offset.y, a.z + offset.z);
        }

        public static BlockPos operator +(BlockPos a, Vec3i offset)
        {
            return new BlockPos(a.x + offset.x, a.y + offset.y, a.z + offset.z);
        }

        //public static bool operator ==(BlockPos a, BlockPos b)
        //{
        //    return a.x == b.x && a.y == b.y && a.z == b.z;
        //}

        //public static bool operator !=(BlockPos a, BlockPos b)
        //{
        //    return a.x != b.x || a.y != b.y | a.z != b.z;
        //}

        public static BlockPos From(Vector3Int pos)
        {
            return new BlockPos(pos.x, pos.y, pos.z);
        }

        public static BlockPos ChunkFrom(Vector3Int pos)
        {
            return new BlockPos(pos.x << 4, pos.y << 4, pos.z << 4);
        }
    }
}
