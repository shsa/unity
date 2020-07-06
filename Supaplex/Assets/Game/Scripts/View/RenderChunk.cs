using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Logic;
using System.Linq.Expressions;

namespace Game.View
{
    public class RenderChunk : IDisposable
    {
        public byte sides;
        byte[] data;
        public Chunk chunk { get; private set; }
        public Bounds bounds { get; private set; }
        public int frameIndex { get; private set; }
        public Mesh mesh;
        List<Vector3> vertices;
        List<int> triangles;
        List<Vector2> uv;

        public RenderChunk(Chunk chunk)
        {
            this.chunk = chunk;
            this.bounds = chunk.position.bounds;
            this.chunk.cubeChanged += Chunk_cubeChanged;
            data = new byte[4096]; // 16 * 16 * 16
            vertices = new List<Vector3>(); // 16 * 16 * 16 * 24?
            triangles = new List<int>();
            uv = new List<Vector2>();

            mesh = new Mesh();
        }

        public byte this[BlockPos pos] {
            get {
                return data[Chunk.GetBlockIndex(pos)];
            }
        }

        private void Chunk_cubeChanged(object sender, ChunkChangeEvent e)
        {
            //throw new System.NotImplementedException();
        }

        public bool SetFrameIndex(int value)
        {
            if (frameIndex == value)
            {
                return false;
            }
            else
            {
                frameIndex = value;
                return true;
            }
        }

        public bool IsVisible(Facing facing)
        {
            return ((sides >> (int)facing) & 1) == 1;
        }

        public void CalcVisibility()
        {
            UnityEngine.Profiling.Profiler.BeginSample("CalcVisibiltity");

            var min = chunk.position.min;
            var max = chunk.position.max;
            var pos = new BlockPos();
            var offset = new BlockPos();
            var chunkPos = new ChunkPos();
            Chunk tmpChunk = null;
            sides = 0;
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    for (int z = min.z; z <= max.z; z++)
                    {
                        pos.Set(x, y, z);
                        chunkPos.Set(pos);
                        tmpChunk = chunk.world.GetChunk(chunkPos);
                        if (!tmpChunk.IsEmpty(pos))
                        {
                            byte tmp = (1 << (int)Facing.South) | (1 << (int)Facing.North);
                            byte set = 0;
                            for (Facing facing = Facing.First; facing <= Facing.Last; facing++)
                            {
                                offset.Set(pos);
                                offset.Add(facing.GetVector());
                                chunkPos.Set(offset);
                                if (chunkPos.z < 0)
                                {
                                    set |= (byte)(1 << (int)facing);
                                }
                                else
                                {
                                    tmpChunk = chunk.world.GetChunk(chunkPos);
                                    if (facing != Facing.North && tmpChunk.IsEmpty(offset))
                                    {
                                        set |= (byte)(1 << (int)facing);
                                    }
                                }
                            }
                            data[Chunk.GetBlockIndex(pos)] = set;
                            UnityEngine.Profiling.Profiler.BeginSample("AddBlock");
                            AddBlock(pos, set);
                            //AddBlock(pos, (byte)(set & tmp));
                            UnityEngine.Profiling.Profiler.EndSample();
                        }
                        else
                        {
                            if ((x & 0xF) == 0)
                            {
                                sides |= 1 << (int)Facing.West;
                            }
                            else
                            if ((x & 0xF) == 0xF)
                            {
                                sides |= 1 << (int)Facing.East;
                            }

                            if ((y & 0xF) == 0)
                            {
                                sides |= 1 << (int)Facing.Down;
                            }
                            else
                            if ((y & 0xF) == 0xF)
                            {
                                sides |= 1 << (int)Facing.Up;
                            }

                            if ((z & 0xF) == 0)
                            {
                                sides |= 1 << (int)Facing.South;
                            }
                            else
                            if ((z & 0xF) == 0xF)
                            {
                                sides |= 1 << (int)Facing.North;
                            }
                        }
                    }
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uv);
            mesh.RecalculateNormals();
            //mesh.Optimize();

            UnityEngine.Profiling.Profiler.EndSample();
        }

        void AddBlock(BlockPos pos, byte facings)
        {
            for (Facing facing = Facing.First; facing <= Facing.Last; facing++)
            {
                if (((facings >> (int)facing) & 1) == 1)
                {
                    var side = Geometry.cube.sides[(int)facing];
                    var l = vertices.Count;
                    for (int i = 0; i < side.vertices.Length; i++)
                    {
                        vertices.Add(pos.ToVector(side.vertices[i]));
                    }
                    uv.AddRange(side.uv);
                    for (int i = 0; i < side.triangles.Length; i++)
                    {
                        triangles.Add(l + side.triangles[i]);
                    }
                }
            }
        }

        public void Dispose()
        {
            chunk.cubeChanged -= Chunk_cubeChanged;
        }
    }
}
