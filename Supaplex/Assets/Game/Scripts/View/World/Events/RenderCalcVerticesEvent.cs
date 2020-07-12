using Game.Logic.World;
using System.Collections.Generic;
using UnityEngine;

namespace Game.View.World
{
    public sealed class RenderCalcVerticesEvent : ChunkEvent
    {
        static int[] DF; // index offsets

        public RenderChunk render;
        public IWorldReader world;
        public IChunkReader chunk;
        BlockPos pos = new BlockPos();
        BlockPos offset = new BlockPos();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        public override void Execute()
        {
            vertices.Clear();
            triangles.Clear();
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
            
            var v = e.vertices;
            e.vertices = vertices;
            vertices = v;

            var t = e.triangles;
            e.triangles = triangles;
            triangles = t;

            var u = e.uv;
            e.uv = uv;
            uv = u;

            e.Publish();
        }

        void AddBlock(BlockPos pos, byte facings)
        {
            var blockData = chunk.GetBlockData(pos);
            Vector3 p = pos.ToVector();
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
                        vertices.Add(p + v);
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
