using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.World
{
    public enum DirectionEnum
    {
        None    = 0x0,
        Top     = 0x1,
        Left    = 0x2,
        Right   = 0x4,
        Bottom  = 0x8
    }

    public static class DirectionEnumExtension
    {
        public static DirectionEnum GetOpposite(this DirectionEnum dir)
        {
            switch (dir)
            {
                case DirectionEnum.Top: return DirectionEnum.Bottom;
                case DirectionEnum.Left: return DirectionEnum.Right;
                case DirectionEnum.Right: return DirectionEnum.Left;
                case DirectionEnum.Bottom: return DirectionEnum.Top;
                default: return DirectionEnum.None;
            }
        }
    }
}
