using UnityEngine;

namespace Game.Logic.World
{
    public class Vec2i
    {
        public int x;
        public int y;

        public Vec2i()
        {
            this.x = 0;
            this.y = 0;
        }

        public Vec2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(Vec2i v)
        {
            x = v.x;
            y = v.y;
        }

        public void Add(Vec2i offset)
        {
            x = x + offset.x;
            y = y + offset.y;
        }

        bool Equals(Vec2i obj)
        {
            return x == obj.x && y == obj.y;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Vec2i);
        }

        public override int GetHashCode()
        {
            // from unity Vector3.GetHashCode()
            return x.GetHashCode() ^ (y.GetHashCode() << 2);

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
            return $"({x}, {y})";
        }

        public static bool operator ==(Vec2i a, Vec2i b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(Vec2i a, Vec2i b)
        {
            return !Equals(a, b);
        }

        public static Vec2i operator +(Vec2i a, Vec2i offset)
        {
            return new Vec2i(a.x + offset.x, a.y + offset.y);
        }

        public static Vec2i operator +(Vec2i a, Vector2Int offset)
        {
            return new Vec2i(a.x + offset.x, a.y + offset.y);
        }
    }
}
