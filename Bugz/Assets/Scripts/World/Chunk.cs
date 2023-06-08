using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MetaType = System.Byte;

namespace Assets.Scripts.World
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ChunkValue : IEquatable<ChunkValue>
    {
        [FieldOffset(0)] public UInt16 data;

        [FieldOffset(0)] public ItemEnum item;
        [FieldOffset(1)] public MetaType meta;

        public ChunkValue(UInt16 data)
        {
            this.item = ItemEnum.None;
            this.meta = 0;
            this.data = data;
        }

        public ChunkValue(ItemEnum item, MetaType meta)
        {
            this.data = 0;
            this.item = item;
            this.meta = meta;
        }

        public ChunkValue(ItemEnum item, DirectionEnum meta)
        {
            this.data = 0;
            this.item = item;
            this.meta = (MetaType)meta;
        }

        public override bool Equals(object obj)
        {
            if (obj is ChunkValue)
            {
                return this.Equals((ChunkValue)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public bool Equals(ChunkValue other)
        {
            return this.data == other.data;
        }
    }

    public class Chunk
    {
        public World world;
        public ChunkPos pos;
        public ChunkValue[] data = new ChunkValue[256];

        public Chunk(World world, ChunkPos pos)
        {
            this.world = world;
            this.pos = pos;
        }

        public ChunkValue this[byte x, byte y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return data[y * 16 + x];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                var index = y * 16 + x;
                if (!data[index].Equals(value))
                {
                    data[index] = value;
                    //UpdateItem((pos.x << 4) + x, (pos.y << 4) + y);
                }
            }
        }

        public void UpdateItem(int x, int y)
        {
            var value = this[(byte)(x & 0xF), (byte)(y & 0xF)];
            var _class = Items.Item.GetClass(value.item);
            value.meta = _class.GetMeta(x, y);
            this[(byte)(x & 0xF), (byte)(y & 0xF)] = value;
            world.UpdateItem(x - 1, y);
            world.UpdateItem(x + 1, y);
            world.UpdateItem(x, y - 1);
            world.UpdateItem(x, y + 1);
        }
    }
}
