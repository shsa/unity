using UnityEngine;

namespace Game.Logic
{
    public enum Facing : int
    {
        First = 0,

        South = 0,
        North = 1,
        West = 2,
        East = 3,
        Up = 4,
        Down = 5,

        Last = 5
    }

    public enum FacingSet
    {
        None = 0x00,
        South = 0x01,
        North = 0x02,
        West = 0x04,
        East = 0x08,
        Up = 0x10,
        Down = 0x20,
        All = South | North | West | East | Up | Down
    }

    public static class FacingExtention
    {
        public static Vec3i south = new Vec3i(0, 0, -1);
        public static Vec3i north = new Vec3i(0, 0, 1);
        public static Vec3i[] faceVector = new Vec3i[6];
        public static Facing[] faceOpposite = new Facing[6];
        public static FacingSet[] faceSet = new FacingSet[6];

        public static Vec3i GetVector(this Facing side)
        {
            return faceVector[(int)side];
        }

        public static BlockPos Offset(this BlockPos pos, Facing side)
        {
            return pos + faceVector[(int)side];
        }

        public static ChunkPos Offset(this ChunkPos pos, Facing side)
        {
            return pos + faceVector[(int)side];
        }

        public static Facing Opposite(this Facing side)
        {
            return faceOpposite[(int)side];
        }

        public static FacingSet ToSet(this Facing side)
        {
            return faceSet[(int)side];
        }

        public static bool HasFacing(this FacingSet sides, Facing facing)
        {
            return sides.HasFlag(faceSet[(int)facing]);
        }

        static FacingExtention()
        {
            faceOpposite[(int)Facing.South] = Facing.North;
            faceOpposite[(int)Facing.North] = Facing.South;
            faceOpposite[(int)Facing.West] = Facing.East;
            faceOpposite[(int)Facing.East] = Facing.West;
            faceOpposite[(int)Facing.Up] = Facing.Down;
            faceOpposite[(int)Facing.Down] = Facing.Up;

            faceSet[(int)Facing.South] = FacingSet.South;
            faceSet[(int)Facing.North] = FacingSet.North;
            faceSet[(int)Facing.West] = FacingSet.West;
            faceSet[(int)Facing.East] = FacingSet.East;
            faceSet[(int)Facing.Up] = FacingSet.Up;
            faceSet[(int)Facing.Down] = FacingSet.Down;

            faceVector[(int)Facing.South] = new Vec3i(0, 0, -1);
            faceVector[(int)Facing.North] = new Vec3i(0, 0, 1);
            faceVector[(int)Facing.West] = new Vec3i(-1, 0, 0);
            faceVector[(int)Facing.East] = new Vec3i(1, 0, 0);
            faceVector[(int)Facing.Up] = new Vec3i(0, 1, 0);
            faceVector[(int)Facing.Down] = new Vec3i(0, -1, 0);
        }
    }
}
