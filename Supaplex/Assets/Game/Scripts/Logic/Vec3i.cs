using UnityEngine;

namespace Game.Logic
{
    public class Vec3i
    {
        public int x;
        public int y;
        public int z;

        public Vec3i()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public Vec3i(int x, int y, int z)
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

        public void Set(Vec3i v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public void Add(Vec3i offset)
        {
            x = x + offset.x;
            y = y + offset.y;
            z = z + offset.z;
        }

        bool Equals(Vec3i obj)
        {
            return x == obj.x && y == obj.y && z == obj.z;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Vec3i);
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

        public static bool operator ==(Vec3i a, Vec3i b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(Vec3i a, Vec3i b)
        {
            return !Equals(a, b);
        }

        public static Vec3i operator +(Vec3i a, Vec3i offset)
        {
            return new Vec3i(a.x + offset.x, a.y + offset.y, a.z + offset.z);
        }

        public static Vec3i operator +(Vec3i a, Vector3Int offset)
        {
            return new Vec3i(a.x + offset.x, a.y + offset.y, a.z + offset.z);
        }

        public static Vec3i From(Vec3i pos)
        {
            return new Vec3i(pos.x >> 4, pos.y >> 4, pos.z >> 4);
        }
    }
}
