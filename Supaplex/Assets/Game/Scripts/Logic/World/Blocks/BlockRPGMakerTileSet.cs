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
            public int index;
            public int index2;

            public FacingD(int x, int y, int z, int index)
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
            list.Add(new FacingD(0, 1, 0, 0)); // N
            list.Add(new FacingD(1, 1, 0, 1)); // NE
            list.Add(new FacingD(1, 0, 0, 2)); // E
            list.Add(new FacingD(1, -1, 0, 3)); // SE
            list.Add(new FacingD(0, -1, 0, 4)); // S
            list.Add(new FacingD(-1, -1, 0, 5)); // SW
            list.Add(new FacingD(-1, 0, 0, 6)); // W
            list.Add(new FacingD(-1, 1, 0, 7)); // NW
            list.Add(new FacingD(0, 1, -1, 0)); // N
            list.Add(new FacingD(1, 0, -1, 2)); // E
            list.Add(new FacingD(0, -1, -1, 4)); // S
            list.Add(new FacingD(-1, 0, -1, 6)); // W

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
                        item.index2 = fullBlock[item.vector].index;
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

        public override void OnNeighborChange(IWorldAccess world, BlockPos pos, BlockPos neighbor, BlockData neighborData)
        {

        }

        BlockPos pos = new BlockPos();
        public override void OnBlockPlaced(BlockPlacedEvent e)
        {
            var state = BlockState.None;
            foreach (var node in dirs)
            {
                pos.Set(e.pos);
                pos.Add(node.vector);
                if (!e.world.GetBlockData(pos).IsEmpty())
                {
                    state |= (BlockState)(1 << node.index);
                }
            }
            e.world.SetBlockData(e.pos, id.GetBlockData(state));
        }
    }
}
