using System;
using UnityEngine;
using Game.Logic;

namespace Game.View
{
    public class RenderChunk : IDisposable
    {
        public CubeSideSet sides;
        CubeSideSet[] data;
        public Chunk chunk { get; private set; }

        public RenderChunk(Chunk chunk)
        {
            this.chunk = chunk;
            this.chunk.cubeChanged += Chunk_cubeChanged;
            data = new CubeSideSet[4096]; // 16 * 16 * 16
        }

        public CubeSideSet this[Vector3Int pos] {
            get {
                return data[ChunkCube.GetIndex(pos)];
            }
        }

        private void Chunk_cubeChanged(object sender, ChunkChangeEvent e)
        {
            throw new System.NotImplementedException();
        }

        public void CalcVisibility()
        {
            var min = chunk.position.Min();
            var max = chunk.position.Max();
            var pos = Vector3Int.zero;
            sides = CubeSideSet.None;
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    for (int z = min.z; z <= max.z; z++)
                    {
                        pos.Set(x, y, z);
                        if (!chunk.world.IsEmpty(pos))
                        {
                            var set = CubeSideSet.None;
                            for (CubeSide side = CubeSide.First; side <= CubeSide.Last; side++)
                            {
                                if (chunk.world.IsEmpty(pos.Offset(side)))
                                {
                                    set |= side.GetSet();
                                }
                            }
                            data[ChunkCube.GetIndex(pos)] = set;
                        }
                        else
                        {
                            if ((x & 0xF) == 0)
                            {
                                sides |= CubeSideSet.West;
                            }
                            else
                            if ((x & 0xF) == 0xF)
                            {
                                sides |= CubeSideSet.East;
                            }

                            if ((y & 0xF) == 0)
                            {
                                sides |= CubeSideSet.Down;
                            }
                            else
                            if ((y & 0xF) == 0xF)
                            {
                                sides |= CubeSideSet.Up;
                            }

                            if ((z & 0xF) == 0)
                            {
                                sides |= CubeSideSet.South;
                            }
                            else
                            if ((z & 0xF) == 0xF)
                            {
                                sides |= CubeSideSet.North;
                            }
                        }
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
