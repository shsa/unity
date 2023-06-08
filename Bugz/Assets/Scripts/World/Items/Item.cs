using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MetaType = System.Byte;

namespace Assets.Scripts.World.Items
{
    public class Item
    {
        public static Item[] Items = new Item[255];

        static void Register<T>(ItemEnum index) where T: Item, new()
        {
            Items[(byte)index] = new T();
        }

        public static Item GetClass(ItemEnum index)
        {
            return Items[(byte)index];
        }

        static Item()
        {
            Register<Item>(ItemEnum.None);
            Register<Pipe>(ItemEnum.Pipe);
        }

        public virtual byte GetMeta(int x, int y)
        {
            return 0;
        }

        public virtual GameObject GetGameObject(byte meta)
        {
            return null;
        }

        public GameObject GetGameObject(DirectionEnum meta)
        {
            return GetGameObject((MetaType)meta);
        }
    }
}
