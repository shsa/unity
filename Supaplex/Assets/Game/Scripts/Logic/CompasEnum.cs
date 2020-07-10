using System;

namespace Game
{
    public enum CompasEnum
    {
        N = 0,
        NE = 1,
        E = 2,
        SE = 3,
        S = 4,
        SW = 5,
        W = 6,
        NW = 7
    }

    public static class CompasEnumExtension
    {
        // clockwise: N, NE = n, E, ES = e, S, SW = s, W, WN = w
        public static CompasEnum toCompas(char c)
        {
            switch (c)
            {
                case 'N': return CompasEnum.N;
                case 'n': return CompasEnum.NE;
                case 'E': return CompasEnum.E;
                case 'e': return CompasEnum.SE;
                case 'S': return CompasEnum.S;
                case 's': return CompasEnum.SW;
                case 'W': return CompasEnum.W;
                case 'w': return CompasEnum.NW;
                default: throw new NotImplementedException();
            }
        }

        public static char toChar(this CompasEnum dir)
        {
            switch (dir)
            {
                case CompasEnum.N: return 'N';
                case CompasEnum.NE: return 'n';
                case CompasEnum.E: return 'E';
                case CompasEnum.SE: return 'e';
                case CompasEnum.S: return 'S';
                case CompasEnum.SW: return 's';
                case CompasEnum.W: return 'W';
                case CompasEnum.NW: return 'w';
                default: throw new NotImplementedException();
            }
        }

        public static byte ToCompasSet(string text)
        {
            byte set = 0;
            for (int i = 0; i < text.Length; i++)
            {
                set |= (byte)(1 << (int)toCompas(text[i]));
            }
            return set;
        }

        public static string ToText(byte mask)
        {
            var text = "";
            for (int i = 0; i < 8; i++)
            {
                if (((mask >> i) & 1) == 1)
                {
                    text += toChar((CompasEnum)i);
                }
            }
            return text;
        }
    }
}