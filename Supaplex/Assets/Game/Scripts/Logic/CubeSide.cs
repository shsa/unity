using UnityEngine;

namespace Game.Logic
{
    public enum CubeSide : int
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

    public enum CubeSideSet
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

    public static class CubeSideExtention
    {
        public static Vector3Int south = new Vector3Int(0, 0, -1);
        public static Vector3Int north = new Vector3Int(0, 0, 1);
        public static Vector3Int[] sideVector = new Vector3Int[6];
        public static CubeSide[] sideOpposite = new CubeSide[6];
        public static CubeSideSet[] sideSet = new CubeSideSet[6];

        public static Vector3Int GetVector(this CubeSide side)
        {
            return sideVector[(int)side];
        }

        public static Vector3Int Offset(this Vector3Int pos, CubeSide side)
        {
            return pos + sideVector[(int)side];
        }

        public static CubeSide Opposite(this CubeSide side)
        {
            return sideOpposite[(int)side];
        }

        public static CubeSideSet GetSet(this CubeSide side)
        {
            return sideSet[(int)side];
        }

        static CubeSideExtention()
        {
            sideOpposite[(int)CubeSide.South] = CubeSide.North;
            sideOpposite[(int)CubeSide.North] = CubeSide.South;
            sideOpposite[(int)CubeSide.West] = CubeSide.East;
            sideOpposite[(int)CubeSide.East] = CubeSide.West;
            sideOpposite[(int)CubeSide.Up] = CubeSide.Down;
            sideOpposite[(int)CubeSide.Down] = CubeSide.Up;

            sideSet[(int)CubeSide.South] = CubeSideSet.South;
            sideSet[(int)CubeSide.North] = CubeSideSet.North;
            sideSet[(int)CubeSide.West] = CubeSideSet.West;
            sideSet[(int)CubeSide.East] = CubeSideSet.East;
            sideSet[(int)CubeSide.Up] = CubeSideSet.Up;
            sideSet[(int)CubeSide.Down] = CubeSideSet.Down;

            sideVector[(int)CubeSide.South] = south;
            sideVector[(int)CubeSide.North] = north;
            sideVector[(int)CubeSide.West] = Vector3Int.left;
            sideVector[(int)CubeSide.East] = Vector3Int.right;
            sideVector[(int)CubeSide.Up] = Vector3Int.up;
            sideVector[(int)CubeSide.Down] = Vector3Int.down;
        }
    }
}
