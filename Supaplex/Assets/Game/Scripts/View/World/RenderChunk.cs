using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Logic.World;

namespace Game.View
{
    public class RenderChunk : IDisposable
    {
        public byte visibleFacing;
        public Chunk chunk { get; private set; }
        public Bounds bounds { get; private set; }
        public int frameIndex { get; private set; }
        public bool isCalculated { get; private set; }
        public Mesh mesh { get; private set; }
        public int empty;

        static int[] DF; // index offsets

        bool[] data;
        List<Vector3> vertices;
        List<int> triangles;
        List<Vector2> uv;

        public RenderChunk(Chunk chunk)
        {
            this.chunk = chunk;
            this.bounds = chunk.position.bounds;
            this.chunk.cubeChanged += Chunk_cubeChanged;
            empty = 4096;
            data = new bool[empty]; // 16 * 16 * 16
            vertices = new List<Vector3>(); // 16 * 16 * 16 * 24?
            triangles = new List<int>();
            uv = new List<Vector2>();
            isCalculated = false;

            mesh = null;
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
            return ((visibleFacing >> (int)facing) & 1) == 1;
        }

        public IEnumerator CalcVisibility()
        {
            isCalculated = true;
            visibleFacing = 0;

            var min = chunk.position.min;
            var max = chunk.position.max;
            var pos = new BlockPos();
            UnityEngine.Profiling.Profiler.BeginSample("CalcVisibiltity");
            for (int z = min.z; z <= max.z; z++)
            {
                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        pos.Set(x, y, z);
                        if (chunk.IsEmpty(pos))
                        {
                            data[Chunk.GetBlockIndex(pos)] = false;
                            if ((x & 0xF) == 0)
                            {
                                visibleFacing |= 1 << (int)Facing.West;
                            }
                            else
                            if ((x & 0xF) == 0xF)
                            {
                                visibleFacing |= 1 << (int)Facing.East;
                            }

                            if ((y & 0xF) == 0)
                            {
                                visibleFacing |= 1 << (int)Facing.Down;
                            }
                            else
                            if ((y & 0xF) == 0xF)
                            {
                                visibleFacing |= 1 << (int)Facing.Up;
                            }

                            if ((z & 0xF) == 0)
                            {
                                visibleFacing |= 1 << (int)Facing.South;
                            }
                            else
                            if ((z & 0xF) == 0xF)
                            {
                                visibleFacing |= 1 << (int)Facing.North;
                            }
                        }
                        else
                        {
                            data[Chunk.GetBlockIndex(pos)] = true;
                            empty--;
                        }
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();

            yield break;
        }

        public IEnumerator CalcMesh()
        {
            mesh = new Mesh();

            var min = chunk.position.min;
            var max = chunk.position.max;
            var pos = new BlockPos();
            var offset = new BlockPos();
            var chunkPos = new ChunkPos();
            Chunk tmpChunk = null;
            //UnityEngine.Profiling.Profiler.BeginSample("CalcMesh");
            for (int z = min.z; z <= max.z; z++)
            {
                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        pos.Set(x, y, z);
                        var index = Chunk.GetBlockIndex(pos);
                        if (data[index])
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
                                    chunkPos.Set(offset);
                                    if (chunkPos.z < 0)
                                    {
                                        set |= (byte)(1 << (int)facing);
                                    }
                                    else
                                    {
                                        tmpChunk = chunk.world.GetChunk(chunkPos);
                                        if (tmpChunk.IsEmpty(offset))
                                        {
                                            set |= (byte)(1 << (int)facing);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!data[index + DF[(int)facing]])
                                    {
                                        set |= (byte)(1 << (int)facing);
                                    }
                                }
                            }
                            AddBlock(pos, set);
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uv);
            mesh.RecalculateNormals();
            mesh.Optimize();

            //UnityEngine.Profiling.Profiler.EndSample();

            yield break;
        }

        void AddBlock(BlockPos pos, byte facings)
        {
            var objectType = chunk.GetBlockId(pos);
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
                    return;
            }
            for (Facing facing = Facing.First; facing <= Facing.Last; facing++)
            {
                if (((facings >> (int)facing) & 1) == 1)
                {
                    var side = Geometry.cube.sides[(int)facing];
                    var l = vertices.Count;
                    for (int i = 0; i < side.vertices.Length; i++)
                    {
                        var v = side.vertices[i];
                        if (objectType == BlockType.Stone4x4)
                        {
                            vertices.Add(p + v * 4);
                        }
                        else
                        {
                            vertices.Add(p + v);
                        }
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

        static RenderChunk()
        {
            var dx = Chunk.GetBlockIndex(new BlockPos(1, 0, 0));
            var dy = Chunk.GetBlockIndex(new BlockPos(0, 1, 0));
            var dz = Chunk.GetBlockIndex(new BlockPos(0, 0, 1));
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
