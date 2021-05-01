using Leopotam.Ecs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DefenceFactory.Game.World
{
    public enum DirectionEnum
    {
        First = 0,
        N   = 0,
        NE  = 1,
        E   = 2,
        SE  = 3,
        S   = 4,
        SW  = 5,
        W   = 6,
        NW  = 7,
        Last = 7
    }

    public enum DirectionSet
    {
        None = 0,
        N   = 0x01,
        NE  = 0x02,
        E   = 0x04,
        SE  = 0x08,
        S   = 0x10,
        SW  = 0x20,
        W   = 0x40,
        NW  = 0x80
    }

    public static class DirectionEnumExtension
    {
        static Int3[] dirs = new Int3[8];
        static DirectionSet[] dirSets = new DirectionSet[8];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Int3 GetDirection(this DirectionEnum dir)
        {
            return ref dirs[(int)dir];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DirectionSet Set(this DirectionEnum dir)
        {
            return dirSets[(int)dir];
        }

        public static DirectionEnum Next(this DirectionEnum dir)
        {
            switch (dir)
            {
                case DirectionEnum.N: return DirectionEnum.NE;
                case DirectionEnum.NE: return DirectionEnum.E;
                case DirectionEnum.E: return DirectionEnum.SE;
                case DirectionEnum.SE: return DirectionEnum.S;
                case DirectionEnum.S: return DirectionEnum.SW;
                case DirectionEnum.SW: return DirectionEnum.W;
                case DirectionEnum.W: return DirectionEnum.NW;
                case DirectionEnum.NW: return DirectionEnum.N;
                default:
                    throw new NotImplementedException();
            }
        }

        public static DirectionEnum Prev(this DirectionEnum dir)
        {
            switch (dir)
            {
                case DirectionEnum.N: return DirectionEnum.NW;
                case DirectionEnum.NE: return DirectionEnum.N;
                case DirectionEnum.E: return DirectionEnum.NE;
                case DirectionEnum.SE: return DirectionEnum.E;
                case DirectionEnum.S: return DirectionEnum.SE;
                case DirectionEnum.SW: return DirectionEnum.S;
                case DirectionEnum.W: return DirectionEnum.SW;
                case DirectionEnum.NW: return DirectionEnum.W;
                default:
                    throw new NotImplementedException();
            }
        }

        static DirectionEnumExtension()
        {
            dirs[(int)DirectionEnum.N]  = new Int3(0, 1, 0);
            dirs[(int)DirectionEnum.NE] = new Int3(1, 1, 0);
            dirs[(int)DirectionEnum.E]  = new Int3(1, 0, 0);
            dirs[(int)DirectionEnum.SE] = new Int3(1, -1, 0);
            dirs[(int)DirectionEnum.S]  = new Int3(0, -1, 0);
            dirs[(int)DirectionEnum.SW] = new Int3(-1, -1, 0);
            dirs[(int)DirectionEnum.W]  = new Int3(-1, 0, 0);
            dirs[(int)DirectionEnum.NW] = new Int3(-1, 1, 0);

            dirSets[(int)DirectionEnum.N]   = DirectionSet.N;
            dirSets[(int)DirectionEnum.NE]  = DirectionSet.NE;
            dirSets[(int)DirectionEnum.E]   = DirectionSet.E;
            dirSets[(int)DirectionEnum.SE]  = DirectionSet.SE;
            dirSets[(int)DirectionEnum.S]   = DirectionSet.S;
            dirSets[(int)DirectionEnum.SW]  = DirectionSet.SW;
            dirSets[(int)DirectionEnum.W]   = DirectionSet.W;
            dirSets[(int)DirectionEnum.NW]  = DirectionSet.NW;
        }
    }
}
