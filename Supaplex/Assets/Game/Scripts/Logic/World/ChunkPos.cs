using UnityEngine;

namespace Game.Logic.World
{
    public sealed class ChunkPos1 : Vec3i
    {
        public ChunkPos1() : base(0, 0, 0)
        {
        }

        public ChunkPos1(ChunkPos1 pos) : base(pos.x, pos.y, pos.z)
        {
        }

        public ChunkPos1(int x, int y, int z) : base(x, y, z)
        {
        }

        public void Set(BlockPos pos)
        {
            x = pos.x >> 4;
            y = pos.y >> 4;
            z = pos.z >> 4;
        }

        public Vector3Int ToVector3Int()
        {
            return new Vector3Int(x << 4, y << 4, z << 4);
        }

        public Vector3Int min {
            get {
                return ToVector3Int();
            }
        }

        public Vector3Int max {
            get {
                return ToVector3Int() + new Vector3Int(15, 15, 15);
            }
        }

        public Bounds bounds {
            get {
                return new Bounds((Vector3)(min + max) * 0.5f, Vector3.one * 16);
            }
        }

        public static ChunkPos1 From(BlockPos pos)
        {
            return new ChunkPos1(pos.x >> 4, pos.y >> 4, pos.z >> 4);
        }

        public static ChunkPos1 From(Vector3Int pos)
        {
            return new ChunkPos1(pos.x >> 4, pos.y >> 4, pos.z >> 4);
        }

        public static ChunkPos1 operator +(ChunkPos1 a, Vec3i offset)
        {
            return new ChunkPos1(a.x + offset.x, a.y + offset.y, a.z + offset.z);
        }
    }
}
