using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic.World
{
    public class BlockRPGMakerTileSet : Block
    {
        class SD
        {
            public Vec3i vector;
            public int index;
        }

        class FacingD
        {
            public Vec3i vector;
            public CompasEnum index;
            public int index3D;

            public FacingD(int x, int y, int z, CompasEnum index)
            {
                vector = new Vec3i(x, y, z);
                this.index = index;
            }
        }

        class FacingDList : List<FacingD>
        {
        }

        Dictionary<Vec3i, SD> fullBlock = new Dictionary<Vec3i, SD>();
        SD[] dirs;
        FacingDList[] set = new FacingDList[6];

        public BlockRPGMakerTileSet() : base("Stone1", ModelType.RPGMakerTileSet)
        {
            var index = 0;
            for (int z = -1; z <= 1; z++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (x != 0 || y != 0 || z != 0)
                        {
                            if (x == 0 || y == 0 || z == 0)
                            {
                                var node = new SD();
                                node.vector = new Vec3i(x, y, z);
                                node.index = index++;
                                fullBlock.Add(node.vector, node);
                            }
                        }
                    }
                }
            }
            dirs = fullBlock.Values.ToArray();

            var list = new FacingDList();
            list.Add(new FacingD(0, 1, 0, CompasEnum.N)); // N
            list.Add(new FacingD(1, 1, 0, CompasEnum.NE)); // NE
            list.Add(new FacingD(1, 0, 0, CompasEnum.E)); // E
            list.Add(new FacingD(1, -1, 0, CompasEnum.SE)); // SE
            list.Add(new FacingD(0, -1, 0, CompasEnum.S)); // S
            list.Add(new FacingD(-1, -1, 0, CompasEnum.SW)); // SW
            list.Add(new FacingD(-1, 0, 0, CompasEnum.W)); // W
            list.Add(new FacingD(-1, 1, 0, CompasEnum.NW)); // NW
            list.Add(new FacingD(0, 1, -1, CompasEnum.N)); // N
            list.Add(new FacingD(1, 0, -1, CompasEnum.E)); // E
            list.Add(new FacingD(0, -1, -1, CompasEnum.S)); // S
            list.Add(new FacingD(-1, 0, -1, CompasEnum.W)); // W

            void AddToFacing(Facing facing, Quaternion rotation)
            {
                var lst = new FacingDList();
                for (int i = 0; i < list.Count; i++)
                {
                    var v = new Vector3(list[i].vector.x, list[i].vector.y, list[i].vector.z);
                    v = rotation * v;
                    var item = new FacingD(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z), list[i].index);
                    lst.Add(item);
                    if (fullBlock.ContainsKey(item.vector))
                    {
                        item.index3D = fullBlock[item.vector].index;
                    }
                    else
                    {
                        Debug.LogError("sss");
                    }
                }
                set[(int)facing] = lst;
            }

            AddToFacing(Facing.North, Quaternion.Euler(0, 180, 0));
            AddToFacing(Facing.East, Quaternion.Euler(0, -90, 0));
            AddToFacing(Facing.South, Quaternion.identity);
            AddToFacing(Facing.West, Quaternion.Euler(0, 90, 0));
            AddToFacing(Facing.Up, Quaternion.Euler(90, 0, 0));
            AddToFacing(Facing.Down, Quaternion.Euler(-90, 0, 0));
        }

        public byte CalcSide(Facing facing, BlockState state)
        {
            var list = set[(int)facing];
            byte result = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var f = list[i];
                if ((((int)state >> f.index3D) & 0x1) == 1)
                {
                    result |= (byte)(1 << (int)f.index);
                }
            }
            return result;
        }

        public override void OnBlockChange(BlockChangeEvent e)
        {
            var state = BlockState.None;
            var blockPos = e.blockPos;
            var pos = e.pos;
            foreach (var node in dirs)
            {
                var v = node.vector;
                pos.Set(blockPos.x + v.x, blockPos.y + v.y, blockPos.z + v.z);
                if (!e.world.GetBlockData(pos).IsEmpty())
                {
                    state |= (BlockState)(1 << node.index);
                }
            }
            e.world.SetBlockData(blockPos, id.GetBlockData(state));
        }
    }
}
