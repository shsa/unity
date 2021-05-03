using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace DefenceFactory.Game.World
{
    public enum ChunkFlag
    {
        None        = 0x000,
        Load        = 0x001,
        Redraw      = 0x002,
        Destroy     = 0x004,

        Generate    = 0x010,
        Updating    = 0x020,
        Update      = 0x040,

        Loaded      = 0x100,
    }


    public static class ChunkFlagExtension
    {
        public static ref ChunkFlag Add(this ref ChunkFlag flag, ChunkFlag value)
        {
            flag |= value;
            return ref flag;
        }

        public static ref ChunkFlag Remove(this ref ChunkFlag flag, ChunkFlag value)
        {
            flag &= ~value;
            return ref flag;
        }
    }
}
