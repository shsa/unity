using Game.View.World;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic.World
{
    public sealed class RenderCalcVerticesEvent : ChunkEvent
    {
        static int[] DF; // index offsets

        public RenderChunk render;
        public IWorldReader world;
        public IChunkReader chunk;
        BlockPos pos = new BlockPos();
        BlockPos offset = new BlockPos();
        List<Vector3> vertices;
        List<int> triangles;
        List<Vector2> uv;

        public override void Execute()
        {
            vertices = render.vertices;
            vertices.Clear();
            triangles = render.triangles;
            triangles.Clear();
            uv = render.uv;
            uv.Clear();

            var min = chunk.position.min;
            var max = chunk.position.max;

            for (int z = min.z; z <= max.z; z++)
            {
                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        pos.Set(x, y, z);
                        var index = pos.GetIndex();
                        if (render.data[index])
                        {
                            byte set = 0;
                            for (Facing facing = Facing.First; facing <= Facing.Last; facing++)
                            {
                                if (facing == Facing.North)
                                {
                                    continue;
                                }

                                offset.Set(pos);
                                offset.Add(facing.GetVector());

                                if (offset.x < min.x || offset.x > max.x || offset.y < min.y || offset.y > max.y || offset.z < min.z || offset.z > max.z)
                                {
                                    if (offset.z < 0)
                                    {
                                        set |= (byte)(1 << (int)facing);
                                    }
                                    else
                                    {
                                        if (world.GetBlockData(offset).IsEmpty())
                                        {
                                            set |= (byte)(1 << (int)facing);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!render.data[index + DF[(int)facing]])
                                    {
                                        set |= (byte)(1 << (int)facing);
                                    }
                                }
                            }
                            AddBlock(pos, set);
                        }
                    }
                }
            }

            var e = render.calcMeshProvider.Create();
            e.render = render;
            e.Publish();
        }

        void AddBlock(BlockPos pos, byte facings)
        {
            var blockData = chunk.GetBlockData(pos);
            var objectType = blockData.GetBlockId();
            Vector3 p = pos.ToVector();
            switch (objectType)
            {
                case BlockType.Stone:
                    break;
                case BlockType.Stone4x4:
                    p = new Vector3(p.x + 1.5f, p.y + 1.5f, p.z + 1.5f);
                    facings = (1 << (int)Facing.South) | (1 << (int)Facing.Up) | (1 << (int)Facing.Down) | (1 << (int)Facing.West) | (1 << (int)Facing.East);
                    break;
                default:
                    break;
            }
            var block = blockData.GetBlock();
            var state = blockData.GetBlockState();
            var model = Model.GetModel(block.model);
            for (Facing facing = Facing.First; facing <= Facing.Last; facing++)
            {
                if (((facings >> (int)facing) & 1) == 1)
                {
                    var part = model.GetBlockPart(facing, block, state);
                    var l = vertices.Count;
                    for (int i = 0; i < part.vertices.Length; i++)
                    {
                        var v = part.vertices[i];
                        if (objectType == BlockType.Stone4x4)
                        {
                            vertices.Add(p + v * 4);
                        }
                        else
                        {
                            vertices.Add(p + v);
                        }
                    }
                    uv.AddRange(part.uv);
                    for (int i = 0; i < part.triangles.Length; i++)
                    {
                        triangles.Add(l + part.triangles[i]);
                    }
                }
            }
        }

        static RenderCalcVerticesEvent()
        {
            var dx = (new BlockPos(1, 0, 0)).GetIndex();
            var dy = (new BlockPos(0, 1, 0)).GetIndex();
            var dz = (new BlockPos(0, 0, 1)).GetIndex();
            DF = new int[6];
            DF[(int)Facing.North] = dz;
            DF[(int)Facing.South] = -dz;
            DF[(int)Facing.East] = dx;
            DF[(int)Facing.West] = -dx;
            DF[(int)Facing.Up] = dy;
            DF[(int)Facing.Down] = -dy;
        }
    }
}
