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
        None    = 0,

        N       = 1,
        NE      = 2,
        E       = 3,
        SE      = 4,
        S       = 5,
        SW      = 6,
        W       = 7,
        NW      = 8,

        First = 1,
        Last = 8
    }

    public enum DirectionSet
    {
        None    = 0x00,
        N       = 0x01,
        NE      = 0x02,
        E       = 0x04,
        SE      = 0x08,
        S       = 0x10,
        SW      = 0x20,
        W       = 0x40,
        NW      = 0x80
    }

    public static class DirectionEnumExtension
    {
        static Int2[] dirs = new Int2[9];
        static DirectionSet[] dirSets = new DirectionSet[9];
        static char[] chars = new char[9];
        static string[] keys = new string[256];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Int2 GetVector2(this DirectionEnum dir)
        {
            return ref dirs[(int)dir];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char Char(this DirectionEnum dir)
        {
            return chars[(int)dir];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Key(this DirectionSet dirs)
        {
            return keys[(int)dirs];
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
            dirs[(int)DirectionEnum.None] = new Int2(0, 0);
            dirs[(int)DirectionEnum.N]  = new Int2(0, 1);
            dirs[(int)DirectionEnum.NE] = new Int2(1, 1);
            dirs[(int)DirectionEnum.E]  = new Int2(1, 0);
            dirs[(int)DirectionEnum.SE] = new Int2(1, -1);
            dirs[(int)DirectionEnum.S]  = new Int2(0, -1);
            dirs[(int)DirectionEnum.SW] = new Int2(-1, -1);
            dirs[(int)DirectionEnum.W]  = new Int2(-1, 0);
            dirs[(int)DirectionEnum.NW] = new Int2(-1, 1);

            dirSets[(int)DirectionEnum.None] = DirectionSet.None;
            dirSets[(int)DirectionEnum.N]   = DirectionSet.N;
            dirSets[(int)DirectionEnum.NE]  = DirectionSet.NE;
            dirSets[(int)DirectionEnum.E]   = DirectionSet.E;
            dirSets[(int)DirectionEnum.SE]  = DirectionSet.SE;
            dirSets[(int)DirectionEnum.S]   = DirectionSet.S;
            dirSets[(int)DirectionEnum.SW]  = DirectionSet.SW;
            dirSets[(int)DirectionEnum.W]   = DirectionSet.W;
            dirSets[(int)DirectionEnum.NW]  = DirectionSet.NW;

            chars[(int)DirectionEnum.None] = '.';
            chars[(int)DirectionEnum.N] = 'N';
            chars[(int)DirectionEnum.NE] = 'n';
            chars[(int)DirectionEnum.E] = 'E';
            chars[(int)DirectionEnum.SE] = 'e';
            chars[(int)DirectionEnum.S] = 'S';
            chars[(int)DirectionEnum.SW] = 's';
            chars[(int)DirectionEnum.W] = 'W';
            chars[(int)DirectionEnum.NW] = 'w';

            for (var i = 0; i < 256; i++)
            {
                var key = "";
                for (var d = DirectionEnum.First; d <= DirectionEnum.Last; d++)
                {
                    if (((DirectionSet)i & d.Set()) == d.Set())
                    {
                        key += d.Char();
                    }
                    else
                    {
                        key += '.';
                    }
                }
                keys[i] = key;
            }
        }
    }
}
