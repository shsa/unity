using UnityEngine;

namespace Game.Logic.World
{
    public sealed class BlockPos : Vec3i
    {
        public BlockPos() : base(0, 0, 0)
        {
        }

        public BlockPos(BlockPos pos) : base(pos.x, pos.y, pos.z)
        {
        }

        public BlockPos(int x, int y, int z) : base(x, y, z)
        {
        }

        public Vector3 ToVector(Vector3 v)
        {
            return new Vector3(x + v.x, y + v.y, z + v.z);
        }

        public Vector3 ToVector()
        {
            return new Vector3(x, y, z);
        }

        public static BlockPos operator +(BlockPos a, Vector3Int offset)
        {
            return new BlockPos(a.x + offset.x, a.y + offset.y, a.z + offset.z);
        }

        public static BlockPos operator +(BlockPos a, Vec3i offset)
        {
            return new BlockPos(a.x + offset.x, a.y + offset.y, a.z + offset.z);
        }
    }
}
